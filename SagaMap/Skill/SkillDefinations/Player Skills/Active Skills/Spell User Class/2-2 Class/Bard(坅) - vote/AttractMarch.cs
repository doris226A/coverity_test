﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.ActorEventHandlers;
namespace SagaMap.Skill.SkillDefinations.Bard
{
    /// <summary>
    /// 攻擊進行曲（アトラクトマーチ）
    /// </summary>
    public class AttractMarch : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (SkillHandler.Instance.isBossMob(dActor))
            {
                return -13;
            }
            if (Skill.SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.STRINGS) || sActor.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
            {
                return 0;
            }

            return -5;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 5000 + 1000 * level;
            int rate = 10 * level;
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, "AttractMarch", rate))
            {
                if (dActor.type == ActorType.PC)
                {
                    /*dActor.HP = 0;
                    dActor.e.OnDie();
                    args.affectedActors.Add(dActor);
                    args.Init();
                    args.flag[0] = SagaLib.AttackFlag.DIE;*/
                    Additions.Global.Stiff Stiff = new SagaMap.Skill.Additions.Global.Stiff(args.skill, dActor, lifetime);
                    SkillHandler.ApplyAddition(dActor, Stiff);
                }
                else
                {
                    AttractMarchBuff skill = new AttractMarchBuff(args.skill, sActor, dActor, lifetime);
                    SkillHandler.ApplyAddition(dActor, skill);
                }
                /*AttractMarchBuff skill = new AttractMarchBuff(args.skill, sActor, dActor, lifetime);
                SkillHandler.ApplyAddition(dActor, skill);*/
            }
        }
        public class AttractMarchBuff : DefaultBuff
        {
            Actor sActor;
            public AttractMarchBuff(SagaDB.Skill.Skill skill, Actor sActor, Actor dActor, int lifetime)
                : base(skill, dActor, "AttractMarch", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.sActor = sActor;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
                if (actor.type == ActorType.MOB)
                {
                    MobEventHandler mh = (MobEventHandler)actor.e;
                    mh.AI.Master = sActor;

                }
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                if (actor.type == ActorType.MOB)
                {
                    MobEventHandler mh = (MobEventHandler)actor.e;
                    mh.AI.Master = null;
                }
            }
        }
        #endregion
    }
}
