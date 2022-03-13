using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace NamazuTTS.Modules
{
    public class TTSModule : ModuleBase<SocketCommandContext>
    {
        private Settings _settings;
        private readonly LavaNode _lavaNode;
        private readonly List<User> _userList;
        public TTSModule(Settings settings, LavaNode lavaNode, List<User> userList)
        {
            _settings = settings;
            _lavaNode = lavaNode;
            _userList = userList;
        }
        [Command("tts-s", RunMode = RunMode.Async)]
        public async Task TTSStream(string message)
        {
/*            TTSService service = new TTSService();
            using (var stream = service.CreateSpeechToStream(message))
            {
                var player = _lavaNode.GetPlayer(Context.Guild);
                
                await SendAsync();
            }*/
        }
        private async Task SendAsync(IAudioClient client, string path, Stream stream)
        {
            // Create FFmpeg using the previous example
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await stream.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }
        public async Task FakeTTS(string message)
        {
            TTSService ttsService = new TTSService();
            string file = _settings.SoundPath + "tts.mp3";
            var fileCreated = await ttsService.CreateSpeechToFile(file, message);
            if (fileCreated)
            {
                if (string.IsNullOrWhiteSpace(file))
                {
                    await ReplyAsync("Please provide search terms.");
                    return;
                }
                if (!_lavaNode.HasPlayer(_settings.CurrentGuild))
                {
                    await ReplyAsync("I'm not connected to a voice channel.");
                    return;
                }
                var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, file);
                if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
                {
                    await ReplyAsync($"I wasn't able to find anything for `{file}`.");
                    return;
                }

                var player = _lavaNode.GetPlayer(_settings.CurrentGuild);
                var track = searchResponse.Tracks.FirstOrDefault();
                player.Queue.Enqueue(track);

               // await ReplyAsync($"Enqueued {track?.Title}");


                if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
                {
                    return;
                }

                player.Queue.TryDequeue(out var lavaTrack);
                await player.PlayAsync(x => {
                    x.Track = lavaTrack;
                    x.ShouldPause = false;
                });
            }
        }

        [Command("tts", RunMode = RunMode.Async)]
        public async Task TTS(string message)
        {
            TTSService ttsService = new TTSService();
            string file = _settings.SoundPath + "tts.mp3";
            var fileCreated = await ttsService.CreateSpeechToFile(file, message);
            if(fileCreated)
            {
                if (string.IsNullOrWhiteSpace(file))
                {
                    await ReplyAsync("Please provide search terms.");
                    return;
                }
                if (!_lavaNode.HasPlayer(Context.Guild))
                {
                    await ReplyAsync("I'm not connected to a voice channel.");
                    return;
                }
                var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, file);
                if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
                {
                    await ReplyAsync($"I wasn't able to find anything for `{file}`.");
                    return;
                }

                var player = _lavaNode.GetPlayer(Context.Guild);
                var track = searchResponse.Tracks.FirstOrDefault();
                player.Queue.Enqueue(track);

                await ReplyAsync($"Enqueued {track?.Title}");
            

                if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
                {
                    return;
                }

                player.Queue.TryDequeue(out var lavaTrack);
                await player.PlayAsync(x => {
                    x.Track = lavaTrack;
                    x.ShouldPause = false;
                });
            }
        }
    }
}
