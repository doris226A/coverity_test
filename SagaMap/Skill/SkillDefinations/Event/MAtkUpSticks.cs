
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Event
{
    /// <summary>
    /// 魔法训练
    /// </summary>
    public class MAtkUpSticks : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "MAtkUpSticks", 600000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int MaxMAtk = (int)(actor.Status.max_matk * 0.25f);
            int MinMAtk = (int)(actor.Status.min_matk * 0.25f);
            //最大魔攻
            int max_matk_add = MaxMAtk;
            if (skill.Variable.ContainsKey("MAtkUpSticks_max_matk"))
                skill.Variable.Remove("MAtkUpSticks_max_matk");
            skill.Variable.Add("MAtkUpSticks_max_matk", max_matk_add);
            actor.Status.max_matk_skill += (short)max_matk_add;

            //最小魔攻
            int min_matk_add = MinMAtk;
            if (skill.Variable.ContainsKey("MAtkUpSticks_min_matk"))
                skill.Variable.Remove("MAtkUpSticks_min_matk");
            skill.Variable.Add("MAtkUpSticks_min_matk", min_matk_add);
            actor.Status.min_matk_skill += (short)min_matk_add;

            actor.Buff.MinMagicAtkUp = true;
            actor.Buff.MaxMagicAtkUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //最大魔攻
            actor.Status.max_matk_skill -= (short)skill.Variable["MAtkUpSticks_max_matk"];

            //最小魔攻
            actor.Status.min_matk_skill -= (short)skill.Variable["MAtkUpSticks_min_matk"];

            actor.Buff.MinMagicAtkUp = false;
            actor.Buff.MaxMagicAtkUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}
