
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaDB.Item;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// ラッシュボム（ラッシュボム）J45極速炸彈
    /// </summary>
    public class RushBom : ISkill
    {
        #region ISkill Members
        uint itemID = 10021901;//200耐久陷阱工具
        uint itemID2 = 10022307;//定時炸彈
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            SagaDB.Item.Item item1 = sActor.Inventory.GetItem(itemID, Inventory.SearchType.ITEM_ID);
            SagaDB.Item.Item item2 = sActor.Inventory.GetItem(itemID2, Inventory.SearchType.ITEM_ID);
            if (SkillHandler.Instance.CountItem(sActor, itemID2) > 0)
            {
                SkillHandler.Instance.TakeItem(sActor, itemID2, 1);
                return 0;
            }
            else if (SkillHandler.Instance.CountItem(sActor, itemID) > 0)
            {
                if (item1.Durability > 0)
                {
                    item1.Durability -= 1;
                    Network.Client.MapClient.FromActorPC((ActorPC)sActor).SendItemInfo(item1);
                    return 0;
                }
            }
            return -2;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            uint RushBomSeq = 2411, RushBomSeq2 = 2412;

            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(RushBomSeq, level, 1000));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(RushBomSeq, level, 1000));
            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(RushBomSeq2, level, 0));
            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(RushBomSeq2, level, 2000));
        }
        #endregion
    }
}