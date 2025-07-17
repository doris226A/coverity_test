
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 窒息毒氣（チョーキングガス）
    /// </summary>
    public class ChokingGas : Trap
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            uint itemID = 10035003; 
            if (SkillHandler.Instance.CountItem(sActor, itemID) > 0)
            {
                SkillHandler.Instance.TakeItem(sActor, itemID, 1);
                return 0;
            }
            return -12;
        }
        public ChokingGas()
            :base(true ,200, PosType.sActor)
        {
            
        }
        public override void BeforeProc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            LifeTime = 30000;
        }
        public override void ProcSkill(Actor sActor, Actor mActor, ActorSkill actor, SkillArg args, Map map, int level, float factor)
        {
            int rate = 35 + 10 * level;
            List<Actor> affected = map.GetActorsArea(mActor, 350, true);
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Silence, rate))
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        Additions.Global.Silence skill = new SagaMap.Skill.Additions.Global.Silence(args.skill, act, 10000);
                    SkillHandler.ApplyAddition(act, skill);
                }
                }
            }
        }
    }
}