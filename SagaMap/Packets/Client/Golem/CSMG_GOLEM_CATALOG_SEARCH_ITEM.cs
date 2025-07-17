using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_GOLEM_CATALOG_SEARCH_ITEM : Packet
    {
        public CSMG_GOLEM_CATALOG_SEARCH_ITEM()
        {
            this.offset = 2;
        }

        public uint ItemID
        {
            get
            {
                return this.GetUInt(2);
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_GOLEM_CATALOG_SEARCH_ITEM();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnGolemCatalogSearchItemID(this);
        }
    }
}
