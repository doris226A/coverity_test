using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.Partner
{
    public class PartnerAdvanceStatus
    {
        public uint id, pictid;
        public string name;
        public byte level;
        public uint hp, mp, sp;
        public uint hp_rec, mp_rec, sp_rec;
        public ushort atk_min, atk_max;
        public ushort matk_min, matk_max;
        public ushort def, def_add;
        public ushort mdef, mdef_add;
        public ushort hit_melee, hit_ranged, hit_magic, hit_critical;
        public ushort avoid_melee, avoid_ranged, avoid_magic, avoid_critical;
        public short aspd, cspd, aspd_fn, cspd_fn;
    }
}
