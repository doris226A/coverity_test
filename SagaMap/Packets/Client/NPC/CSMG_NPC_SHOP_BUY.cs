using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_NPC_SHOP_BUY : Packet
    {
        public CSMG_NPC_SHOP_BUY()
        {
            this.offset = 2;
        }

        public uint[] Goods
        {
            get
            {
                byte num = this.GetByte(2);
                uint[] goods = new uint[num];
                for (int i = 0; i < num; i++)
                {
                    goods[i] = this.GetUInt((ushort)(3 + i * 4));
                }
                return goods;
            }
        }

        public uint[] Counts
        {
            get
            {
                byte num = this.GetByte(2);
                uint[] goods = new uint[num];
                for (int i = 0; i < num; i++)
                {
                    goods[i] = this.GetUInt((ushort)(4 + num * 4 + i * 4));
                }
                return goods;
            }
        }

        public bool IsCreditCardDeal
        {
            get
            {
                byte num = this.GetByte(2);
                var value = this.GetByte((ushort)(4 + 8 * num));
                return value == 1;
            }
        }

        public bool Unknow
        {
            get
            {
                byte num = this.GetByte(2);
                var value = this.GetByte((ushort)(5 + 8 * num));
                return value == 1;
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_NPC_SHOP_BUY();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnNPCShopBuy(this);
        }

    }
}