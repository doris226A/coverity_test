using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaMap.Mob;
using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 挑釁
    /// </summary>
    public class Provocation : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            //技能禁止對人
            if (dActor.type == ActorType.PC)
                return -14;
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            MobAI mobAI = ((ActorEventHandlers.MobEventHandler)dActor.e).AI;
            if (mobAI != null)
            {
                if (dActor.type == ActorType.MOB)
                {
                    ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)dActor.e;
                    if (eh.AI.Hate.ContainsKey(sActor.ActorID))
                    {
                        eh.AI.Hate[sActor.ActorID] += (uint)(dActor.MaxHP * 5);
                    }
                    else
                    {
                        eh.AI.Hate.TryAdd(sActor.ActorID, (uint)(dActor.MaxHP * 5));
                    }
                    mobAI.attackStamp = DateTime.Now;
                }
            }
        }
        #endregion
    }
}
