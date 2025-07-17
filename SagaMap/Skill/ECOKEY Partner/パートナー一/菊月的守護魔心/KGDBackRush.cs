using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.TreasureHunter
{
    /// <summary>
    /// 纏繞捆綁（バックラッシュ）
    /// </summary>
    public class KGDBackRush : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
                return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 5000;
            Stiff dskill = new Stiff(args.skill, dActor, lifetime);
            SkillHandler.ApplyAddition(dActor, dskill);
            Stiff sskill = new Stiff(args.skill, sActor, lifetime);
            SkillHandler.ApplyAddition(sActor, sskill);
        }
        #endregion
    }
}
