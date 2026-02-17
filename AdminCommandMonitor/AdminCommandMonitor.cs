using Exiled.API.Features;
using System;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player; // 🔥 Player事件命名空间

namespace AdminCommandMonitor
{
    public class AdminCommandMonitor : Plugin<Config>
    {
        public override string Name => "Admin Command Monitor";
        public override string Author => "无牙";
        public override string Prefix => "acm";
        public override PluginPriority Priority => PluginPriority.Medium;

        public override Version Version { get; } = new Version(1, 0, 4);
        public override Version RequiredExiledVersion { get; } = new Version(8, 9, 11);

        private EventHandlers _handlers;

        public override void OnEnabled()
        {
            try
            {
                _handlers = new EventHandlers(this);

                // 🔥 8.9.11在Player类中注册：SendingValidCommand
                Exiled.Events.Handlers.Player.SendingValidCommand += _handlers.OnSendingValidCommand;

                Log.Info($"✅ 管理行为监督插件已启用 v{Version} (EXILED 8.9.11 Player.SendingValidCommand)");

                base.OnEnabled();
            }
            catch (Exception ex)
            {
                Log.Error($"启用插件失败: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void OnDisabled()
        {
            try
            {
                if (_handlers != null)
                {
                    Exiled.Events.Handlers.Player.SendingValidCommand -= _handlers.OnSendingValidCommand;
                }

                Log.Info("❌ 管理行为监督插件已禁用");

                base.OnDisabled();
            }
            catch (Exception ex)
            {
                Log.Error($"禁用插件失败: {ex.Message}");
            }
        }
    }
}