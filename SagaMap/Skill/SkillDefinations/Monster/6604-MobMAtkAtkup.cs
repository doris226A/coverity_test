using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// 全力支援（行會商人）
    /// </summary>
    public class MobMAtkAtkup : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 10000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "MobMAtkAtkup", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int add = 240;
            actor.Status.max_atk1_skill += (short)add;
            actor.Status.max_atk2_skill += (short)add;
            actor.Status.max_atk3_skill += (short)add;
            actor.Status.min_atk1_skill += (short)add;
            actor.Status.min_atk2_skill += (short)add;
            actor.Status.min_atk3_skill += (short)add;
            actor.Status.max_matk_skill += (short)add;
            actor.Status.min_matk_skill += (short)add;

            actor.Buff.MinAtkUp = true;
            actor.Buff.MaxAtkUp = true;
            actor.Buff.MinMagicAtkUp = true;
            actor.Buff.MaxMagicAtkUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            int add = 240;
            actor.Status.max_atk1_skill -= (short)add;
            actor.Status.max_atk2_skill -= (short)add;
            actor.Status.max_atk3_skill -= (short)add;
            actor.Status.min_atk1_skill -= (short)add;
            actor.Status.min_atk2_skill -= (short)add;
            actor.Status.min_atk3_skill -= (short)add;
            actor.Status.max_matk_skill -= (short)add;
            actor.Status.min_matk_skill -= (short)add;

            actor.Buff.MinAtkUp = false;
            actor.Buff.MaxAtkUp = false;
            actor.Buff.MinMagicAtkUp = false;
            actor.Buff.MaxMagicAtkUp = false;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
