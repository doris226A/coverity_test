using SagaLib;
using SagaLib.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaDB.Item
{
    public class TradeConditionFactory : Singleton<TradeConditionFactory>
    {
        public Dictionary<uint, TradeCondition> Items = new Dictionary<uint, TradeCondition>();

        public void Init(string path, Encoding encoding)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);
            string[] paras;
            uint TID = 0;
            ItemTransform it;
            while (!sr.EndOfStream)
            {
                string line = "";
                line = sr.ReadLine();
                try
                {
                    if (line == "") continue;
                    if (line.Substring(0, 1) == "#")
                        continue;
                    paras = line.Split(',');
                    if (paras.Length <= 0) continue;
                    if (paras[0] == "") continue;
                    if (paras[1] == "") continue;

                    uint itemID = uint.Parse(paras[0]);

                    if(!ItemFactory.Instance.Items.ContainsKey(itemID))
                    {
                        Logger.ShowError("Try to Set Unknow Item Trade Condition With ID: " + itemID);
                        continue;
                    }

                    TradeCondition tc = new TradeCondition();
                    tc.ItemID = itemID;
                    tc.TradeMask = int.Parse(paras[1]);
                    tc.GMLevelToIgnoreCondition = byte.Parse(paras[2]);

                    if (this.Items.ContainsKey(itemID))
                        this.Items[itemID] = tc;
                    else
                        this.Items.Add(itemID, tc);

                    if ((tc.TradeMask & 1) > 0)
                        ItemFactory.Instance.Items[itemID].noDrop = true;
                    if ((tc.TradeMask & 2) > 0)
                        ItemFactory.Instance.Items[itemID].noTrade = true;
                    if ((tc.TradeMask & 4) > 0)
                        ItemFactory.Instance.Items[itemID].noVending = true;
                    if ((tc.TradeMask & 8) > 0)
                        ItemFactory.Instance.Items[itemID].noSoldToNPC = true;
                    if ((tc.TradeMask & 16) > 0)
                        ItemFactory.Instance.Items[itemID].noGolemSellShop = true;
                    if ((tc.TradeMask & 32) > 0)
                        ItemFactory.Instance.Items[itemID].noStore = true;
                    if ((tc.TradeMask & 64) > 0)
                        ItemFactory.Instance.Items[itemID].noGardenStore = true;

                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }
            sr.Close();
        }
    }
}
