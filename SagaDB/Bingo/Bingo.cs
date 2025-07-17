using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;

namespace SagaDB.Bingo
{
    /// <summary>
    /// Bingo遊戲
    /// </summary>
    public class BingoData
    {
        /// <summary>
        /// 編號
        /// </summary>
        public byte ID;

        /// <summary>
        /// 類型
        /// </summary>
        public byte Type;

        /// <summary>
        /// 任務的事件名稱（pc.CMask["放這裡的"]）
        /// </summary>
        public string EventName;

        /// <summary>
        /// 任務的事件判斷完成的編號（pc.CMask["XX"].Value）
        /// </summary>
        public uint EventID;

        /// <summary>
        /// 任務所需的次數
        /// </summary>
        public uint AllNum;
    }
    public class BingoReward
    {
        public byte ID;
        public uint ItemID;
        public byte Num;
    }

    public class Bingo
    {
        byte id;
        byte reward;
        Dictionary<byte, uint> nownum = new Dictionary<byte, uint>();
        Dictionary<byte, bool> complete = new Dictionary<byte, bool>();

        /// <summary>
        /// 編號
        /// </summary>
        public byte ID
        {
            get { return this.id; }
            set { this.id = value; }
        }
        /// <summary>
        /// 現在完成的次數
        /// </summary>
        public Dictionary<byte, uint> NowNum { get { return this.nownum; } set { this.nownum = value; } }

        /// <summary>
        /// 是否完成任務
        /// </summary>
        public Dictionary<byte, bool> Complete { get { return this.complete; } set { this.complete = value; } }
        
        /// <summary>
        /// 領獎紀錄
        /// </summary>
        public byte Reward { get { return this.reward; } set { this.reward = value; } }




    }
}
