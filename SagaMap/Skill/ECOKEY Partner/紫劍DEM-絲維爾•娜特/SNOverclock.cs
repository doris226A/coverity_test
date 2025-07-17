using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    /// <summary>
    /// 超頻
    /// </summary>
    public class SNOverclock : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 30000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "SNOverclock", lifetime);
            skill.OnAdditionStart += skill_OnAdditionStart;
            skill.OnAdditionEnd += skill_OnAdditionEnd;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void skill_OnAdditionStart(Actor actor, DefaultBuff skill)
        {
            short maxatk = (short)(actor.Status.max_atk1 * 0.1);
            short minatk = (short)(actor.Status.min_atk1 * 0.1);
            short maxmatk = (short)(actor.Status.max_matk * 0.1);
            short minmatk = (short)(actor.Status.min_matk * 0.1);
            short hit = 50;
            short avoidmelee = (short)actor.Status.avoid_melee;
            short avoidranged = (short)actor.Status.avoid_ranged;

            if (skill.Variable.ContainsKey("SNOverclock_maxatk"))
                skill.Variable.Remove("SNOverclock_maxatk");
            skill.Variable.Add("SNOverclock_maxatk", maxatk);
            actor.Status.max_atk1_skill += maxatk;

            if (skill.Variable.ContainsKey("SNOverclock_minatk"))
                skill.Variable.Remove("SNOverclock_minatk");
            skill.Variable.Add("SNOverclock_minatk", minatk);
            actor.Status.min_atk1_skill += minatk;

            if (skill.Variable.ContainsKey("SNOverclock_maxmatk"))
                skill.Variable.Remove("SNOverclock_maxmatk");
            skill.Variable.Add("SNOverclock_maxmatk", maxmatk);
            actor.Status.max_matk_skill += maxmatk;

            if (skill.Variable.ContainsKey("SNOverclock_minmatk"))
                skill.Variable.Remove("SNOverclock_minmatk");
            skill.Variable.Add("SNOverclock_minmatk", minmatk);
            actor.Status.min_matk_skill += minmatk;

            if (skill.Variable.ContainsKey("SNOverclock_hit"))
                skill.Variable.Remove("SNOverclock_hit");
            skill.Variable.Add("SNOverclock_hit", hit);
            actor.Status.hit_ranged_skill += hit;
            actor.Status.hit_melee_skill += hit;

            if (skill.Variable.ContainsKey("SNOverclock_avoidmelee"))
                skill.Variable.Remove("SNOverclock_avoidmelee");
            skill.Variable.Add("SNOverclock_avoidmelee", avoidmelee);
            actor.Status.avoid_melee_skill += avoidmelee;

            if (skill.Variable.ContainsKey("SNOverclock_avoidranged"))
                skill.Variable.Remove("SNOverclock_avoidranged");
            skill.Variable.Add("SNOverclock_avoidranged", avoidranged);
            actor.Status.avoid_ranged_skill += avoidranged;

            actor.Buff.MaxAtkUp = true;
            actor.Buff.MinAtkUp = true;
            actor.Buff.MaxMagicAtkUp = true;
            actor.Buff.MinMagicAtkUp = true;
            actor.Buff.LongHitUp = true;
            actor.Buff.ShortHitUp = true;
            actor.Buff.ShortDodgeUp = true;
            actor.Buff.LongDodgeUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void skill_OnAdditionEnd(Actor actor, DefaultBuff skill)
        {
            actor.Status.max_atk1_skill -= (short)skill.Variable["SNOverclock_maxatk"];
            actor.Status.min_atk1_skill -= (short)skill.Variable["SNOverclock_minatk"];
            actor.Status.max_matk_skill -= (short)skill.Variable["SNOverclock_maxmatk"];
            actor.Status.min_matk_skill -= (short)skill.Variable["SNOverclock_minmatk"];

            actor.Status.hit_ranged_skill -= (short)skill.Variable["SNOverclock_hit"];
            actor.Status.hit_melee_skill -= (short)skill.Variable["SNOverclock_hit"];

            actor.Status.avoid_melee_skill -= (short)skill.Variable["SNOverclock_avoidmelee"];
            actor.Status.avoid_ranged_skill -= (short)skill.Variable["SNOverclock_avoidranged"];
            actor.Buff.MaxAtkUp = false;
            actor.Buff.MinAtkUp = false;
            actor.Buff.MaxMagicAtkUp = false;
            actor.Buff.MinMagicAtkUp = false;
            actor.Buff.LongHitUp = false;
            actor.Buff.ShortHitUp = false;
            actor.Buff.ShortDodgeUp = false;
            actor.Buff.LongDodgeUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }



        #endregion
    }
}
