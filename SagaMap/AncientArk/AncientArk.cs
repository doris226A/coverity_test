using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaDB.Partner;
using SagaLib;
using SagaMap.Dungeon;

namespace SagaMap.AncientArks
{
    public class QuestAsk
    {
        public int id;
        public string question;
        public List<string> ask;
    }
    public enum DestroyType
    {
        TimeOver,
        PartyDismiss,
    }
    public enum AncientArkGateType
    {
        Enter,
        East,
        West,
        South,
        North,
        Next,
        End
    }
    /// <summary>
    /// 任務類別
    /// </summary>
    public enum AncientArkType
    {
        /// <summary>
        /// 討罰
        /// </summary>
        HUNT = 1,
        /// <summary>
        /// 點燃全部蠟燭
        /// </summary>
        CANDLE_1,
        /// <summary>
        /// 點燃正確蠟燭
        /// </summary>
        CANDLE_2,
        /// <summary>
        /// 特定職業蠟燭
        /// </summary>
        CANDLE_3,
        /// <summary>
        /// 特定種族蠟燭
        /// </summary>
        CANDLE_4,
        /// <summary>
        /// 消滅所有蠟燭
        /// </summary>
        CANDLE_5,
        /// <summary>
        /// 全員摸蠟燭
        /// </summary>
        CANDLE_6,
        /// <summary>
        /// 等待指定秒數
        /// </summary>
        WAIT,
        /// <summary>
        /// 破壞正確箱子
        /// </summary>
        HUNT_BOX,
        /// <summary>
        /// 回答問題
        /// </summary>
        ASK,
        /// <summary>
        /// 討伐（無公告）
        /// </summary>
        HUNT_NO,
        /// <summary>
        /// 特殊
        /// </summary>
        SPECIAL,
    }
    /*public class AncientArk_Map
    {
        /// <summary>
        /// AA ID
        /// </summary>
        public uint AncientArkID;

        /// <summary>
        /// 是第幾層
        /// </summary>
        public byte layer;

        /// <summary>
        /// 是第幾個房間
        /// </summary>
        public byte room;

    }*/
    public class AncientArkGimmick
    {

        /// <summary>
        /// Gimmick的ID
        /// </summary>
        public uint id;

        /// <summary>
        /// Gimmick的名稱
        /// </summary>
        public string name;

        /// <summary>
        /// Gimmick的類型
        /// </summary>
        public AncientArkType type;

        /// <summary>
        /// 任務對象的ID1
        /// </summary>
        public uint Current_ID1;

        /// <summary>
        /// 任務對象的ID2
        /// </summary>
        public uint Current_ID2;

        /// <summary>
        /// 任務對象的ID3
        /// </summary>
        public uint Current_ID3;

        /// <summary>
        /// 任務對象的數量
        /// </summary>
        public int CurrentCount;

        /// <summary>
        /// 任務的時間懲罰
        /// </summary>
        public int Time_penalty;
    }
    public class AncientArkWarp
    {
        /// <summary>
        /// 地圖ID
        /// </summary>
        public uint MapID;
        /// <summary>
        /// [方向,X軸]
        /// </summary>
        public Dictionary<AncientArkGateType, byte> Warp_X = new Dictionary<AncientArkGateType, byte>();
        /// <summary>
        /// [方向,Y軸]
        /// </summary>
        public Dictionary<AncientArkGateType, byte> Warp_Y = new Dictionary<AncientArkGateType, byte>();

    }
    public class AncientArkRoom
    {
        /// <summary>
        /// 層數
        /// </summary>
        public byte Layer;

        /// <summary>
        /// 房間ID 顯示在地圖的位置
        /// </summary>
        public byte Room_ID;

        /// <summary>
        /// 地圖ID
        /// </summary>
        public uint mapID;
/*
        /// <summary>
        /// 進場的位置X
        /// </summary>
        public byte into_x;

        /// <summary>
        /// 進場的位置Y
        /// </summary>
        public byte into_y;
*/
        /// <summary>
        /// 這個房間的所有任務ID
        /// </summary>
        public List<uint> gimmick_room_id = new List<uint>();

        /// <summary>
        /// 通關這層的任務ID
        /// </summary>
        public List<uint> gimmick_layer_id = new List<uint>();


        /// <summary>
        /// 這層會出現的房間傳送點[方向,房間號]
        /// </summary>
        public Dictionary<AncientArkGateType, byte> Rooms_warp = new Dictionary<AncientArkGateType, byte>();

        /// <summary>
        /// 這層的怪物生成
        /// </summary>
        public string spawnfile;
    }
    public class AncientArkInfo
    {
        uint id;
        string name;
        int time;
        byte lv;
        bool rebirth;

        /// <summary>
        /// 任務ID
        /// </summary>
        public uint ID { get { return this.id; } set { this.id = value; } }
        /// <summary>
        /// 任務名稱
        /// </summary>
        public string Name { get { return this.name; } set { this.name = value; } }

        /// <summary>
        /// 任務時間限制（秒）
        /// </summary>
        public int TimeLimit { get { return this.time; } set { this.time = value; } }

        /// <summary>
        /// 等級限制
        /// </summary>
        public byte Level { get { return this.lv; } set { this.lv = value; } }
        
        /// <summary>
        /// 轉生限制
        /// </summary>
        public bool Rebirth { get { return this.rebirth; } set { this.rebirth = value; } }
        /*
        /// <summary>
        /// 層數 / 該層的房間
        /// </summary>
        public Dictionary<byte, List<AncientArkRoom>> Rooms = new Dictionary<byte, List<AncientArkRoom>>();
        */
        /// <summary>
        /// [層數 , [房間ID,房間地圖]]
        /// </summary>
        public Dictionary<byte, Dictionary<byte, AncientArkRoom>> Rooms { get { return this.rooms; } set { this.rooms = value; } }

        Dictionary<byte, Dictionary<byte, AncientArkRoom>> rooms = new Dictionary<byte, Dictionary<byte, AncientArkRoom>>();


        public string Treasurefile;
        public override string ToString()
        {
            return this.name;
        }
    }

    //地圖副本專用-會變動
    public class AncientArkRoom_Maps
    {

        /// <summary>
        /// AA ID
        /// </summary>
        public uint AncientArkID;
        /// <summary>
        /// 層數
        /// </summary>
        public byte Layer;

        /// <summary>
        /// 房間ID 顯示在地圖的位置 25之一
        /// </summary>
        public byte Room_ID;

        /// <summary>
        /// 是否完成這個房間
        /// </summary>
        public bool complete;


        /// <summary>
        /// 是否完成階層任務
        /// </summary>
        public bool complete_layer;
        /// <summary>
        /// 當前階層任務
        /// </summary>
        public uint Gimmick_ID_layer;

        uint gimmick_id;
        int gimmick_count;
        byte gimmick_complete;

        /// <summary>
        /// 當前gimmickID
        /// </summary>
        public uint Gimmick_ID { get { return this.gimmick_id; } set { this.gimmick_id = value; } }

        /// <summary>
        /// 當前gimmick完成數量/討罰怪物/倒數
        /// </summary>
        public int Gimmick_Count { get { return this.gimmick_count; } set { this.gimmick_count = value; } }

        /// <summary>
        /// 當前完成gimmick數量
        /// </summary>
        public byte Gimmick_Complete { get { return this.gimmick_complete; } set { this.gimmick_complete = value; } }


        Tasks.AncientArk.AncientArkQuest task;
        /// <summary>
        /// 任務時間倒數Task
        /// </summary>
        public Tasks.AncientArk.AncientArkQuest QuestTask { get { return this.task; } set { this.task = value; } }

        /// <summary>
        /// 問答專用ID紀錄
        /// </summary>
        public QuestAsk Ask;
    }

    public class AncientArk
    {
        public AncientArk(uint id)
        {
            int ran = SagaLib.Global.Random.Next(1, 3);
            switch (ran)
            {
                case 1:
                    info = AncientArkFactory.Instance.Items[id];
                    break;
                case 2:
                    info = AncientArkMode2Factory.Instance.Items[id];
                    break;
                case 3:
                    info = AncientArkMode3Factory.Instance.Items[id];
                    break;
            }
            //info = AncientArkFactory.Instance.Items[id];
        }
        AncientArkInfo info;
        DateTime endTime;
        ActorPC creator;
        int time;
        Dictionary<byte, Map> maps = new Dictionary<byte, Map>();
        Tasks.AncientArk.AncientArk task;

        uint aa_id;

        /// <summary>
        /// 圖書館開圖ID，參考dungeonID
        /// </summary>
        public uint AncientArkID { get { return aa_id; } set { aa_id = value; } }

        /// <summary>
        /// 任務資訊
        /// </summary>
        public AncientArkInfo Detail { get { return info; } }



        /// <summary>
        /// 任務時間倒數Task
        /// </summary>
        public Tasks.AncientArk.AncientArk DestroyTask { get { return this.task; } set { this.task = value; } }

        /// <summary>
        /// 剩餘時間
        /// </summary>
        public int Time { get { return this.time; } set { this.time = value; } }


        /// <summary>
        /// 創建者
        /// </summary>
        public ActorPC Creator { get { return this.creator; } set { this.creator = value; } }

        /// <summary>
        /// [層數 , [房間ID,房間地圖]]
        /// </summary>
        public Dictionary<byte, Dictionary<byte, Map>> Layer_Rooms { get { return this.layer_rooms; } set { this.layer_rooms = value; } }

        Dictionary<byte, Dictionary<byte, Map>> layer_rooms = new Dictionary<byte, Dictionary<byte, Map>>();


        /*
        /// <summary>
        /// 層數 / 地圖
        /// </summary>
        public Dictionary<byte, Map> Maps { get { return this.maps; } set { this.maps = value; } }


        /// <summary>
        /// 層數 / 一層有好多房間
        /// </summary>
        public Dictionary<byte, Dictionary<byte, Map>> Layer_Rooms { get { return this.layer_rooms; } set { this.layer_rooms = value; } }

        byte layer_now;
        /// <summary>
        /// 當前到達層數
        /// </summary>
        public byte Layer_Now { get { return this.layer_now; } set { this.layer_now = value; } }*/

        /*uint gimmick_id;
        int gimmick_count;
        byte gimmick_complete;

        /// <summary>
        /// 當前gimmickID
        /// </summary>
        public uint Gimmick_ID { get { return this.gimmick_id; } set { this.gimmick_id = value; } }

        /// <summary>
        /// 當前gimmick完成數量/討罰怪物/倒數
        /// </summary>
        public int Gimmick_Count { get { return this.gimmick_count; } set { this.gimmick_count = value; } }

        /// <summary>
        /// 當前完成gimmick數量
        /// </summary>
        public byte Gimmick_Complete { get { return this.gimmick_complete; } set { this.gimmick_complete = value; } }

        Dictionary<byte, bool> layer_complete = new Dictionary<byte, bool>();
        /// <summary>
        /// 目前層數完成進度 層數/是否完成
        /// </summary>
        public Dictionary<byte, bool> Layer_Complete { get { return this.layer_complete; } set { this.layer_complete = value; } }
        */
        public void Destory(DestroyType type)
        {
            switch (type)
            {
                case DestroyType.PartyDismiss:
                    {
                        foreach (byte i in Layer_Rooms.Keys)
                        {
                            foreach (Map j in Layer_Rooms[i].Values)
                            {
                                if(j.AncientArk.QuestTask != null)
                                {
                                    j.AncientArk.QuestTask.Deactivate();
                                    j.AncientArk.QuestTask = null;
                                }
                                Manager.MapManager.Instance.DeleteMapInstance(j.ID);
                            }
                        }
                        maps.Clear();
                        this.Creator.AncientArkID = 0;
                        AncientArkFactory.Instance.RemoveAncientArk(this.AncientArkID);
                    }
                    break;
                case DestroyType.TimeOver:
                    {
                        foreach (byte i in Layer_Rooms.Keys)
                        {
                            foreach (Map j in Layer_Rooms[i].Values)
                            {
                                Manager.MapManager.Instance.DeleteMapInstance(j.ID);
                            }
                        }

                        maps.Clear();
                        this.Creator.AncientArkID = 0;
                        this.Creator.AncientArk_QuestID = 0;
                        AncientArkFactory.Instance.RemoveAncientArk(this.AncientArkID);
                    }
                    break;
            }
            
            
        }

        public void Quest(ActorPC pc)
        {
            
        }
    }
}
