﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.ActorEventHandlers;
namespace SagaMap.Skill.SkillDefinations.Necromancer
{
    /// <summary>
    /// 恐怖（テラー）
    /// </summary>
    public class Terror : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 2000 + 1000 * level;
            int rate = 10 * level;
            Map map = Manager.MapManager.Instance.GetMap(dActor.MapID);
            List<Actor> affected = map.GetActorsArea(dActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, "Terror", rate))
                    {
                        DefaultBuff skill = new DefaultBuff(args.skill, act, "Terror", lifetime);
                        skill.OnAdditionStart += this.StartEventHandler;
                        skill.OnAdditionEnd += this.EndEventHandler;
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            //目標逃亡時撞到牆壁為止
            if (actor.type == ActorType.MOB)
            {
                MobEventHandler mh = (MobEventHandler)actor.e;
                mh.AI.Mode = new SagaMap.Mob.AIMode(9);
            }
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.MOB)
            {
                ActorMob m = (ActorMob)actor;
                MobEventHandler eh = (MobEventHandler)actor.e;
                if (SagaMap.Mob.MobAIFactory.Instance.Items.ContainsKey(m.MobID))
                    eh.AI.Mode = SagaMap.Mob.MobAIFactory.Instance.Items[m.MobID];
                else
                    eh.AI.Mode = new SagaMap.Mob.AIMode(0);
            }

        }
        #endregion
    }
}
