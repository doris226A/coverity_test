using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    // 客户端请求设置故障回路封包
    // 1E 52 64 3E 00 / 1E 52 64 3E 44
    // IE 5E 为封包命令ID
    // 0x64 意义不明
    // 0x3E 为故障回路设置点, 由左上角第一个格向右, 当第一行占满后从第二行最左边第一个继续计算, 数值为16禁止
    // 0x00/0x44 意义不明
    public class CSMG_DEM_DEMIC_ENGAGETASK_REQUEST : Packet
    {
        public CSMG_DEM_DEMIC_ENGAGETASK_REQUEST()
        {
            this.offset = 2;
        }

        public byte Page
        {
            get
            {
                return this.GetByte(2);
            }
        }

        public byte Location
        {
            get
            {
                return this.GetByte(3);
            }
        }

        public byte Location2
        {
            get
            {
                return this.GetByte(4);
            }
        }

        public override Packet New()
        {
            return new CSMG_DEM_DEMIC_ENGAGETASK_REQUEST();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnDEMDemicEngageTaskConfirm(this);//ECOKEY DEM黃點點設定
        }
    }
}
