using SagaLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaMap.Packets.Client
{
    public class CSMG_AAA_GROUP_MEMBER_KICK_REQUEST : Packet
    {
        public CSMG_AAA_GROUP_MEMBER_KICK_REQUEST()
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

        public int CharID
        {
            get
            {
                return this.GetInt(3);
            }
        }

        public override Packet New()
        {
            return new CSMG_AAA_GROUP_MEMBER_KICK_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            //((MapClient)(client)).OnAAAGroupMemberKickRequest(this);
        }
    }
}
