using SagaLib;
using System.Collections.Generic;

namespace SagaMap.Packets.Server
{
    public class SSMG_STORY_STAGE_LIST : Packet
    {
        public SSMG_STORY_STAGE_LIST()
        {
            this.data = new byte[35];
            this.offset = 2;
            this.ID = 0x1CB8;
        }

        public List<int> StageList
        {
            set
            {
                this.PutByte((byte)value.Count, 2);
                for (int i = 0; i < value.Count; i++)
                {
                    this.PutInt(value[i], (ushort)(3 + 4 * i));
                }
            }
        }
    }
}
