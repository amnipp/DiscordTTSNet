using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.Responses.Search;

namespace NamazuTTS.Modules
{
    public class PlaySoundModule : ModuleBase<SocketCommandContext>
    {
        private readonly Settings _settings;
        private readonly LavaNode _lavaNode;
        public PlaySoundModule(Settings settings, LavaNode lavaNode)
        {
            _settings = settings;
            _lavaNode = lavaNode;
        }
        [Command("Join")]
        public async Task JoinAsync()
        {
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
                _settings.CurrentGuild = Context.Guild;
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Leave")]
        public async Task LeaveAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to any voice channels!");
                return;
            }

            var voiceChannel = (Context.User as IVoiceState)?.VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null)
            {
                await ReplyAsync("Not sure which voice channel to disconnect from.");
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);
                await ReplyAsync($"I've left {voiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }
        [Command("Play")]
        public async Task PlayAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }
            if(!name.Contains("http"))
                name = "sounds/" + name + ".mp3";
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }
            var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct, name);
            if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{name}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                player.Queue.Enqueue(searchResponse.Tracks);
                await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} songs.");
            }
            else
            {
                var track = searchResponse.Tracks.FirstOrDefault();
                player.Queue.Enqueue(track);

                await ReplyAsync($"Enqueued {track?.Title}");
            }

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

        [Command("Skip")]
        public async Task SkipAsync()
        {
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }
            var player = _lavaNode.GetPlayer(Context.Guild);
            if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
            {
                await player.StopAsync();
            }
        }

        /*
        [Command("play", RunMode = RunMode.Async)]
        public async Task Play(string soundName)
        {
            string sound = _settings.SoundPath + soundName + ".mp3";
            if (!File.Exists(sound) ||
                _settings.AudioClient == null)
                return;
            await SendAsync(_settings.AudioClient, sound);
        }

        private async Task SendAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                catch(Exception ex) { Console.WriteLine(ex.ToString()); }
                finally { await discord.FlushAsync(); }
            }
        }
        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel info -i \"{path}\" -ac 2 -f s16le -ar 44100 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }*/
    }
}
