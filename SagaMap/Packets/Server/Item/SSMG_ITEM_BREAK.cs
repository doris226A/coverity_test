using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaDB.Item;

namespace SagaMap.Packets.Server
{
    public class SSMG_ITEM_BREAK : Packet
    {
        public SSMG_ITEM_BREAK()
        {
            this.data = new byte[9];
            this.offset = 2;
            this.ID = 0x1FD6;
        }

        public List<Item> Pets
        {
            set
            {
                PutByte((byte)value.Count);

                foreach (var item in value)
                {
                    PutUInt(item.Slot);
                }
                PutUInt(0u);
                PutByte(01);
                PutShort(100);
                PutByte(0);
            }
        }
    }
}

