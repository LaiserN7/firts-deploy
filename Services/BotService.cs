using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            // use proxy if configured in appsettings.*.json
            Client = string.IsNullOrEmpty(_config.Host)
                ? new TelegramBotClient(_config.BotToken)
                : new TelegramBotClient(
                    _config.BotToken,
                    GetProxyClient(_config.Host, _config.Port, _config.UserName, _config.Password));
        }

        public HttpClient GetProxyClient(string host, int port, string username, string password)
        {
            var proxy = new WebProxy($"{host}:{port}/", false, new string[] { },
                new NetworkCredential(username, password));

            var httpClientHandler = new HttpClientHandler()
            {
                //Proxy = proxy,
                SslProtocols = SslProtocols.Tls | SslProtocols.Ssl3 | SslProtocols.Ssl2
            };

            var client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
            return client;
        }

        public TelegramBotClient Client { get; }
    }
}