using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
namespace SagaMap.Packets.Server
{
    public class SSMG_AARCH_QUEST_FINAL : Packet
    {
        public SSMG_AARCH_QUEST_FINAL()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 8114;
        }
    }
}

