﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 移動技能
    /// </summary>
    public class EventMoveSkill : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            uint TargetMobID = 14110000;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
             foreach (KeyValuePair<uint, Actor> act in map.Actors)
            {
                if (act.Value.type == ActorType.MOB)
                {
                    ActorMob m = (ActorMob)act.Value;
                    if (m.BaseData.id == TargetMobID)
                    {
                        SkillHandler.Instance.AttractMob(sActor, m);
                        Manager.MapClientManager.Instance.FindClient((ActorPC)sActor).map.SendActorToMap(m, Manager.MapManager.Instance.GetMap(sActor.MapID), sActor.X, sActor.Y);
                        m.invisble = false;
                        Manager.MapClientManager.Instance.FindClient((ActorPC)sActor).map.OnActorVisibilityChange(m);
                        return;
                    }
                }
            }
        }
        #endregion
    }
}