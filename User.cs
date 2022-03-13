using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamazuTTS
{
    public class User
    {
        public ulong DiscordID { get; set; }
        public bool IsAdmin { get; set; }
        public bool TTSEnable { get; set; }
        public User(ulong id, bool admin, bool ttsEnable)
        {
            DiscordID = id;
            IsAdmin = admin;
            TTSEnable = ttsEnable;
        }
    }
}
