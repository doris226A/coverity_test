using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_PARTNER_EXP_SEND : Packet
    {
        public SSMG_PARTNER_EXP_SEND()
        {
            this.data = new byte[14];
            this.offset = 2;
            this.ID = 0x2190;
        }

        public byte PartnerLevel
        {
            set
            {
                this.PutInt(value, 2);
            }
        }

        public int ExpPercent
        {
            set
            {
                this.PutInt(value, 6);
            }
        }
    }
}
