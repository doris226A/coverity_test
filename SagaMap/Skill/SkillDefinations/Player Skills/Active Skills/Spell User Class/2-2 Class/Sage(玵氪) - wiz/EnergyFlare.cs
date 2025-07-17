using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Sage
{
    /// <summary>
    /// 燃燒生命（エナジーフレア）
    /// </summary>
    public class EnergyFlare : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (SkillHandler.Instance.CheckValidAttackTarget(pc, dActor))
            {
                return 0;
            }
            else
            {
                return -14;
            }
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 6.0f + 0.7f * level;
            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Neutral, factor);
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, "EnergyFlare", 40) && !SkillHandler.Instance.isBossMob(dActor))
            {
                EnergyFlareBuff skill = new EnergyFlareBuff(args, sActor, dActor, 20000, 2000);
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        public class EnergyFlareBuff : DefaultBuff
        {
            SkillArg args;
            Actor sActor;
            private EffectArg eff;
            public EnergyFlareBuff(SkillArg args, Actor sActor, Actor actor, int lifetime, int period)
                : base(args.skill, actor, "EnergyFlare", lifetime, period)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.OnUpdate += this.UpdateTimeHandler;
                this.args = args.Clone();
                this.sActor = sActor;
                this.eff = new EffectArg();
                this.eff.actorID = actor.ActorID;
                this.eff.effectID = 5275;
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
            void UpdateTimeHandler(Actor actor, DefaultBuff skill)
            {
                if (actor.HP > 0 && !actor.Buff.Dead)
                {
                    Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                    int demage = Math.Min((int)(actor.MaxHP * (0.013f + 0.004f * skill.skill.Level)), 3000);
                    SkillHandler.Instance.FixAttack(sActor, actor, args, SagaLib.Elements.Neutral, demage);
                    //map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, args, actor, false);
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, this.eff, actor, true);
                }
                else
                {
                    actor.Status.Additions["EnergyFlare"].AdditionEnd();
                    actor.Status.Additions.TryRemove("EnergyFlare", out _);
                }
            }
        }
        #endregion
    }
}
