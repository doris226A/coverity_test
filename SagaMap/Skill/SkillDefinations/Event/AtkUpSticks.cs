
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Event
{
    /// <summary>
    /// 进攻训练
    /// </summary>
    public class AtkUpSticks : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "AtkUpSticks", 600000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int MaxAtk = (int)(actor.Status.max_atk1 * 0.25f);
            int MinAtk = (int)(actor.Status.min_atk1 * 0.25f);
            //最大攻擊
            int max_atk1_add = MaxAtk;
            if (skill.Variable.ContainsKey("AtkUpSticks_max_atk1"))
                skill.Variable.Remove("AtkUpSticks_max_atk1");
            skill.Variable.Add("AtkUpSticks_max_atk1", max_atk1_add);
            actor.Status.max_atk1_skill += (short)max_atk1_add;

            //最大攻擊
            int max_atk2_add = MaxAtk;
            if (skill.Variable.ContainsKey("AtkUpSticks_max_atk2"))
                skill.Variable.Remove("AtkUpSticks_max_atk2");
            skill.Variable.Add("AtkUpSticks_max_atk2", max_atk2_add);
            actor.Status.max_atk2_skill += (short)max_atk2_add;

            //最大攻擊
            int max_atk3_add = MaxAtk;
            if (skill.Variable.ContainsKey("AtkUpSticks_max_atk3"))
                skill.Variable.Remove("AtkUpSticks_max_atk3");
            skill.Variable.Add("AtkUpSticks_max_atk3", max_atk3_add);
            actor.Status.max_atk3_skill += (short)max_atk3_add;

            //最小攻擊
            int min_atk1_add = MinAtk;
            if (skill.Variable.ContainsKey("AtkUpSticks_min_atk1"))
                skill.Variable.Remove("AtkUpSticks_min_atk1");
            skill.Variable.Add("AtkUpSticks_min_atk1", min_atk1_add);
            actor.Status.min_atk1_skill += (short)min_atk1_add;

            //最小攻擊
            int min_atk2_add = MinAtk;
            if (skill.Variable.ContainsKey("AtkUpSticks_min_atk2"))
                skill.Variable.Remove("AtkUpSticks_min_atk2");
            skill.Variable.Add("AtkUpSticks_min_atk2", min_atk2_add);
            actor.Status.min_atk2_skill += (short)min_atk2_add;

            //最小攻擊
            int min_atk3_add = MinAtk;
            if (skill.Variable.ContainsKey("AtkUpSticks_min_atk3"))
                skill.Variable.Remove("AtkUpSticks_min_atk3");
            skill.Variable.Add("AtkUpSticks_min_atk3", min_atk3_add);
            actor.Status.min_atk3_skill += (short)min_atk3_add;

            actor.Buff.MinAtkUp = true;
            actor.Buff.MaxAtkUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //最大攻擊
            actor.Status.max_atk1_skill -= (short)skill.Variable["AtkUpSticks_max_atk1"];

            //最大攻擊
            actor.Status.max_atk2_skill -= (short)skill.Variable["AtkUpSticks_max_atk2"];

            //最大攻擊
            actor.Status.max_atk3_skill -= (short)skill.Variable["AtkUpSticks_max_atk3"];

            //最小攻擊
            actor.Status.min_atk1_skill -= (short)skill.Variable["AtkUpSticks_min_atk1"];

            //最小攻擊
            actor.Status.min_atk2_skill -= (short)skill.Variable["AtkUpSticks_min_atk2"];

            //最小攻擊
            actor.Status.min_atk3_skill -= (short)skill.Variable["AtkUpSticks_min_atk3"];

            actor.Buff.MinAtkUp = false;
            actor.Buff.MaxAtkUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
