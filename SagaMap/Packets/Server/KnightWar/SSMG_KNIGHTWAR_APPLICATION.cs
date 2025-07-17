using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap.Manager;

namespace SagaMap.Packets.Server
{
    public class SSMG_KNIGHTWAR_APPLICATION : Packet
    {
        public SSMG_KNIGHTWAR_APPLICATION()
        {
            //this.data = new byte[22];
            this.data = new byte[45];
            this.offset = 2;
            this.ID = 0x1B58;
        }
        //這裡填入秒數
        public int Time
        {
            set
            {
                this.PutInt(value, 2);
            }
        }

        public int EastCount
        {
            set
            {
                this.PutInt(value, 6);
            }
        }

        public int WestCount
        {
            set
            {
                this.PutInt(value, 10);
            }
        }

        public int SouthCount
        {
            set
            {
                this.PutInt(value, 14);
            }
        }

        public int NorthCount
        {
            set
            {
                this.PutInt(value, 18);
            }
        }

        public int AA
        {
            set
            {
                this.PutInt(value, 22);
            }
        }
        public int BB
        {
            set
            {
                this.PutInt(value, 26);
            }
        }
        public int CC
        {
            set
            {
                this.PutInt(value, 30);
            }
        }
        public int DD
        {
            set
            {
                this.PutInt(value, 34);
            }
        }

        public int mapid
        {
            set
            {
                this.PutInt(value, 38);
            }
        }
        public int type
        {
            set
            {
                this.PutInt(value, 42);
            }
        }
    }
}

