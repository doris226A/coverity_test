using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.AnotherBook
{
    public class AnotherTransform
    {
        public byte ID { get; set; }
        public byte Level { get; set; }
        public byte PVPMode { get; set; }
        public ushort EffectID { get; set; }
        public byte MaxValueFlag { get; set; }
        public ushort Str { get; set; }
        public ushort Mag { get; set; }
        public ushort Vit { get; set; }
        public ushort Dex { get; set; }
        public ushort Agi { get; set; }
        public ushort Int { get; set; }
        public ushort HP_ADD { get; set; }
        public ushort MP_ADD { get; set; }
        public ushort SP_ADD { get; set; }
        public ushort MIN_ATK_ADD { get; set; }
        public ushort MAX_ATK_ADD { get; set; }
        public ushort MIN_MATK_ADD { get; set; }
        public ushort MAX_MATK_ADD { get; set; }
        public ushort DEF_ADD { get; set; }
        public ushort MDEF_ADD { get; set; }
        public ushort HIT_MELEE_ADD { get; set; }
        public ushort HIT_RANGE_ADD { get; set; }
        public ushort AVOID_MELEE_ADD { get; set; }
        public ushort AVOID_RANGE_ADD { get; set; }
        public ushort APSD_ADD { get; set; }
        public ushort CSPD_ADD { get; set; }
    }
}
