using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// 散佈安眠藥
    /// </summary>
    public class MSHSleepCircle : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int rate = 20;
            int lifetime = 30000;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
               
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Sleep, rate))
                {
                    Additions.Global.Sleep skill1 = new SagaMap.Skill.Additions.Global.Sleep(args.skill, act, lifetime);
                    if (act.type == ActorType.MOB)
                    {
                        SkillHandler.ApplyAddition(act, skill1);
                }
                }
            }
        }
        #endregion
    }
}
