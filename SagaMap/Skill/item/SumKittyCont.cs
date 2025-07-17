using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 陽夜子召喚 [接續技能]
    /// </summary>
    public class SumKittyCont : ISkill
    {
        private Elements element;
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        ActorPC pc;
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                foreach (Actor act in affected)
                {
                    if (act.type == ActorType.PC || act.type == ActorType.PET || act.type == ActorType.SHADOW)
                    {
                        if (pc.PossessionTarget == 0)
                        {
                            realAffected.Add(act);
                        }
                    }
                }
            }
            else
            {
                foreach (Actor act in affected)
                {
                    if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        realAffected.Add(act);
                    }
                }

            }

            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SkillHandler.DefType.IgnoreAll, SagaLib.Elements.Holy, -5.0f);

            //map.DeleteActor(sActor);
            /*ActorMob mob = (ActorMob)sActor.Slave[0];
            map.DeleteActor(mob);
            sActor.Slave.RemoveAt(0);*/
        }
        #endregion
    }
}



