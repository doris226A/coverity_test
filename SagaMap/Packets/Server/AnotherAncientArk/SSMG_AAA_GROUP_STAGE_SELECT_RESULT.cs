using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_AAA_GROUP_STAGE_SELECT_RESULT : Packet
    {
        public SSMG_AAA_GROUP_STAGE_SELECT_RESULT()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x23F3;
        }
      
        public byte Type
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public int StageID
        {
            set
            {
                this.PutInt(value, 3);
            }
        }
    }
}

