using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_ANOTHERBOOK_PAPER_UNEQUIP : Packet
    {
        public CSMG_ANOTHERBOOK_PAPER_UNEQUIP()
        {
            this.offset = 2;
        }
        public override SagaLib.Packet New()
        {
            return new CSMG_ANOTHERBOOK_PAPER_UNEQUIP();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnAnotherBookPaperUnequip(this);
        }

    }
}