using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTTS
{
    public class Settings
    {
        private char _chatPrefix = '-';
        public char ChatPrefix { get { return _chatPrefix; } set { _chatPrefix = value; } }
        public Settings(char chatPrefix)
        {
            _chatPrefix = chatPrefix;
        }
    }
}
