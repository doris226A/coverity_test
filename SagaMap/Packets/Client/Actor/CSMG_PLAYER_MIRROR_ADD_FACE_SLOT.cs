using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_PLAYER_MIRROR_ADD_FACE_SLOT : Packet
    {
        public CSMG_PLAYER_MIRROR_ADD_FACE_SLOT()
        {
            this.offset = 2;
        }

        public override Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_PLAYER_MIRROR_ADD_FACE_SLOT();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnMirrorAddFaceSlot(this);
        }
    }
}
