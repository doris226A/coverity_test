using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaLib;
using SagaMap.AncientArks;
namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public void OnNaviOpen(Packets.Client.CSMG_NAVI_OPEN p)
        {
            MapClient client1 = MapClient.FromActorPC(this.Character);
            uint Event = 91000530;
            client1.EventActivate(Event);
        }
        //ECOKEY 送禮
        public void OnECOShopGiftSend(Packets.Client.CSMG_CHAT_ECOSHOP_GIFT_SEND p)
        {
            //Logger.ShowInfo("DumpData  " + p.DumpData());
            /*Logger.ShowInfo("道具ID  " + p.Items.Count());
            Logger.ShowInfo("道具數量  " + p.Counts.Count());
            Logger.ShowInfo("道具單價  " + p.GiftAmount.Count());
            Logger.ShowInfo("目前玩家持有的點數  " + p.RemainShopPoint);
            Logger.ShowInfo("要送的對象  " + p.ReceiverName);
            Logger.ShowInfo("訊息  " + p.Message);*/
            for (int i = 0; i <= 10; i++)
            {
                this.Character.TInt["gift" + i.ToString()] = 0;
                this.Character.TInt["giftcount" + i.ToString()] = 0;
            }
            int num = 0;
            foreach (var i in p.Items)
            {
                this.Character.TInt["gift" + num.ToString()] = (int)i;
                num++;
            }
            int numA = 0;
            foreach (var i in p.Counts)
            {
                this.Character.TInt["giftcount" + numA.ToString()] = (int)i;
                numA++;
            }
            Logger.ShowInfo("道具單價  " + p.GiftAmount.Count());
            Logger.ShowInfo("目前玩家持有的點數  " + p.RemainShopPoint);
            this.Character.TStr["giftName"] = p.ReceiverName;
            this.Character.TStr["giftMessage"] = p.Message;
            Logger.ShowInfo("信件內容  " + this.Character.TStr["giftMessage"]);

            MapClient client1 = MapClient.FromActorPC(this.Character);
            uint Event = 99100000;
            client1.EventActivate(Event);
            
        }

        public bool AncientArk = false;
        public void OnAncientArkQuest(Packets.Client.CSMG_AARCH_QUEST_ID p)
        {
            uint questid = p.QuestID;
            
            if (questid < 300000)
            {
                this.Character.AncientArk_QuestID = questid;
            }
            AncientArk = false;
        }
    }
}
