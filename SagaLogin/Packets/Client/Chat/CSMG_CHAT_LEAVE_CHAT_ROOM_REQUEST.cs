using SagaLib;
using SagaLogin.Network.Client;

namespace SagaLogin.Packets.Client
{
    public class CSMG_CHAT_LEAVE_CHAT_ROOM_REQUEST : Packet
    {
        public CSMG_CHAT_LEAVE_CHAT_ROOM_REQUEST()
        {
            this.offset = 2;
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaLogin.Packets.Client.CSMG_CHAT_LEAVE_CHAT_ROOM_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((LoginClient)(client)).OnLeaveChatRoom(this);
        }
    }
}
