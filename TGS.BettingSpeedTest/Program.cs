using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using NLog;
using TGS.BettingSpeedTest.Clients;
using TGS.BettingSpeedTest.GameProviderApis;
using TGS.BettingSpeedTest.Messages.Global;
using TGS.BettingSpeedTest.Settings;

namespace TGS.BettingSpeedTest
{
    internal static class Program
    {
        private static readonly IGlobalApiClient GlobalApiClient = new GlobalApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly CancellationTokenSource Cts = new();

        private static readonly IDictionary<string, IGameProviderApi> GameProviderApiList =
            new Dictionary<string, IGameProviderApi>
            {
                //{"AG", new AsiaGamingApi()},
                {"SBO", new SboGamingApi()}
            };

        private static readonly
            IDictionary<string, Func<IGameProviderApi, string, CreatePlayerAndLoginResponse, int, ValueTask>>
            ScenarioList =
                new Dictionary<string, Func<IGameProviderApi, string, CreatePlayerAndLoginResponse, int, ValueTask>>
                {
                    ["PlaceAndSettle"] = (gpApi, userId, createPlayerAndLoginResponse, count) =>
                        gpApi.PlaceSingleBet(userId, createPlayerAndLoginResponse, count),
                    ["MultipleBets"] = (gpApi, userId, createPlayerAndLoginResponse, count) =>
                        count % 3 == 0
                            ? gpApi.PlaceMultipleBets(userId, createPlayerAndLoginResponse, count)
                            : gpApi.PlaceSingleBet(userId, createPlayerAndLoginResponse, count),
                    ["PlaceAndSettleWithDuplicates"] = (gpApi, userId, createPlayerAndLoginResponse, count) =>
                        gpApi.PlaceDuplicateSingleBet(userId, createPlayerAndLoginResponse, count),
                    ["PlaceOppositeBetsAndSettle"] = (gpApi, userId, createPlayerAndLoginResponse, count) =>
                        gpApi.PlaceOppositeBets(userId, createPlayerAndLoginResponse, count)
                };

        private static readonly AutoResetEvent WaitEvent = new(false);

        internal static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Cts.Cancel();
                WaitEvent.Set();
                Console.WriteLine("Process exited by killing process!");
            };

            Console.CancelKeyPress += (s, e) =>
            {
                Cts.Cancel();
                WaitEvent.Set();
                Console.WriteLine("Process exited by ctrl + c!");
            };

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => RunTest(opts).GetAwaiter().GetResult())
                .WithNotParsed(HandleParseError);

            WaitEvent.WaitOne();
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var err in errs)
            {
                Console.WriteLine(err.Tag);
            }
        }

        private static async Task RunTest(Options options)
        {
            var playCycle = options.PlayCycleOfPercentage;
            var millisecondsDelay = options.MillisecondsDelay;
            var percentageOfPlay = options.PercentageOfPlay;

            var numberOfPlayers = options.NumberOfPlayers;

            ServicePointManager.DefaultConnectionLimit = numberOfPlayers * 3;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            if (!GameProviderApiList.TryGetValue(options.GameProviderCode, out var gpApi))
            {
                Logger.Warn($"Failed to get game provider {options.GameProviderCode}");
                return;
            }

            if (!ScenarioList.TryGetValue(options.Scenario, out var scenario))
            {
                Logger.Warn($"Failed to get scenario {options.Scenario}");
                return;
            }

            var gpEndpoint = gpApi.SetupHttpsForEndpoint(options.EnableHttps);

            Console.WriteLine($"Executing {options.Scenario} scenario");
            Console.WriteLine($"Executing {numberOfPlayers} players testing");
            Console.WriteLine($"Version {typeof(Program).Assembly.GetName().Version}");
            Console.WriteLine($"Api: {gpEndpoint}");
            Console.WriteLine("Press ctrl+c to stop service........");

            Logger.Info(Environment.NewLine);

            var totalPlayers = Enumerable.Range(0, numberOfPlayers).ToArray();

            // initialize players in batches to avoid large thread initialization at the same time
            // which may lead to unexpected timeout
            const int batchSize = 50;
            var numberOfBatches = (int) Math.Ceiling((double) numberOfPlayers / batchSize);

            for (var currentBatch = 0; currentBatch < numberOfBatches; currentBatch++)
            {
                var currentBatchSize = totalPlayers.Skip(currentBatch * batchSize).Take(batchSize);

                var tasks = Enumerable.Range(currentBatch * batchSize, currentBatchSize.ToArray().Length).Select(i =>
                    Task.Factory.StartNew(async () =>
                    {
                        var userId = $"{Environment.MachineName}-{i:D3}";
                        Console.WriteLine(
                            $"{userId} starts at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} :{Thread.CurrentThread.ManagedThreadId}");

                        Logger.Info($"Create player {userId} and login");

                        var createPlayerAndLoginResponse = await GlobalApiClient.CreatePlayerAndLoginAsync(new()
                        {
                            BrandCode = options.BrandCode,
                            Username = userId,
                            CurrencyCode = PlayerSettings.Currency,
                            InitialWalletAmount = PlayerSettings.InitWalletAmount
                        });

                        if (createPlayerAndLoginResponse == null || createPlayerAndLoginResponse.Error > 0)
                        {
                            Logger.Warn($"Failed to create and login player {userId}");
                            return;
                        }

                        var j = 1;
                        while (!Cts.IsCancellationRequested)
                        {
                            if (j % playCycle == 0 && !CanPlay(percentageOfPlay))
                            {
                                j++;
                                await Task.Delay(millisecondsDelay);
                                continue;
                            }

                            try
                            {
                                Logger.Info($"Player {userId} starts scenario");

                                await scenario(gpApi, userId, createPlayerAndLoginResponse, j);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                            finally
                            {
                                j++;
                                await Task.Delay(millisecondsDelay);
                            }
                        }
                    }, TaskCreationOptions.LongRunning));

                await Task.WhenAll(tasks);

                // sleep in between each batch
                Thread.Sleep(5000);
            }
        }

        private static bool CanPlay(byte percentageOfPlay)
        {
            switch (percentageOfPlay)
            {
                case 100:
                case > 100:
                    return true;
                case 0:
                    return false;
            }

            var rnd = new Random();
            var rndNumber = rnd.Next(1, 100);

            if (percentageOfPlay > 50) return rndNumber > 100 - percentageOfPlay;
            return rndNumber <= percentageOfPlay;
        }
    }
}