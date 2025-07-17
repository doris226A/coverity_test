using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Item;

namespace SagaMap.Packets.Server
{
    public class SSMG_GOLEM_SHOP_BUY_SETUP : Packet
    {
        private short TitleLength = 0;
        public SSMG_GOLEM_SHOP_BUY_SETUP()
        {
            this.data = new byte[14];
            this.offset = 2;
            this.ID = 0x181B;
            MaxItemCount = 32;
            TitleLength = 0;
        }

        public byte Unknown
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public byte MaxItemCount
        {
            set
            {
                this.PutByte(value, 3);
            }
        }

        public ulong BuyLimit
        {
            set
            {
                this.PutULong(value, 7);
            }
        }

        public string Comment
        {
            set
            {
                byte[] comment = Global.Unicode.GetBytes(value + "\0");
                byte[] buf = new byte[14 + comment.Length];
                this.data.CopyTo(buf, 0);
                this.data = buf;

                this.PutByte((byte)comment.Length, 15);
                this.PutBytes(comment, 16);
                TitleLength = (short)(16 + comment.Length);
            }
        }

        public byte IsAddtoGolemCatalog
        {
            set
            {
                this.PutByte(value, TitleLength + 1);
            }
        }

        public byte GolemShopType
        {
            set
            {
                this.PutByte(value, TitleLength + 2);
            }
        }
    }
}

