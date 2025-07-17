using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_PLAYER_MIRROR_ADD_HAIR_SLOT_RESULT : Packet
    {
        public SSMG_PLAYER_MIRROR_ADD_HAIR_SLOT_RESULT()
        {
            this.data = new byte[8];
            this.offset = 2;
            this.ID = 0x02B9;
        }

        /// <summary>
        /// GAME_LOOKCHGEXTENDMSG_HAIR_ERR0	髪型の保存枠を拡張しました
        /// GAME_LOOKCHGEXTENDMSG_ERR-1	現在見た目の保管枠の拡張を行えない状態です
        /// GAME_LOOKCHGEXTENDMSG_ERR-2	イベント中は見た目の保管枠の拡張を行えません
        /// GAME_LOOKCHGEXTENDMSG_ERR-3	既に全ての枠が開放されています
        /// GAME_LOOKCHGEXTENDMSG_ERR-4	 ECOSHOP にて該当アイテムをご購入ください
        /// GAME_LOOKCHGEXTENDMSG_ERR-4_0	ECOショップ にて「%s」をご購入ください$r今すぐECOショップを開きますか？
        /// GAME_LOOKCHGEXTENDMSG_ERR-4_1	「%s」はアイテムカテゴリ$r「オシャレ系アイテム」にてお求めいただけます
        /// </summary>
        public byte Result
        {
            set
            {
                this.PutByte(value, 2);
            }
        }
    }
}
