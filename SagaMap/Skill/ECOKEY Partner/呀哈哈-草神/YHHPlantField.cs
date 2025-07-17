using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    /// <summary>
    /// 呀哈哈哈！	利用呀哈哈之力，造成三次範圍傷害，低機率使其陷入麻痺
    /// </summary>
    public class YHHPlantField : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            if (map.CheckActorSkillInRange(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 300))
            {
                return -17;
            }
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置
            actor.MapID = sActor.MapID;
            actor.X = dActor.X;
            actor.Y = dActor.Y;
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
            float factor = 0f;
            int countMax = 0, count = 0, lifetime = 0;

            public Activator(Actor caster, ActorSkill actor, SkillArg args, byte level)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
                int periods = 150;
                lifetime = 3000;
                int rate = SagaLib.Global.Random.Next(0, 1);
                factor = 1.7f;
                countMax = 3;
                this.period = periods;
                this.dueTime = 0;

            }

            public override void CallBack()
            {
                //同步锁，表示之后的代码是线程安全的，也就是，不允许被第二个线程同时访问ClientManager.EnterCriticalArea();
                try
                {
                    if (count < countMax)
                    {
                        //取得设置型技能，技能体周围7x7范围的怪（范围300，300代表3格，以自己为中心的3格范围就是7x7）
                        List<Actor> actors = map.GetActorsArea(actor, 200, false);
                        skill.affectedActors.Clear();
                        foreach (Actor i in actors)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(caster, i))
                            {
                                if (SkillHandler.Instance.CanAdditionApply(caster, i, SkillHandler.DefaultAdditions.Paralyse, 10))
                                {
                                    Additions.Global.Paralysis skill = new SagaMap.Skill.Additions.Global.Paralysis(this.skill.skill, i, 3000);
                                    SkillHandler.ApplyAddition(i, skill);
                                }
                                int magdmg = (int)(SkillHandler.Instance.CalcDamage(true, caster, i, this.skill, SkillHandler.DefType.Def, SagaLib.Elements.Neutral, 1, factor));
                                int dhydmg = (int)(SkillHandler.Instance.CalcDamage(false, caster, i, this.skill, SkillHandler.DefType.MDef, SagaLib.Elements.Neutral, 1, factor));
                                SkillHandler.Instance.FixAttack(caster, i, this.skill, SagaLib.Elements.Neutral, magdmg + dhydmg);
                                SkillHandler.Instance.ShowVessel(i, magdmg + dhydmg);
                            }

                        }
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
                //解开同步锁ClientManager.LeaveCriticalArea();
            }
        }
        #endregion
    }
}
