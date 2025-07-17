using SagaLib;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_NPC_HAIR_CHANGE_CONFIRM : Packet
    {
        public CSMG_NPC_HAIR_CHANGE_CONFIRM()
        {
            this.offset = 2;
        }

        public uint InventoryID
        {
            get
            {
                return this.GetUInt(2);
            }
        }

        public short HairStyle
        {
            get
            {
                return this.GetShort(6);
            }
        }

        public short WigStyle
        {
            get
            {
                return this.GetShort(8);
            }
        }

        public override Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_NPC_HAIR_CHANGE_CONFIRM();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnHairChangeConfirm(this);
        }
    }
}
