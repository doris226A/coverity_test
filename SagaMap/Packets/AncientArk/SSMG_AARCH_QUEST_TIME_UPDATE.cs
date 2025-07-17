using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
namespace SagaMap.Packets.Server
{
    public class SSMG_AARCH_QUEST_TIME_UPDATE : Packet
    {
        public SSMG_AARCH_QUEST_TIME_UPDATE()
        {
            this.data = new byte[20];
            this.offset = 2;
            this.ID = 8112;
        }
        public uint Time
        {
            set
            {
                this.PutUInt(value);
            }
        }
    }
}

