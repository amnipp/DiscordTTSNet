using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTTS
{
    public class User
    {
        private ulong _id;
        public ulong ID { get { return _id; } }
        private bool _isTTSActive = false;
        public bool IsTTSActive { get { return _isTTSActive; }}

        public User(string id, bool activeTTS)
        {
            bool idParsed = ulong.TryParse(id, out _id);
            if (!idParsed) _id = 0;
            _isTTSActive = activeTTS;
        }
        //TODO more user settings
        public async Task<IUser> GetDiscordUser(DiscordSocketClient client)
        {
            return await client.GetUserAsync(_id);
        }
    }
}
