﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaDB.Skill;

namespace SagaMap.Skill.Additions.Global
{
    public class 鈍足2 : DefaultBuff
    {
        public 鈍足2(SagaDB.Skill.Skill skill, Actor actor, int lifetime)
            : base(skill, actor, "鈍足2", (int)(lifetime * (1f - actor.AbnormalStatus[SagaLib.AbnormalStatus.MoveSpeedDown] / 255)))
        {
            if (SkillHandler.Instance.isBossMob(actor))
            {
                if (!actor.Status.Additions.ContainsKey("BOSS鈍足免疫") && !actor.Status.Additions.ContainsKey("Frosen"))
                {
                    DefaultBuff BOSS鈍足免疫 = new DefaultBuff(skill, actor, "BOSS鈍足免疫", 30000);
                    SkillHandler.ApplyAddition(actor, BOSS鈍足免疫);
                }
                else
                    this.Enabled = false;
            }



            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {
            if (actor.SpeedCut < 7)
                actor.SpeedCut += 1;
            if (actor.SpeedCut >= 7)
            {
                Freeze 冰冻 = new Freeze(skill.skill, actor, 5000);
                SkillHandler.ApplyAddition(actor, 冰冻);
                actor.SpeedCut = 0;
            }
            float rate = 0.6f + 0.05f * actor.SpeedCut;
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.SpeedDown = true;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            if (skill.Variable.ContainsKey("SpeedDown2"))
                skill.Variable.Remove("SpeedDown2");
            int value = (int)(actor.Speed * rate);
            skill.Variable.Add("SpeedDown2", value);
            actor.Speed -= (ushort)value;
            if (actor.type == ActorType.MOB)
            {
                ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)actor.e;
                //设置AI活动性可以更新AI的移动速度
                SagaMap.Mob.Activity act = eh.AI.AIActivity;
                eh.AI.AIActivity = act;
            }
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            if (skill.endTime < DateTime.Now)
            {
                actor.SpeedCut = 0;
                actor.Buff.SpeedDown = false;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
            

            int value = skill.Variable["SpeedDown2"];
            actor.Speed = 600;
            if (actor.type == ActorType.MOB)
            {
                ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)actor.e;
                //设置AI活动性可以更新AI的移动速度
                SagaMap.Mob.Activity act = eh.AI.AIActivity;
                eh.AI.AIActivity = act;
            }
        }
    }
}
