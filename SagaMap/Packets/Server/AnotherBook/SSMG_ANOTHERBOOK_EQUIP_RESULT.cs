using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ANOTHERBOOK_PAPER_EQUIP_RESULT : Packet
    {
        public SSMG_ANOTHERBOOK_PAPER_EQUIP_RESULT()
        {
            this.data = new byte[5];
            this.offset = 2;
            this.ID = 0x23AB;
        }

        public byte Result
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public ushort PaperID
        {
            set
            {
                this.PutUShort(value, 3);
            }
        }
    }
}

