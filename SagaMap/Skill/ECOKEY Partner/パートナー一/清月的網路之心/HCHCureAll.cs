﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Druid
{
    /// <summary>
    /// 女神的加護（アレス）
    /// </summary>
    public class HCHCureAll : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (dActor.type == ActorType.MOB)
            {
                ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)dActor.e;
                if (eh.AI.Mode.Symbol)
                    return -14;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor =  -1.0f;
            float factors = factor;
            factors += sActor.Status.Cardinal_Rank;
            int rate = 96;
            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SkillHandler.DefType.IgnoreAll, SagaLib.Elements.Holy, factors);
            bool cure = false;
            if (SagaLib.Global.Random.Next(0, 99) < rate)
            {
                cure = true;
            }
            if (cure)
            {
                RemoveAddition(dActor, "Poison");
                RemoveAddition(dActor, "MoveSpeedDown");
                RemoveAddition(dActor, "MoveSpeedDown2");
                RemoveAddition(dActor, "Stone");
                RemoveAddition(dActor, "Silence");
                RemoveAddition(dActor, "Stun");
                RemoveAddition(dActor, "Sleep");
                RemoveAddition(dActor, "Frosen");
                RemoveAddition(dActor, "Confuse");
            }
        }
        public void RemoveAddition(Actor actor, String additionName)
        {
            if (actor.Status.Additions.ContainsKey(additionName))
            {
                Addition addition = actor.Status.Additions[additionName];
                actor.Status.Additions.TryRemove(additionName, out _);
                if (addition.Activated)
                {
                    addition.AdditionEnd();
                }
                addition.Activated = false;
            }
        }

        #endregion
    }
}
