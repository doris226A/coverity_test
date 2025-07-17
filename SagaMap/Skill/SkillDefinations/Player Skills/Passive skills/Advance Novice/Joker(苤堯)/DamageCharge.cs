using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 傷害蔓延
    /// </summary>
    public class DamageCharge : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }


        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "JokerDamageCharge", 300000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.TInt["JokerDamageCharge_Level"] = skill.skill.Level;
            switch (skill.skill.Level)
            {
                case 1:
                case 2:
                    actor.TInt["JokerDamageCharge_Max"] = 1;
                    break;
                case 3:
                case 4:
                    actor.TInt["JokerDamageCharge_Max"] = 2;
                    break;
                case 5:
                    actor.TInt["JokerDamageCharge_Max"] = 3;
                    break;
            }
            actor.TInt["JokerDamageCharge_now"] = 0;
            actor.Buff.三转人血管 = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.TInt["JokerDamageCharge_Level"] = 0;
            actor.TInt["JokerDamageCharge_now"] = 0;
            actor.TInt["JokerDamageCharge_Max"] = 0;
            actor.Buff.三转人血管 = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        #endregion
    }
}
