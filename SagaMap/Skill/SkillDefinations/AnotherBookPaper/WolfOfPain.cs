using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    /// <summary>
    /// ウルフオブペイン
    /// </summary>
    public class WolfOfPain : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = new float[] { 0, 18.0f, 19.5f, 23.5f, 27.5f, 32.0f, 34.5f, 37.5f, 41.0f, 45.0f, 48.5f, 53.0f, 55.5f, 59.0f, 62.5f, 66.0f }[level];
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
