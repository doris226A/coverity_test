
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Knight
{
    /// <summary>
    /// 神聖的一擊（ヴァルキリー）
    /// </summary>

    #region ISkill Members
    public class Valkyrie : ISkill
    {

        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*if ((uint)((double)((long)dActor.MaxHP * (long)(100 - 10 * (int)level)) / 100.0) >= dActor.MaxHP)
                return;*/

            float[] factors = { 0f, 2f, 3.4f, 4.8f, 6.2f, 7.6f };
            List<Actor> affected = new List<Actor>();
            affected.Add(dActor);
            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, SkillHandler.DefType.IgnoreRight, SagaLib.Elements.Holy, 0, factors[level], false);
            Additions.Global.Stiff skills = new SagaMap.Skill.Additions.Global.Stiff(args.skill, sActor, 500);
            SkillHandler.ApplyAddition(sActor, skills);
        }
    }
}
#endregion
