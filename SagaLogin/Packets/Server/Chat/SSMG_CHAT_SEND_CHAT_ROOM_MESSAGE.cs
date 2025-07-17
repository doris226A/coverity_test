using SagaLib;

namespace SagaLogin.Packets.Server
{
    public class SSMG_CHAT_SEND_CHAT_ROOM_MESSAGE : Packet
    {
        int senderend = 0;
        public SSMG_CHAT_SEND_CHAT_ROOM_MESSAGE()
        {
            this.data = new byte[8];
            this.ID = 0x0141;
        }

        public string Sender
        {
            set
            {
                byte[] buf = Global.Unicode.GetBytes(value);
                byte[] buff = new byte[buf.Length + this.data.Length + 1];
                this.data.CopyTo(buff, 0);
                this.data = buff;
                this.PutByte((byte)(buf.Length + 1), 2);
                this.PutBytes(buf, 3);
                senderend = 3 + buf.Length;
            }
        }

        public string Content
        {
            set
            {
                byte[] buf = Global.Unicode.GetBytes(value);
                byte[] buff = new byte[buf.Length + this.data.Length + 1];
                this.data.CopyTo(buff, 0);
                this.data = buff;
                this.PutByte((byte)(buf.Length + 1), senderend + 1);
                this.PutBytes(buf, senderend + 2);
            }
        }
    }
}
