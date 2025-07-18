﻿using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_FG_WARE_PUT : Packet
    {
        public CSMG_FG_WARE_PUT()
        {
            this.offset = 2;
        }

        public uint InventoryID
        {
            get
            {
                return this.GetUInt(2);
            }
        }

        public ushort Count
        {
            get
            {
                //return this.GetUShort(6);
                return this.GetUShort(8);//ECOKEY 修正封包讀取
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_FG_WARE_PUT();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnFGardenWarePut(this);
        }

    }
}
