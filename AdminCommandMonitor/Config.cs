using System.Collections.Generic;
using Exiled.API.Interfaces;

namespace AdminCommandMonitor
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        // 排除列表：格式必须是 SteamID64@steam
        public List<string> ExcludedAdmins { get; set; } = new List<string>
        {
            "76561198123456789@steam"
        };

        public float HintDuration { get; set; } = 5f;
        public int FontSize { get; set; } = 25;
    }
}