namespace Telegram.Bot.Examples.DotNetCoreWebHook
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }

        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Version { get; set; }

        public string ConfigType { get; set; }

        public long DefaultChatId { get; set; }
    }
}
