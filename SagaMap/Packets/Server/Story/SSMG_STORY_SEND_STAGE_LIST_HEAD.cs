using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_STORY_SEND_STAGE_LIST_HEAD : Packet
    {
        public SSMG_STORY_SEND_STAGE_LIST_HEAD()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x1CB6;
        }
    }
}
