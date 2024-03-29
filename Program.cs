﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
using Hangfire.Mongo;
using MongoDB.Driver;
using Hangfire;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;

namespace TelegramOnlyBot
{
    class Program
    {
        private static readonly CountriesService CountriesDB = new();

        private static readonly CitiesService CitiesDB = new();

        private static readonly UniversitiesService UniversitiesDB = new();

        private static readonly FacultiesService FacultiesDB = new();

        private static readonly SpecialtiesService SpecialitiesDB = new();

        private static readonly TimeTablesService TimeTablesDB = new();

        static void Main(string[] args)
        {
            /*var options = new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new DropMongoMigrationStrategy(),
                    BackupStrategy = new NoneMongoBackupStrategy()
                }
            };
            GlobalConfiguration.Configuration.UseMongoStorage("mongodb://localhost:27017/Telegram", options);
            var mongoStorage = new MongoStorage(
                            MongoClientSettings.FromConnectionString("mongodb://localhost:27017"),
                            "Telegram", // database name
                            options);
            using (var server = new BackgroundJobServer(mongoStorage))
            {
                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                Console.ReadKey();
                RecurringJob.AddOrUpdate("easyjob", () => Console.Write("Easy!"), Cron.Minutely);
            }

            GlobalConfiguration.Configuration.UseMongoStorage("mongodb://localhost:27017/Telegram", options);

            using (var server = new BackgroundJobServer())
            {
                server.Dispose();
                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                Console.ReadKey();
               
            }
            Console.WriteLine(1);
            var mongoStorage = new MongoStorage(
                            MongoClientSettings.FromConnectionString("mongodb://localhost:27017"),
                            "Telegram", // database name
                            options);
            Console.WriteLine(2);

            using (var server = new BackgroundJobServer(mongoStorage))
            {
                Console.WriteLine("Hangfire Server started. Press any key to exit...");
                Console.ReadKey();
                RecurringJob.AddOrUpdate("easyjob", () => Console.Write("LOL!"), Cron.Minutely);

            }
            Console.WriteLine(5);
            RecurringJob.AddOrUpdate("easyjob", () => Console.Write("LOL!"), Cron.Minutely);*/


            Dictionary<int, string> days = new(6);
            days.Add(1, "Понедельник");
            days.Add(2, "Вторник");
            days.Add(3, "Среда");
            days.Add(4, "Четверг");
            days.Add(5, "Пятница");
            days.Add(6, "Суббота");

            var botClient = new TelegramBotClient("1837593586:AAElIgx9Anhpm3tz58zjGNlwO0zcsBxTNdY");

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

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
                        Console.WriteLine($"Received a '{update.CallbackQuery.Data}' message in chat {update.CallbackQuery.Message.Chat.Id} {update.CallbackQuery.Message.Chat.Username}.");
                        string[] array = update.CallbackQuery.Data.Split(":");
                        if (array[0] == "1")
                        {
                            var cities = await CitiesDB.GetCities(array[2]);
                            var citiesNameString = "";
                            var citiesIdString = "";
                            foreach (Cities i in cities)
                            {
                                citiesNameString += i.Name + ",";
                                citiesIdString += "2:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(citiesNameString.Remove(citiesNameString.Length - 1).Split(","), citiesIdString.Remove(citiesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(chatId: update.CallbackQuery.Message.Chat.Id, messageId: Int32.Parse(array[1]), text: "Выбери город", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                        }
                        else if (array[0] == "2")
                        {
                            var universities = await UniversitiesDB.GetUniversities(array[2]);
                            var universitiesNameString = "";
                            var universitiesIdString = "";
                            foreach (Universities i in universities)
                            {
                                universitiesNameString += i.Name + ",";
                                universitiesIdString += "3:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(universitiesNameString.Remove(universitiesNameString.Length - 1).Split(","), universitiesIdString.Remove(universitiesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(chatId: update.CallbackQuery.Message.Chat.Id, messageId: Int32.Parse(array[1]), text: "Выбери университет", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                        }
                        else if (array[0] == "3")
                        {
                            var faculties = await FacultiesDB.GetFaculties(array[2]);
                            var facultiesNameString = "";
                            var facultiesIdString = "";
                            foreach (Faculties i in faculties)
                            {
                                facultiesNameString += i.Name + ",";
                                facultiesIdString += "4:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(facultiesNameString.Remove(facultiesNameString.Length - 1).Split(","), facultiesIdString.Remove(facultiesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(chatId: update.CallbackQuery.Message.Chat.Id, messageId: Int32.Parse(array[1]), text: "Выбери факультет", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                        }
                        else if (array[0] == "4")
                        {
                            var specialities = await SpecialitiesDB.GetSpecialties(array[2]);
                            var specialitiesNameString = "";
                            var specialitiesIdString = "";
                            foreach (Specialties i in specialities)
                            {
                                specialitiesNameString += i.Name + ",";
                                specialitiesIdString += "5:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(specialitiesNameString.Remove(specialitiesNameString.Length - 1).Split(","), specialitiesIdString.Remove(specialitiesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(chatId: update.CallbackQuery.Message.Chat.Id, messageId: Int32.Parse(array[1]), text: "Выбери специальность", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                        }
                        else if (array[0] == "5")
                        {
                            var timeTables = await TimeTablesDB.GetTimeTables(array[2]);
                            var timeTablesNameString = "";
                            var timeTablesIdString = "";
                            foreach (TimeTables i in timeTables)
                            {
                                timeTablesNameString += i.Group + ",";
                                timeTablesIdString += "6:" + array[1] + ":" + i.Id + ",";
                            }
                            var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(timeTablesNameString.Remove(timeTablesNameString.Length - 1).Split(","), timeTablesIdString.Remove(timeTablesIdString.Length - 1).Split(",")));
                            await botClient.EditMessageTextAsync(chatId: update.CallbackQuery.Message.Chat.Id, messageId: Int32.Parse(array[1]), text: "Выбери группу", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                        }
                        else if (array[0] == "6")
                        {
                            await TimeTablesDB.UpdateStudents(array[2], update.CallbackQuery.Message.Chat.Id);
                            await botClient.EditMessageTextAsync(chatId: update.CallbackQuery.Message.Chat.Id, messageId: Int32.Parse(array[1]), text: "Вы успешно добавлены в базу данных!", cancellationToken: cancellationToken);
                        }
                    }
                    return;
                }
                if (update.Message.Type != MessageType.Text)
                    return;
                var chatId = update.Message.Chat.Id;

                Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId} {update.Message.Chat.Username}.");

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
                        if (dayNow.DayOfWeek == 0)
                            weekNumber++;
                        foreach (Week week in timeTables.Weeks)
                        {
                            if (week.Number == weekNumber % 2)
                            {
                                string responseMessage = "*Расписание на неделю:*";
                                await botClient.SendTextMessageAsync(
                                    parseMode: ParseMode.Markdown,
                                    chatId: chatId,
                                    text: responseMessage,
                                    replyMarkup: new ReplyKeyboardRemove(),
                                    cancellationToken: cancellationToken);
                                foreach (Day day in week.Days)
                                {
                                    responseMessage = "";
                                    responseMessage += "*" + days[day.Number] + "*\n\n";
                                    lol = 0;
                                    foreach (Lesson lesson in day.Lessons)
                                    {
                                        lol++;
                                        if (lesson.Number != -16)
                                            responseMessage += "*" + lesson.Number + " пара(" + lesson.Time + ")*\n";
                                        responseMessage += lesson.Name + " - ";
                                        responseMessage += lesson.Teacher + "\nАудитория - ";
                                        if (lesson.Audience == "")
                                            responseMessage += "ДОТ\n\n";
                                        else
                                            responseMessage += lesson.Audience + "\n\n";
                                    }
                                    if (lol != 0)
                                    {
                                        await botClient.SendTextMessageAsync(parseMode: ParseMode.Markdown, chatId: chatId, text: responseMessage, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                                    }
                                }
                            }
                        }
                    }
                    else
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Вашего расписания нет в базе данных", cancellationToken: cancellationToken);
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
                                            if (lesson.Number != -16)
                                                responseMessage += "*" + lesson.Number + " пара(" + lesson.Time + ")*\n";
                                            responseMessage += lesson.Name + " - ";
                                            responseMessage += lesson.Teacher + "\nАудитория - ";
                                            if (lesson.Audience == "")
                                                responseMessage += "ДОТ\n\n";
                                            else
                                                responseMessage += lesson.Audience + "\n\n";
                                        }
                                        c = 1;
                                        await botClient.SendTextMessageAsync(parseMode: ParseMode.Markdown, chatId: chatId, text: responseMessage, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                                    }
                                }
                            }
                        }
                        if (c == 0)
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Сегодня нет пар!", cancellationToken: cancellationToken);
                    }
                    else
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Вашего расписания нет в базе данных", cancellationToken: cancellationToken);
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
                                            if (lesson.Number != -16)
                                                responseMessage += "*" + lesson.Number + " пара(" + lesson.Time + ")*\n";
                                            responseMessage += lesson.Name + " - ";
                                            responseMessage += lesson.Teacher + "\nАудитория - ";
                                            if (lesson.Audience == "")
                                                responseMessage += "ДОТ\n\n";
                                            else
                                                responseMessage += lesson.Audience + "\n\n";
                                        }
                                        c = 1;
                                        await botClient.SendTextMessageAsync(parseMode: ParseMode.Markdown, chatId: chatId, text: responseMessage, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                                    }
                                }
                            }
                        }
                        if (c == 0)
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Завтра нет пар!", cancellationToken: cancellationToken);
                    }
                    else
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Вашего расписания нет в базе данных", cancellationToken: cancellationToken);
                }
                else if (message.Text == "/setgroup" || message.Text == "/start")
                {
                    var countries = await CountriesDB.GetCounties();
                    var countriesNameString = "";
                    var countriesIdString = "";
                    Message messageKeyboard = await botClient.SendTextMessageAsync(chatId: chatId, text: "Выбери страну", cancellationToken: cancellationToken);
                    foreach (Countries i in countries)
                    {
                        countriesNameString += i.Name + ",";
                        countriesIdString += "1:" + messageKeyboard.MessageId + ":" + i.Id + ",";
                    }
                    var inlineKeyboard = new InlineKeyboardMarkup(GetInlineKeyboard(countriesNameString.Remove(countriesNameString.Length - 1).Split(","), countriesIdString.Remove(countriesIdString.Length - 1).Split(",")));
                    await botClient.EditMessageTextAsync(chatId: messageKeyboard.Chat.Id, messageId: messageKeyboard.MessageId, text: "Выбери страну", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                }
                else
                    await botClient.SendTextMessageAsync(parseMode: ParseMode.Markdown, chatId: chatId, text: "Используй команды из меню", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
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