using SagaLib;
using SagaMap.Network.Client;
using System.Collections.Generic;

namespace SagaMap.Packets.Client
{
    public class CSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP : Packet
    {
        private ushort skillcount = 0;

        public CSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP()
        {
            this.offset = 2;

        }

        public ushort SkillCount
        {
            get
            {
                this.skillcount = this.GetUShort(2);
                return skillcount;
            }
        }

        public List<ushort> SkillList
        {
            get
            {
                List<ushort> lst = new List<ushort>();
                for (int i = 0; i < skillcount; i++)
                {
                    lst.Add(this.GetUShort((ushort)(5 + 2 * i)));
                }
                return lst;
            }
        }

        public List<ulong> ExpList
        {
            get
            {
                List<ulong> lst = new List<ulong>();
                for (int i = 0; i < skillcount; i++)
                {
                    lst.Add(this.GetULong((ushort)(18 + 8 * i)));
                }
                return lst;
            }
        }

        public override Packet New()
        {
            return new CSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnAnotherBookPaperSkillGainExp(this);
        }
    }
}
