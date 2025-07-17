using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ITEM_CHANGE_RESULT : Packet
    {
        public SSMG_ITEM_CHANGE_RESULT()
        {
            this.data = new byte[8];
            this.ID = 0x1ebf;
            this.offset = 2;
        }

        public int Result
        {
            set
            {
                this.PutInt(value, 2);
            }
        }
    }
}
