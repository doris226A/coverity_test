using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.Partner
{
    public class PartnerRank
    {
        public byte RankLevel { get; set; }
        public string Rank { get; set; }
        public int Exp { get; set; }
        public byte GetPoint { get; set; }
    }
}
