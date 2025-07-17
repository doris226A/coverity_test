
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Stryder
{
    public class PartyBivouac : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(sActor, 300, true);
            List<Actor> affected = new List<Actor>();
            foreach (Actor i in actors)
            {
                HPRecovery skill = new HPRecovery(args.skill, sActor, 300000, 5000);
                SkillHandler.ApplyAddition(sActor, skill);
            }*/

            if (sActor.type != ActorType.PC) return;
            ActorPC pc = (ActorPC)sActor;
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置
            actor.MapID = sActor.MapID;
            actor.X = sActor.X;
            actor.Y = sActor.Y;
            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //設置系
            actor.Stackable = false;
            //创建技能效果处理对象
            Activator timer = new Activator(sActor, actor, args, level);
            timer.Activate();

            List<ActorSkill> skillList = new List<ActorSkill>();
            switch (level)
            {
                case 1:
                    if (pc.Stackableskill.ContainsKey("PressureLv1"))
                    {
                        skillList = pc.Stackableskill["PressureLv1"];
                        if (pc.Stackableskill["PressureLv1"].Count >= 3)
                        {
                            map.DeleteActor(pc.Stackableskill["PressureLv1"][0]);
                            pc.Stackableskill["PressureLv1"].Remove(pc.Stackableskill["PressureLv1"][0]);
                        }
                        pc.Stackableskill["PressureLv1"].Add(actor);
                    }
                    else
                    {
                        skillList.Add(actor);
                        pc.Stackableskill.TryAdd("PressureLv1", skillList);
                    }
                    break;
                case 2:
                    if (pc.Stackableskill.ContainsKey("PressureLv2"))
                    {
                        skillList = pc.Stackableskill["PressureLv2"];
                        if (pc.Stackableskill["PressureLv2"].Count >= 3)
                        {
                            map.DeleteActor(pc.Stackableskill["PressureLv2"][0]);
                            pc.Stackableskill["PressureLv2"].Remove(pc.Stackableskill["PressureLv2"][0]);
                        }
                        pc.Stackableskill["PressureLv2"].Add(actor);
                    }
                    else
                    {
                        skillList.Add(actor);
                        pc.Stackableskill.TryAdd("PressureLv3", skillList);
                    }
                    break;
                case 3:
                    if (pc.Stackableskill.ContainsKey("PressureLv3"))
                    {
                        skillList = pc.Stackableskill["PressureLv3"];
                        if (pc.Stackableskill["PressureLv3"].Count >= 3)
                        {
                            map.DeleteActor(pc.Stackableskill["PressureLv3"][0]);
                            pc.Stackableskill["PressureLv3"].Remove(pc.Stackableskill["PressureLv3"][0]);
                        }
                        pc.Stackableskill["PressureLv3"].Add(actor);
                    }
                    else
                    {
                        skillList.Add(actor);
                        pc.Stackableskill.TryAdd("PressureLv3", skillList);
                    }
                    break;
                case 4:
                    if (pc.Stackableskill.ContainsKey("PressureLv4"))
                    {
                        skillList = pc.Stackableskill["PressureLv4"];
                        if (pc.Stackableskill["PressureLv4"].Count >= 3)
                        {
                            map.DeleteActor(pc.Stackableskill["PressureLv4"][0]);
                            pc.Stackableskill["PressureLv4"].Remove(pc.Stackableskill["PressureLv4"][0]);
                        }
                        pc.Stackableskill["PressureLv4"].Add(actor);
                    }
                    else
                    {
                        skillList.Add(actor);
                        pc.Stackableskill.TryAdd("PressureLv4", skillList);
                    }
                    break;
                case 5:
                    if (pc.Stackableskill.ContainsKey("PressureLv5"))
                    {
                        skillList = pc.Stackableskill["PressureLv5"];
                        if (pc.Stackableskill["PressureLv5"].Count >= 3)
                        {
                            map.DeleteActor(pc.Stackableskill["PressureLv5"][0]);
                            pc.Stackableskill["PressureLv5"].Remove(pc.Stackableskill["PressureLv5"][0]);
                        }
                        pc.Stackableskill["PressureLv5"].Add(actor);
                    }
                    else
                    {
                        skillList.Add(actor);
                        pc.Stackableskill.TryAdd("PressureLv5", skillList);
                    }
                    break;
            }
        }
        #endregion


        #region Timer

        private class Activator : MultiRunTask
        {
            ActorSkill actor;
            Actor caster;
            SkillArg skill;
            Map map;
            int countMax = 3, count = 0;
            int lifeTime = 0;
            short bouns = 0;
            public Activator(Actor caster, ActorSkill actor, SkillArg args, byte level)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
                this.period = 500;
                this.dueTime = 0;
                this.lifeTime = 180000;
                countMax = 180000 / period;
                bouns = new short[] { 0, 50, 80, 100, 150, 200 }[args.skill.Level];
            }
            public override void CallBack()
            {
                //同步锁，表示之后的代码是线程安全的，也就是，不允许被第二个线程同时访问
                //测试去除技能同步锁ClientManager.EnterCriticalArea();
                try
                {
                    if (map.GetActor(actor.ActorID) == null)
                    {
                        this.Deactivate();
                        map.DeleteActor(actor);
                        return;
                    }
                    if (count < countMax)
                    {
                        //取得设置型技能，技能体周围7x7范围的怪（范围300，300代表3格，以自己为中心的3格范围就是7x7）
                        List<Actor> actors = map.GetActorsArea(actor, 150, false);
                        List<Actor> affected = new List<Actor>();
                        //取得有效Actor

                        skill.affectedActors.Clear();
                        foreach (Actor act in actors)
                        {
                            if (act.type == ActorType.PC || act.type == ActorType.PARTNER)
                            {
                                if (act.Buff.Dead == true) continue;

                                if (!act.Status.Additions.ContainsKey("PartyBivouac"))
                                {
                                    DefaultBuff skill2 = new DefaultBuff(skill.skill, act, "PartyBivouac", lifeTime - count * period, 200);
                                    skill2.OnUpdate += this.TimerEventHandler;
                                    SkillHandler.ApplyAddition(act, skill2);
                                }
                            }
                        }
                        //广播技能效果
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, skill, actor, false);
                        count++;
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
                    this.Deactivate();
                    map.DeleteActor(actor);
                }
                //解开同步锁
                //测试去除技能同步锁ClientManager.LeaveCriticalArea();
            }
            void TimerEventHandler(Actor actor, DefaultBuff skill)
            {
                int ranges = Map.Distance(this.actor, actor);
                if (ranges >= 200)
                {
                    actor.Status.Additions["PartyBivouac"].AdditionEnd();
                    actor.Status.Additions.TryRemove("PartyBivouac", out _);
                }
            }
        }
        #endregion
    }
}