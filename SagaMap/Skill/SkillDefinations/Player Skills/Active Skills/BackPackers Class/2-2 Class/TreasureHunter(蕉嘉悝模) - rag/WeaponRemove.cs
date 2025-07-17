
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.TreasureHunter
{
    /// <summary>
    /// 武裝解除（ウエポンキャプチャー）
    /// </summary>
    public class WeaponRemove : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (Skill.SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.ROPE) || sActor.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
            {
                return 0;
            }
            return -5;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0.01f;
            int atk = 0;
            atk = SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            if (atk == 0)
            {
                return;
            }
            if (dActor.type == ActorType.PC)
            {
                int dePossessionRate = 10 + 10 * level;
                if (SagaLib.Global.Random.Next(0, 99) < dePossessionRate)
                {
                    ActorPC actor = (ActorPC)dActor;
                    SkillHandler.Instance.PossessionCancel(actor, SagaLib.PossessionPosition.RIGHT_HAND);

                    if (actor.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND))
                    {
                        SagaMap.Network.Client.MapClient client = SagaMap.Network.Client.MapClient.FromActorPC(actor);
                        SagaDB.Item.Item item = actor.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND];
                        item = actor.Inventory.Equipments[item.EquipSlot[0]];
                        Packets.Client.CSMG_ITEM_MOVE p = new SagaMap.Packets.Client.CSMG_ITEM_MOVE();
                        p.data = new byte[20];
                        p.Target = SagaDB.Item.ContainerType.BODY;
                        p.InventoryID = item.Slot;
                        p.Count = item.Stack;
                        client.OnItemMove(p);
                    }
                }
            }
        }
        #endregion
    }
}