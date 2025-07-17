﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 彈飛
    /// </summary>
    public class MPCharge : ISkill
    {
        #region ISkill Members
        bool MobUse;
        public MPCharge()
        {
            this.MobUse = false;
        }
        public MPCharge(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
            /* if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
             {
                 if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType == SagaDB.Item.ItemType.SHIELD)
                     return 0;
             }
             return -5;*/
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

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0;
            args.dActor = sActor.ActorID;
            if (MobUse)
            {
                level = 3;
            }
            if (CheckPossible(sActor))
            {
                args.type = ATTACK_TYPE.BLOW;
                factor = 2.0f;

                SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);

                //推开3格
                SkillHandler.Instance.PushBack(sActor, dActor, 4);
            }
        }

        #endregion
    }
}
