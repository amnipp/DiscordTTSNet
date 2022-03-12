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

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            // Read in JSON config
            StreamReader r = new StreamReader("config.json");
            string jsonString = r.ReadToEnd();
            // TODO: store these into settings 
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            var token = config["token"];

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
	}
}
