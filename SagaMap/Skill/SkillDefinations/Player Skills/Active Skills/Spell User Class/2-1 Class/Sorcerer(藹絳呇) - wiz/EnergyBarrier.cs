﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Sorcerer
{
    /// <summary>
    /// エナジーバリア
    /// </summary>
    public class EnergyBarrier : ISkill
    {
        bool MobUse;
        public EnergyBarrier()
        {
            this.MobUse = false;
        }
        public EnergyBarrier(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int life = 0;
            if (MobUse)
            {
                level = 5;
            }

            if (sActor is ActorPC)
            {
                ActorPC pc = sActor as ActorPC;
                if (pc.Skills.ContainsKey(3113))
                    levelnum = pc.Skills[3113].Level;
            }

            switch (level)
            {
                case 1:
                    life = 600000;
                    break;
                case 2:
                    life = 500000;
                    break;
                case 3:
                    life = 400000;
                    break;
                case 4:
                    life = 300000;
                    break;
                case 5:
                    life = 200000;
                    break;
            }

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 250, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    SkillHandler.RemoveAddition(act, "DevineBarrier");
                    SkillHandler.RemoveAddition(act, "EnergyShield");
                    DefaultBuff skill = new DefaultBuff(args.skill, act, "EnergyBarrier", life);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(act, skill);
                    EffectArg arg2 = new EffectArg();
                    arg2.effectID = 5168;
                    arg2.actorID = act.ActorID;


                    Manager.MapManager.Instance.GetMap(act.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, act, true);
                }
            }
        }
        int levelnum = 1;
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int atk1 = 0, atk2 = 0;
            switch (levelnum)
            {
                case 1:
                    atk1 = 3;
                    atk2 = 5;
                    break;
                case 2:
                    atk1 = 3;
                    atk2 = 10;
                    break;
                case 3:
                    atk1 = 6;
                    atk2 = 10;
                    break;
                case 4:
                    atk1 = 6;
                    atk2 = 15;
                    break;
                case 5:
                    atk1 = 9;
                    atk2 = 15;
                    break;
            }

            if (skill.Variable.ContainsKey("Def"))
                skill.Variable.Remove("Def");
            skill.Variable.Add("Def", atk1);
            actor.Status.def_skill += (short)atk1;
            if (skill.Variable.ContainsKey("DefAdd"))
                skill.Variable.Remove("DefAdd");
            skill.Variable.Add("DefAdd", atk2);
            actor.Status.def_add_skill += (short)atk2;

            actor.Buff.DefUp = true;
            actor.Buff.DefRateUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            int value = skill.Variable["Def"];
            actor.Status.def_skill -= (short)value;
            value = skill.Variable["DefAdd"];
            actor.Status.def_add_skill -= (short)value;

            actor.Buff.DefUp = false;
            actor.Buff.DefRateUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        public void RemoveAddition(Actor actor, String additionName)
        {
            if (actor.Status.Additions.ContainsKey(additionName))
            {
                Addition addition = actor.Status.Additions[additionName];
                actor.Status.Additions.TryRemove(additionName, out _);
                if (addition.Activated)
                {
                    addition.AdditionEnd();
                }
                addition.Activated = false;
            }
        }
        #endregion
    }
}