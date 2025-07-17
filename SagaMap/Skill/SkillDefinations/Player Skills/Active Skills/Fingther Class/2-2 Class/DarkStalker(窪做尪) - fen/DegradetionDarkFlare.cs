using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.DarkStalker
{
    /// <summary>
    /// 魔王之火（ブラックフレア）（黑暗烈焰）
    /// </summary>
    public class DegradetionDarkFlare : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float[] factors = { 0f, 5.0f, 6.5f, 8.0f };
            float factor = factors[level];
            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Dark, factor);
            int rate = 5 + 10 * level;
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, "DegradetionDarkFlare", rate) && !SkillHandler.Instance.isBossMob(dActor))
            {
                DegradetionDarkFlareBuff skill = new DegradetionDarkFlareBuff(sActor, args, dActor, 20000, 2000);
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        public class DegradetionDarkFlareBuff : DefaultBuff
        {
            private SkillArg args;
            private Actor sActor;
            private EffectArg eff;
            public DegradetionDarkFlareBuff(Actor sActor, SkillArg args, Actor actor, int lifetime, int period)
                : base(args.skill, actor, "DegradetionDarkFlareBuff", lifetime, period)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.OnUpdate += this.TimerUpdate;
                this.args = args.Clone();
                this.sActor = sActor;
                this.eff = new EffectArg();
                this.eff.actorID = actor.ActorID;
                this.eff.effectID = 5270;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Buff.Flare = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Buff.Flare = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
            void TimerUpdate(Actor actor, DefaultBuff skill)
            {
                if (actor.HP > 0 && !actor.Buff.Dead)
                {
                    float damagerate = new float[] { 0, 0.009f, 0.012f, 0.015f }[skill.skill.Level];
                    uint HP_Lost = (uint)(actor.MaxHP * damagerate);
                    SkillHandler.Instance.FixAttack(sActor, actor, args, sActor.WeaponElement, HP_Lost);
                    Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                    //map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, args, actor, false);
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, this.eff, actor, true);
                }
                else
                {
                    actor.Status.Additions["DegradetionDarkFlareBuff"].AdditionEnd();
                    actor.Status.Additions.TryRemove("DegradetionDarkFlareBuff", out _);
                }

            }
        }

        #endregion
    }
}
