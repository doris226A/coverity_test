
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Explorer
{
    /// <summary>
    /// 裝死（死んだふり）
    /// </summary>
    public class FakeDeath : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 60000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "FakeDeath", lifetime, 500);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            skill.OnUpdate += this.TimerUpdate;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            //X
            if (skill.Variable.ContainsKey("FakeDeath_X"))
                skill.Variable.Remove("FakeDeath_X");
            skill.Variable.Add("FakeDeath_X", actor.X);

            //Y
            if (skill.Variable.ContainsKey("FakeDeath_Y"))
                skill.Variable.Remove("FakeDeath_Y");
            skill.Variable.Add("FakeDeath_Y", actor.Y);

            actor.Buff.PlayingDead = true;
            // 隱身技能處理邏輯
            List<ActorMob> actorsInRange = new List<ActorMob>();
            foreach (Actor Mapactor in Manager.MapManager.Instance.GetMap(actor.MapID).Actors.Values)
            {
                if (Mapactor != null && Mapactor.type == ActorType.MOB)
                {
                    actorsInRange.Add((ActorMob)Mapactor);
                }
            }
            foreach (ActorMob actorInRange in actorsInRange)
            {
                if (actorInRange != null && ((ActorEventHandlers.MobEventHandler)actorInRange.e).AI.Hate.ContainsKey(actor.ActorID))
                {
                    ((ActorEventHandlers.MobEventHandler)actorInRange.e).AI.Hate.TryRemove(actor.ActorID, out _);//二哈更改Hate
                }
            }
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

            if (!actor.Status.Additions.ContainsKey("Through"))
            {
                SagaDB.Skill.Skill skillA = SagaDB.Skill.SkillFactory.Instance.GetSkill(100, 1);
                Skill.Additions.Global.DefaultBuff Through = new Skill.Additions.Global.DefaultBuff(skillA, actor, "Through", 60000);
                Skill.SkillHandler.ApplyAddition(actor, Through);
            }
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.Status.Additions.ContainsKey("Through"))
            {
                actor.Status.Additions["Through"].AdditionEnd();
                actor.Status.Additions.TryRemove("Through", out _);
            }
            actor.Buff.PlayingDead = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
            if (actor.X != (short)skill.Variable["FakeDeath_X"] || actor.Y != (short)skill.Variable["FakeDeath_Y"])
            {
                actor.Buff.PlayingDead = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                if (actor.Status.Additions.ContainsKey("Through"))
                {
                    actor.Status.Additions["Through"].AdditionEnd();
                    actor.Status.Additions.TryRemove("Through", out _);
                }
                actor.Status.Additions["FakeDeath"].AdditionEnd();
                actor.Status.Additions.TryRemove("FakeDeath", out _);
            }
        }
        #endregion
    }
}
