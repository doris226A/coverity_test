using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ITEM_WARE_WITHDRAW_MONEY_RESULT : Packet
    {
        public SSMG_ITEM_WARE_WITHDRAW_MONEY_RESULT()
        {
            this.data = new byte[8];
            this.ID = 0x0A02;
            this.offset = 2;
        }

        public ulong Balance
        {
            set
            {
                this.PutULong(value, 2);
            }
        }
    }
}
