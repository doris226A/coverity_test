using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_AAA_GROUP_SEND_GROUP_STATE : Packet
    {
        public SSMG_AAA_GROUP_SEND_GROUP_STATE()
        {
            this.data = new byte[16];
            this.offset = 2;
            this.ID = 0x23DD;
        }

        public int AAAGroupID
        {
            set
            {
                this.PutInt(value, 3);
            }
        }

        public byte GroupType
        {
            set
            {
                this.PutByte(value, 7);
            }
        }

        public byte GroupSize
        {
            set
            {
                this.PutByte(value, 8);
            }
        }
    }
}
