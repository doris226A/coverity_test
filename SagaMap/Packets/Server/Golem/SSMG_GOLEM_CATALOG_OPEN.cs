using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_GOLEM_CATALOG_OPEN : Packet
    {
        public SSMG_GOLEM_CATALOG_OPEN()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x18E4;
        }

        public uint Type
        {
            set
            {
                this.PutUInt(value, 2);
            }
        }
    }
}
