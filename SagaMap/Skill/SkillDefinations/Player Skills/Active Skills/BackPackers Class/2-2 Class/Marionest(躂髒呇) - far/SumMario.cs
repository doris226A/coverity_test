using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Marionest
{
    /// <summary>
    /// 召喚活動木偶皇帝
    /// </summary>
    public class SumMario : ISkill
    {
        private uint MobID;
        private uint NextSkillID;
        public SumMario(uint MobID, uint NextSkillID)
        {
            this.MobID = MobID;
            this.NextSkillID = NextSkillID;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);

            if (map.GetActor(args.dActor) == null || map.GetActor(args.dActor).Buff.Dead)
            {
                return;
            }

            ActorMob mob;
            if (NextSkillID == 3341)
            {
                mob = map.SpawnMob(MobID, (short)(dActor.X + SagaLib.Global.Random.Next(1, 10))
                                             , (short)(dActor.Y + SagaLib.Global.Random.Next(1, 10))
                                             , 50, sActor);
            }
            else
            {
                mob = map.SpawnMob(MobID, (short)(sActor.X + SagaLib.Global.Random.Next(1, 10))
                                             , (short)(sActor.Y + SagaLib.Global.Random.Next(1, 10))
                                             , 50, sActor);
            }

            //招換
            /*ActorMob mob = map.SpawnMob(MobID, (short)(sActor.X + SagaLib.Global.Random.Next(1, 10))
                                             , (short)(sActor.Y + SagaLib.Global.Random.Next(1, 10))
                                             , 50, sActor);*/
            mob.Status = sActor.Status;
            mob.Owner = sActor;
            //sActor.Slave.Add(mob);
            ActorEventHandlers.MobEventHandler eh2 = (ActorEventHandlers.MobEventHandler)mob.e;
            if (NextSkillID == 3341)
            {
                eh2.AI.CastSkill(NextSkillID, level, mob);
            }
            else
            {
                eh2.AI.CastSkill(NextSkillID, level, args.dActor, SagaLib.Global.PosX8to16(args.x, eh2.AI.map.Width), SagaLib.Global.PosY8to16(args.y, eh2.AI.map.Height));

            }
            if (map.GetActor(args.dActor) == null || map.GetActor(args.dActor).Buff.Dead)
            {
                map.DeleteActor(mob);
            }

            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(NextSkillID,level,500));

        }
        #endregion
    }
}



