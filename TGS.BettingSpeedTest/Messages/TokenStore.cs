using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TGS.BettingSpeedTest.GameProviderApis;

namespace TGS.BettingSpeedTest.Messages
{
    public class TokenStore
    {
        public TokenStore(Func<object, bool> shouldRefreshToken, Func<IGameProviderApi, Task<string>> getTokenTokenAsync)
        {
            ShouldRefreshToken = shouldRefreshToken;
            GetTokenFuncAsync = getTokenTokenAsync;
        }

        public string Token { get; set; }
        private Func<object, bool> ShouldRefreshToken { get; }
        private Func<IGameProviderApi, Task<string>> GetTokenFuncAsync { get; }
        public string ExpiredToken { get; set; }

        private readonly SemaphoreSlim _invokeGetTokenSemaphore = new SemaphoreSlim(1);
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task GetTokenAsync(IGameProviderApi gpApi)
        {
            Token = await GetTokenFuncAsync(gpApi);
        }

        public async Task RefreshTokenIfNeeded<T>(T response, IGameProviderApi gpApi)
        {
            if (!ShouldRefreshToken(response))
                return;

            try
            {
                await _invokeGetTokenSemaphore.WaitAsync();

                if (Token == ExpiredToken)
                {
                    Logger.Info($"Detected expired token {Token} udpated to {Token}, release thread :{Thread.CurrentThread.ManagedThreadId}");
                    return;
                }
                Logger.Info($"token expired. Before get token from {ExpiredToken} to {Token} , thread:{Thread.CurrentThread.ManagedThreadId}");
                ExpiredToken = Token;
                await GetTokenAsync(gpApi);

                Logger.Info($"token expired.  after get  token from {ExpiredToken} to {Token} , thread:{Thread.CurrentThread.ManagedThreadId}");
            }
            finally
            {
                Logger.Info($"released thread :{Thread.CurrentThread.ManagedThreadId}");
                _invokeGetTokenSemaphore.Release();
            }
        }
    }
}