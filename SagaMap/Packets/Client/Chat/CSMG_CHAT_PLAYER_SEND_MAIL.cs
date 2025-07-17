using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_CHAT_PLAYER_SEND_MAIL : Packet
    {
        public CSMG_CHAT_PLAYER_SEND_MAIL()
        {
            this.offset = 2;
        }

        public string Receiver
        {
            get
            {
                return this.GetString();
            }
        }

        public string Title
        {
            get
            {
                return this.GetString();
            }
        }

        public string Content
        {
            get
            {
                return this.GetString();
            }
        }

        public override Packet New()
        {
            return new CSMG_CHAT_PLAYER_SEND_MAIL();
        }

        public override void Parse(SagaLib.Client client)
        {
            //((MapClient)(client)).OnPlayerSendMail(this);
        }
    }
}
