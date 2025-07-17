using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using System.Collections.Generic;

namespace SagaMap.Skill.SkillDefinations.Global
{
    public class GlitteringPupilStone : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 200, false);
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act) && !SkillHandler.Instance.isBossMob(act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stone, 100))
                    {
                        SagaMap.Skill.Additions.Global.Stone skill = new SagaMap.Skill.Additions.Global.Stone(args.skill, act, 4000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
        }

    }
}
