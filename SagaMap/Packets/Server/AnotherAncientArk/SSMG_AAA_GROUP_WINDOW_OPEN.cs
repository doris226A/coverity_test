using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_AAA_GROUP_WINDOW_OPEN : Packet
    {
        public SSMG_AAA_GROUP_WINDOW_OPEN()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x1FA6;
        }

        public byte Type
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public byte SelfLocation
        {
            set
            {
                this.PutByte(value, 6);
            }
        }
    }
}
