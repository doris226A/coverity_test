
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Cabalist
{
    /// <summary>
    /// 刻印（刻印）
    /// </summary>
    public class ChopMark : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actorsInRange = map.GetActorsArea(PosX8to16(args.x, map.Width), PosY8to16(args.y, map.Height), 50, true);
            foreach (Actor actorInRange in actorsInRange)
            {
                if (actorInRange != sActor && SkillHandler.Instance.CheckValidAttackTarget(sActor, actorInRange))
                {
                    return -33;
                }
            }
            if (sActor.Slave.Count >= 1)
            {
                foreach (Actor i in sActor.Slave)
                {
                    if (i.X == PosX8to16(args.x, map.Width) && i.Y == PosY8to16(args.y, map.Height))
                    {
                        return -17;
                    }
                }
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            if (sActor.Slave.Count >= 5)
            {
                map.DeleteActor(sActor.Slave[0]);
                sActor.Slave.Remove(sActor.Slave[0]);
            }
            //建立設置型技能實體
            ActorSkill actor = new ActorSkill(args.skill, sActor);

            //設定技能位置
            actor.MapID = dActor.MapID;
            actor.X = SagaLib.Global.PosX8to16(args.x, map.Width);
            actor.Y = SagaLib.Global.PosY8to16(args.y, map.Height);
            //設定技能的事件處理器，由於技能體不需要得到消息廣播，因此建立空處理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地圖註冊技能Actor
            map.RegisterActor(actor);
            //設置Actor隱身屬性為False
            actor.invisble = false;
            //廣播隱身屬性改變事件，以便讓玩家看到技能實體
            map.OnActorVisibilityChange(actor);
            //建立技能效果處理物件
            Activator timer = new Activator(sActor, actor, args, level);

            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(actor, 5081);
            sActor.Slave.Add(actor);

            timer.Activate();
        }
        #endregion

        short PosX8to16(byte pos, ushort width)
        {
            return (short)((pos - ((double)width / 2)) * 100 + 50);
        }

        short PosY8to16(byte pos, ushort height)
        {
            return (short)(-(pos - ((double)height / 2)) * 100 - 50);
        }

        #region Timer
        private class Activator : MultiRunTask
        {
            Actor sActor;
            ActorSkill actor;
            SkillArg skill;
            float factor;
            Map map;
            int lifetime;
            public Activator(Actor _sActor, ActorSkill _dActor, SkillArg _args, byte level)
            {
                sActor = _sActor;
                actor = _dActor;
                skill = _args.Clone();
                factor = 0.1f * level;
                this.dueTime = 0;
                this.period = 1000;
                //lifetime = 5000 * level;
                lifetime = 6000 + (2000 * level);
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
            }
            public override void CallBack()
            {
                //同步鎖，表示之後的代碼是執行緒安全的，也就是，不允許被第二個執行緒同時訪問
                //测试去除技能同步锁ClientManager.EnterCriticalArea();
                try
                {
                    if (sActor.Slave.Count == 0)
                    {
                        this.Deactivate();
                        if (actor != null)
                        {
                            map.DeleteActor(actor);
                        }
                    }
                    if (lifetime > 0)
                    {
                        lifetime -= this.period;
                        // 检查目标是否已经受到 "SleepCloud" 效果
                        if (!actor.Status.Additions.ContainsKey("Stiff"))
                        {
                            foreach (Actor j in map.GetActorsArea(actor, 50, true))
                        {
                            if (j != sActor && SkillHandler.Instance.CheckValidAttackTarget(sActor, j))
                            {
                                    // 确保玩家没有处于睡眠状态
                                    if (!j.Status.Additions.ContainsKey("Stiff"))
                                    {
                                        Additions.Global.Stiff skill1 = new SagaMap.Skill.Additions.Global.Stiff(skill.skill, j, lifetime);
                                SkillHandler.ApplyAddition(j, skill1);
                                    }
                                }
                            }
                    }
                    }
                    else
                    {
                        this.Deactivate();
                        //在指定地图删除技能体（技能效果结束）
                        map.DeleteActor(actor);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
                //解開同步鎖
                //测试去除技能同步锁ClientManager.LeaveCriticalArea();
            }
        }
        #endregion


    }
}



