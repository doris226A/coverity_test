using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_PLAYER_THE_CERIFICATE_OF_FETTERS_WINDOW_OPEN : Packet
    {
        public SSMG_PLAYER_THE_CERIFICATE_OF_FETTERS_WINDOW_OPEN()
        {
            this.data = new byte[10];
            this.offset = 2;
            this.ID = 0x0609;
        }

        public uint Visable
        {
            set
            {
                this.PutUInt(value, 2);
            }
        }

        public string PartnerName
        {
            set
            {
                this.PutTSTR(value, 6);
            }
        }
    }
}
