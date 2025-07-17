using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Enchanter
{
    /// <summary>
    /// 重力刃 (グラヴィティフォール)
    /// </summary>
    public class BALFRGravityFall : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 4.6f;
            int lifetime = 6000;
            int rate = 20 + level * 6;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(dActor, 250, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.鈍足, rate))
                    {
                        Additions.Global.MoveSpeedDown skill = new SagaMap.Skill.Additions.Global.MoveSpeedDown(args.skill, act, lifetime);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                    realAffected.Add(act);
                }
            }

            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SagaLib.Elements.Earth, factor);
        }
        #endregion
    }
}
