using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_STORY_GROUP_WINDOW_OPEN : Packet
    {
        public SSMG_STORY_GROUP_WINDOW_OPEN()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x1CB8;
        }

        public byte Type
        {
            set
            {
                this.PutByte(value, 2);
            }
        }

        public ushort UnknowWord
        {
            set
            {
                this.PutUShort(value, 3);
            }
        }

        public uint StageID
        {
            set
            {
                this.PutUInt(value, 5);
            }
        }

        public byte Unknow2
        {
            set
            {
                this.PutByte(value, 9);
            }
        }
    }
}
