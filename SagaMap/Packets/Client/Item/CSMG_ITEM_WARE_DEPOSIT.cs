using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_ITEM_WARE_DEPOSIT : Packet
    {
        public CSMG_ITEM_WARE_DEPOSIT()
        {
            this.offset = 2;
        }

        public ulong Gold
        {
            get
            {
                return this.GetULong(2);
            }
        }

        public override Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_ITEM_WARE_DEPOSIT();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnDeposit(this);
        }
    }
}
