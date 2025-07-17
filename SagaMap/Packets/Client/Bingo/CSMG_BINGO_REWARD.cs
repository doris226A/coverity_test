using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB;
using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_BINGO_REWARD : Packet
    {
        public CSMG_BINGO_REWARD()
        {
            this.offset = 2;
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_BINGO_REWARD();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).BingoReward(this);
        }
    }
}
