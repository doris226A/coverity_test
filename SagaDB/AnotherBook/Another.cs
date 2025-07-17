using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;

namespace SagaDB.AnotherBook
{

    [Serializable]
    public class Another
    {
        public uint ID;
        public byte Type;
        public string Name;
        public byte Level;
        public byte Icon;
        public List<uint> PaperItems1 = new List<uint>();
        public List<uint> PaperItems2 = new List<uint>();
        public uint RequestItem1;
        public uint ReqeustItem2;
        public uint AwakeSkillID;
        public byte AwakeSkillMaxLV;
        public Dictionary<uint, List<byte>> Skills = new Dictionary<uint, List<byte>>();
        public ushort Str;
        public ushort Mag;
        public ushort Vit;
        public ushort Dex;
        public ushort Agi;
        public ushort Int;
        public short HP_ADD;
        public short MP_ADD;
        public short SP_ADD;
        public ushort MIN_ATK_ADD;
        public ushort MAX_ATK_ADD;
        public ushort MIN_MATK_ADD;
        public ushort MAX_MATK_ADD;
        public ushort DEF_ADD;
        public ushort MDEF_ADD;
        public ushort HIT_MELEE_ADD;
        public ushort HIT_RANGE_ADD;
        public ushort AVOID_MELEE_ADD;
        public ushort AVOID_RANGE_ADD;
    }
    public class AnotherDetail
    {
        public byte Level;
        public BitMask_Long PaperValue;
        public Dictionary<uint, ulong> Skills = new Dictionary<uint, ulong>();
    }
}
