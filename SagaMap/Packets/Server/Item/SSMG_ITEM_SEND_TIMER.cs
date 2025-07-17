using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ITEM_SEND_TIMER : Packet
    {
        
        public SSMG_ITEM_SEND_TIMER()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x065E;
        }

        public uint Slot
        {
            set
            {
                this.PutUInt(value, 2);
            }
        }

        public uint ItemID
        {
            set
            {
                this.PutUInt(value, 6);
            }
        }
        public uint StartTime
        {
            set
            {
                this.PutUInt(value, 10);
            }
        }
        public uint EndTime
        {
            set
            {
                this.PutUInt(value, 14);
            }
        }
        public ushort Unknow
        {
            set
            {
                this.PutUShort(value, 18);
            }
        }
    }
}

