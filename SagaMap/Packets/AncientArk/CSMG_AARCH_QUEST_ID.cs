using SagaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SagaMap.Network.Client;


namespace SagaMap.Packets.Client
{
    public class CSMG_AARCH_QUEST_ID : Packet
    {
        public CSMG_AARCH_QUEST_ID()
        {
            this.offset = 2;
        }

        public uint QuestID
        {
            get
            {
                return this.GetUInt(2);
            }
        }

        public override Packet New()
        {
            return new CSMG_AARCH_QUEST_ID();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnAncientArkQuest(this);
        }
    }
}
