using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 小丑的寂寞
    /// </summary>
    public class DamageCharge_Boom : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 300, false);
            List<Actor> realAffected = new List<Actor>();
            int damage = 2000 * sActor.TInt["JokerDamageCharge_Level"] - 1000;
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                    SkillHandler.Instance.ShowVessel(dActor, damage);
                }
            }
            SkillHandler.Instance.FixAttackNoref(sActor, realAffected, args, SagaLib.Elements.Neutral, damage);
        }
        #endregion
    }
}
