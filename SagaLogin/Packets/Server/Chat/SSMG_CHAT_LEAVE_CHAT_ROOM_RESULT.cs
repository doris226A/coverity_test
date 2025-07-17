using SagaLib;

namespace SagaLogin.Packets.Server
{
    public class SSMG_CHAT_LEAVE_CHAT_ROOM_RESULT : Packet
    {
        public SSMG_CHAT_LEAVE_CHAT_ROOM_RESULT()
        {
            this.data = new byte[8];
            this.ID = 0x0137;
        }

        /// <summary>
        /// 0: means success;
        /// </summary>
        public uint Result
        {
            set
            {
                this.PutUInt(value);
            }
        }
    }
}
