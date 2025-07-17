using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Scout
{
    public class Hiding2 : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (pc.Status.Additions.ContainsKey("Hiding"))
                return -7;
            else
                return 0;
            /*   if (pc.Status.Additions.ContainsKey("Hiding"))
                   return -7;
               //騎士團時禁止施放
               if (pc.Mode == PlayerMode.KNIGHT_EAST ||
                       pc.Mode == PlayerMode.KNIGHT_WEST ||
                       pc.Mode == PlayerMode.KNIGHT_SOUTH ||
                       pc.Mode == PlayerMode.KNIGHT_NORTH)
                   return -146;
               else
                   return 0;*/
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.dActor = 0;//不显示效果
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Hiding", 5000 * level);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);

        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            SkillHandler.Instance.ShowEffect((ActorPC)actor, actor, 4102);
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
