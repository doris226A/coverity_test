using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaDB.Map;
using SagaMap.Manager;
using SagaMap.Mob;

namespace SagaMap.Skill.SkillDefinations.Global
{
    public class top2 : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ActorPC pc = (ActorPC)sActor;
            ActorMob Lobo =  map.SpawnMob(30590000, SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 10, null);


            List<Actor> affected = map.GetActorsArea(sActor, 400, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (act.type == ActorType.MOB && SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    MobAI mobAI = ((ActorEventHandlers.MobEventHandler)act.e).AI;
                    if (mobAI != null)
                    {
                        ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)act.e;
                        if (eh.AI.Hate.ContainsKey(Lobo.ActorID))
                        {
                            eh.AI.Hate[Lobo.ActorID] += (uint)(act.MaxHP * 5);
                        }
                        else
                        {
                            eh.AI.Hate.TryAdd(Lobo.ActorID, (uint)(act.MaxHP * 5));
                        }
                        mobAI.attackStamp = DateTime.Now;
                    }
                }
            }


            
        }
        #endregion
    }
}
