﻿using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_IRIS_CARD_CLEAR : Packet
    {
        public CSMG_IRIS_CARD_CLEAR()
        {
            this.offset = 2;
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_IRIS_CARD_CLEAR();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnIrisCardClear(this);
        }

    }
}