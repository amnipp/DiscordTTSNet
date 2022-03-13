using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace NamazuTTS.Modules
{
    public class UserModules : ModuleBase<SocketCommandContext>
    {
        private readonly List<User> _userList;
        public UserModules(List<User> userList)
        {
            _userList = userList;
        }
        [Command("AddUser", RunMode = RunMode.Async)]
        public Task AddUserAsync(SocketUser user)
        {
            _userList.Add(new User(user.Id, false, true));
            return Task.CompletedTask;
        }
    }
}
