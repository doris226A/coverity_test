using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ANOTHERBOOK_PAPER_USE_RESULT : Packet
    {
        public SSMG_ANOTHERBOOK_PAPER_USE_RESULT()
        {
            this.data = new byte[13];
            this.offset = 2;
            this.ID = 0x23A7;
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
        public ulong PageValue
        {
            set
            {
                this.PutULong(value, 5);
            }
        }
    }
}

