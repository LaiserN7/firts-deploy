using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Examples.DotNetCoreWebHook.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        private readonly ILogger<UpdateService> _logger;
        // 449279856 - Наиль
        private static int[] _trustedUsers;
        private static int[] TrustedUsers => _trustedUsers ?? (_trustedUsers = new[] { 449279856 });
        private static Dictionary<string, string> _chatFiles;
        private static Dictionary<string, string> ChatFiles => _chatFiles ?? (_chatFiles = new Dictionary<string, string> { { "Sticker_Ti_pidor", "CAADAgADbgEAAjbsGwVfHkWISq9DiQI" } });
        private readonly BotConfiguration _config;
        private static Dictionary<string, string> _chatOptions;
        private static Dictionary<string, string> ChatOptions => _chatOptions ??
            (_chatOptions = new Dictionary<string, string> { { "repeat", "false" } });

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            _botService = botService;
            _logger = logger;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            _logger.LogInformation("Received Message from {0}", message.Chat.Id);

            await Сheck(update);

            //if (message.Type == MessageType.Text)
            //{
            //    // Echo each Message
            //    await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
            //}
            //else if (message.Type == MessageType.Photo)
            //{
            //    // Download Photo
            //    var fileId = message.Photo.LastOrDefault()?.FileId;
            //    var file = await _botService.Client.GetFileAsync(fileId);

            //    var filename = file.FileId + "." + file.FilePath.Split('.').Last();

            //    using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
            //    {
            //        await _botService.Client.DownloadFileAsync(file.FilePath, saveImageStream);
            //    }

            //    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            //}
        }

        private async Task Сheck(Update update)
        {
            var message = update.Message;

            if (message == null || message.Type != MessageType.Text) return;


            if (Regex.IsMatch(message.Text, @"туп.*бот|бот.*туп|бот.*глуп", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Kiss my shiny metal arse!!!");
                return;
            }

            if (Regex.IsMatch(message.Text, @"руслан.*знаешь|знаешь.*руслан|руслан.*ведь|дим.*ведь", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["Sticker_Ti_pidor"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"красава.*бот|бот.*красава", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Спасибо, бро)))");
                return;
            }

            if (Regex.IsMatch(message.Text, @"ping", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "pong");
                return;
            }

            if (Regex.IsMatch(message.Text, @"^нет", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Пидора ответ");
                return;
            }

            if (!TrustedUsers.Contains(message.From.Id)) return;

            switch (message.Text.Split(' ').First())
            {

                // send inline keyboard
                case "/inline":
                    await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                    await Task.Delay(500); // simulate longer running task

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("1.1"),
                            InlineKeyboardButton.WithCallbackData("1.2"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("2.1"),
                            InlineKeyboardButton.WithCallbackData("2.2"),
                        }
                    });

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: inlineKeyboard);
                    break;

                // send custom keyboard
                case "/keyboard":
                    ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "1.1", "1.2" },
                        new[] { "2.1", "2.2" },
                    };

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: ReplyKeyboard);
                    break;

                // send a photo
                //case "/photo":
                //    await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                //    const string file = @"Files/tux.png";

                //    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                //    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                //    {
                //        await _botService.Client.SendPhotoAsync(
                //            message.Chat.Id,
                //            fileStream,
                //            "Nice Picture");
                //    }
                //    break;

                // request location or contact
                case "/request":
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestLocation("Location"),
                        KeyboardButton.WithRequestContact("Contact"),
                    });

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: RequestReplyKeyboard);
                    break;

                case "/hello":
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id,
                        "Дратути");
                    break;
                case "/chatId":
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id,
                        $"Current chatId: {message.Chat.Id}");
                    break;
                case "/ver":
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id,
                        $"Current version is: {_config.Version}");
                    break;
                case "/config":
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id,
                        $"Config Type: {_config.ConfigType}");
                    break;
                case "/repeat":
                    ReplyKeyboardMarkup _keyboard = new[]
                    {
                        new[] { "Enable", "Disable" }
                    };

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: _keyboard);
                    break;

                case "/info":
                    const string usage =
                        @"Usage:
                        /inline   - send inline keyboard
                        /keyboard - send custom keyboard
                        /request  - request location or contact
                        /hello - send a helo text
                        /ver - watch a version of bot
                        /chatId - watch id of current chat
                        /config - watch a type of config
                        /repeat - endable/disable repeat message";

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }
    }
}
