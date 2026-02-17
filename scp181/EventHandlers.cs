using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using Exiled.Events.EventArgs.Warhead;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
// 删除：using Random = UnityEngine.Random;  不再使用

namespace SCP181
{
    public class EventHandlers
    {
        // 核弹是否正在倒计时
        private bool isWarheadInProgress = false;

        public void RoundStarte()
        {
            // 重置核弹状态
            isWarheadInProgress = false;

            // 1. 获取D级人员列表
            var dClassPlayers = Exiled.API.Features.Player.List
                .Where(p => p.Role.Type == RoleTypeId.ClassD)
                .ToList();

            // 2. 获取科学家列表
            var scientistPlayers = Exiled.API.Features.Player.List
                .Where(p => p.Role.Type == RoleTypeId.Scientist)
                .ToList();

            // 3. 检查D级人员（必须有D级才能继续）
            if (dClassPlayers.Count == 0)
            {
                Log.Info("没有找到D级人员，跳过SCP181刷新");
                return;
            }

            // 4. 从D级中随机选第一个SCP181（使用基于时间的真随机）
            Log.Info($"正在从{dClassPlayers.Count}名D级人员中刷新SCP181");
            Player dClassPlayer = dClassPlayers[RandomUtils.Range(0, dClassPlayers.Count)];
            Roles.Scp181(dClassPlayer);
            Log.Info($"{dClassPlayer.Nickname} 被选为D级人员中的SCP181");

            // 5. 处理科学家分配（181和懂王）
            if (scientistPlayers.Count == 1)
            {
                // 只有一个科学家，50%概率181，50%概率懂王
                Log.Info("只有1名科学家，50%概率SCP-181，50%概率懂王");
                Player scientist = scientistPlayers[0];

                if (RandomUtils.RollPercent(50))
                {
                    Roles.Scp181(scientist);
                    Log.Info($"{scientist.Nickname} 被选为科学家中的SCP181(50%概率命中)");
                }
                else
                {
                    Roles.Trump(scientist);
                    Log.Info($"{scientist.Nickname} 被选为懂王(50%概率命中)");
                }
            }
            else if (scientistPlayers.Count == 2)
            {
                // 两个科学家，一个181，另一个50%概率变懂王
                Log.Info("有2名科学家，分配SCP-181和懂王");

                // 第一个选为181（使用基于时间的真随机）
                Player scientist181 = scientistPlayers[RandomUtils.Range(0, scientistPlayers.Count)];
                Roles.Scp181(scientist181);
                Log.Info($"{scientist181.Nickname} 被选为科学家中的SCP181");

                // 另一个50%概率变懂王
                Player otherScientist = scientistPlayers.First(p => p != scientist181);
                if (RandomUtils.RollPercent(50))
                {
                    Roles.Trump(otherScientist);
                    Log.Info($"{otherScientist.Nickname} 被选为懂王(50%概率命中)");
                }
                else
                {
                    Log.Info($"{otherScientist.Nickname} 运气不好，没有成为懂王");
                }
            }
            else if (scientistPlayers.Count > 2)
            {
                // 两个以上科学家，一个181，剩余中选一个懂王
                Log.Info($"有{scientistPlayers.Count}名科学家，分配SCP-181和懂王");

                // 选181（使用基于时间的真随机）
                Player scientist181 = scientistPlayers[RandomUtils.Range(0, scientistPlayers.Count)];
                Roles.Scp181(scientist181);
                Log.Info($"{scientist181.Nickname} 被选为科学家中的SCP181");

                // 从剩余中选懂王（使用基于时间的真随机）
                var remainingScientists = scientistPlayers.Where(p => p != scientist181).ToList();
                Player scientistTrump = remainingScientists[RandomUtils.Range(0, remainingScientists.Count)];
                Roles.Trump(scientistTrump);
                Log.Info($"{scientistTrump.Nickname} 被选为懂王");
            }
            else
            {
                Log.Info("没有找到科学家，只刷新了一个SCP181");
            }
        }

        // 核弹启动事件
        public void OnWarheadStarting(StartingEventArgs e)
        {
            isWarheadInProgress = true;
            Log.Info("核弹已启动，SCP-181禁止所有开门操作");
        }

        // 核弹停止事件
        public void OnWarheadStopping(StoppingEventArgs e)
        {
            if (e.IsAllowed)
            {
                isWarheadInProgress = false;
                Log.Info("核弹已停止，SCP-181可以开门操作");
            }
        }

        public void hurting(HurtingEventArgs e)
        {
            if (e.Player != null)
            {
                // 处理SCP-181的免伤（使用基于时间的真随机）
                if (Roles.scp181.Contains(e.Player))
                {
                    if (RandomUtils.RollPercent(35))
                    {
                        e.Player.ShowHint("<color=red>你躲避了一次伤害</color>", 2f);
                        e.IsAllowed = false;
                    }
                    else
                    {
                        e.IsAllowed = true;
                    }
                    return;
                }

                // 处理懂王的SCP伤害减免
                if (Roles.trump.Contains(e.Player) && e.Attacker != null)
                {
                    // 检查攻击者是否是SCP
                    RoleTypeId attackerRole = e.Attacker.Role.Type;
                    float customDamage = -1f;

                    switch (attackerRole)
                    {
                        case RoleTypeId.Scp049:
                            customDamage = 10f;
                            break;
                        case RoleTypeId.Scp106:
                            customDamage = 30f;
                            break;
                        case RoleTypeId.Scp096:
                            customDamage = 20f;
                            break;
                        case RoleTypeId.Scp173:
                            customDamage = 50f;
                            break;
                        case RoleTypeId.Scp0492:
                            customDamage = 5f;
                            break;
                        case RoleTypeId.Scp939:
                            customDamage = 15f;
                            break;
                    }

                    // 如果是SCP攻击，取消原伤害并施加自定义伤害
                    if (customDamage > 0)
                    {
                        e.IsAllowed = false; // 取消原始伤害

                        // 延迟一帧施加自定义伤害，确保取消生效
                        Timing.CallDelayed(0.1f, () =>
                        {
                            if (e.Player != null && e.Player.IsAlive)
                            {
                                e.Player.Health -= customDamage;
                                e.Player.ShowHint($"<color=yellow>懂王减免：受到 {(int)customDamage} 点伤害</color>", 2f);

                                // 判断血量是否小于等于0，如果是则杀死玩家
                                if (e.Player.Health <= 0)
                                {
                                    // 根据攻击者类型设置死亡原因字符串
                                    string deathReason;
                                    switch (attackerRole)
                                    {
                                        case RoleTypeId.Scp049:
                                            deathReason = "被 SCP-049 杀死了（懂王减免后）";
                                            break;
                                        case RoleTypeId.Scp106:
                                            deathReason = "被 SCP-106 杀死了（懂王减免后）";
                                            break;
                                        case RoleTypeId.Scp096:
                                            deathReason = "被 SCP-096 杀死了（懂王减免后）";
                                            break;
                                        case RoleTypeId.Scp173:
                                            deathReason = "被 SCP-173 杀死了（懂王减免后）";
                                            break;
                                        case RoleTypeId.Scp0492:
                                            deathReason = "被 SCP-049-2 杀死了（懂王减免后）";
                                            break;
                                        case RoleTypeId.Scp939:
                                            deathReason = "被 SCP-939 杀死了（懂王减免后）";
                                            break;
                                        default:
                                            deathReason = "被 SCP 杀死了（懂王减免后）";
                                            break;
                                    }

                                    e.Player.Kill(deathReason);
                                }
                            }
                        });

                        // SCP-049特殊处理：清除心脏骤停效果
                        if (attackerRole == RoleTypeId.Scp049)
                        {
                            Timing.CallDelayed(0.2f, () =>
                            {
                                if (e.Player != null && e.Player.IsAlive)
                                {
                                    // 清除049的心脏骤停效果
                                    e.Player.DisableEffect(EffectType.CardiacArrest);
                                }
                            });
                        }
                    }
                }
            }
        }

        public void OnDoor(InteractingDoorEventArgs e)
        {
            if (e.Player != null && Roles.scp181.Contains(e.Player))
            {
                // 检查是否是撤离点门（EscapePrimary 或 EscapeSecondary）
                // 181永远无法操作撤离点门
                if (e.Door.Type == DoorType.EscapeFinal)
                {
                    e.Player.ShowHint("<color=red>你无法操作撤离点门</color>", 2f);
                    e.IsAllowed = false;
                    return;
                }

                // 检查是否是SCP-079相关的门（181禁止开启079的门）
                if (e.Door.Type == DoorType.Scp079First ||
                    e.Door.Type == DoorType.Scp079Second ||
                    e.Door.Type == DoorType.Scp079Armory)
                {
                    e.Player.ShowHint("<color=red>你无法操作SCP-079的门</color>", 2f);
                    e.IsAllowed = false;
                    return;
                }

                // 核弹开启后，181完全禁止开门操作
                if (isWarheadInProgress || Warhead.IsInProgress || Warhead.IsDetonated)
                {
                    e.Player.ShowHint("<color=red>核弹启动中，无法操作任何门</color>", 2f);
                    e.IsAllowed = false;
                    return;
                }

                // 允许撬开任何门（包括上锁的门）
                if (!e.IsAllowed)
                {
                    // 100%概率撬门
                    e.IsAllowed = true;
                    e.Player.ShowHint("你撬开了一扇门", 2f);
                }
            }
        }

        public void Ublood(UsingItemEventArgs e)
        {
            if (e.Player != null && e.Item != null)
            {
                if (Roles.scp181.Contains(e.Player) || Roles.trump.Contains(e.Player))
                {
                    if (e.Item.Type == ItemType.Medkit)
                    {
                        // 使用基于时间的真随机，1%概率
                        if (RandomUtils.RollPercent(1))
                        {
                            Timing.CallDelayed(4f, () =>
                            {
                                e.IsAllowed = true;
                                e.Player.ShowHint("回满了状态", 3f);
                                e.Player.Heal(e.Player.MaxHealth);
                                e.Player.HumeShield = 100;
                            });
                        }
                        else
                        {
                            e.IsAllowed = true;
                        }
                    }
                }
            }
        }

        public void Dy(DyingEventArgs e)
        {
            if (e.Player != null)
            {
                if (Roles.scp181.Contains(e.Player))
                {
                    Roles.scp181.Remove(e.Player);
                    e.Player.RankName = null;
                    e.Player.RankColor = null;
                }

                if (Roles.trump.Contains(e.Player))
                {
                    Roles.trump.Remove(e.Player);
                    e.Player.RankName = null;
                    e.Player.RankColor = null;
                }
            }
        }

        public void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs e)
        {
            // 懂王免疫106的口袋维度传送
            if (Roles.trump.Contains(e.Player))
            {
                e.IsAllowed = false;
            }
        }
    }
}
