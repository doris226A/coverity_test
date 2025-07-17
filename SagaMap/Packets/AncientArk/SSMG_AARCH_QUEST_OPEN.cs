using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
namespace SagaMap.Packets.Server
{
    public class SSMG_AARCH_QUEST_OPEN : Packet
    {
        public SSMG_AARCH_QUEST_OPEN()
        {
            this.data = new byte[80];
            this.offset = 2;
            this.ID = 0x1FA6;//8102
        }
    }
}

