using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_AAA_GROUP_SET_PARTNER_RESULT : Packet
    {
        public SSMG_AAA_GROUP_SET_PARTNER_RESULT()
        {
            this.data = new byte[20];
            this.offset = 2;
            this.ID = 0x23BF;
        }

        public byte Type
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public uint InventoryID
        {
            set
            {
                this.PutUInt(value, 3);
            }
        }

        public ushort Unknow
        {
            set
            {
                this.PutUShort(value, 7);
            }
        }
    }
}
