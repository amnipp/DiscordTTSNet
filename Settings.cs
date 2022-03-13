using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuTTS
{
    public class Settings
    {
        public IAudioClient AudioClient { get; set; }
        public char Prefix { get; set; } = '-';
        public string SoundPath { get; set; } = "sounds/";
        // A stupid hack from a stupid guy
        public SocketGuild CurrentGuild { get; set; }
    }
}
