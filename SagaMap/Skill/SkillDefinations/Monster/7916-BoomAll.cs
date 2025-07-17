
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// 自爆-無差別攻擊
    /// </summary>
    public class BoomAll : ISkill
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

            SkillHandler.Instance.FixAttackNoref(sActor, affected, args, SagaLib.Elements.Neutral, 2222);
            /*List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }*/
            //int Damage = (int)sActor.HP;
            //realAffected.Add(sActor);
            //SkillHandler.Instance.MagicAttack(sActor, affected, args, SagaLib.Elements.Neutral, 15.0f);
            //SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
