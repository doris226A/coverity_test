using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_PLAYER_MIRROR_CHANGE_SETTING_CONFIRM : Packet
    {
        public CSMG_PLAYER_MIRROR_CHANGE_SETTING_CONFIRM()
        {
            this.offset = 2;
        }

        public byte FaceIndex
        {
            get
            {
                return this.GetByte(2);
            }
        }

        public byte HairStyleIndex
        {
            get
            {
                return this.GetByte(3);
            }
        }

        public byte HairColorIndex
        {
            get
            {
                return this.GetByte(4);
            }
        }

        public override Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_PLAYER_MIRROR_CHANGE_SETTING_CONFIRM();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnMirrorChangeSettingConfirm(this);
        }
    }
}
