﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// ファイアアート
    /// </summary>
    public class FireArt : ISkill
    {
        //bool MobUse;
        //public aStormSword()
        //{
        //    this.MobUse = false;
        //}
        //public aStormSword(bool MobUse)
        //{
        //    this.MobUse = MobUse;
        //}
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0;
            factor = 5.0f;
            

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 200, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SagaLib.Elements.Fire, factor);
            SkillHandler.Instance.FixAttack(sActor, sActor, args, SagaLib.Elements.Neutral, sActor.MaxHP);
        }
        #endregion
    }
}
