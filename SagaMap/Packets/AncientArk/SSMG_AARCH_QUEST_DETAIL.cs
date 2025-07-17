using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap.AncientArks;
using SagaMap.Network.Client;
namespace SagaMap.Packets.Server
{
    public class SSMG_AARCH_QUEST_DETAIL : Packet
    {
        public SSMG_AARCH_QUEST_DETAIL()
        {
            this.data = new byte[200];
            this.offset = 2;
            this.ID = 0x1FAD;//8109
            /*
            this.PutByte(1);//顯示在地圖的第幾格，左上是1
            this.PutUInt(80200000);//顯示的地圖ID
            this.PutUInt(100001);//暫時用途不明，但必須佔一格UInt以下才能正常執行
            this.PutByte(10);//要顯示幾個任務，目前測試最多10個
            this.PutUInt(20010002);//任務ID，詳參aa_gimmick.csv
            this.PutUInt(20000000);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20000001);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20010000);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20010001);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20010002);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20010003);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20010004);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20010005);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutUInt(20010006);//任務ID，詳參aa_gimmick.csv，根據任務數量，可不寫
            this.PutByte((byte)1);//可以讓任務變成藍色，規律暫時不明，需兩個配合
            this.PutByte((byte)0);//可以讓任務變成藍色，規律暫時不明，需兩個配合
            */
        }
        public byte Map_Room_ID
        {
            set
            {
                this.PutByte(value, 2);
            }
        }
        public uint MapID_Ori
        {
            set
            {
                this.PutUInt(value, 3);//這裡要放oriID
            }
        }

        public uint Unknown
        {
            set
            {
                this.PutUInt(value, 7);
            }
        }
        SagaDB.Actor.ActorPC pc;
        public SagaDB.Actor.ActorPC player
        {
            set
            {
                pc = value;
            }
        }

        public AncientArk Gimmick
        {
            set
            {
                byte layer = pc.AncientArk_Layer;
                byte room = pc.AncientArk_Room;
                AncientArkRoom aa_room = value.Detail.Rooms[layer][room];
                int a = 0;
                if (value.Layer_Rooms[layer][room].AncientArk.complete)
                {
                    a = (int)aa_room.gimmick_room_id.Count;
                    this.PutByte((byte)a);//要顯示幾個任務，目前測試最多10個
                    foreach(uint i in aa_room.gimmick_room_id)
                    {
                        this.PutUInt(i);
                    }
                }
                else
                {
                    a = value.Layer_Rooms[layer][room].AncientArk.Gimmick_Complete + 1;
                    this.PutByte((byte)a);//要顯示幾個任務，目前測試最多10個
                    if(aa_room.gimmick_room_id.Count != 0)
                    {
                        switch (a)
                        {
                            case 1:
                                this.PutUInt(aa_room.gimmick_room_id[0]);//任務ID，詳參aa_gimmick.csv
                                this.PutByte(1);//可以讓任務變成藍色
                                break;
                            case 2:
                                this.PutUInt(aa_room.gimmick_room_id[0]);//任務ID，詳參aa_gimmick.csv
                                this.PutUInt(aa_room.gimmick_room_id[1]);//任務ID，詳參aa_gimmick.csv
                                this.PutByte(2);//可以讓任務變成藍色
                                this.PutByte(1);//可以讓任務變成藍色
                                break;
                            case 3:
                                this.PutUInt(aa_room.gimmick_room_id[0]);//任務ID，詳參aa_gimmick.csv
                                this.PutUInt(aa_room.gimmick_room_id[1]);//任務ID，詳參aa_gimmick.csv
                                this.PutUInt(aa_room.gimmick_room_id[2]);//任務ID，詳參aa_gimmick.csv
                                this.PutByte(3);//可以讓任務變成藍色
                                this.PutByte(2);//可以讓任務變成藍色
                                this.PutByte(1);//可以讓任務變成藍色
                                break;
                        }
                    }
                }

                // 1 0 第一個藍色亮
                // 2 1 第二個藍色亮
                // 3 2 1 第三個藍色亮
            }
        }

    }
}

