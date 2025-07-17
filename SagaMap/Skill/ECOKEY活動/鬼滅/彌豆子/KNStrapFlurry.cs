using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;


namespace SagaMap.Skill.SkillDefinations.Stryder
{
    class KNStrapFlurry : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.1f + 0.3f * level;
           
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(sActor, 400, true);
            List<Actor> affected = new List<Actor>();
            short[] pos = new short[2];
            pos[0] = sActor.X;
            pos[1] = sActor.Y;
            foreach (Actor i in actors)
            {
                //这里,应该是判定为可以攻击的对象才会被拉过来.
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {
                    affected.Add(i);

                    map.MoveActor(Map.MOVE_TYPE.START, i, pos, i.Dir, 20000, true, MoveType.BATTLE_MOTION);
                }
            }

            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
