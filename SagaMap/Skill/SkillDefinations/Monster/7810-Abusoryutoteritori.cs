﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// アブソリュートテリトリー 
    /// </summary>
    public class Abusoryutoteritori : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 10.0f;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 500, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.MagicAttack (sActor, realAffected, args, SagaLib.Elements.Wind , factor);
            int hp_recovery = 0;
            foreach (int hp in args.hp)
            {
                hp_recovery += hp;
            }
            SkillHandler.Instance.FixAttack(sActor, sActor, args, SagaLib.Elements.Holy, -hp_recovery);
        }
        #endregion
    }
}