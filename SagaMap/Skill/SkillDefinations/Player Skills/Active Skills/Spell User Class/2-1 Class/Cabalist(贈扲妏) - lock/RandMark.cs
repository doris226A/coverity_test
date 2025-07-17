
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
using SagaMap.Mob;
namespace SagaMap.Skill.SkillDefinations.Cabalist
{
    /// <summary>
    /// 闇術師刻印（インフェナルマーク）
    /// </summary>
    public class RandMark : ISkill
    {
        public class Loc
        {
            public short x;
            public short y;
        }
        Dictionary<int, Loc> loc = new Dictionary<int, Loc>();
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actorsInRange = map.GetActorsArea(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 50, true);
            foreach (Actor actorInRange in actorsInRange)
            {
                if (actorInRange != sActor && SkillHandler.Instance.CheckValidAttackTarget(sActor, actorInRange))
                {
                    return -33;
                }
            }
            if (sActor.Skills2_1.ContainsKey(3166))
            {
                if (sActor.Skills2_1[3166].Level < args.skill.Level)
                {
                    return -12;
                }
            }
            else if (sActor.Skills2.ContainsKey(3166))
            {
                if (sActor.Skills2[3166].Level < args.skill.Level)
                {
                    return -12;
                }
            }
            else if (sActor.SkillsReserve.ContainsKey(3166))
            {
                if (sActor.SkillsReserve[3166].Level < args.skill.Level)
                {
                    return -12;
                }
            }
            else
            {
                return -12;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (loc.Count == 0)
            {
                LocAdd(1, 0, 0);
                LocAdd(2, 0, 100);
                LocAdd(3, 100, 0);
                LocAdd(4, 100, 100);
                LocAdd(5, 0, -100);
                LocAdd(6, -100, 0);
                LocAdd(7, 100, -100);
                LocAdd(8, -100, 100);
            }


            Mark(sActor, dActor, args, level, -100, -100);
            short numA = (short)SagaLib.Global.Random.Next(1, 8);
            Mark(sActor, dActor, args, level, loc[numA].x, loc[numA].y);
            short numB = (short)SagaLib.Global.Random.Next(1, 8);
            if (numA == numB && numB != 8)
            {
                numB++;
            }
            if (numA == numB && numB == 8)
            {
                numB--;
            }
            Mark(sActor, dActor, args, level, loc[numB].x, loc[numB].y);
        }
        #endregion


        void LocAdd(int num, short x, short y)
        {
            Loc A = new Loc();
            A.x = x;
            A.y = y;
            loc.Add(num, A);
        }

        void Mark(Actor sActor, Actor dActor, SkillArg args, byte level, short X, short Y)
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
            actor.X += X;
            actor.Y += Y;
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

            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(actor, SagaLib.Global.PosX16to8(actor.X, map.Width), SagaLib.Global.PosY16to8(actor.Y, map.Height), 5081);
            sActor.Slave.Add(actor);

            timer.Activate();
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
                lifetime = 5000 * level;
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
                        foreach (Actor j in map.GetActorsArea(actor, 50, true))
                        {
                            if (j != sActor && SkillHandler.Instance.CheckValidAttackTarget(sActor, j))
                            {
                                Additions.Global.Stiff skill1 = new SagaMap.Skill.Additions.Global.Stiff(skill.skill, j, 1000);
                                SkillHandler.ApplyAddition(j, skill1);
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
