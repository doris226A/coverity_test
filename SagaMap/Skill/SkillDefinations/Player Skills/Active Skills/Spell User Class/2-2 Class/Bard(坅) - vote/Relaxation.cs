﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Bard
{
    /// <summary>
    /// 合唱（リラクゼーション）
    /// </summary>
    public class Relaxation : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (Skill.SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.STRINGS) || sActor.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
            {
                return 0;
            }
            return -5;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 30000 + 30000 * level;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 200, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (act.type == ActorType.PC || act.type == ActorType.PET || act.type == ActorType.PARTNER)
                {
                    if (act.Buff.Dead == true) continue;
                    DefaultBuff skill = new DefaultBuff(args.skill, act, "Relaxation", lifetime);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(act, skill);
                }
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.hp_recover_skill += 25;
            actor.Status.mp_recover_skill += 25;
            actor.Status.sp_recover_skill += 25;
            actor.Buff.HPRegenUp = true;
            actor.Buff.SPRegenUp = true;
            actor.Buff.MPRegenUp = true;
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.hp_recover_skill -= 25;
            actor.Status.mp_recover_skill -= 25;
            actor.Status.sp_recover_skill -= 25;
            actor.Buff.HPRegenUp = false;
            actor.Buff.SPRegenUp = false;
            actor.Buff.MPRegenUp = false;
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
