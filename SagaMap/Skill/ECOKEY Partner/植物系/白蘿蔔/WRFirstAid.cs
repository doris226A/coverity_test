﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Blacksmith
{
    /// <summary>
    /// 緊急治療（ファーストエイド）
    /// </summary>
    public class WRFirstAid : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
                return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            uint hp_recovery = (uint)(dActor.MaxHP * 0.1f);
            if (dActor.HP + hp_recovery <= dActor.MaxHP)
            {
                if (!dActor.Buff.NoRegen)
                    dActor.HP += hp_recovery;
            }
            else
            {
                dActor.HP = dActor.MaxHP;
            }
            args.affectedActors.Add(dActor);
            args.Init();
            if (args.flag.Count > 0)
            {
                args.flag[0] |= SagaLib.AttackFlag.HP_HEAL | SagaLib.AttackFlag.NO_DAMAGE;
            }
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, dActor, true);
        }
        #endregion
    }
}