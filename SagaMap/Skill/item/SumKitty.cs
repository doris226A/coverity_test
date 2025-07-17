using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.ActorEventHandlers;
using SagaMap.Manager;
namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 陽夜子召喚
    /// </summary>
    public class SumKitty : ISkill
    {
        private uint MobID;
        private uint NextSkillID;
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ActorMob mob = map.SpawnMob(30830000, SagaLib.Global.PosX8to16(args.x, map.Width)
                                             , SagaLib.Global.PosY8to16(args.y, map.Height)
                                             , 0, sActor);

            mob.Status = sActor.Status;
            mob.Owner = sActor;
            ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)mob.e;
            eh.Defending += Onskill;
            eh.AI.CastSkill(9251, 1, mob);
        }
        void Onskill(MobEventHandler e, ActorPC pc)
        {
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            e.OnDie();
        }
    }
}



