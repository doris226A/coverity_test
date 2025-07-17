using SagaLib;
using System.Collections.Generic;

namespace SagaMap.Packets.Server
{
    public class SSMG_ANO_SKILL_LIST : Packet
    {
        public SSMG_ANO_SKILL_LIST()
        {
            this.data = new byte[15];
            this.offset = 2;
            this.ID = 0x23A1;
        }

        public List<ushort> SkillList
        {
            set
            {
                this.PutByte((byte)value.Count);
                foreach (var item in value)
                {
                    this.PutUShort(item);
                }
            }
        }

        public List<byte> SkillLevel
        {
            set
            {
                this.PutByte((byte)value.Count);
                foreach (var item in value)
                {
                    this.PutByte(item);
                }
            }
        }
    }
}
