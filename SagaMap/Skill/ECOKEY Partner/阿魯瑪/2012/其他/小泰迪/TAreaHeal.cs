using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Druid
{
    /// <summary>
    /// 範圍治癒（エリアヒール）
    /// </summary>
    public class TAreaHeal : ISkill
    {
        bool MobUse;
        public TAreaHeal()
        {
            this.MobUse = false;
        }
        public TAreaHeal(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
            /* //ECOKEY 不能補PE身上的人
             if (pc.PossessionTarget != 0)
             {
                 return -7;
             }
             else
             {
                 return 0;
             }*/

        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (MobUse)
            {
                level = 5;
            }
            float factor = -1.4f;

            //  if (sActor.Status.Additions.ContainsKey("Cardinal"))//3转10技提升治疗量
            factor += sActor.Status.Cardinal_Rank;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                foreach (Actor act in affected)
                {
                    if (act.type == ActorType.PC || act.type == ActorType.PET || act.type == ActorType.PARTNER || act.type == ActorType.SHADOW)
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

            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SkillHandler.DefType.IgnoreAll, SagaLib.Elements.Holy, factor);
        }

        #endregion
    }
}

