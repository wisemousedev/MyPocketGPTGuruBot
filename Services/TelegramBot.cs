using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;


namespace MyPocketGPTGuruBot.Services
{
    public static class TelegramBot
    {
        private static readonly string TelegramBotToken = "5943890296:AAEff4_vL9xRz-UP4AZDQtu6LSGw2gGbTPw";
        private static readonly string ChatGptApiToken = "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";

        private static ITelegramBotClient _botClient;
        private static ChatGptApiHelper _chatGptApiHelper;

        public static async Task Main()
        {
            _botClient = new TelegramBotClient(TelegramBotToken);
            _chatGptApiHelper = new ChatGptApiHelper(ChatGptApiToken);

            var me = await _botClient.GetMeAsync();
            Console.Title = me.Username;

            Console.WriteLine($"Bot {me.Username} is running...");

            int offset = 0;
            while (true)
            {
                var updates = await _botClient.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    if (update.Message != null && update.Message.Text != null)
                    {
                        Console.WriteLine($"Received a message from {update.Message.From.FirstName}: {update.Message.Text}");

                        // Get response from ChatGPT
                        string chatGptResponse;
                        try
                        {
                            chatGptResponse = await _chatGptApiHelper.GetResponseAsync(update.Message.Text);
                            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, chatGptResponse); // Add this line
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine($"StackTrace: {ex.StackTrace}");
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                                Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                            }
                            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, I couldn't process your request. Please try again later.");
                            continue;
                        }


                        offset = update.Id + 1;
                    }

                    Thread.Sleep(1000);
                }
            }
        }
    }
}

