using SagaMap.Packets.Client;
using SagaMap.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public void OnAAAGroupStageSelectRequest(CSMG_AAA_GROUP_STAGE_SELECT_REQUEST p)
        {
            var id = p.StageID;
            SSMG_AAA_GROUP_STAGE_SELECT_RESULT p1 = new SSMG_AAA_GROUP_STAGE_SELECT_RESULT();
            p1.Type = 0;
            p1.StageID = (int)id;
            this.netIO.SendPacket(p1);
        }
    }
}
