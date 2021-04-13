using System.Collections.Generic;
using TGS.BettingSpeedTest.Messages;

namespace TGS.BettingSpeedTest.Settings
{
    public static class OperatorSettings
    {
#if LOCAL
        public static readonly string ApiEndpoint = "http://localhost:62673/api";
        public static readonly string LobbyApiEndpoint = "http://localhost:9567/api";
        public static readonly string SystemApiEndpoint = "http://localhost:37169/api";
        public static readonly string SunbetCreditApiEndpoint = "http://localhost:9090";
#endif

#if QA
        public static readonly string ApiEndpoint = "http://qa.tgpaccess.com/api";
        public static readonly string LobbyApiEndpoint = "http://qa-lobby.tgpaccess.com/api";
        public static readonly string SystemApiEndpoint = "http://qa-system.tgpaccess.com/api";
        public static readonly string SunbetCreditApiEndpoint = "http://sb-gogp.qwerasdf.gq";
#endif

#if PRODUCTION
        public static readonly string ApiEndpoint = "http://perf-test-op.tgpaccess.com/api";
        public static readonly string LobbyApiEndpoint = "http://lobby.tgpaccess.com/api";
        public static readonly string SystemApiEndpoint = "http://system.tgpaccess.com/api";
        public static readonly string SunbetCreditApiEndpoint = "http://sb-gogp.qwerasdf.gq";
#endif

        public static class Player
        {
            public static string IpAddress = "127.0.0.1";
            public static string Language = "zh-CN";
            public static string Currency = "RMB";
            public static int BetlimitId = 1;
            public static bool IsTestPlayer = true;
        }

        public static class Lobby
        {
            public static string SunbetCreditPath = "sunbetcredit";
        }

        public static class OAuth
        {
            public static string ClientId = "GPIntegration";
            public static string ClientSecret = "jKAAY6bIp6MD7G2ciwxAz36gQeTAdD4qgNAT3BbTpu1";
            public static string GrantType = "client_credentials";
            public static string Scope = "playerapi";
            public static string TokenType = "Bearer";

            public static string SunbetCreditDeskTopClientId = "Merw0g9fgkppre";
            public static string SunbetCreditDeskTopClientSecret = "MerSsdsdiidgoiuwewe";
        }

        public static class ApiPath
        {
            public static string OAuthToken = "/oauth/token";
            public static string PlayerAuthorize = "/player/authorize";
            public static string WalletCredit = "/wallet/credit";
            public static string PlayerInfo = "/ugs/player?token=";

            public static string SunbetCreditAuthMany = "/sunbetcredit/auth/many";
            public static string SunbetCreditTokenAuthed = "/sunbetcredit/token/authed";
            public static string SunbetCreditWalletCredit = "/sunbetcredit/credit";
            public static string SunbetCreditWalletDebit = "/sunbetcredit/debit";

            public static string SunbetCreditPlayerInfo = "/api/gp/GetAccountInfo";
        }

        public static class Wallet
        {
            public static decimal CreditAmount = 10000;
        }
        
        public static readonly Dictionary<string, CredentialAndLobbyPath> Credentials =
            new Dictionary<string, CredentialAndLobbyPath>
            {
                {"GPI", new CredentialAndLobbyPath
                {
                    ClientId = "GPIntegration",
                    ClientSecret = "jKAAY6bIp6MD7G2ciwxAz36gQeTAdD4qgNAT3BbTpu1",
                    LobbyPath = "gpiai"
                }},
                {"TGPPlay", new CredentialAndLobbyPath
                {
                    ClientId = "TGPPlay",
                    ClientSecret = "pPTii7Jsg8Thzkl174hSD9h7Dlb0VNdR2pXAjigV76Aq9egdLXTB9d",
                    LobbyPath = "LOCALT5D"
                }},
                {"SunbetCredit", new CredentialAndLobbyPath
                {
                    ClientId = "SunbetCredit",
                    ClientSecret = "4OT8ohc6GdTQmGiseTrK3kgaqbAGnM1olBNzIoFU6M8DL9p9BwbIBL",
                    LobbyPath = "sunbetcredit"
                }},
                {"PROXYBRAND1", new CredentialAndLobbyPath
                {
                    ClientId = "PROXYBRAND1",
                    ClientSecret = "g5Uw9QCJt4MQBy9exWYUu7v8J2ZgB0hKYbvvPVFP8qnb5008oBl6qx",
                    LobbyPath = "gpiai"
                }},
                {"TgpJackpot", new CredentialAndLobbyPath
                {
                    ClientId = "TgpJackpot",
                    ClientSecret = "Z1E6hGgWYWT04YIhTB0hkLGE5RJGVxzFXMD1RtZeTgh2",
                    LobbyPath = "tgpjackpot"
                }}
            };
    }
}