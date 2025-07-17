using SagaLib;
using SagaLogin.Network.Client;

namespace SagaLogin.Packets.Client
{
    public class CSMG_CHAT_CREATE_CHAT_ROOM_REQUEST : Packet
    {
        int nameend = 3;
        int commentend = 0;

        public CSMG_CHAT_CREATE_CHAT_ROOM_REQUEST()
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

        public string ChatRoomName
        {
            get
            {
                var namelength = this.GetByte(3);
                nameend += namelength;
                return this.GetStringFixedSize(4, namelength);
            }
        }

        public string Comment
        {
            get
            {
                var commentlength = this.GetByte((ushort)(nameend + 1));
                commentend = (nameend + commentlength + 1);
                return this.GetStringFixedSize((ushort)(nameend + 2), commentlength);
            }
        }

        public string Password
        {
            get
            {
                var passlength = this.GetByte((ushort)(commentend + 1));
                return this.GetStringFixedSize((ushort)(commentend + 2), passlength);
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaLogin.Packets.Client.CSMG_CHAT_CREATE_CHAT_ROOM_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((LoginClient)(client)).OnCreateChatRoom(this);
        }
    }
}
