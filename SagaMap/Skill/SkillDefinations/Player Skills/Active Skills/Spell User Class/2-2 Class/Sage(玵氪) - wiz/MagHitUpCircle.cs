
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
    public class MagHitUpCircle : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int[] lifetimes = { 0, 120000, 1350000, 150000, 165000, 180000 };
            float rate = 1.0f + 0.2f * level;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            foreach (Actor act in affected)
            {
                if (act.type == ActorType.PC)
                {
                    MagHitUpCircleBuff skill = new MagHitUpCircleBuff(args.skill, act, lifetimes[level], rate);
                    SkillHandler.ApplyAddition(act, skill);
                    map.SendEffect(act, 5292);
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
                SagaMap.Skill.SkillHandler.SendSystemMessage(actor, "魔法啟示狀態已進入");
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Buff.MagicHitUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                SagaMap.Skill.SkillHandler.SendSystemMessage(actor, "魔法啟示狀態已結束");
            }
        }
        #endregion
    }
}
