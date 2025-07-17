using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ANOTHERBOOK_UNEQUIP_RESULT : Packet
    {
        public SSMG_ANOTHERBOOK_UNEQUIP_RESULT()
        {
            this.data = new byte[5];
            this.offset = 2;
            this.ID = 0x23AD;
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

