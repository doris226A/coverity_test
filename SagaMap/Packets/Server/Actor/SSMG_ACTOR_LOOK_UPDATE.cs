using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_ACTOR_LOOK_UPDATE : Packet
    {
        public SSMG_ACTOR_LOOK_UPDATE()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x0210;
            this.PutByte(0xFF, 7);
        }

        public ushort HairStyle
        {
            set
            {
                this.PutUShort(value, 2);
            }
        }

        public byte HairColor
        {
            set
            {
                this.PutByte(value, 4);
            }
        }

        public ushort Wig
        {
            set
            {
                this.PutUShort(value, 5);
            }
        }

        public ushort Face
        {
            set
            {
                this.PutUShort(value, 8);
            }
        }
    }
}
