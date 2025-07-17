using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap.AncientArks;
using SagaMap.Network.Client;
namespace SagaMap.Packets.Server
{
    public class SSMG_AARCH_QUEST_DETAIL_LAYER : Packet
    {
        public SSMG_AARCH_QUEST_DETAIL_LAYER()
        {
            this.data = new byte[200];
            this.offset = 2;
            this.ID = 8108;//8109
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
                this.PutByte(2);
                if (value.Layer_Rooms[layer][room].AncientArk.complete)
                {
                    foreach (uint i in aa_room.gimmick_layer_id)
                    {
                        this.PutUInt(i);
                    }
                    if (value.Layer_Rooms[layer][room].AncientArk.complete_layer)
                    {
                        this.PutByte(0);
                    }
                    else
                    {
                        this.PutByte(4);//可以讓任務變成藍色
                    }
                }
            }
        }

    }
}

