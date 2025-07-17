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
    public class ASParry : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            // 检查格挡是否可能
            if (CheckPossible(pc) && args.result != -5)
            {
                // 在这里添加一定的概率来决定是否成功格挡攻击
                if (RandomChance(30)) // 假设概率为 30%
                {
                    return 0; // 成功格挡
                }
                else
                {
                    return -5; // 未成功格挡
                }
            }
            else
            {
                return -5;
            }
            //return 0;
            /*   if (!SkillHandler.Instance.CheckSkillCanCastForWeapon(pc, args))
               {
                   return -5;
               }

               // 检查格挡是否可能
               if (CheckPossible(pc) && args.result != -5)
               {
                   // 在这里添加一定的概率来决定是否成功格挡攻击
                   if (RandomChance(30)) // 假设概率为 30%
                   {
                       return 0; // 成功格挡
                   }
                   else
                   {
                       return -5; // 未成功格挡
                   }
               }
               else
               {
                   return -5;
               }*/
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 10000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "Parry", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }


        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_ENTER, skill.skill.Name));
            }
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_LEAVE, skill.skill.Name));
            }
        }

        bool RandomChance(int percentage)
        {
            Random rand = new Random();
            return rand.Next(0, 100) < percentage;
        }

        bool CheckPossible(Actor sActor)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND) || pc.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
        #endregion
    }
}
