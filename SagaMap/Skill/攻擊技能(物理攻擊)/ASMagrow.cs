
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Merchant
{
    /// <summary>
    /// 高聲放歌（ファシーボイス）
    /// </summary>
    public class ASMagrow : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 60000;
            int rate = 30;
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, "Magrow", rate))
            {
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Magrow", lifetime);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            //最小魔攻
            int min_matk_add = (int)(actor.Status.min_matk * 0.2f);
            if (skill.Variable.ContainsKey("AtkUpOne_min_matk"))
                skill.Variable.Remove("AtkUpOne_min_matk");
            skill.Variable.Add("AtkUpOne_min_matk", min_matk_add);
            actor.Status.min_matk_skill -= (short)min_matk_add;
            //最大魔攻
            int max_matk_add = (int)(actor.Status.max_matk * 0.2f);
            if (skill.Variable.ContainsKey("AtkUpOne_max_matk"))
                skill.Variable.Remove("AtkUpOne_max_matk");
            skill.Variable.Add("AtkUpOne_max_matk", max_matk_add);
            actor.Status.max_matk_skill -= (short)max_matk_add;

            actor.Buff.MinMagicAtkDown = true;
            actor.Buff.MaxMagicAtkDown = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //最小魔攻
            actor.Status.min_matk_skill += (short)skill.Variable["AtkUpOne_min_matk"];
            //最大魔攻
            actor.Status.max_matk_skill += (short)skill.Variable["AtkUpOne_max_matk"];

            actor.Buff.MinMagicAtkDown = false;
            actor.Buff.MaxMagicAtkDown = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
