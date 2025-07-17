using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Warlock
{
    /// <summary>
    /// マジックコンフュージョン
    /// </summary>
    public class CARangeConfuse : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(sActor, 250, false); //200
            int rate = 60;
            int lifetime = 6000;
            foreach (var i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, i, SkillHandler.DefaultAdditions.Confuse, rate))
                    {
                        Additions.Global.Confuse skill = new SagaMap.Skill.Additions.Global.Confuse(args.skill, i, lifetime);
                        SkillHandler.ApplyAddition(i, skill);
                    }
                }
            }
            
        }
        #endregion
    }
}
