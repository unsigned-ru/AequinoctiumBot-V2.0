﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AequinoctiumBot
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Timers;
    public class EventHandler
    {
        DiscordSocketClient _client;
        IServiceProvider _serviceProvider;
        Timer midnightTimer;

        public EventHandler(DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            _client.Log += Client_Log;
            _client.Ready += Client_Ready;
            _client.UserJoined += Client_UserJoined;
            _client.UserLeft += Client_UserLeft;
            _client.UserBanned += Client_UserBanned;
            _client.MessageReceived += Client_MessageReceived;
            _client.UserVoiceStateUpdated += OnVoiceStateUpdated;
            
            //MidnightTimer
            midnightTimer = new Timer((DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds);
            midnightTimer.AutoReset = false;
            midnightTimer.Elapsed += On_MidnightTimer;
            midnightTimer.Start();

            //VoiceRewardTimer
            Timer VoiceRewardTimer = new Timer(300000);
            VoiceRewardTimer.AutoReset = true;
            VoiceRewardTimer.Elapsed += OnVoiceRewardTimer_Elapse;
            VoiceRewardTimer.Start();
        }

        private void OnVoiceRewardTimer_Elapse(object sender, ElapsedEventArgs e)
        {
            Program.LogConsole("VoiceRewardTimer", ConsoleColor.Magenta, $"Elapsed -- {DateTime.Now}");
            foreach (IGuildUser user in Program.guild.Users)
            {
                if (user.VoiceChannel != null && user.VoiceChannel.Id != 601175267817291822)
                {
                    UserDataService.GrantExp(2, user,false,false);
                    UserDataService.GrantDrak(0.5f,user,true);
                }
            }
        }

        private Task OnVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            if (!UserDataService.HasGottenFirstConnectionToVoiceOfDay(arg1) && arg3.VoiceChannel != null && arg3.VoiceChannel.Id != 601175267817291822)
            {
                UserDataService.GrantExp(20, arg1, false, true);
                UserDataService.GrantDrak(5, arg1);
            }
            return Task.CompletedTask;
        }

        private void On_MidnightTimer(object sender, ElapsedEventArgs e)
        {
            UserDataService.On_MidnightTimer();
            UserDataService.BackupUserData();
            GiveAwayService.BackupGiveAways();
            CleanBackups();

            midnightTimer = new Timer((DateTime.Today.AddDays(1) - DateTime.Now).TotalMilliseconds);
            midnightTimer.AutoReset = false;
            midnightTimer.Elapsed += On_MidnightTimer;
        }

        private Task Client_Log(LogMessage arg)
        {
            Program.LogConsole("CLIENT LOG", ConsoleColor.Red, arg.Message);
            if (arg.Exception != null) { Console.WriteLine(arg.Exception); }
            return Task.CompletedTask;
        }

        public Task Client_Ready()
        {
            var wowService = _serviceProvider.GetRequiredService<WoWService>();
            var userDataService = _serviceProvider.GetRequiredService<UserDataService>();
            UserDataService.LoadUserData();
            userDataService.InitializeRanks(_client);
            wowService.Initialize();
            GiveAwayService.LoadGiveAways();
            return Task.CompletedTask;
        }

        private Task Client_UserBanned(SocketUser arg1, SocketGuild arg2)
        {
            Program.LogConsole("ADMINISTRATION", ConsoleColor.Red, $"User {arg1.Username} has been banned.");

            UserDataService.On_UserLeft(arg1 as SocketGuildUser);

            return Task.CompletedTask;
        }

        private Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Channel as IDMChannel != null && arg.Author.IsBot == false)
            {
                Program.LogConsole("MESSAGELOG", ConsoleColor.Yellow, "Messaged by user: " + arg.Author.Username + " \n" +
                "Message: \n" + arg.Content);
            }
            else
            {
                if (!UserDataService.HasGottenFirstMessageOfTheDay(arg.Author))
                {
                    UserDataService.GrantExp(15, arg.Author,true);
                    UserDataService.GrantDrak(2.5f, arg.Author);
                }
            }
            return Task.CompletedTask;
        }

        public Task Client_UserJoined(SocketGuildUser user)
        {
            if (user.Nickname != null)
            {
                Program.LogConsole("JOINLEAVEHANDLE", ConsoleColor.Cyan, $"User: {user.Nickname} has joined the Server.");
            } 
            else
            {
                Program.LogConsole("JOINLEAVEHANDLE", ConsoleColor.Cyan, $"User: {user.Username} has joined the Server.");
            }
            UserDataService.On_UserJoined(user);
            return Task.CompletedTask;
        }

        public Task Client_UserLeft(SocketGuildUser user)
        {
            if (user.Nickname != null)
                Program.LogConsole("JOINLEAVEHANDLE", ConsoleColor.Cyan, $"User: {user.Nickname} has left the Server.");
            else
                Program.LogConsole("JOINLEAVEHANDLE", ConsoleColor.Cyan, $"User: {user.Username} has left the Server.");

            UserDataService.On_UserLeft(user);

            return Task.CompletedTask;
        }

        Task CleanBackups()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"/Backups/UserDataBackups/UserDataBackup {DateTime.Now.AddDays(-7).ToString("dd-MM-yy")}.xml"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + $"/Backups/UserDataBackups/UserDataBackup {DateTime.Now.AddDays(-7).ToString("dd-MM-yy")}.xml");
            }
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"/Backups/GiveAwayBackups/GiveAwayBackup {DateTime.Now.AddDays(-7).ToString("dd-MM-yy")}.xml"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + $"/Backups/GiveAwayBackups/GiveAwayBackup {DateTime.Now.AddDays(-7).ToString("dd-MM-yy")}.xml");
            }
            return Task.CompletedTask;
        }
    }
}
