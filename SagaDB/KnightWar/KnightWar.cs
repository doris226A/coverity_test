using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SagaDB.KnightWar
{
    /// <summary>
    /// 骑士团演习
    /// </summary>
    public class KnightWar
    {
        public class PcLocation
        {
            public int id;
            public byte x;
            public byte y;
            public string pos;
        }

        public class NpcLocation
        {
            public byte x;
            public byte y;
            public byte dir;
            public uint npcid;
            public string pos;
        }

        uint id;
        string var;
        uint mapID;
        byte maxLV;
        DateTime startTime;
        int duration;
        bool started;
        Dictionary<string, PcLocation> loc = new Dictionary<string, PcLocation>();
        Dictionary<string, NpcLocation> npcloc = new Dictionary<string, NpcLocation>();
        int joinnum;


        public uint ID { get { return this.id; } set { this.id = value; } }

        public string VAR { get { return this.var; } set { this.var = value; } }

        /// <summary>
        /// 演习地图ID
        /// </summary>
        public uint MapID { get { return this.mapID; } set { this.mapID = value; } }

        /// <summary>
        /// 最高等级限制
        /// </summary>
        public byte MaxLV { get { return this.maxLV; } set { this.maxLV = value; } }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get { return this.startTime; } set { this.startTime = value; } }

        /// <summary>
        /// 持续时间（分钟）
        /// </summary>
        public int Duration { get { return this.duration; } set { this.duration = value; } }

        /// <summary>
        /// 參加軍團數量
        /// </summary>
        public int JoinNum { get { return this.joinnum; } set { this.joinnum = value; } }

        /// <summary>
        /// 玩家出現的位置
        /// </summary>
        public Dictionary<string, PcLocation> Loc { get { return this.loc; } }

        /// <summary>
        /// NPC出現的位置
        /// </summary>
        public Dictionary<string, NpcLocation> NpcLoc { get { return this.npcloc; } }

        /// <summary>
        /// 騎士團是否开始
        /// </summary>
        public bool Started { get { return this.started; } set { this.started = value; } }


    }
}
