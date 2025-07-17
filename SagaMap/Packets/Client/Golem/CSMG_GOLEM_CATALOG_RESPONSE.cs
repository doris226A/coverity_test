using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_GOLEM_CATALOG_RESPONSE : Packet
    {
        public CSMG_GOLEM_CATALOG_RESPONSE()
        {
            this.offset = 2;
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_GOLEM_CATALOG_RESPONSE();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnGolemCatalogOpenResult(this);
        }
    }
}
