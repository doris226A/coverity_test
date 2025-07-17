using SagaLib;

namespace SagaMap.Packets.Server
{
    // 请求设置故障回路服务器返回封包
    // 1E 53 64 00
    // 0x64 意义不明
    // 0x00 意义不明
    public class SSMG_DEM_DEMIC_ENGAGETASK_RESULT : Packet
    {
        public enum Results
        {
            OK,
            FAILED = -1,
            NOT_ENOUGH_EP = -2
        }
        public SSMG_DEM_DEMIC_ENGAGETASK_RESULT()
        {
            //ECOKEY DEM黃點點結果
            this.data = new byte[8];
            this.offset = 2;
            this.ID = 0x1E53;
        }

        public byte Page
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public Results Result
        {
            set
            {
                this.PutByte((byte)value, 3);
            }
        }
    }
}
