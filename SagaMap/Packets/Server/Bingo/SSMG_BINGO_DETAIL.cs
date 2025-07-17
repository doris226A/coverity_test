using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap.Manager;

namespace SagaMap.Packets.Server
{
    public class SSMG_BINGO_DETAIL : Packet
    {
        public SSMG_BINGO_DETAIL()
        {
            this.data = new byte[80];
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
        //第一個印章
        public short Stamp1
        {
            set
            {
                this.PutShort((short)value, 40);
            }
        }
        //第二個印章
        public short Stamp2
        {
            set
            {
                this.PutShort(0, 42);
                this.PutShort(0, 44);
                this.PutShort((short)value, 46);
            }
        }
        //第三個印章
        public short Stamp3
        {
            set
            {
                this.PutShort(0, 48);
                this.PutShort((short)value, 50);
            }
        }
        //第四個印章
        public short Stamp4
        {
            set
            {
                this.PutShort(0, 52);
                this.PutShort((short)value, 54);
            }
        }
        //第五個印章
        public short Stamp5
        {
            set
            {
                this.PutShort(0, 56);
                this.PutShort((short)value, 58);
            }
        }
        //第六個印章
        public short Stamp6
        {
            set
            {
                this.PutShort(0, 60);
                this.PutShort((short)value, 62);
            }
        }
        //第七個印章
        public short Stamp7
        {
            set
            {
                this.PutShort(0, 64);
                this.PutShort((short)value, 66);
            }
        }
        //第八個印章
        public short Stamp8
        {
            set
            {
                this.PutShort(0, 68);
                this.PutShort((short)value, 70);
            }
        }
        //第九個印章
        public short Stamp9
        {
            set
            {
                this.PutShort(0, 72);
                this.PutShort((short)value, 74);
                this.PutShort(0, 76);
            }
        }
    }
}

