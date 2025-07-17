using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    /// <summary>
    /// 雷之舞
    /// </summary>
    public class STWindMagic : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 5.0f;
            List<Actor> actors = Manager.MapManager.Instance.GetMap(dActor.MapID).GetActorsArea(dActor, 100, true);
            List<Actor> affected = new List<Actor>();
            foreach (Actor i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                    affected.Add(i);
            }
            SkillHandler.Instance.MagicAttack(sActor, affected, args, SagaLib.Elements.Wind, factor);

            foreach (Actor i in affected)
            {
                if (SkillHandler.Instance.CanAdditionApply(sActor, i, SkillHandler.DefaultAdditions.Paralyse, 40))
                {
                    Additions.Global.Paralysis skill = new SagaMap.Skill.Additions.Global.Paralysis(args.skill, i, 5000);
                    SkillHandler.ApplyAddition(i, skill);
                }
            }

        }
        #endregion
    }
}
