using SagaLib;

namespace SagaMap.Packets.Server
{
    public class SSMG_NPC_CHANGE_HAIR_RESULT : Packet
    {
        public SSMG_NPC_CHANGE_HAIR_RESULT()
        {
            this.data = new byte[18];
            this.offset = 2;
            this.ID = 0x0618;
        }

        /// <summary>
        /// 0: Success; -1: Faild
        /// </summary>
        public int Result
        {
            set
            {
                this.PutInt(value, 2);
            }
        }

        /// <summary>
        /// 固定0?
        /// </summary>
        public int Type
        {
            set
            {
                this.PutInt(value, 6);
            }
        }

        /// <summary>
        /// 卷轴的ID
        /// </summary>
        public int ItemID
        {
            set
            {
                this.PutInt(value, 10);
            }
        }

        public short HairStyle
        {
            set
            {
                this.PutShort(value, 14);
            }
        }

        public short WigStyle
        {
            set
            {
                this.PutShort(value, 16);
            }
        }
    }
}
