﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Hawkeye
{
    public class PointRain : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            int numdown = args.skill.Level;
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            if (map.CheckActorSkillInRange(dActor.X, dActor.Y, 200))
            {
                return -17;
            }
            else
            {
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND))
                {
                    if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.BOW)
                    {
                        if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
                        {
                            if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType == SagaDB.Item.ItemType.ARROW)
                            {
                                if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Stack >= numdown)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return -55;
                                }
                            }
                            else
                                return -34;
                        }
                        return -34;
                    }
                    else if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.GUN ||
                        pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.DUALGUN ||
                        pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.RIFLE)
                    {
                        if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
                        {
                            if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType == SagaDB.Item.ItemType.BULLET)
                            {
                                if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Stack >= numdown)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return -56;
                                }
                            }

                            return -35;
                        }
                        return -35;
                    }
                    else
                        return -5;
                }
                else
                    return -5;
            }
            
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int numdown = level;
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND))
                {
                    if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.BOW)
                    {
                        if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
                        {
                            if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType == SagaDB.Item.ItemType.ARROW)
                            {
                                if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Stack >= numdown)
                                {
                                    Network.Client.MapClient.FromActorPC(pc).DeleteItem(pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Slot, (ushort)numdown, false);
                                }

                            }
                        }
                    }
                    if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.GUN ||
                        pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.DUALGUN ||
                        pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.RIFLE)
                    {
                        if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
                        {
                            if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType == SagaDB.Item.ItemType.BULLET)
                            {
                                if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Stack >= numdown)
                                {
                                    Network.Client.MapClient.FromActorPC(pc).DeleteItem(pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Slot, (ushort)numdown, false);
                                }
                            }
                        }
                    }
                }
            }
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置
            actor.MapID = dActor.MapID;
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
            Activator timer = new Activator(sActor, dActor, actor, args, level);
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
            int countMax = 0, count = 0;
            float factor = 0;
            Actor dActor;
            public Activator(Actor caster, Actor theDActor, ActorSkill actor, SkillArg args, byte level)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
                this.period = 300;
                this.dueTime = 0;
                int[] Counts = { 0, 4, 7, 8, 10, 15 };
                countMax = Counts[level];
                factor = 1.54f + 0.16f * level;
                dActor = theDActor;
            }
            public override void CallBack()
            {
                //同步锁，表示之后的代码是线程安全的，也就是，不允许被第二个线程同时访问
                //测试去除技能同步锁ClientManager.EnterCriticalArea();
                try
                {
                    if (count < countMax)
                    {
                        if(caster.type==ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)caster;
                            if(!pc.Online)
                            {
                                this.Deactivate();
                                //在指定地图删除技能体（技能效果结束）
                                map.DeleteActor(actor);
                                return;
                            }
                        }
                        //取得设置型技能，技能体周围7x7范围的怪（范围300，300代表3格，以自己为中心的3格范围就是7x7）
                        short skillrange = 0;
                        if (skill.skill.Level == 1 || skill.skill.Level == 2 || skill.skill.Level == 4)
                            skillrange = 100;
                        else
                            skillrange = 200;
                        List<Actor> actors = map.GetActorsArea(dActor, skillrange, true);
                        List<Actor> affected = new List<Actor>();
                        //取得有效Actor（即怪物）
                        //skill.argType = SkillArg.ArgType.Attack;
                        skill.type = ATTACK_TYPE.STAB;
                        skill.affectedActors.Clear();
                        foreach (Actor i in actors)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(caster, i) && !i.Buff.Dead)
                            {
                                affected.Add(i);
                                //skill.affectedActors.Add(i);
                            }
                        }
                        SkillHandler.Instance.PhysicalAttack(caster, affected, skill, caster.WeaponElement, factor);

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
