using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;

namespace SagaDB.Bingo
{
    public class BingoRewardFactory : Factory<BingoRewardFactory, BingoReward>
    {
        public BingoRewardFactory()
        {
            this.loadingTab = "Loading BingoReward database";
            this.loadedTab = " BingoReward loaded.";
            this.databaseName = "Bingo";
            this.FactoryType = FactoryType.CSV;
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, BingoReward item)
        {
            throw new NotImplementedException();
        }

        protected override uint GetKey(BingoReward item)
        {
            return item.ID;
        }

        protected override void ParseCSV(BingoReward item, string[] paras)
        {
            item.ID = byte.Parse(paras[0]);
            item.ItemID = uint.Parse(paras[1]);
            item.Num = byte.Parse(paras[2]);
        }
    }
}
