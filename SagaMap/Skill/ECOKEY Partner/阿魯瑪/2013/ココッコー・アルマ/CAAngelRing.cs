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
    public class CAAngelRing : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.0f;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(sActor, 250, true); //200
            List<Actor> affected = new List<Actor>();
            int rate = 60;
            int lifetime = 6000;
            SkillHandler.Instance.MagicAttack(sActor, affected, args, Elements.Neutral, factor);
            foreach (var i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {
                    affected.Add(i);
                    if (SkillHandler.Instance.CanAdditionApply(sActor, i, SkillHandler.DefaultAdditions.Confuse, rate))
                    {
                        Additions.Global.Confuse skill = new SagaMap.Skill.Additions.Global.Confuse(args.skill, i, lifetime);
                        SkillHandler.ApplyAddition(i, skill);
                    }
                    SkillHandler.Instance.PushBack(sActor, i, 3);
                }
            }
            
        }
        #endregion
    }
}
