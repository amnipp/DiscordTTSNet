using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text.Json;
using DiscordTTS.Commands;
using Discord.Interactions;

namespace DiscordTTS
{
    public class DiscordTTS
    {
        private IConfiguration _config;
        public static Task Main(string[] args) => new DiscordTTS().MainAsync();

        private List<User> _users;
        private Settings _settings;

        public void BuildServices()
        {
            var _builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(path: "config.json");

            // build the configuration and assign to _config          
            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            LoadSavedSettings();
            string jsonString = "";
            // Read in JSON config
            using (StreamReader r = new StreamReader("config.json"))
                jsonString = r.ReadToEnd();
            // TODO: store these into settings 
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            var token = config["token"];
             DiscordBot bot = new DiscordBot(_settings, _users);
             await bot.CreateBotAsync(token);




            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        // TODO: Load settings from file
        private void LoadSavedSettings()
        {
            _settings = new Settings('-');
            _users = new List<User>();
            //temp: create a user in code
            _users.Add(new User("164948299265081344", true));
        }
    }
}
