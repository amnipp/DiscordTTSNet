using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordTTS.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordTTS
{
    public class DiscordBot
    {
        private readonly DiscordSocketClient _client;
        private readonly Settings _settings;
        private readonly List<User> _users;
        private readonly CommandService _commandService;
        private readonly CommandHandler _commandHandler;

        public DiscordBot(Settings savedSettings, List<User> userList)
        {
            _client = new DiscordSocketClient();
            _settings = savedSettings;
            _users = userList;
            _commandService = new CommandService();
            _commandHandler = new CommandHandler(_client, _commandService, _settings);
        }
        ~DiscordBot()
        {
            SaveSettings();
        }
        public async Task CreateBotAsync(string token)
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await _commandHandler.InstallCommandsAsync();
            _client.Log += LogAsync;
            _client.Ready += Ready;
        }

        private Task LogAsync(LogMessage msg)
        {
            if (msg.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{msg.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{msg.Severity}] {msg}");

            return Task.CompletedTask;
        }
        private Task Ready()
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        }

        private Task SaveSettings()
        {
            var savedSettings = JsonSerializer.Serialize(_settings);
            var savedUsers = JsonSerializer.Serialize(_users);

            using(StreamWriter settingsFile = new StreamWriter("config.json"))
                settingsFile.Write(savedSettings);
            using(StreamWriter userFile = new StreamWriter("config.json"))
                userFile.Write(savedUsers);

            return Task.CompletedTask;
        }

    }
}
