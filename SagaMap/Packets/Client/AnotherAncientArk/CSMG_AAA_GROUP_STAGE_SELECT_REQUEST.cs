using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_AAA_GROUP_STAGE_SELECT_REQUEST : Packet
    {
        public CSMG_AAA_GROUP_STAGE_SELECT_REQUEST()
        {
            this.offset = 2;
        }

        public uint StageID
        {
            get
            {
                return this.GetUInt(2);
            }
        }

        public override Packet New()
        {
            return new CSMG_AAA_GROUP_STAGE_SELECT_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnAAAGroupStageSelectRequest(this);
        }
    }
}
