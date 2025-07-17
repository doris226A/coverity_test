using SagaLib;

namespace SagaMap.Packets.Client
{
    public class CSMG_AAA_GROUP_MEMBER_LEAVE_REQUEST : Packet
    {
        public CSMG_AAA_GROUP_MEMBER_LEAVE_REQUEST()
        {
            this.offset = 2;
        }

        public override Packet New()
        {
            return new CSMG_AAA_GROUP_MEMBER_LEAVE_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            //((MapClient)(client)).OnAAAGroupMemberLeaveRequest(this);
        }
    }
}
