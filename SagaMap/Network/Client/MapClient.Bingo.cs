using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaLib;
using SagaDB.Bingo;
namespace SagaMap.Network.Client
{
    /*public enum Bingo_Reward
    {
        一條線 = 0x1,//1
        兩條線 = 0x2,//2
        三條線 = 0x4,//4
        四條線 = 0x8,//8
        五條線 = 0x10,//16
        六條線 = 0x20,//32
        七條線 = 0x40,//64
        八條線 = 0x80,//128
    }*/
    public partial class MapClient
    {
        public void OpenBingo(Packets.Client.CSMG_BINGO_OPEN p)
        {
            /*  //BINGO任務4
              if (!this.Character.Bingo.Complete[4] && this.Character.AMask["Neko_01"].Test(7))
              {
                  this.Character.Bingo.Complete[4] = true;
              }
              //BINGO任務5
              if (!this.Character.Bingo.Complete[5] && this.Character.CMask["ECOchen"].Test(64))
              {
                  this.Character.Bingo.Complete[5] = true;
              }*/
          /*  //BINGO任務5 - 給聖誕老人換裝
            if (!this.Character.Bingo.Complete[5] && this.Character.AMask["ECOMerryChristmas"].Test(16))
            {
                this.Character.Bingo.Complete[5] = true;
            }
            //BINGO任務9 - 解狗勾送貨任務
            if (!this.Character.Bingo.Complete[9] && this.Character.AMask["MarryX_2014"].Test(128))
            {
                this.Character.Bingo.Complete[9] = true;
            }*/

            SagaMap.Packets.Server.SSMG_BINGO_DETAIL p5 = new SagaMap.Packets.Server.SSMG_BINGO_DETAIL();
            p5.Stamp1 = Convert.ToInt16(this.Character.Bingo.Complete[1]);
            p5.Stamp2 = Convert.ToInt16(this.Character.Bingo.Complete[2]);
            p5.Stamp3 = Convert.ToInt16(this.Character.Bingo.Complete[3]);
            p5.Stamp4 = Convert.ToInt16(this.Character.Bingo.Complete[4]);
            p5.Stamp5 = Convert.ToInt16(this.Character.Bingo.Complete[5]);
            p5.Stamp6 = Convert.ToInt16(this.Character.Bingo.Complete[6]);
            p5.Stamp7 = Convert.ToInt16(this.Character.Bingo.Complete[7]);
            p5.Stamp8 = Convert.ToInt16(this.Character.Bingo.Complete[8]);
            p5.Stamp9 = Convert.ToInt16(this.Character.Bingo.Complete[9]);
            this.netIO.SendPacket(p5);
            short reward = 0;
            switch (this.Character.Bingo.Reward)
            {
                case 1:
                    reward = 1;
                    break;
                case 2:
                    reward = 3;
                    break;
                case 3:
                    reward = 7;
                    break;
                case 4:
                    reward = 15;
                    break;
                case 5:
                    reward = 31;
                    break;
                case 6:
                    reward = 63;
                    break;
                case 7:
                    reward = 127;
                    break;
                case 8:
                    reward = 255;
                    break;
            }
            SagaMap.Packets.Server.SSMG_BINGO_REWARD p2 = new SagaMap.Packets.Server.SSMG_BINGO_REWARD();
            p2.rewardstamp = reward;
            this.netIO.SendPacket(p2);

        }
        public void BingoReward(Packets.Client.CSMG_BINGO_REWARD p)
        {
            byte BingoLine = 0;
            if (this.Character.Bingo.Complete[1] && this.Character.Bingo.Complete[2] && this.Character.Bingo.Complete[3]) BingoLine++;
            if (this.Character.Bingo.Complete[4] && this.Character.Bingo.Complete[5] && this.Character.Bingo.Complete[6]) BingoLine++;
            if (this.Character.Bingo.Complete[7] && this.Character.Bingo.Complete[8] && this.Character.Bingo.Complete[9]) BingoLine++;
            if (this.Character.Bingo.Complete[1] && this.Character.Bingo.Complete[4] && this.Character.Bingo.Complete[7]) BingoLine++;
            if (this.Character.Bingo.Complete[2] && this.Character.Bingo.Complete[5] && this.Character.Bingo.Complete[8]) BingoLine++;
            if (this.Character.Bingo.Complete[3] && this.Character.Bingo.Complete[6] && this.Character.Bingo.Complete[9]) BingoLine++;
            if (this.Character.Bingo.Complete[1] && this.Character.Bingo.Complete[5] && this.Character.Bingo.Complete[9]) BingoLine++;
            if (this.Character.Bingo.Complete[7] && this.Character.Bingo.Complete[5] && this.Character.Bingo.Complete[3]) BingoLine++;

            //BitMask<Bingo_Reward> Bingo_Reward_mask = new BitMask<Bingo_Reward>();
            //Bingo_Reward_mask.SetValue(Bingo_Reward.一條線, true);

            foreach (SagaDB.Bingo.BingoReward b in SagaDB.Bingo.BingoRewardFactory.Instance.Items.Values)
            {
                if (BingoLine >= b.ID && this.Character.Bingo.Reward < b.ID)
                {
                    SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(b.ItemID);
                    item.Stack = b.Num;
                    AddItem(item, true);
                }
            }
            this.Character.Bingo.Reward = BingoLine;
            short reward = 0;
            switch (BingoLine)
            {
                case 1:
                    reward = 1;
                    break;
                case 2:
                    reward = 3;
                    break;
                case 3:
                    reward = 7;
                    break;
                case 4:
                    reward = 15;
                    break;
                case 5:
                    reward = 31;
                    break;
                case 6:
                    reward = 63;
                    break;
                case 7:
                    reward = 127;
                    break;
                case 8:
                    reward = 255;
                    break;
            }

            SagaMap.Packets.Server.SSMG_BINGO_REWARD p2 = new SagaMap.Packets.Server.SSMG_BINGO_REWARD();
            p2.rewardstamp = reward;
            this.netIO.SendPacket(p2);
        }

        public void BingoCheck(byte id, bool complete)
        {
            if (this.Character.Bingo.Complete.ContainsKey(id) && !this.Character.Bingo.Complete[id])
            {
                this.Character.Bingo.Complete[id] = complete;

                SagaMap.Packets.Server.SSMG_BINGO_DETAIL p5 = new SagaMap.Packets.Server.SSMG_BINGO_DETAIL();
                p5.Stamp1 = Convert.ToInt16(this.Character.Bingo.Complete[1]);
                p5.Stamp2 = Convert.ToInt16(this.Character.Bingo.Complete[2]);
                p5.Stamp3 = Convert.ToInt16(this.Character.Bingo.Complete[3]);
                p5.Stamp4 = Convert.ToInt16(this.Character.Bingo.Complete[4]);
                p5.Stamp5 = Convert.ToInt16(this.Character.Bingo.Complete[5]);
                p5.Stamp6 = Convert.ToInt16(this.Character.Bingo.Complete[6]);
                p5.Stamp7 = Convert.ToInt16(this.Character.Bingo.Complete[7]);
                p5.Stamp8 = Convert.ToInt16(this.Character.Bingo.Complete[8]);
                p5.Stamp9 = Convert.ToInt16(this.Character.Bingo.Complete[9]);
                this.netIO.SendPacket(p5);
            }
        }

        public void BingoCheck(byte id, uint num)
        {
            if (this.Character.Bingo.Complete.ContainsKey(id) && !this.Character.Bingo.Complete[id])
            {
                if (this.Character.Bingo.NowNum.ContainsKey(id))
                {
                    this.Character.Bingo.NowNum[id] += num;
                }
                else
                {
                    this.Character.Bingo.NowNum.Add(id, 1);
                }

                SagaDB.Bingo.BingoData b = SagaDB.Bingo.BingoDataFactory.Instance.GetBingoData(id);
                if (b != null && b.AllNum != 0)
                {
                    if (this.Character.Bingo.NowNum[id] >= b.AllNum)
                    {
                        this.Character.Bingo.Complete[id] = true;

                        SagaMap.Packets.Server.SSMG_BINGO_DETAIL p5 = new SagaMap.Packets.Server.SSMG_BINGO_DETAIL();
                        p5.Stamp1 = Convert.ToInt16(this.Character.Bingo.Complete[1]);
                        p5.Stamp2 = Convert.ToInt16(this.Character.Bingo.Complete[2]);
                        p5.Stamp3 = Convert.ToInt16(this.Character.Bingo.Complete[3]);
                        p5.Stamp4 = Convert.ToInt16(this.Character.Bingo.Complete[4]);
                        p5.Stamp5 = Convert.ToInt16(this.Character.Bingo.Complete[5]);
                        p5.Stamp6 = Convert.ToInt16(this.Character.Bingo.Complete[6]);
                        p5.Stamp7 = Convert.ToInt16(this.Character.Bingo.Complete[7]);
                        p5.Stamp8 = Convert.ToInt16(this.Character.Bingo.Complete[8]);
                        p5.Stamp9 = Convert.ToInt16(this.Character.Bingo.Complete[9]);
                        this.netIO.SendPacket(p5);
                    }
                }
            }
        }
    }
}
