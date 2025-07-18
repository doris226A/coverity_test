﻿using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_AAA_GROUP_SELF_LEAVE_RESULT : Packet
    {
        public SSMG_AAA_GROUP_SELF_LEAVE_RESULT()
        {
            this.data = new byte[3];
            this.offset = 2;
            this.ID = 0x23E6;
        }

        public byte Reason
        {
            set
            {
                this.PutByte(value, 2);
            }
        }
    }
}

