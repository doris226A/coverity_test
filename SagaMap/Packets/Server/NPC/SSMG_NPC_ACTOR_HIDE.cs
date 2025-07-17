using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap.Scripting;

namespace SagaMap.Packets.Server
{
    public class SSMG_NPC_ACTOR_HIDE : Packet
    {
        public SSMG_NPC_ACTOR_HIDE()
        {
            this.data = new byte[11];
            this.offset = 2;
            this.ID = 0x0615;
        }
        /// <summary>
        /// 开关？ 2为启用，0为关闭
        /// </summary>
        public int Appear
        {
            set
            {
                PutInt(value, 2);
            }
        }
        /// <summary>
        /// 隐藏范围？3为玩家，0x0c为周围actor
        /// </summary>
        public int Value2
        {
            set
            {
                PutInt(value, 6);
            }
        }
        /// <summary>
        /// Unknown
        /// </summary>
        public byte value3
        {
            set
            {
                PutByte(value, 10);
            }
        }
    }
}
