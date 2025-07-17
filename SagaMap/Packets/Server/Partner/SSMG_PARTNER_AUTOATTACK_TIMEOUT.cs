using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_PARTNER_AUTOATTACK_TIMEOUT : Packet
    {
        public SSMG_PARTNER_AUTOATTACK_TIMEOUT()
        {
            this.data = new byte[8];
            this.offset = 2;
            this.ID = 0x0426;
        }
    }
}
