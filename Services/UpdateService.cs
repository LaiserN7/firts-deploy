using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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


        public UpdateService(IBotService botService, ILogger<UpdateService> logger)
        {
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

            if (!TrustedUsers.Contains(message.From.Id)) return;

            if (message == null || message.Type != MessageType.Text) return;

            if (Regex.IsMatch(message.Text, @"туп.*бот|бот.*туп", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Kiss my shiny metal arse!!!");
                return;
            }

            if (Regex.IsMatch(message.Text, @"руслан.*знаешь|знаешь.*руслан", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendStickerAsync(message.Chat.Id, ChatFiles["Sticker_Ti_pidor"]);
                return;
            }

            if (Regex.IsMatch(message.Text, @"красава.*бот|бот.*красава", RegexOptions.IgnoreCase))
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Спасибо, бро)))");
                return;
            }

            switch (message.Text.Split(' ').First())
            {
                case "ping":
                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "pong");
                    break;

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
                case "/photo":
                    await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string file = @"Files/tux.png";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await _botService.Client.SendPhotoAsync(
                            message.Chat.Id,
                            fileStream,
                            "Nice Picture");
                    }
                    break;

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
                case "/info":
                    const string usage = @"
Usage:
/inline   - send inline keyboard
/keyboard - send custom keyboard
/photo    - send a photo
/request  - request location or contact
/hello - send a helo text";

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }
    }
}
