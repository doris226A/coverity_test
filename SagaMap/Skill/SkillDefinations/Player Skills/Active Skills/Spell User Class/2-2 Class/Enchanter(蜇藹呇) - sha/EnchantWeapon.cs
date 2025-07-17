﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Enchanter
{
    /// <summary>
    /// 迷惑武器（エンチャントウエポン）
    /// </summary>
    public class EnchantWeapon : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "EnchantWeapon", 120000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            /*if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                pc.CInt["EnchantWeaponLevel"] = skill.skill.Level;
            }*/
            actor.TInt["EnchantWeaponLevel"] = skill.skill.Level;
            actor.Buff.EnchantWeapon = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            /*if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                pc.CInt["EnchantWeaponLevel"] = 0;
            }*/
            actor.TInt["EnchantWeaponLevel"] = 0;
            actor.Buff.EnchantWeapon = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
