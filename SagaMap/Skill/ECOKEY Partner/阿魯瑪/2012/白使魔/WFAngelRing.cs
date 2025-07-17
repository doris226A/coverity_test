using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Cardinal
{
    /// <summary>
    /// エンジェルリング
    /// </summary>
    public class WFAngelRing : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 3.0f;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(dActor, 250, true); //200
            List<Actor> affected = new List<Actor>();
            args.affectedActors.Clear();
            foreach (var i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                    affected.Add(i);
                if (dActor.type == ActorType.PC || dActor.type == ActorType.PARTNER)
                {
                    return;
                }
                else
                {
                    Additions.Global.Stun skill = new SagaMap.Skill.Additions.Global.Stun(args.skill, i, 5000);
                SkillHandler.ApplyAddition(i, skill);
                }
            }
            SkillHandler.Instance.MagicAttack(sActor, affected, args, Elements.Holy, factor);
        }
        #endregion
    }
}
