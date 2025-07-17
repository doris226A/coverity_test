using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Item;

namespace SagaMap.Packets.Server
{
    public class SSMG_PLAYER_SHOP_CLOSE : Packet
    {
        public SSMG_PLAYER_SHOP_CLOSE()
        {
            this.data = new byte[6];
            this.offset = 2;
            this.ID = 0x1916;
        }

        /// <summary>
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR1,"露店商の商品は売り切れました"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR2,"露店商が商品の再設定をしています"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR3,"露店商が遠く離れました"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR4,"露店商がマップ移動しました"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR5,"露店商がトレードを始めました"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR6,"露店商が憑依しました"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR8,"露店商が行動不能状態となりました"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR9,"露店商がいなくなりました"
        /// GAME_SMSG_STALL_VEN_CLOSE_ERR10,"露店商がイベントを始めました"
        /// </summary>
        public int Reason
        {
            set
            {
                this.PutInt(value, 2);
            }
        }
    }
}

