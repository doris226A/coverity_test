using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Item;

namespace SagaMap.Packets.Server
{

    public class SSMG_PLAYER_SHOP_ANSWER : Packet
    {
        public SSMG_PLAYER_SHOP_ANSWER()
        {
            this.data = new byte[6];
            this.offset = 2;
            this.ID = 0x191B;
        }
        /// <summary>
        /// GAME_SMSG_STALL_VEN_DEAL_ERR1,"露店を見ていないのに取引しようとしました"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR2,"露店を見ている最中に相手が変わりました"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR3,"指定した人は商人ではありません"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR4,"露店を開いていません"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR5,"露店の客として認識されていませんでした"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR6,"指定した露店は自分の露店です"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR7,"取引可能な商品がありませんでした"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR8,"現在の所持金が取引金額に達していませんでした"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR9,"これ以上アイテムを所持することはできません"
        /// GAME_SMSG_STALL_VEN_DEAL_ERR10,"相手の所持金が上限に達したためキャンセルされました"
        /// </summary>
        public int Result
        {
            set
            {
                this.PutInt(value, 2);
            }
        }
    }
}

