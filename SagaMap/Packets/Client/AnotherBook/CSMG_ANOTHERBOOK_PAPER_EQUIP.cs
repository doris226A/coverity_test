using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_ANOTHERBOOK_PAPER_EQUIP : Packet
    {
        public CSMG_ANOTHERBOOK_PAPER_EQUIP()
        {
            this.offset = 2;
        }
        public byte paperID
        {
            get
            {
                return GetByte(3);
            }
        }
        public override Packet New()
        {
            return new CSMG_ANOTHERBOOK_PAPER_EQUIP();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnAnotherBookPaperEquip(this);
        }

    }
}