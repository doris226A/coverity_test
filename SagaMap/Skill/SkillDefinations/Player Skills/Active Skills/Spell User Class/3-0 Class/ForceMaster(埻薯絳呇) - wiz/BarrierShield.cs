﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.ForceMaster
{
    public class BarrierShield : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (!dActor.Status.Additions.ContainsKey("BarrierShield"))
            {
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "BarrierShield", 600000, 1000);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                skill.OnUpdate += this.Update;
                SkillHandler.ApplyAddition(dActor, skill);
            }
            else
            {
                dActor.Status.Additions["BarrierShield"].OnTimerEnd();
            }
        }
        void Update(Actor actor, DefaultBuff skill)
        {
            uint[] MP_down = { 0, 30, 25, 20, 15, 10 };
            uint mp_realdown = MP_down[skill.skill.Level];

            if (actor.MP < mp_realdown)
            {
                actor.Status.Additions["BarrierShield"].OnTimerEnd();
                actor.Status.Additions.TryRemove("BarrierShield", out _);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
            else
            {
                actor.MP -= mp_realdown;
            }
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            float[] def_downs = { 0, 1.0f, 0.95f, 0.85f, 0.75f, 0.6f };
            short def_down = (short)(actor.Status.def * def_downs[skill.skill.Level]);
            actor.Status.def_skill -= def_down;
            if (skill.Variable.ContainsKey("BarrierShield_Def"))
                skill.Variable.Remove("BarrierShield_Def");
            skill.Variable.Add("BarrierShield_Def", def_down);

            actor.Buff.DefRateDown = true;
            actor.Buff.三转魔法抗体 = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.def_skill += (short)skill.Variable["BarrierShield_Def"];
            actor.Buff.DefRateDown = false;
            actor.Buff.三转魔法抗体 = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
