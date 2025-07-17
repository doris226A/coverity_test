using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap.Manager;

namespace SagaMap.Packets.Server
{
    public class SSMG_BINGO_REWARD : Packet
    {
        public SSMG_BINGO_REWARD()
        {
            this.data = new byte[41];
            this.offset = 2;
            this.ID = 0x240E;



            this.PutByte(9, 2);//總印章數(9)
            this.PutByte(9, 39);//要蓋的印章數(9)
            this.PutInt(1, 3);
            this.PutInt(2, 7);
            this.PutInt(3, 11);
            this.PutInt(4, 15);
            this.PutInt(5, 19);
            this.PutInt(6, 23);
            this.PutInt(7, 27);
            this.PutInt(8, 31);
            this.PutInt(9, 35);
        }
        public short rewardstamp
        {
            set
            {
                this.PutShort((byte)value, 39);
            }
        }
    }
}

