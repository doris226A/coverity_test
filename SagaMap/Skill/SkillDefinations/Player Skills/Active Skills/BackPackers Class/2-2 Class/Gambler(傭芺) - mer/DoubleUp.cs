
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 意志昂揚（ダブルアップ）
    /// </summary>
    public class DoubleUp : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*bool active = true;
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "DoubleUp", active);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);*/
            int lifetime = 60000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "DoubleUp", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);

        }
        /*void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            actor.Status.doubleUpList.Add(2127);//連續投擲
            actor.Status.doubleUpList.Add(3286);//擲骰子
            actor.Status.doubleUpList.Add(3287);//幸福輪盤
            actor.Status.doubleUpList.Add(2374);//卡飛旋鏢
            actor.Status.doubleUpList.Add(2375);//一擲千金
            actor.Buff.DoubleUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            actor.Status.doubleUpList.Clear();
            actor.Buff.DoubleUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }*/
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.doubleUpList.Add(2127);//連續投擲
            actor.Status.doubleUpList.Add(3286);//擲骰子
            actor.Status.doubleUpList.Add(3287);//幸福輪盤
            actor.Status.doubleUpList.Add(2374);//卡飛旋鏢
            actor.Status.doubleUpList.Add(2375);//一擲千金
            actor.Buff.DoubleUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5290);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.doubleUpList.Clear();
            actor.Buff.DoubleUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}

