using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gunner
{
    /// <summary>
    /// 重點射擊（バイタルショット）
    /// </summary>
    public class CKRVitalShot : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (SkillHandler.Instance.CheckValidAttackTarget(sActor, dActor))
            {
                return 0;
            }
            else
            {
                return -5;
            }
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 2.1f + 0.1f * 3;
            int rateDown = 25 + 5 * 3, rateSlow = 20 + 5 * 3;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            if (SagaLib.Global.Random.Next(0, 99) < rateDown)
            {
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "VitalShot", 15000);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.鈍足, rateSlow))
            {
                int lifetime = 4000 + 500 * 3;
                Additions.Global.MoveSpeedDown skill2 = new SagaMap.Skill.Additions.Global.MoveSpeedDown(args.skill, dActor, lifetime);
                SkillHandler.ApplyAddition(dActor, skill2);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            //近命中
            int hit_melee_add = -(int)(actor.Status.hit_melee * (0.05f + 0.1f * 3));
            if (skill.Variable.ContainsKey("VitalShot_hit_melee"))
                skill.Variable.Remove("VitalShot_hit_melee");
            skill.Variable.Add("VitalShot_hit_melee", hit_melee_add);
            actor.Status.hit_melee_skill = (short)hit_melee_add;

            //遠命中
            int hit_ranged_add = -(int)(actor.Status.hit_ranged * (0.13f + 0.09f * 3));
            if (skill.Variable.ContainsKey("VitalShot_hit_ranged"))
                skill.Variable.Remove("VitalShot_hit_ranged");
            skill.Variable.Add("VitalShot_hit_ranged", hit_ranged_add);
            actor.Status.hit_ranged_skill = (short)hit_ranged_add;

            //近戰迴避
            int avoid_melee_add = -(int)(actor.Status.avoid_melee * (0.1f + 0.02f * 3));
            if (skill.Variable.ContainsKey("VitalShot_avoid_melee"))
                skill.Variable.Remove("VitalShot_avoid_melee");
            skill.Variable.Add("VitalShot_avoid_melee", avoid_melee_add);
            actor.Status.avoid_melee_skill = (short)avoid_melee_add;

            //遠距迴避
            int avoid_ranged_add = -(int)(actor.Status.avoid_ranged * (0.09f + 0.03f * 3));
            if (skill.Variable.ContainsKey("VitalShot_avoid_ranged"))
                skill.Variable.Remove("VitalShot_avoid_ranged");
            skill.Variable.Add("VitalShot_avoid_ranged", avoid_ranged_add);
            actor.Status.avoid_ranged_skill = (short)avoid_ranged_add;
            actor.Buff.ShortHitDown = true;
            actor.Buff.LongHitDown = true;
            actor.Buff.ShortDodgeDown = true;
            actor.Buff.LongDodgeDown = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //近命中
            actor.Status.hit_melee_skill -= (short)skill.Variable["VitalShot_hit_melee"];

            //遠命中
            actor.Status.hit_ranged_skill -= (short)skill.Variable["VitalShot_hit_ranged"];

            //近戰迴避
            actor.Status.avoid_melee_skill -= (short)skill.Variable["VitalShot_avoid_melee"];

            //遠距迴避
            actor.Status.avoid_ranged_skill -= (short)skill.Variable["VitalShot_avoid_ranged"];
            actor.Buff.ShortHitDown = false;
            actor.Buff.LongHitDown = false;
            actor.Buff.ShortDodgeDown = false;
            actor.Buff.LongDodgeDown = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}
