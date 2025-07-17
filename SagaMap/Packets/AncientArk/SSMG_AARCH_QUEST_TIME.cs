using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
namespace SagaMap.Packets.Server
{
    public class SSMG_AARCH_QUEST_TIME : Packet
    {
        public SSMG_AARCH_QUEST_TIME()
        {
            this.data = new byte[200];
            this.offset = 2;
            this.ID = 8107;
        }
        public uint Quest_ID
        {
            set
            {
                this.PutUInt(value);
            }
        }

        public int Time
        {
            set
            {
                this.PutInt(value);
            }
        }
    }
}

