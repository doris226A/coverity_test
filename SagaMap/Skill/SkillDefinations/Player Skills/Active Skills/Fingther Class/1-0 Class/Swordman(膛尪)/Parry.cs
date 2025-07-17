using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 防衛板
    /// </summary>
    public class Parry : ISkill
    {
        #region ISkill Members
        // TryCast 方法用於檢查是否能夠施放技能
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            // 檢查技能是否適用於當前武器
            if (!SkillHandler.Instance.CheckSkillCanCastForWeapon(pc, args))
            {
                return -5; // 返回 -5 表示無法施放技能
            }

            // 檢查格擋是否可能並且不在無效狀態
            if (CheckPossible(pc) && args.result != -5)
                return 0; // 返回 0 表示可以施放技能
            else
                return -5; // 返回 -5 表示無法施放技能
        }

        // Proc 方法用於處理技能的實際效果
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 10000; // 效果持續時間，單位為毫秒
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "Parry", lifetime);
            skill.OnAdditionStart += this.StartEventHandler; // 添加效果開始事件處理程序
            skill.OnAdditionEnd += this.EndEventHandler;     // 添加效果結束事件處理程序
            SkillHandler.ApplyAddition(dActor, skill);       // 應用效果到目標角色
        }

        // 當效果開始時觸發的事件處理程序
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_ENTER, skill.skill.Name));
            }
        }

        // 當效果結束時觸發的事件處理程序
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_LEAVE, skill.skill.Name));
            }
        }

        // 檢查格擋是否可能
        bool CheckPossible(Actor sActor)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                // 檢查玩家是否持有右手武器或者右手副手上是否有裝備
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND) || pc.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
                    return true; // 可以格擋
                else
                    return false; // 不能格擋
            }
            else
                return true; // 非玩家角色默認可以格擋
        }

        #endregion
    }
}