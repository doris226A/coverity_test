using SagaLib;
using SagaMap.Network.Client;
using SagaMap.Skill.SkillDefinations.Elementaler;
using System.Collections.Generic;
using System.Text;

namespace SagaMap.Packets.Client
{
    public class CSMG_CHAT_ECOSHOP_GIFT_SEND : Packet
    {
        public CSMG_CHAT_ECOSHOP_GIFT_SEND()
        {
            this.offset = 2;
        }
        //ECOKEY 新增Items和Counts
        public uint[] Items
        {
            get
            {
                byte count = GetByte(2);
                uint[] items = new uint[count];
                for (int i = 0; i < count; i++)
                {
                    items[i] = GetUInt((ushort)(3 + i * 4));
                }
                return items;
            }
        }
        public uint[] Counts
        {
            get
            {
                byte count = GetByte(2);
                uint[] items = new uint[count];
                for (int i = 0; i < count; i++)
                {
                    items[i] = GetUInt((ushort)(4 + count * 4 + i * 4));
                }
                return items;
            }
        }
        /*public List<uint> GiftID
        {
            get
            {
                var giftcount = this.GetByte();
                Logger.ShowInfo("GiftIDgiftcount  " + giftcount);
                List<uint> lst = new List<uint>();
                for (int i = 0; i < giftcount; i++)
                {
                    lst.Add(this.GetUInt());
                    Logger.ShowInfo("giftcountGetUInt  " + lst[i]);
                }
                return lst;
            }
        }*/

        public List<uint> GiftAmount
        {
            get
            {
                var giftcount = this.GetByte();
                List<uint> lst = new List<uint>();
                for (int i = 0; i < giftcount; i++)
                {
                    lst.Add(this.GetUInt());
                }
                return lst;
            }
        }

        /*public List<uint> GiftUnitPrice
        {
            get
            {
                var giftcount = this.GetByte();
                Logger.ShowInfo("GiftUnitPricegiftcount  " + giftcount);
                List<uint> lst = new List<uint>();
                for (int i = 0; i < giftcount; i++)
                {
                    lst.Add(this.GetUInt());
                    Logger.ShowInfo("GiftUnitPricegiftcountGetUInt  " + lst[i]);
                }
                return lst;
            }
        }*/

        public uint RemainShopPoint {
            get
            {
                return this.GetUInt();
            }
        }

        public string ReceiverName
        {
            get
            {
                byte size = this.GetByte();
                size--;
                //return Global.Unicode.GetString(GetBytes(size, 3)).Replace("\0", "");
                //return this.GetString(size);
                return Encoding.UTF8.GetString(this.GetBytes(size));
            }
        }

        public string Message
        {
            get
            {
                byte size = this.GetByte();
                size--;
                //return Global.Unicode.GetString(GetBytes(size, 3)).Replace("\0", "");
                //return this.GetString(size);
                //return this.GetString();

                return Global.Unicode.GetString(this.GetBytes(this.GetByte(2), 3)).Replace("\0", "");
                //return Global.Unicode.GetString(this.GetBytes(size));
                //return Encoding.UTF8.GetString(this.GetBytes(size));
            }
        }
        
        public override Packet New()
        {
            return new CSMG_CHAT_ECOSHOP_GIFT_SEND();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnECOShopGiftSend(this);
        }
    }
}
