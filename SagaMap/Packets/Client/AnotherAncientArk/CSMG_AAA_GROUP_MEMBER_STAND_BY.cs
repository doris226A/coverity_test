using SagaLib;

namespace SagaMap.Packets.Client
{
    public class CSMG_AAA_GROUP_MEMBER_STAND_BY : Packet
    {
        public CSMG_AAA_GROUP_MEMBER_STAND_BY()
        {
            this.offset = 2;
        }

        public byte Type
        {
            get
            {
                return this.GetByte(2);
            }
        }

        public byte Status
        {
            get
            {
                return this.GetByte(3);
            }
        }

        public override Packet New()
        {
            return new CSMG_AAA_GROUP_MEMBER_STAND_BY();
        }

        public override void Parse(SagaLib.Client client)
        {
            //((MapClient)(client)).OnAAAGroupMemberStandByStatusChange(this);
        }
    }
}
