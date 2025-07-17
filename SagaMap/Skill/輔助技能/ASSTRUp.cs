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
    public class ASSTRUp : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.dActor = 0;//不显示效果
            int life = 65000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "STRUp", life);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short STR = 2;

            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                short minatk = (short)Math.Floor(STR + Math.Pow(Math.Floor((double)(STR / 9)), 2));
                short maxatk = (short)(STR + Math.Pow(Math.Floor((double)((float)(STR + 14) / 5.0f)), 2));
                actor.Status.min_atk1_skill += minatk;
                actor.Status.min_atk2_skill += minatk;
                actor.Status.min_atk3_skill += minatk;
                actor.Status.max_atk1_skill += maxatk;
                actor.Status.max_atk2_skill += maxatk;
                actor.Status.max_atk3_skill += maxatk;

                if (skill.Variable.ContainsKey("STR_UP_minatk"))
                    skill.Variable.Remove("STR_UP_minatk");
                skill.Variable.Add("STR_UP_minatk", minatk);
                if (skill.Variable.ContainsKey("STR_UP_maxatk"))
                    skill.Variable.Remove("STR_UP_maxatk");
                skill.Variable.Add("STR_UP_maxatk", maxatk);
            }
            else
            {
                //STR
                if (skill.Variable.ContainsKey("STRUP_STR"))
                    skill.Variable.Remove("STRUP_STR");
                skill.Variable.Add("STRUP_STR", STR);
                actor.Status.str_skill += STR;
            }
            actor.Buff.STRUp = true;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                actor.Status.min_atk1_skill -= (short)skill.Variable["STR_UP_minatk"];
                actor.Status.min_atk2_skill -= (short)skill.Variable["STR_UP_minatk"];
                actor.Status.min_atk3_skill -= (short)skill.Variable["STR_UP_minatk"];
                actor.Status.max_atk1_skill -= (short)skill.Variable["STR_UP_maxatk"];
                actor.Status.max_atk2_skill -= (short)skill.Variable["STR_UP_maxatk"];
                actor.Status.max_atk3_skill -= (short)skill.Variable["STR_UP_maxatk"];
            }
            else
            {
                actor.Status.str_skill -= (short)skill.Variable["STRUP_STR"];
            }

            actor.Buff.STRUp = false;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
