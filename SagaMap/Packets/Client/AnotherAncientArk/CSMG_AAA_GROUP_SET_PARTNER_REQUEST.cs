using SagaLib;

namespace SagaMap.Packets.Client
{
    public class CSMG_AAA_GROUP_SET_PARTNER_REQUEST : Packet
    {
        public CSMG_AAA_GROUP_SET_PARTNER_REQUEST()
        {
            this.offset = 2;
        }

        public byte Group
        {
            get
            {
                return this.GetByte(2);
            }
        }

        public byte Location
        {
            get
            {
                return this.GetByte(3);
            }
        }

        public int InventoryID
        {
            get
            {
                return this.GetInt(4);
            }
        }

        public override Packet New()
        {
            return new CSMG_AAA_GROUP_SET_PARTNER_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            //((MapClient)(client)).OnAAAGroupSetPartnerRequest(this);
        }
    }
}
