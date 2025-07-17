﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Archer
{
    public class ConArrow: ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            int numdown = 2;
            if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND))
            {
                if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.BOW)
                {
                    if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
                    {
                        if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType == SagaDB.Item.ItemType.ARROW)
                        {
                            if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Stack >= numdown)
                            {
                                return 0;
                            }
                            else
                            {
                                return -55;
                            }
                        }
                        else
                            return -34;
                    }
                    return -34;
                }
                else
                    return -5;
            }
            else
                return -5;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int numdown = 2;
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND))
                {
                    if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.BOW)
                    {
                        if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
                        {
                            if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType == SagaDB.Item.ItemType.ARROW)
                            {
                                if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Stack >= numdown)
                                {
                                    for (int i = 0; i < numdown; i++)
                                        Network.Client.MapClient.FromActorPC(pc).DeleteItem(pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].Slot, 1, false);
                                }

                            }
                        }
                    }
                }
            }
            int combo = 2;
            float factor = 0;
            args.argType = SkillArg.ArgType.Attack;
            switch (level)
            {
                case 1:
                    factor = 0.9f;
                    break;
                case 2:
                    factor = 1.0f;
                    break;
                case 3:
                    factor = 1.1f;
                    break;
                case 4:
                    factor = 1.2f;
                    break;
                case 5:
                    factor = 1.3f;
                    break;
                case 6:
                    factor = 2.2f;
                    break;
            }

            List<Actor> target = new List<Actor>();
            args.delayRate = 2f;
            for (int i = 0; i < combo; i++)
            {
                target.Add(dActor);
            }
            SkillHandler.Instance.PhysicalAttack(sActor, target, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
