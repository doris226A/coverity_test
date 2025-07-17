using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_PLAYER_MIRROR_ADD_HAIR_SLOT : Packet
    {
        public CSMG_PLAYER_MIRROR_ADD_HAIR_SLOT()
        {
            this.offset = 2;
        }

        public byte Unknow
        {
            get
            {
                return this.GetByte(2);
            }
        }

        public override Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_PLAYER_MIRROR_ADD_HAIR_SLOT();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnMirrorAddHairSlot(this);
        }
    }
}
