using SagaLib;
using System.Collections.Generic;

namespace SagaMap.Packets.Server
{
    public class SSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP_RESULT : Packet
    {
        public SSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP_RESULT()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x23A3;
        }

        public byte Result
        {
            set
            {
                this.PutByte(value, offset);
            }
        }

        public List<ushort> SkillIDList
        {
            set
            {
                this.PutByte((byte)value.Count, offset);
                foreach (var item in value)
                {
                    this.PutUShort(item, offset);
                }
            }
        }

        public List<ulong> SkillExpList
        {
            set
            {
                this.PutByte((byte)value.Count, offset);
                foreach (var item in value)
                {
                    this.PutULong(item, offset);
                }
            }
        }

        public ulong PageExpRemain
        {
            set
            {
                this.PutULong(value, offset);
            }
        }
    }
}
