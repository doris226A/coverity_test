using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_PLAYER_MIRROR_CHANGE_SETTING_RESULT : Packet
    {
        public SSMG_PLAYER_MIRROR_CHANGE_SETTING_RESULT()
        {
            this.data = new byte[8];
            this.offset = 2;
            this.ID = 0x02B5;
        }

        /// <summary>
        /// 0: 見た目を変更いたしました(Success)
        /// -1: 現在見た目の変更を行えない状態です
        /// -2: イベント中は見た目の変更を行えません
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
