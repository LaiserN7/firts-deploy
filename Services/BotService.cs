using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public class BotService : IBotService
    {
        private IOptions<BotConfiguration> Config { get; }

        public BotService(IOptions<BotConfiguration> config)
        {
            Config = config;
            // use proxy if configured in appsettings.*.json
            Client = string.IsNullOrEmpty(Config.Value.Host)
                ? new TelegramBotClient(Config.Value.BotToken)
                : new TelegramBotClient(
                    Config.Value.BotToken,
                    GetProxyClient(Config.Value.Host, Config.Value.Port, Config.Value.UserName, Config.Value.Password));
        }

        private static HttpClient GetProxyClient(string host, int port, string username, string password)
        {
            var proxy = new WebProxy($"{host}:{port}/", false, System.Array.Empty<string>(),
                new NetworkCredential(username, password));

            var httpClientHandler = new HttpClientHandler()
            {
                //Proxy = proxy,
                SslProtocols = SslProtocols.Tls
            };

            var client = new HttpClient(httpClientHandler, true);
            return client;
        }

        public TelegramBotClient Client { get; }
    }
}