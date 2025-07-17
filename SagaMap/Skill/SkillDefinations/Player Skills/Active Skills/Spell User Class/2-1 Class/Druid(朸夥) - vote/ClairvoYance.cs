
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Druid
{
    /// <summary>
    /// 真實之眼（クレアボヤンス）
    /// </summary>
    public class ClairvoYance : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 30000 * level;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "ClairvoYance", lifetime, 1000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            skill.OnUpdate += this.TimerUpdate;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            //List<Actor> actors = Manager.MapManager.Instance.GetMap(actor.MapID).GetActorsArea(actor, 300, true);
            //取得有效Actor（即怪物）
            /*foreach (Actor i in actors)
            {
                i.Buff.Transparent = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, i, false);
            }*/
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(actor.MapID).Actors.Values)
            {
                if (j.type == ActorType.PC && j.Buff.Transparent)
                {
                    ActorPC pp = (ActorPC)j;
                    if (pp.Account.GMLevel <= 200)
                    {
                        pp.Buff.Transparent = false;
                        actor.e.OnActorChangeBuff(pp);
                    }
                }
            }
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(actor.MapID).Actors.Values)
            {
                if (j.type == ActorType.PC && j.Buff.Transparent)
                {
                    ActorPC pp = (ActorPC)j;
                    if (pp.Account.GMLevel <= 200)
                    {
                        pp.Buff.Transparent = false;
                        actor.e.OnActorChangeBuff(pp);
                    }
                }
            }
        }
        #endregion
    }
}
