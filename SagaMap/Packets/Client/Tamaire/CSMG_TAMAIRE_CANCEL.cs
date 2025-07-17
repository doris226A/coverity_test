using SagaLib;
using SagaMap.Network.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaMap.Packets.Client
{
    public class CSMG_TAMAIRE_CANCEL : Packet
    {
        public CSMG_TAMAIRE_CANCEL()
        {
            this.offset = 2;
        }

        public override Packet New()
        {
            return new CSMG_TAMAIRE_CANCEL();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).CloseTamaireListUI();
        }
    }
}
