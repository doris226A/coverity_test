
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    /// <summary>
    /// 御劍
    /// </summary>
    public class SNAtkMinToMax : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 20000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "SNAtkMinToMax", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            float[] MinAttack = new float[] { 0, 0.7f, 0.8f, 0.9f};

            //最小攻擊
            int min_atk_add = (int)(actor.Status.max_atk1 - actor.Status.min_atk1);
            if (min_atk_add < 0)
                min_atk_add = 0;
            if (skill.Variable.ContainsKey("SNAtkMinToMax"))
                skill.Variable.Remove("SNAtkMinToMax");
            skill.Variable.Add("SNAtkMinToMax", min_atk_add);
            actor.Status.min_atk1_skill += (short)min_atk_add;

            actor.Buff.三转アドバンスアビリテイー = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //最小攻擊
            actor.Status.min_atk1_skill -= (short)skill.Variable["SNAtkMinToMax"];

            actor.Buff.三转アドバンスアビリテイー = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
