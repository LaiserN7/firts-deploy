using System;
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
        private static Dictionary<string, string> ChatFiles => _chatFiles ?? (_chatFiles = new Dictionary<string, string>
        { { "Sticker_Ti_pidor", "CAADAgADbgEAAjbsGwVfHkWISq9DiQI" }, {"Bille_ok", "CAADAgADxgYAAkb7rATY_tcr05tnDQI" }, {"pikachu_coffe", "CAADAgADvwIAAjZ2IA6DDqb7e0kSxQI" },
            {"capitan", "CAADAgADOgEAAhZ8aAOmH9gbWog58wI" }, {"buxat_student", "CAADBQADoQMAAukKyAM19jBdMVuSAgI"}, {"gde_vse", "CAADBQADqgMAAukKyAOMMrddotAFYQI" },
            {"krasava", "CAADBQADewMAAukKyANR7TNPzLj7awI"}, { "go_buxat", "CAADBQADbwMAAukKyAOvzr7ZArpddAI"}, { "privet", "CAADAgADBQYAAhhC7giZXSreX-e4UgI"} });
        private readonly BotConfiguration _config;
        private static Dictionary<string, string> _chatOptions;
        private static Dictionary<string, string> ChatOptions => _chatOptions ??
            (_chatOptions = new Dictionary<string, string> { { "repeat", "false" } });
        private static Dictionary<string, string> _chatsIds;
        private static Dictionary<string, string> ChatsIds => _chatsIds ??
           (_chatsIds = new Dictionary<string, string> { { "main", "-1001300792680" }, {"private", "449279856" } });

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            _botService = botService;
            _logger = logger;
        }

        public Task EchoAsync(Update update)
        {
            //if (!string.IsNullOrEmpty(update.Message.Sticker.FileId) && update.Message.Chat.Id == _config.DefaultChatId)
            //    _botService.Client.SendTextMessageAsync(_config.DefaultChatId, update.Message.Sticker.FileId);

            switch (update.Type)
            {
                case UpdateType.Message:
                    return Сheck(update);

                case UpdateType.CallbackQuery:
                    return HandlingCallback(update.CallbackQuery);
            }

            throw new ApplicationException($"Type '{update.Type}' not support");
            //var message = update.Message;

            //_logger.LogInformation("Received Message from {0}", message.Chat.Id);

            

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

        private async Task HandlingCallback(CallbackQuery callback)
        {
            (byte commandId, long chatId, string value) = GetInfoFromCallBack(callback.Data);
            if (commandId  == (byte) Commands.repeat)
            {
                ChatOptions["repeat"] = value;
                await _botService.Client.SendTextMessageAsync(chatId, $"success update state of repeat to {value}");
            }
        }


        private async Task Сheck(Update update)
        {
            var message = update.Message;

            if (message == null || message.Type != MessageType.Text) return;

            if (ChatOptions["repeat"] == "true")
            {
                // Echo each Message
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
            }


            if (Regex.IsMatch(message.Text, @"туп.*бот|бот.*туп|бот.*глуп", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Kiss my shiny metal arse!!!");
                return;
            }

            if (Regex.IsMatch(message.Text, @"руслан.*знаешь|знаешь.*руслан|руслан.*ведь|дим.*ведь|андрей.*ведь|антон.*ведь", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["Sticker_Ti_pidor"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"красава.*бот|бот.*красава|cтаня.*красава", RegexOptions.IgnoreCase))
            {
                //await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Спасибо, бро)))");
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["krasava"]);
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

            if (Regex.IsMatch(message.Text, @"^Всем привет|^Привет Станя|^Здорова Станя", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["privet"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"^Станя го бухать", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["go_buxat"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"^го бухать ребята", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["buxat_student"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"так точно", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["capitan"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"кофе", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["pikachu_coffe"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"где\sты|где\sвы", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["gde_vse"]);
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
                    var rowButtons = new List<IEnumerable<InlineKeyboardButton>>();
                    var _keyboard = new InlineKeyboardMarkup(rowButtons);
                    rowButtons.Add(GetInlineKeyboard("Enable", $"commandId={Commands.repeat}&value=true"));
                    rowButtons.Add(GetInlineKeyboard("Disable", $"commandId={Commands.repeat}&value=false"));

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

        private InlineKeyboardButton[] GetInlineKeyboard(string taskName, string callBackData) => new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData(text:taskName, callbackData:callBackData)
        };

        private (byte command, long chatId, string value) GetInfoFromCallBack(string callBack)
        {
            const string pattern = @"^commandId=(?<commandId>-?\d+)&chatId=(?<chatId>-?\d+)&value=(?<value>-?.*)";

            var m = Regex.Match(callBack, pattern);

            if (m.Length > 0)
                if (byte.TryParse(m.Groups["commandId"].Value, out byte command)
                    && long.TryParse(m.Groups["chatId"].Value, out long chatId)
                    && m.Groups["value"].Value != null)
                {
                    return (command, chatId, m.Groups["value"].Value);
                }

              throw new ApplicationException($"Wrong query callBack = {callBack}");
        }

        public enum Commands: byte{
            repeat
        }
    }
}
