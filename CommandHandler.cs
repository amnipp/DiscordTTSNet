using Discord;
using Discord.Commands;
using Discord.Commands.Builders;
using Discord.Interactions;
using Discord.WebSocket;
using NamazuTTS.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Victoria;

namespace NamazuTTS
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly Settings _settings;
        private readonly List<User> _users;

        private readonly LavaNode _lava;
        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, Settings settings, List<User> users, LavaNode node)
        {
            _commands = commands;
            _client = client;
            _services = services;
            _settings = settings;
            _users = users;
            _lava = node;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            Console.WriteLine(messageParam);
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            /*if(message.HasCharPrefix(_settings.Prefix, ref argPos) &&
                _users.Where(x => x.DiscordID == message.Author.Id).FirstOrDefault() != null)
            {
                var user = _users.Where(x => x.DiscordID == message.Author.Id).FirstOrDefault();
                if (user.TTSEnable)
                {
                    message = new SocketUserMessage();
                }
            }*/
            User foundUser = null;
            if(_users.Count > 0)
                foundUser = _users.Where(x => x.DiscordID == message.Author.Id).First();
                    // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if ((foundUser == null && 
                !(message.HasCharPrefix(_settings.Prefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos))) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);
            if (foundUser != null && !message.HasCharPrefix(_settings.Prefix, ref argPos))
            {
                //stupid hack from a stupid guy :)
                TTSModule tts = new(_settings, _lava, _users);
                await tts.FakeTTS( message.Content);
            }
            else
                // Execute the command with the command context we just
                // created, along with the service provider for precondition checks.
                await _commands.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: _services);
        }
    }
}
