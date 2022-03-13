using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordTTS
{
    public class DiscordTTS
    {
		public static Task Main(string[] args) => new DiscordTTS().MainAsync();

        private List<User> _users;
        private Settings _settings;
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
