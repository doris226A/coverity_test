using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB;
using SagaDB.Actor;
using SagaDB.Skill;
using SagaLib;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Eraser
{
    /// <summary>
    /// 影縫い(無即死)
    /// </summary>
    public class DAShadowSeam : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 200, true);
            int stiffran = 20;
            int stifftime = 3;
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stiff, stiffran))
                    {
                        Additions.Global.Stiff skill = new SagaMap.Skill.Additions.Global.Stiff(args.skill, act, stifftime);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }

        }
        #endregion
    }
}
