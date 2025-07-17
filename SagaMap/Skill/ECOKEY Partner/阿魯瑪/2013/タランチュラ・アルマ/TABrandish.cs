using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.Skill.SkillDefinations.Swordman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SagaMap.Skill.SkillDefinations.BountyHunter
{
    public class TABrandish : ISkill
    {
        public int TryCast(SagaDB.Actor.ActorPC sActor, SagaDB.Actor.Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.2f;
            args.argType = SkillArg.ArgType.Attack;
            args.type = ATTACK_TYPE.SLASH;
            List<Actor> target = new List<Actor>();
            for (int i = 0; i < 10; i++)
            {
                target.Add(dActor);
            }
            args.delayRate = 4.5f;
            SkillHandler.Instance.PhysicalAttack(sActor, target, args, sActor.WeaponElement, factor);
        }
    }
}
