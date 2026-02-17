using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace AdminCommandMonitor
{
    public class EventHandlers
    {
        private readonly AdminCommandMonitor _plugin;

        public EventHandlers(AdminCommandMonitor plugin)
        {
            _plugin = plugin;
        }

        public void OnSendingValidCommand(SendingValidCommandEventArgs ev)
        {
            try
            {
                // 基础检查
                if (ev.Player == null || string.IsNullOrEmpty(ev.Player.UserId))
                    return;

                if (!ev.Player.RemoteAdminAccess)
                    return;

                // 检查排除列表
                if (IsExcludedAdmin(ev.Player.UserId))
                {
                    if (_plugin.Config.Debug)
                        Log.Debug($"管理员 {ev.Player.Nickname} 在排除列表中，跳过广播");
                    return;
                }

                // 🔥 修复点1：使用 Query 属性获取完整命令（而非 Name + Arguments）
                // EXILED 8.9.11 的 SendingValidCommandEventArgs 通常只有 Query 属性
                string fullCommand = ev.Query ?? "未知命令";
                string adminName = ev.Player.Nickname ?? "未知管理员";

                // 提取命令名（Query 的第一个词）
                string commandName = fullCommand.Split(' ')[0];

                // 创建带颜色的提示文字
                string hintMessage = BuildColoredHint(adminName, commandName);

                // 广播给所有在线玩家
                BroadcastToAllPlayers(hintMessage);

                // 记录到日志
                LogCommand(ev.Player, fullCommand);
            }
            catch (Exception ex)
            {
                Log.Error($"处理命令监听时出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private string BuildColoredHint(string adminName, string command)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<color=red>[管理行为监督]</color> ");
            sb.Append($"<color=yellow>{adminName}</color> ");
            sb.Append($"<color=white>使用了</color> ");
            sb.Append($"<color=orange>{command}</color> ");
            sb.Append($"<color=white>命令</color>");
            return sb.ToString();
        }

        private void BroadcastToAllPlayers(string message)
        {
            try
            {
                var players = Player.List.Where(p =>
                    p != null &&
                    !string.IsNullOrEmpty(p.UserId) &&
                    !(p is Exiled.API.Features.Npc)
                ).ToList();

                if (!players.Any())
                    return;

                foreach (var player in players)
                {
                    try
                    {
                        // 🔥 修复点2：简化 ShowHint 调用，避免 HintEffect 依赖
                        player.ShowHint(
                            $"<size={_plugin.Config.FontSize}>{message}</size>",
                            _plugin.Config.HintDuration
                        );
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"向玩家 {player.Nickname} 发送提示失败: {ex.Message}");
                    }
                }

                if (_plugin.Config.Debug)
                    Log.Debug($"成功广播给 {players.Count} 名玩家");
            }
            catch (Exception ex)
            {
                Log.Error($"广播过程出错: {ex.Message}");
            }
        }

        private bool IsExcludedAdmin(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var excludedList = _plugin.Config.ExcludedAdmins ?? new List<string>();

            return excludedList.Any(excluded =>
                excluded?.Equals(userId, StringComparison.OrdinalIgnoreCase) == true
            );
        }

        private void LogCommand(Player admin, string command)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logsFolder = Path.Combine(Exiled.API.Features.Paths.Exiled, "Logs");

                if (!Directory.Exists(logsFolder))
                    Directory.CreateDirectory(logsFolder);

                string logPath = Path.Combine(logsFolder, "admin_commands.log");
                string logEntry = $"[{timestamp}] {admin.Nickname}({admin.UserId}) -> {command}\n";

                File.AppendAllText(logPath, logEntry);
                Log.Info($"🛡️ 管理员命令: {admin.Nickname} 执行了 {command}");
            }
            catch (Exception ex)
            {
                Log.Error($"写入日志失败: {ex.Message}");
            }
        }
    }
}