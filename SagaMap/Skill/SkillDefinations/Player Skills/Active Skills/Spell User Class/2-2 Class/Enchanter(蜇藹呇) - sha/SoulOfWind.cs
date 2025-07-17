﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Enchanter
{
    /// <summary>
    /// 神風勢力（ウィンドオーラ）
    /// </summary>
    public class SoulOfWind : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            foreach (KeyValuePair<string, Addition> e in dActor.Status.Additions)
            {
                string AdditionName = e.Key;
                if (AdditionName.IndexOf("WeaponDC") >= 0 || AdditionName.IndexOf("PoisonReate") >= 0)
                {
                    ExtendCancelTypeAddition(dActor, AdditionName, level);
                }
            }
            Manager.MapManager.Instance.GetMap(dActor.MapID).SendEffect(dActor, 5233);

        }
        public void ExtendCancelTypeAddition(Actor actor, String additionName, byte level)
        {
            if (actor.Status.Additions.ContainsKey(additionName))
            {
                Addition addition = actor.Status.Additions[additionName];
                addition.TotalLifeTime += 5000 * level;
                SagaMap.Skill.SkillHandler.SendSystemMessage(actor, "延遲取消技能時間延長");
            }
        }
        #endregion
    }
}
