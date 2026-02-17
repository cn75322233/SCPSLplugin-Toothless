using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCP181
{
    public class Roles
    {
        public static List<Player> scp181 = new List<Player>();
        public static List<Player> trump = new List<Player>(); // 懂王列表

        public static void Scp181(Player player)
        {
            scp181.Add(player);

            // 保存原始角色类型
            var originalRole = player.Role.Type;

            // 删除了 player.Role.Set(PlayerRoles.RoleTypeId.ClassD); 
            // 现在玩家保持原角色外观（科学家还是科学家，D级还是D级）

            // 根据原始角色显示不同的提示
            string roleDisplay = originalRole == RoleTypeId.Scientist ? "科学家" : "D级人员";
            player.ShowHint($"\n\n<color=orange>你作为{roleDisplay}被选中为SCP-181！</color>\n" +
                            $"<color=red>特性：</color>\n" +
                            $"<color=orange>- 35%概率免伤</color>\n" +
                            $"<color=orange>- 100%概率撬门</color>\n" +
                            $"<color=orange>- 使用医疗包1%概率回满血+100蓝盾</color>", 20f);

            player.MaxHealth = 150;
            player.Health = 150;
            player.RankName = "SCP-181";
            player.RankColor = "orange";
        }

        public static void Trump(Player player)
        {
            trump.Add(player);

            // 保存原始角色类型
            var originalRole = player.Role.Type;
            string roleDisplay = originalRole == RoleTypeId.Scientist ? "科学家" : "D级人员";

            player.ShowHint($"\n\n<color=yellow>你是懂王(Trump)！</color>\n" +
                            $"<color=red>你非常的懂SCP，SCP对你的伤害全部减弱！</color>\n" +
                            $"<color=yellow>伤害减免：</color>\n" +
                            $"<color=orange>- SCP-049: 10点伤害</color>\n" +
                            $"<color=orange>- SCP-106: 30点伤害</color>\n" +
                            $"<color=orange>- SCP-096: 20点伤害</color>\n" +
                            $"<color=orange>- SCP-173: 50点伤害</color>\n" +
                            $"<color=orange>- SCP-049-2: 6点伤害</color>\n" +
                            $"<color=orange>- SCP-939: 15点伤害</color>", 20f);

            player.MaxHealth = 150;
            player.Health = 150;
            player.RankName = "懂王";
            player.RankColor = "yellow";
        }
    }
}
