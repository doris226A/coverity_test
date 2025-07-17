using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SagaDB.Synthese
{
    /// <summary>
    /// 合成信息
    /// </summary>
    public class SyntheseInfo
    {
        uint id;
        byte getexpminlevel;
        byte getexpmaxlevel;
        byte getexpminjoblevel;
        byte getexpmaxjoblevel;
        ushort skillid;
        byte skilllv;
        ushort passiveskillid;
        uint gold;
        uint requiredTool;
        List<ItemElement> material = new List<ItemElement>();
        List<ItemElement> product = new List<ItemElement>();

        /// <summary>
        /// 合成ID
        /// </summary>
        public uint ID { get { return id; } set { id = value; } }

        /// <summary>
        /// 玩家可获取经验的最小等级
        /// </summary>
        public byte GetExpMinLevel { get { return getexpminlevel; }  set { getexpminlevel = value; } }

        /// <summary>
        /// 玩家可获取经验的最打等级
        /// </summary>
        public byte GetExpMaxLevel { get { return getexpmaxlevel; } set { getexpmaxlevel = value; } }

        /// <summary>
        /// 玩家可获取经验的最小职业等级
        /// </summary>
        public byte GetExpMinJobLevel { get { return getexpminjoblevel; } set { getexpminjoblevel = value; } }

        /// <summary>
        /// 玩家可获取经验的最大职业等级
        /// </summary>
        public byte GetExpMaxJobLevel { get { return getexpmaxjoblevel; } set { getexpmaxjoblevel = value; } }

        /// <summary>
        /// 合成使用的技能
        /// </summary>
        public ushort SkillID { get { return skillid; } set { skillid = value; } }

        /// <summary>
        /// 合成使用技能的等级
        /// </summary>
        public byte SkillLv { get { return skilllv; } set { skilllv = value; } }

        /// <summary>
        /// 合成所需要的被动技能
        /// </summary>
        public ushort PassiveSkillID { get { return passiveskillid; } set { passiveskillid = value; } }

        /// <summary>
        /// 合成所需资金
        /// </summary>
        public uint Gold { get { return gold; } set { gold = value; } }

        /// <summary>
        /// 合成所需工具
        /// </summary>
        public uint RequiredTool { get { return requiredTool; } set { requiredTool = value; } }

        /// <summary>
        /// 合成所需材料
        /// </summary>
        public List<ItemElement> Materials { get { return material; } }

        /// <summary>
        /// 合成产物
        /// </summary>
        public List<ItemElement> Products { get { return product; } }
    }

    /// <summary>
    /// 合成物品信息
    /// </summary>
    public class ItemElement
    {
        uint id;
        ushort count;
        int rate;
        ushort itemskillid;

        /// <summary>
        /// 物品ID
        /// </summary>
        public uint ID { get { return id; } set { id = value; } }

        /// <summary>
        /// 物品个数
        /// </summary>
        public ushort Count { get { return count; } set { count = value; } }

        /// <summary>
        /// 几率
        /// </summary>
        public int Rate { get { return rate; } set { rate = value; } }

        /// <summary>
        /// 道具所附加的技能ID
        /// </summary>
        public ushort ItemSkillID { get { return itemskillid; } set { itemskillid = value; } }

    }
}
