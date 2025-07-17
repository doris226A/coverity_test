using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Global
{
    public class EyeballUsual : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "EyeballUsual", true);
            skill.OnAdditionStart += Skill_OnAdditionStart;
            skill.OnAdditionEnd += Skill_OnAdditionEnd;
            SkillHandler.ApplyAddition(sActor, skill);
        }

        private void Skill_OnAdditionStart(Actor actor, DefaultPassiveSkill skill)
        {
        }

        private void Skill_OnAdditionEnd(Actor actor, DefaultPassiveSkill skill)
        {
        }
    }
}
