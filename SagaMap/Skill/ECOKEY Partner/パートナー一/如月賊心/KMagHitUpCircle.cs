
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Sage
{
    /// <summary>
    /// 魔法革命（マジックリベレイション）
    /// </summary>
    public class KMagHitUpCircle : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 60000;
            float rate = 1.2f;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, false);
            foreach (Actor act in affected)
            {
                if (act.type == ActorType.PC || act.type == ActorType.PARTNER || act.type == ActorType.PET || act.type == ActorType.SHADOW)
                {
                    MagHitUpCircleBuff skill = new MagHitUpCircleBuff(args.skill, act, lifetime, rate);
                    SkillHandler.ApplyAddition(act, skill);
                }
            }
        }
        public class MagHitUpCircleBuff : DefaultBuff
        {
            public float Rate;
            public MagHitUpCircleBuff(SagaDB.Skill.Skill skill, Actor actor, int lifetime, float rate)
                : base(skill, actor, "MagHitUpCircle", lifetime)
            {
                Rate = rate;
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Buff.MagicHitUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Buff.MagicHitUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }
        #endregion
    }
}
