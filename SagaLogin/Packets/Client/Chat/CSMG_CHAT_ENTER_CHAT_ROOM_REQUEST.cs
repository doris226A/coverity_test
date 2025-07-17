using SagaLib;
using SagaLogin.Network.Client;

namespace SagaLogin.Packets.Client
{
    public class CSMG_CHAT_ENTER_CHAT_ROOM_REQUEST : Packet
    {
        public CSMG_CHAT_ENTER_CHAT_ROOM_REQUEST()
        {
            this.offset = 2;
        }

        public uint ChatRoomID
        {
            get
            {
                return this.GetUInt(2);
            }
        }

        public byte Type
        {
            get
            {
                return this.GetByte(7);
            }
        }

        public string Password
        {
            get
            {
                var length = this.GetByte(8);
                return this.GetStringFixedSize(9, length);
            }
        }


        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaLogin.Packets.Client.CSMG_CHAT_ENTER_CHAT_ROOM_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((LoginClient)(client)).OnEnterChatRoom(this);
        }
    }
}
