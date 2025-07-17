using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Sorcerer
{
    /// <summary>
    /// 透明化（インビジブル）
    /// </summary>
    public class Invisible : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            ActorPC sPC = (ActorPC)sActor;

            if (sPC.Mode == PlayerMode.KNIGHT_EAST ||
                sPC.Mode == PlayerMode.KNIGHT_WEST ||
                sPC.Mode == PlayerMode.KNIGHT_SOUTH ||
                sPC.Mode == PlayerMode.KNIGHT_NORTH)
            {
                foreach (Actor act in affected)
                {
                    if (act.type == ActorType.PC)
                    {
                        if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                        {
                            DefaultBuff skill = new DefaultBuff(args.skill, act, "Invisible", 30000);
                            skill.OnAdditionStart += this.StartEventHandler;
                            skill.OnAdditionEnd += this.EndEventHandler;
                            SkillHandler.ApplyAddition(act, skill);
                        }
                    }
                }
            }
            else
            {
                foreach (Actor act in affected)
                {
                    if (act.type == ActorType.PC)
                    {
                        ActorPC aPC = (ActorPC)act;
                        if (aPC.Party != null && sPC.Party != null)
                        {
                            if ((aPC.Party.ID == sPC.Party.ID) && aPC.Party.ID != 0 && !aPC.Buff.Dead && aPC.PossessionTarget == 0)
                            {
                                if (aPC.Party.ID == sPC.Party.ID)
                                {
                                    DefaultBuff skill = new DefaultBuff(args.skill, act, "Invisible", 30000);
                                    skill.OnAdditionStart += this.StartEventHandler;
                                    skill.OnAdditionEnd += this.EndEventHandler;
                                    SkillHandler.ApplyAddition(act, skill);
                                }
                            }
                        }
                        else
                        {
                            if (act.ActorID == sActor.ActorID)
                            {
                                DefaultBuff skill = new DefaultBuff(args.skill, act, "Invisible", 30000);
                                skill.OnAdditionStart += this.StartEventHandler;
                                skill.OnAdditionEnd += this.EndEventHandler;
                                SkillHandler.ApplyAddition(act, skill);
                            }
                        }
                    }
                }
            }


        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Transparent = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

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
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Transparent = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
