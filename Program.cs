using HtmlAgilityPack;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
namespace ConsoleApp3
{
   class Program
{
    static TelegramBotClient bot;
    public DateTime d = DateTime.Now;

        static async Task Main(string[] args)
    {

        Console.WriteLine(  "fwe");
        bot = new TelegramBotClient("6408970932:AAGR6KYY9OFG_a43iYSyxQXi82t13p-J6u0");

        bot.StartReceiving(Update, Error);

        var dailySendTime = new TimeSpan(2, 15, 0);
        while (true)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            var timeUntilSend = dailySendTime - now;

            // Если время до отправки сообщения больше нуля, подождите.
            if (timeUntilSend > TimeSpan.Zero)
            {
                await Task.Delay(timeUntilSend);
            }

            // Отправьте сообщение пользователям.
            await SendDailyMessage();

            // Подождите 24 часа перед следующей отправкой.
            await Task.Delay(TimeSpan.FromHours(24));
        }

    }

    static string GetRasp()
    {


            DateTime curday = DateTime.Now;
            string cur = "";
            switch (curday.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    cur = "Вторник";
                    break;

                case DayOfWeek.Tuesday:
                    cur = "Среда";
                    break;
                case DayOfWeek.Wednesday:
                    cur = "Четверг";
                    break;
                case DayOfWeek.Thursday:
                    cur = "Пятница";
                    break;
                case DayOfWeek.Saturday:
                    cur = "Понедельник";
                    break;
                case DayOfWeek.Sunday:
                    cur = "none";
                    break;
            }
            string s = "";
            if (cur != "none")
            {


                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                string url = "https://www.ects.ru/page281.htm";
                HtmlWeb web = new HtmlWeb();
                web.OverrideEncoding = Encoding.GetEncoding("windows-1251");

                HtmlDocument document = web.Load(url);
                HtmlNodeCollection h1Elements = document.DocumentNode.SelectNodes("//a");
                string file = "";
                if (h1Elements != null)
                {
                    foreach (HtmlNode h1Element in h1Elements)
                    {
                        if (h1Element.OuterHtml.Contains(cur))
                        {
                            Console.WriteLine(h1Element.InnerText);
                            foreach (var dd in h1Element.Attributes)
                            {
                                file = dd.Value;

                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("На странице нет элементов h1.");
                }
                string textAll = "";

                using (PdfReader pdfReader = new PdfReader(file))
                {
                    string text = string.Empty;

                    // Итерируем по всем страницам PDF
                    for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                    {
                        // Извлекаем текст с текущей страницы
                        text += PdfTextExtractor.GetTextFromPage(pdfReader, page);

                    }
                    textAll = text;
                    // Выводим извлеченный текст
                    // Console.WriteLine(text);
                }
                s = "";
                var d = textAll.Split();
                for (int i = 0; i < d.Length; i++)
                {
                    if (d[i].Contains("Пр-42"))
                    {
                        
                        s = s + (d[i] + " ");
                        s = s + (d[i + 1] + " ");
                        s = s + (d[i + 2] + " ");
                    }
                }
            }

            if (s == "")
            {
                s = "нет замен";
            }
            return s;
    }
    private static async Task SendDailyMessage()
    {
        try
        {
            // Замените "chatId" на ID чата, куда нужно отправить сообщение.
            long chatId = 292366290; // Замените на реальный chatId.

                // Замените "Your daily message here" на текст вашего ежедневного сообщения.
                string messageText = GetRasp();

            var message = await bot.SendTextMessageAsync(chatId, messageText);

            Console.WriteLine($"Sent message with text: '{messageText}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
            string rasp = "error";
            var message = update.Message;
            if (message != null)
            {
                rasp = GetRasp();
            }

                client.SendTextMessageAsync(message.Chat.Id, rasp);
        



    }

    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }




}
}