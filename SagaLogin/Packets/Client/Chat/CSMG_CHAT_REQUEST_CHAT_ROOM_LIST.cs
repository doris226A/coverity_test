using SagaLib;
using SagaLogin.Network.Client;

namespace SagaLogin.Packets.Client
{
    public class CSMG_CHAT_REQUEST_CHAT_ROOM_LIST : Packet
    {
        public CSMG_CHAT_REQUEST_CHAT_ROOM_LIST()
        {
            this.offset = 2;
        }

        public byte Type
        {
            get
            {
                return this.GetByte(2);
            }
        }

        public uint CurrentPage
        {
            get
            {
                return this.GetUInt(3);
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaLogin.Packets.Client.CSMG_CHAT_REQUEST_CHAT_ROOM_LIST();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((LoginClient)(client)).OnRequestChatRoomList(this);
        }
    }
}
