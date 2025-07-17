using SagaDB.Actor;
using SagaLib;
using SagaMap.Network.Client;
using SagaMap.Skill.Additions.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    /// <summary>
    /// サンサン・サンバ
    /// </summary>
    public class SunSanSamba : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            //Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
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
        }

        #endregion

        #region Timer

        private class Activator : MultiRunTask
        {
            ActorSkill actor;
            Actor caster;
            SkillArg skill;
            Map map;
            //float[] factors = new float[] { 0f, -0.8f, -0.9f, -0.7f, -1.0f, -1.1f, -100f };
            float factor = 0f;
            int[] countMaxs = new int[] { 0, 5, 5, 16, 10, 13 };
            int countMax = 0;
            int count = 0, lifetime = 0;

            public Activator(Actor caster, ActorSkill actor, SkillArg args, byte level)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                this.countMax = new int[] { 0, 7, 7, 7, 8, 8, 8, 10, 10, 10, 12, 12, 12, 14, 14, 14 }[level];// countMaxs[level];
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
                //int[] periods = new int[] { 0, 1000, 1000, 500, 1000, 800, 100 };
                lifetime = new int[] { 0, 7, 7, 7, 8, 8, 8, 10, 10, 10, 12, 12, 12, 14, 14, 14 }[level] * 1000;
                factor = new float[] { 1.1f, 1.11f, 1.12f, 1.13f, 1.14f, 1.15f, 1.16f, 1.17f, 1.18f, 1.19f, 1.20f, 1.21f, 1.22f, 1.23f, 1.25f }[level];
                this.period = 1000;
                this.dueTime = 0;

            }

            public override void CallBack()
            {
                //同步锁，表示之后的代码是线程安全的，也就是，不允许被第二个线程同时访问ClientManager.EnterCriticalArea();
                try
                {
                    List<Actor> actors = null;
                    if (count < countMax)
                    {
                        if (skill.skill.Level <= 10)
                            actors = map.GetActorsArea(actor, 100, false);
                        else
                            actors = map.GetActorsArea(actor, 200, false);
                        List<Actor> affected = new List<Actor>();
                        //取得有效Actor

                        skill.affectedActors.Clear();
                        foreach (Actor i in actors)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(caster, i))
                            {
                                affected.Add(i);
                            }

                        }
                        SkillHandler.Instance.MagicAttack(caster, affected, skill, caster.WeaponElement, factor);
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
                }
                //解开同步锁ClientManager.LeaveCriticalArea();
            }
        }
        #endregion
    }
}
