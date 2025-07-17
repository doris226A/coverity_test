using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
namespace SagaMap.Packets.Server
{
    public class SSMG_AARCH_QUEST : Packet
    {
        public SSMG_AARCH_QUEST()
        {
            this.data = new byte[200];
            this.offset = 2;
            this.ID = 8101;//8101


            /*this.PutByte(9, 2);//總印章數(9)
            this.PutByte(9, 39);//要蓋的印章數(9)
            this.PutInt(1, 3);
            this.PutInt(2, 7);
            this.PutInt(3, 11);
            this.PutInt(4, 15);
            this.PutInt(5, 19);
            this.PutInt(6, 23);
            this.PutInt(7, 27);
            this.PutInt(8, 31);
            this.PutInt(9, 35);*/

            this.PutByte(30, 2);//要顯示幾個任務
            this.PutUInt(100021);
            this.PutUInt(100022);
            /*this.PutUInt(100000, 3);//要顯示的任務
            this.PutUInt(100001, 7);
            this.PutUInt(100002, 11);
            this.PutUInt(100003, 15);*/
            /*this.PutUInt(100004);
            this.PutUInt(100005);
            this.PutUInt(100006);
            this.PutUInt(100007);
            this.PutUInt(100010);
            this.PutUInt(100012);
            this.PutUInt(100013);
            this.PutUInt(100014);
            this.PutUInt(100015);
            this.PutUInt(100016);
            this.PutUInt(100017);
            this.PutUInt(100018);
            this.PutUInt(100019);*/
        }


        public byte AA_Clear
        {
            set
            {
                if(value >= 0) this.PutUInt(100000);
                if (value >= 1) this.PutUInt(100001);
                if (value >= 2) this.PutUInt(100002);
                if (value >= 3) this.PutUInt(100003);
                if (value >= 4) this.PutUInt(100004);
                if (value >= 5) this.PutUInt(100005);
                if (value >= 6) this.PutUInt(100006);
                if (value >= 7) this.PutUInt(100007);
            }
        }
        public bool Rebirth
        {
            set
            {
                if (value)
                {
                    this.PutUInt(100012);
                    this.PutUInt(100013);
                    this.PutUInt(100014);
                    this.PutUInt(100015);
                    this.PutUInt(100016);
                    this.PutUInt(100017);
                    this.PutUInt(100018);
                    this.PutUInt(100019);
                    this.PutUInt(100020);
                }
            }
        }
        public bool Six_boss
        {
            set
            {
                if (value)
                {
                    this.PutUInt(100010);
                }
            }
        }
    }
}

