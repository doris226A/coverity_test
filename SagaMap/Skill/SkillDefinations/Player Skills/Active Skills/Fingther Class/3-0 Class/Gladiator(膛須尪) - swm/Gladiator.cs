﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gladiator
{
    /// <summary>
    /// グラディエイター
    /// </summary>
    public class Gladiator : ISkill
    {
        #region ISkill 成員

        public int TryCast(SagaDB.Actor.ActorPC sActor, SagaDB.Actor.Actor dActor, SkillArg args)
        {
            return 0;
        }
        public ushort speed_old;
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            ActorPC pc = (ActorPC)sActor;
            ApplySkill(pc, args);
        }
        public void ApplySkill(ActorPC dActor, SkillArg args)
        {
            int[] lifetimes = new int[] { 0, 180000, 150000, 120000, 90000, 60000 };
            int lifetime = lifetimes[args.skill.Level];
            if (!dActor.Status.Additions.ContainsKey("Gladiator"))
            {
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Gladiator", lifetime);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
                if (dActor.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)dActor;
                    PC.StatusFactory.Instance.CalcStatus(pc);
                }
            }
            else
            {
                dActor.Status.Additions["Gladiator"].OnTimerEnd();
            }

        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.MainSkillPowerUp3RD = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.MainSkillPowerUp3RD = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
