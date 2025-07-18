﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;

namespace SagaMap.Skill.SkillDefinations.Elementaler
{
    /// <summary>
    /// ラーヴァフロウ
    /// </summary>
    public class LavaFlow : ISkill
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
            actor.X = SagaLib.Global.PosX8to16(args.x, map.Width);
            actor.Y = SagaLib.Global.PosY8to16(args.y, map.Height);
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
            float factor = 1.0f;
            int countMax = 1, count = 1;
            int TotalLv = 1;

            public Activator(Actor caster, ActorSkill actor, SkillArg args, byte level)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                this.countMax = new int[] { 0, 4, 5, 5, 6, 6 }[level];
                int lifetime = new int[] { 0, 4500, 5000, 5500, 6000, 6000 }[level];
                ActorPC Me = (ActorPC)caster;
                if (Me.Skills2.ContainsKey(3013))//Caculate the factor according to skill FireStorm.
                {
                    TotalLv = Me.Skills2[3013].BaseData.lv;
                    switch (level)
                    {
                        case 1:
                            factor = 1.2f;
                            break;
                        case 2:
                            factor = 1.4f;
                            break;
                        case 3:
                            factor = 1.6f;
                            break;
                        case 4:
                            factor = 1.8f;
                            break;
                        case 5:
                            factor = 2.0f;
                            break;

                    }
                }
                if (Me.SkillsReserve.ContainsKey(3013))//Caculate the factor according to skill FireStorm.
                {
                    TotalLv = Me.SkillsReserve[3013].BaseData.lv;
                    switch (level)
                    {
                        case 1:
                            factor = 1.2f;
                            break;
                        case 2:
                            factor = 1.4f;
                            break;
                        case 3:
                            factor = 1.6f;
                            break;
                        case 4:
                            factor = 1.8f;
                            break;
                        case 5:
                            factor = 2.0f;
                            break;

                    }
                }
                if (Me.Skills2.ContainsKey(3049))//Caculate the count according to skill EarthStorm
                {
                    TotalLv = Me.Skills2[3049].BaseData.lv;
                    switch (TotalLv)
                    {
                        case 1:
                            switch (level)
                            {
                                case 1:
                                    countMax = 5;
                                    break;
                                case 2:
                                    countMax = 5;
                                    break;
                                case 3:
                                    countMax = 6;
                                    break;
                                case 4:
                                    countMax = 6;
                                    break;
                                case 5:
                                    countMax = 7;
                                    break;
                            }
                            break;
                        case 2:
                            switch (level)
                            {
                                case 1:
                                    countMax = 5;
                                    break;
                                case 2:
                                    countMax = 6;
                                    break;
                                case 3:
                                    countMax = 6;
                                    break;
                                case 4:
                                    countMax = 7;
                                    break;
                                case 5:
                                    countMax = 8;
                                    break;
                            }
                            break;
                        case 3:
                            switch (level)
                            {
                                case 1:
                                    countMax = 6;
                                    break;
                                case 2:
                                    countMax = 7;
                                    break;
                                case 3:
                                    countMax = 7;
                                    break;
                                case 4:
                                    countMax = 8;
                                    break;
                                case 5:
                                    countMax = 8;
                                    break;
                            }
                            break;
                        case 4:
                            switch (level)
                            {
                                case 1:
                                    countMax = 7;
                                    break;
                                case 2:
                                    countMax = 7;
                                    break;
                                case 3:
                                    countMax = 8;
                                    break;
                                case 4:
                                    countMax = 9;
                                    break;
                                case 5:
                                    countMax = 10;
                                    break;
                            }
                            break;
                        case 5:
                            switch (level)
                            {
                                case 1:
                                    countMax = 8;
                                    break;
                                case 2:
                                    countMax = 9;
                                    break;
                                case 3:
                                    countMax = 9;
                                    break;
                                case 4:
                                    countMax = 10;
                                    break;
                                case 5:
                                    countMax = 11;
                                    break;
                            }
                            break;
                    }
                }
                if (Me.SkillsReserve.ContainsKey(3049))//Caculate the count according to skill EarthStorm
                {
                    TotalLv = Me.SkillsReserve[3049].BaseData.lv;
                    switch (TotalLv)
                    {
                        case 1:
                            switch (level)
                            {
                                case 1:
                                    countMax = 5;
                                    break;
                                case 2:
                                    countMax = 5;
                                    break;
                                case 3:
                                    countMax = 6;
                                    break;
                                case 4:
                                    countMax = 6;
                                    break;
                                case 5:
                                    countMax = 7;
                                    break;
                            }
                            break;
                        case 2:
                            switch (level)
                            {
                                case 1:
                                    countMax = 5;
                                    break;
                                case 2:
                                    countMax = 6;
                                    break;
                                case 3:
                                    countMax = 6;
                                    break;
                                case 4:
                                    countMax = 7;
                                    break;
                                case 5:
                                    countMax = 8;
                                    break;
                            }
                            break;
                        case 3:
                            switch (level)
                            {
                                case 1:
                                    countMax = 6;
                                    break;
                                case 2:
                                    countMax = 7;
                                    break;
                                case 3:
                                    countMax = 7;
                                    break;
                                case 4:
                                    countMax = 8;
                                    break;
                                case 5:
                                    countMax = 8;
                                    break;
                            }
                            break;
                        case 4:
                            switch (level)
                            {
                                case 1:
                                    countMax = 7;
                                    break;
                                case 2:
                                    countMax = 7;
                                    break;
                                case 3:
                                    countMax = 8;
                                    break;
                                case 4:
                                    countMax = 9;
                                    break;
                                case 5:
                                    countMax = 10;
                                    break;
                            }
                            break;
                        case 5:
                            switch (level)
                            {
                                case 1:
                                    countMax = 8;
                                    break;
                                case 2:
                                    countMax = 9;
                                    break;
                                case 3:
                                    countMax = 9;
                                    break;
                                case 4:
                                    countMax = 10;
                                    break;
                                case 5:
                                    countMax = 11;
                                    break;
                            }
                            break;
                    }
                }

                map = Manager.MapManager.Instance.GetMap(actor.MapID);
                this.period = lifetime / countMax;
                this.dueTime = 0;

            }

            public override void CallBack()
            {
                //同步锁，表示之后的代码是线程安全的，也就是，不允许被第二个线程同时访问
                //测试去除技能同步锁ClientManager.EnterCriticalArea();
                try
                {
                    if (count < countMax)
                    {
                        //取得设置型技能，技能体周围7x7范围的怪（范围300，300代表3格，以自己为中心的3格范围就是7x7）
                        List<Actor> actors = map.GetActorsArea(actor, 300, false);
                        List<Actor> affected = new List<Actor>();
                        //取得有效Actor（即怪物）

                        //施加火属性魔法伤害
                        skill.affectedActors.Clear();
                        foreach (Actor i in actors)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(caster, i))
                            {
                             //   Additions.Global.Stiff Stiff = new SagaMap.Skill.Additions.Global.Stiff(skill.skill, i, 400);//Mob can not move as soon as attacked.
                               // SkillHandler.ApplyAddition(i, Stiff);
                                affected.Add(i);
                            }
                        }

                        SkillHandler.Instance.MagicAttack(caster, affected, skill, Elements.Fire, factor);

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
                //解开同步锁
                //测试去除技能同步锁ClientManager.LeaveCriticalArea();
            }
        }
        #endregion
    }
}
