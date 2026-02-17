using Exiled.API.Features;
using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace SCP181
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "FishCuuuuuuuare";
        private EventHandlers eventHandlers;

        // 添加静态实例（方便命令调用）
        public static Plugin Instance { get; private set; }

        public override void OnEnabled()
        {
            base.OnEnabled();
            Log.Info("181特殊角色已启动！");
            Log.Info("懂王(Trump)系统已加载！");

            // 设置静态实例
            Instance = this;

            eventHandlers = new EventHandlers();
            Exiled.Events.Handlers.Server.RoundStarted += eventHandlers.RoundStarte;
            Exiled.Events.Handlers.Player.UsingItem += eventHandlers.Ublood;
            Exiled.Events.Handlers.Player.Hurting += eventHandlers.hurting;
            Exiled.Events.Handlers.Player.InteractingDoor += eventHandlers.OnDoor;
            Exiled.Events.Handlers.Player.Dying += eventHandlers.Dy;
            Exiled.Events.Handlers.Player.EnteringPocketDimension += eventHandlers.OnEnteringPocketDimension;

            // 订阅核弹事件
            Exiled.Events.Handlers.Warhead.Starting += eventHandlers.OnWarheadStarting;
            Exiled.Events.Handlers.Warhead.Stopping += eventHandlers.OnWarheadStopping;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            Exiled.Events.Handlers.Server.RoundStarted -= eventHandlers.RoundStarte;
            Exiled.Events.Handlers.Player.UsingItem -= eventHandlers.Ublood;
            Exiled.Events.Handlers.Player.Hurting -= eventHandlers.hurting;
            Exiled.Events.Handlers.Player.InteractingDoor -= eventHandlers.OnDoor;
            Exiled.Events.Handlers.Player.Dying -= eventHandlers.Dy;
            Exiled.Events.Handlers.Player.EnteringPocketDimension -= eventHandlers.OnEnteringPocketDimension;

            // 取消订阅核弹事件
            Exiled.Events.Handlers.Warhead.Starting -= eventHandlers.OnWarheadStarting;
            Exiled.Events.Handlers.Warhead.Stopping -= eventHandlers.OnWarheadStopping;

            eventHandlers = null;

            // 清理静态实例
            Instance = null;
        }

        // 添加命令处理器类（嵌套在Plugin类内部）
        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class SpawnScp181Command : ICommand
        {
            public string Command => "spawns181";
            public string[] Aliases => new string[0];
            public string Description => "将指定玩家生成为SCP-181（支持D级人员、科学家或任何角色）";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                // 检查参数数量
                if (arguments.Count < 1)
                {
                    response = "用法: spawns181 <玩家ID或昵称>";
                    return false;
                }

                // 获取目标玩家
                string target = arguments.Array[arguments.Offset + 0];
                Player player = Player.Get(target);

                if (player == null)
                {
                    response = $"❌ 找不到玩家: {target}";
                    return false;
                }

                if (!player.IsAlive)
                {
                    response = $"❌ 玩家 {player.Nickname} 已死亡，无法生成SCP-181";
                    return false;
                }

                try
                {
                    // 调用角色生成方法
                    Roles.Scp181(player);
                    response = $"✅ 成功将 {player.Nickname} 生成为SCP-181！";
                    return true;
                }
                catch (Exception ex)
                {
                    response = $"❌ 生成失败: {ex.Message}";
                    return false;
                }
            }
        }

        // 新增懂王命令
        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class SpawnTrumpCommand : ICommand
        {
            public string Command => "spawntrump";
            public string[] Aliases => new string[] { "spawn懂王" };
            public string Description => "将指定玩家生成为懂王(Trump) - 对SCP伤害有减免";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                // 检查参数数量
                if (arguments.Count < 1)
                {
                    response = "用法: spawntrump <玩家ID或昵称>";
                    return false;
                }

                // 获取目标玩家
                string target = arguments.Array[arguments.Offset + 0];
                Player player = Player.Get(target);

                if (player == null)
                {
                    response = $"❌ 找不到玩家: {target}";
                    return false;
                }

                if (!player.IsAlive)
                {
                    response = $"❌ 玩家 {player.Nickname} 已死亡，无法生成懂王";
                    return false;
                }

                try
                {
                    // 调用懂王生成方法
                    Roles.Trump(player);
                    response = $"✅ 成功将 {player.Nickname} 生成为懂王(Trump)！\n" +
                               $"特性：SCP-049(20伤) SCP-106(40伤) SCP-096(25伤) SCP-173(50伤) SCP-049-2(10伤) SCP-939(30伤)";
                    return true;
                }
                catch (Exception ex)
                {
                    response = $"❌ 生成失败: {ex.Message}";
                    return false;
                }
            }
        }
    }
}
