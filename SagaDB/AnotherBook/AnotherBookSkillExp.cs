using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.AnotherBook
{
    public class AnotherBookSkillExp
    {
        public byte ID { get; set; }
        public ushort SkillID { get; set; }
        public byte Level { get; set; }
        public ulong Exp { get; set; }
    }
}
