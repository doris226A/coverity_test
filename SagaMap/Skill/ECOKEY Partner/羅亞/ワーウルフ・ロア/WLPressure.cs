﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaLib;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gladiator
{
    /// <summary>
    /// プレッシャー
    /// </summary>
    public class WLPressure : ISkill
    {
        #region ISkill Members

        public int TryCast(SagaDB.Actor.ActorPC sActor, SagaDB.Actor.Actor dActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            if (map.CheckActorSkillInRange(sActor.X, sActor.Y, 250))
            {
                Logger.ShowInfo("禁止執行?");
                return -17;
            }
            Logger.ShowInfo("執行執行");
            return 0;
        }
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            Logger.ShowInfo("這邊1" + args.skill.Name);
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置
            actor.MapID = sActor.MapID;
            actor.X = sActor.X;
            actor.Y = sActor.Y;
            Logger.ShowInfo("這邊22");
            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            Logger.ShowInfo("這邊333");
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //設置系
            actor.Stackable = false;
            //创建技能效果处理对象
            Activator timer = new Activator(sActor, dActor, actor, args, level);
            timer.Activate();
            Logger.ShowInfo("這邊4444");

            List<ActorSkill> skillList = new List<ActorSkill>();
            if (sActor.Stackableskill.ContainsKey("PressureLv1"))
            {
                skillList = sActor.Stackableskill["PressureLv1"];
                if (sActor.Stackableskill["PressureLv1"].Count >= 3)
                {
                    map.DeleteActor(sActor.Stackableskill["PressureLv1"][0]);
                    sActor.Stackableskill["PressureLv1"].Remove(sActor.Stackableskill["PressureLv1"][0]);
                }
                sActor.Stackableskill["PressureLv1"].Add(actor);
            }
            else
            {
                skillList.Add(actor);
                sActor.Stackableskill.TryAdd("PressureLv1", skillList);
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
            int countMax = 0, count = 0;
            float factor = 0;
            Actor dActor;
            public Activator(Actor caster, Actor theDActor, ActorSkill actor, SkillArg args, byte level)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
                this.period = 1000;
                this.dueTime = 1000;
                int[] Counts = { 0, 30, 30, 30, 40, 45 };
                countMax = Counts[level];
                dActor = theDActor;
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
                        List<Actor> actors = map.GetActorsArea(dActor, 200, true);
                        List<Actor> affected = new List<Actor>();
                        //取得有效Actor（即怪物）

                        //施加魔法伤害
                        skill.affectedActors.Clear();
                        foreach (Actor i in actors)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(caster, i))
                            {
                                DefaultBuff skill2 = new DefaultBuff(skill.skill, i, "Pressure", 5000);
                                skill2.OnAdditionStart += this.StartEventHandler;
                                skill2.OnAdditionEnd += this.EndEventHandler;
                                SkillHandler.ApplyAddition(i, skill2);
                            }
                        }
                        SkillHandler.Instance.MagicAttack(caster, affected, skill, SagaLib.Elements.Neutral, factor);

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
            void StartEventHandler(Actor actor, DefaultBuff skill)
            {
                int level = skill.skill.Level;
                float[] value = { 0, 0.03f, 0.06f, 0.09f, 0.12f, 0.15f };
                float[] speedless = { 0, 180, 210, 240, 270, 300 };
                //降ASPD
                if (skill.Variable.ContainsKey("PRESSURE_ASPD"))
                    skill.Variable.Remove("PRESSURE_ASPD");
                skill.Variable.Add("PRESSURE_ASPD", (short)(actor.Status.aspd * value[level]));
                actor.Status.aspd_skill -= (short)(actor.Status.aspd * value[level]);
                //降CSPD
                if (skill.Variable.ContainsKey("PRESSURE_CSPD"))
                    skill.Variable.Remove("PRESSURE_CSPD");
                skill.Variable.Add("PRESSURE_CSPD", (short)(actor.Status.cspd * value[level]));
                actor.Status.cspd_skill -= (short)(actor.Status.cspd * value[level]);
                //降SPEED
                if (skill.Variable.ContainsKey("PRESSURE_SPEED"))
                    skill.Variable.Remove("PRESSURE_SPEED");
                skill.Variable.Add("PRESSURE_SPEED", (short)(speedless[level]));
                actor.Status.speed_skill -= (short)(speedless[level]);
                actor.Buff.SpeedDown = true;
                actor.Buff.CastSpeedDown = true;
                actor.Buff.AttackSpeedDown = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
            void EndEventHandler(Actor actor, DefaultBuff skill)
            {
                actor.Status.aspd_skill += (short)skill.Variable["PRESSURE_ASPD"];
                actor.Status.cspd_skill += (short)skill.Variable["PRESSURE_CSPD"];
                actor.Status.speed_skill += (short)skill.Variable["PRESSURE_SPEED"];
                actor.Buff.SpeedDown = false;
                actor.Buff.CastSpeedDown = false;
                actor.Buff.AttackSpeedDown = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }
        #endregion

    }
}
