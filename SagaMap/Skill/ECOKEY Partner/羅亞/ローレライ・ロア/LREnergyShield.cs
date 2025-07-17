using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Wizard
{
    public class LREnergyShield : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "LREnergyShield", 10000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short defadd = 20;
            short def = 10;
            if (actor.type == ActorType.PC || actor.type == ActorType.PET || actor.type == ActorType.PARTNER || actor.type == ActorType.SHADOW)
            {
                if (skill.Variable.ContainsKey("LREnergyShield_DEF"))
                    skill.Variable.Remove("LREnergyShield_DEF");
                skill.Variable.Add("LREnergyShield_DEF", def);

                if (skill.Variable.ContainsKey("LREnergyShield_DEFADD"))
                    skill.Variable.Remove("LREnergyShield_DEFADD");
                skill.Variable.Add("LREnergyShield_DEFADD", defadd);
                actor.Status.def_add_skill += defadd;
                actor.Status.def_skill += def;
                actor.Buff.DefUp = true;
            }
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.PC || actor.type == ActorType.PET || actor.type == ActorType.PARTNER || actor.type == ActorType.SHADOW)
            {
                actor.Status.def_skill -= (short)skill.Variable["LREnergyShield_DEF"];
                actor.Status.def_add_skill -= (short)skill.Variable["LREnergyShield_DEFADD"];
                actor.Buff.DefUp = false;
            }
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}