using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TelegramOnlyBot.Helpers.JSON;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramOnlyBot.Models;

namespace TelegramOnlyBot
{
    class Program
    {

        private static readonly TimeTablesService TimeTablesDB = new TimeTablesService();

        private static readonly CountriesService CountriesDB = new CountriesService();

        private static readonly CitiesService CitiesDB = new CitiesService();

        private static readonly UniversitiesService UniversitiesDB = new UniversitiesService();

        static async Task<string> PostURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }

        static readonly HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
            Dictionary<int, string> days = new Dictionary<int, string>(6);
            days.Add(1, "Понедельник");
            days.Add(2, "Вторник");
            days.Add(3, "Среда");
            days.Add(4, "Четверг");
            days.Add(5, "Пятница");
            days.Add(6, "Суббота");


            var botClient = new TelegramBotClient("1837593586:AAGJCGUa3LY9U05r_h8iI-1ZUM91njSzLkI");
            using var cts = new CancellationTokenSource();
            botClient.StartReceiving(new DefaultUpdateHandler(updateHandler: HandleUpdateAsync, errorHandler: HandleErrorAsync),
    cts.Token);
            Console.ReadLine();

            Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                return Task.CompletedTask;
            }

            

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {

                if (update.Type != UpdateType.Message)
                {
                    if (update.Type == UpdateType.CallbackQuery)
                    {
                        string[] array = update.CallbackQuery.Data.Split(":");
                        Console.WriteLine(update.CallbackQuery.Data);
                        if (array[0] == "1")
                        {
                            Console.WriteLine(11);
                            var cities = await CitiesDB.GetCities(array[2]);
                            var citiesNameString = "";
                            var citiesIdString = "";
                            foreach (Cities i in cities)
                            {
                                Console.WriteLine(i.Name);
                                citiesNameString += i.Name + ",";
                                citiesIdString += "2:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(citiesNameString.Remove(citiesNameString.Length - 1).Split(","), citiesIdString.Remove(citiesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(
                                chatId: update.CallbackQuery.Message.Chat.Id,
                                messageId: Int32.Parse(array[1]),
                                text: "Выбери город",
                                replyMarkup: inlineKeyboard);
                        }
                        else if (array[0] == "2")
                        {
                            var universities = await UniversitiesDB.GetUniversities(array[2]);
                            var universitiesNameString = "";
                            var universitiesIdString = "";
                            foreach (Universities i in universities)
                            {
                                Console.WriteLine(i.Name);
                                universitiesNameString += i.Name + ",";
                                universitiesIdString += "3:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(universitiesNameString.Remove(universitiesNameString.Length - 1).Split(","), universitiesIdString.Remove(universitiesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(
                                chatId: update.CallbackQuery.Message.Chat.Id,
                                messageId: Int32.Parse(array[1]),
                                text: "Выбери университет",
                                replyMarkup: inlineKeyboard);
                        }
                        else if (array[0] == "3")
                        {
                            Console.WriteLine(21);
                            var timeTables = await TimeTablesDB.GetTimeTables(array[2]);
                            var timeTablesNameString = "";
                            var timeTablesIdString = "";
                            foreach (TimeTables i in timeTables)
                            {
                                Console.WriteLine(i.Group);
                                timeTablesNameString += i.Group + ",";
                                timeTablesIdString += "4:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(timeTablesNameString.Remove(timeTablesNameString.Length - 1).Split(","), timeTablesIdString.Remove(timeTablesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(
                                chatId: update.CallbackQuery.Message.Chat.Id,
                                messageId: Int32.Parse(array[1]),
                                text: "Выбери группу",
                                replyMarkup: inlineKeyboard);
                        }
                        else if (array[0] == "4")
                        {
                            await TimeTablesDB.RemoveStudent(update.CallbackQuery.Message.Chat.Id);
                            await TimeTablesDB.UpdateStudents(array[2], update.CallbackQuery.Message.Chat.Id);
                            await botClient.EditMessageTextAsync(
                                chatId: update.CallbackQuery.Message.Chat.Id,
                                messageId: Int32.Parse(array[1]),
                                text: "Вы успешно добавлены в базу данных!");
                        }
                    }
                    return;
                }
                if (update.Message.Type != MessageType.Text)
                {

                    return;
                }
                var chatId = update.Message.Chat.Id;


                Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId} {update.Message.Chat.FirstName}.");

                Message message = update.Message;

                if (message.Text == "/check")
                {
                    int lol = 0;
                    TimeTables timeTables = await TimeTablesDB.GetTimeTable(message.Chat.Id);
                    if (timeTables is not null)
                    {
                        DateTime dayNow = DateTime.Today;
                        var cal = new GregorianCalendar();
                        var weekNumber = cal.GetWeekOfYear(dayNow, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                        foreach (Week week in timeTables.Weeks)
                        {
                            if (week.Number == weekNumber % 2)
                            {
                                string responseMessage = "*Расписание на неделю:*";
                                await botClient.SendTextMessageAsync(
                                    parseMode: ParseMode.Markdown,
                                    chatId: chatId,
                                    text: responseMessage,
                                    replyMarkup: new ReplyKeyboardRemove());
                                foreach (Day day in week.Days)
                                {
                                    responseMessage = "";
                                    responseMessage += "*" + days[day.Number] + "*\n\n";
                                    lol = 0;
                                    foreach (Lesson lesson in day.Lessons)
                                    {
                                        lol++;
                                        responseMessage += "*" + lesson.Number + " пара*\n";
                                        responseMessage += lesson.Name + " - ";
                                        responseMessage += lesson.Teacher + "\nАудитория - ";
                                        responseMessage += lesson.Audience + "\n\n";
                                    }
                                    if (lol != 0)
                                    {
                                        await botClient.SendTextMessageAsync(
                                            parseMode: ParseMode.Markdown,
                                            chatId: chatId,
                                            text: responseMessage,
                                            replyMarkup: new ReplyKeyboardRemove());
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Вашего расписания нет в базе данных");
                    }
                }
                else if (message.Text == "/checktoday")
                {
                    TimeTables timeTables = await TimeTablesDB.GetTimeTable(message.Chat.Id);
                    if (timeTables is not null)
                    {
                        DateTime dayNow = DateTime.Today;
                        var cal = new GregorianCalendar();
                        var weekNumber = cal.GetWeekOfYear(dayNow, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                        int c = 0;
                        foreach (Week week in timeTables.Weeks)
                        {
                            if (week.Number == weekNumber % 2)
                            {
                                foreach (Day day in week.Days)
                                {
                                    if (day.Number == (int)dayNow.DayOfWeek)
                                    {
                                        string responseMessage = "*Расписание на сегодня:\n\n*";
                                        foreach (Lesson lesson in day.Lessons)
                                        {

                                            responseMessage += "*" + lesson.Number + " пара*\n";
                                            responseMessage += lesson.Name + " - ";
                                            responseMessage += lesson.Teacher + "\nАудитория - ";
                                            responseMessage += lesson.Audience + "\n\n";
                                        }

                                        c = 1;
                                        await botClient.SendTextMessageAsync(
                                            parseMode: ParseMode.Markdown,
                                            chatId: chatId,
                                            text: responseMessage,
                                            replyMarkup: new ReplyKeyboardRemove());
                                    }
                                }
                            }
                        }
                        if (c == 0)
                        {
                            await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "Сегодня нет пар!");
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Вашего расписания нет в базе данных");
                    }


                }
                else if (message.Text == "/checktomorrow")
                {
                    TimeTables timeTables = await TimeTablesDB.GetTimeTable(message.Chat.Id);
                    if (timeTables is not null)
                    {
                        DateTime dayNow = DateTime.Today.AddDays(1);
                        var cal = new GregorianCalendar();
                        var weekNumber = cal.GetWeekOfYear(dayNow, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

                        int c = 0;
                        foreach (Week week in timeTables.Weeks)
                        {
                            if (week.Number == weekNumber % 2)
                            {
                                foreach (Day day in week.Days)
                                {
                                    if (day.Number == (int)dayNow.DayOfWeek)
                                    {
                                        string responseMessage = "*Расписание на завтра:*\n\n";
                                        foreach (Lesson lesson in day.Lessons)
                                        {

                                            responseMessage += "*" + lesson.Number + " пара*\n";
                                            responseMessage += lesson.Name + " - ";
                                            responseMessage += lesson.Teacher + "\nАудитория - ";
                                            responseMessage += lesson.Audience + "\n\n";
                                        }

                                        c = 1;
                                        await botClient.SendTextMessageAsync(
                                            parseMode: ParseMode.Markdown,
                                            chatId: chatId,
                                            text: responseMessage,
                                            replyMarkup: new ReplyKeyboardRemove());
                                    }
                                }
                            }
                        }
                        if (c == 0)
                        {
                            await botClient.SendTextMessageAsync(
                                            chatId: chatId,
                                            text: "Завтра нет пар!");
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Вашего расписания нет в базе данных");
                    }
                }
                else if (message.Text == "/setgroup")
                {
                    var countries = await CountriesDB.GetCounties();
                    var countriesNameString = "";
                    var countriesIdString = "";
                    Message messageKeyboard = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Выбери страну");
                    foreach (Countries i in countries)
                    {
                        countriesNameString += i.Name + ",";
                        countriesIdString += "1:" + messageKeyboard.MessageId + ":" + i.Id + ",";
                    }
                    var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(countriesNameString.Remove(countriesNameString.Length - 1).Split(","), countriesIdString.Remove(countriesIdString.Length - 1).Split(",")));
                    await botClient.EditMessageTextAsync(
                        chatId: messageKeyboard.Chat.Id,
                        messageId: messageKeyboard.MessageId,
                        text: "Выбери страну",
                        replyMarkup: inlineKeyboard);


                }
                else
                {
                    await botClient.SendTextMessageAsync(
                                            parseMode: ParseMode.Markdown,
                                            chatId: chatId,
                                            text: "Используй команды из меню",
                                            replyMarkup: new ReplyKeyboardRemove());

                }
            }
        }
        private static InlineKeyboardButton[][] GetInlineKeyboard(string[] nameArray, string[] idArray)
        {
           
            var keyboardInline = new InlineKeyboardButton[nameArray.Length][];
            for (var i = 0; i < nameArray.Length; i++)
            {
                keyboardInline[i] = new InlineKeyboardButton[1];
                keyboardInline[i][0] = InlineKeyboardButton.WithCallbackData(text: nameArray[i], callbackData: idArray[i]);
            }
            return keyboardInline;
        }
    }
}