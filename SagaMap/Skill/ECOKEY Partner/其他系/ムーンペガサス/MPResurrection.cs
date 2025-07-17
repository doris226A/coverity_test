﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaMap.Network.Client;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Vates
{
    /// <summary>
    /// 复活术
    /// </summary>
    public class MPResurrection : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
            /*if (dActor.type != ActorType.PC) return -14;
            return 0;*/
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (dActor != null)
            {
                if (dActor.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)dActor;
                    if (pc.Online)
                    {
                        if (pc.Buff.Dead == true)
                        {
                            pc.Buff.TurningPurple = true;
                            MapClient.FromActorPC(pc).Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, pc, true);
                            pc.TInt["Revive"] = level;

                            if (dActor.type == ActorType.PARTNER)
                                MapClient.FromActorPC((ActorPC)dActor).SendSystemMessage(string.Format("寵物 {0} 正在使你复活", sActor.Name));
                            pc.TStr["复活者"] = sActor.Name;

                            MapClient.FromActorPC(pc).EventActivate(0xF1000000);
                        }

                    }
                }
            }
        }
        #endregion
    }
}
