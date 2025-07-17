using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_AAA_GROUP_MEMBER_STAND_BY_STATE_CHANGE : Packet
    {
        public SSMG_AAA_GROUP_MEMBER_STAND_BY_STATE_CHANGE()
        {
            this.data = new byte[8];
            this.offset = 2;
            this.ID = 0x23EF;
        }
        
        public byte Position
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public byte Unknow
        {
            set
            {
                this.PutByte(0x0, 3);
            }
        }

        public byte State
        {
            set
            {
                this.PutByte(value, 4);
            }
        }
    }
}

