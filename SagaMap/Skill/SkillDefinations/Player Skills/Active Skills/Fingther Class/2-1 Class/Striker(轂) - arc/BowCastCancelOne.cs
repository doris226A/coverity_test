
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Striker
{
    /// <summary>
    /// 流水之弓
    /// </summary>
    public class BowCastCancelOne : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        /*public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 15000;
            BowCastCancelOneBuff skill = new BowCastCancelOneBuff(args.skill, dActor, lifetime);
            skill.CancelRate = 0.4f + level * 0.1f;//level / 10f;
            SkillHandler.ApplyAddition(dActor, skill);
        }*/
        #endregion
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 15000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "BowCastCancelOne", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.EaseCt_lv = skill.skill.Level;
            SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("流水之弓狀態已進入。");
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.EaseCt_lv = 0;
            SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("流水之弓狀態已解除。");
        }

        #region BowCastCancelOneBuff
        /*public class BowCastCancelOneBuff : DefaultBuff
        {
            /// <summary>
            /// 短縮時間(%)
            /// </summary>
            public float CancelRate;

            public BowCastCancelOneBuff(SagaDB.Skill.Skill skill, Actor actor, int lifetime)
                : base(skill, actor, "BowCastCancelOne", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
            }
        }*/
        #endregion
    }
}
