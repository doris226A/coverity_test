using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ANOTHERBOOK_WINDOW_UNLOCK_STATE : Packet
    {
        public SSMG_ANOTHERBOOK_WINDOW_UNLOCK_STATE()
        {
            this.data = new byte[5];
            this.offset = 2;
            this.ID = 0x23A0;
        }

        public byte Type
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public ushort UsingPageID
        {
            set
            {
                this.PutUShort(value, 3);
            }
        }
    }
}

