using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 
    /// </summary>
    public class KNFurious : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (pc.Partner != null)
            {
                ActorPartner p = pc.Partner;
                if (p.HP <= p.MaxHP / 2)
                {
                    return 0;
                }
            }
            return -17;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*args.dActor = 0;//不显示效果
            int life = 50000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "AGIUp", life);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);*/

            Berserk bs = new Berserk(args.skill, dActor, 20000);
            SkillHandler.ApplyAddition(dActor, bs);
        }

        /*void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short AGI = 4;
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                short avoid_melee = (short)(AGI + Math.Pow(Math.Floor((float)(AGI + 18) / 9.0f), 2) + Math.Floor((float)actor.Level / 3.0f) - 1);
                short avoid_ranged = (short)(Math.Floor(AGI + Math.Floor((float)actor.Level / 3.0f) + 3));
                actor.Status.avoid_melee_skill += avoid_melee;
                actor.Status.avoid_ranged_skill += avoid_ranged;

                short aspd = (short)(AGI * 3 + Math.Floor(Math.Pow((short)((float)(AGI + 63) / 9.0f), 2)));
                actor.Status.aspd_skill += aspd;

                if (skill.Variable.ContainsKey("AGI_UP_avoid_melee"))
                    skill.Variable.Remove("AGI_UP_avoid_melee");
                skill.Variable.Add("AGI_UP_avoid_melee", avoid_melee);
                if (skill.Variable.ContainsKey("AGI_UP_avoid_ranged"))
                    skill.Variable.Remove("AGI_UP_avoid_ranged");
                skill.Variable.Add("AGI_UP_avoid_ranged", avoid_ranged);
                if (skill.Variable.ContainsKey("AGI_UP_aspd"))
                    skill.Variable.Remove("AGI_UP_aspd");
                skill.Variable.Add("AGI_UP_aspd", aspd);
            }
            else
            {
                //AGI
                if (skill.Variable.ContainsKey("AGI_UP_AGI"))
                    skill.Variable.Remove("AGI_UP_AGI");
                skill.Variable.Add("AGI_UP_AGI", AGI);
                actor.Status.agi_skill += AGI;
            }

            actor.Buff.AGIUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                actor.Status.avoid_melee_skill -= (short)skill.Variable["AGI_UP_avoid_melee"];
                actor.Status.avoid_ranged_skill -= (short)skill.Variable["AGI_UP_avoid_ranged"];

                actor.Status.aspd_skill -= (short)skill.Variable["AGI_UP_aspd"];
            }
            else
            {
                actor.Status.agi_skill -= (short)skill.Variable["AGI_UP_AGI"]; ;
            }
            actor.Buff.AGIUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }*/
        #endregion
    }
}
