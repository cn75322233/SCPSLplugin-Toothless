using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCP181
{
    public class Config : IConfig
    {
        [Description("是否开启SCP181")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
    }
}
