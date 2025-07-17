using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;

namespace SagaDB.Bingo
{
    public class BingoDataFactory : Factory<BingoDataFactory, BingoData>
    {
        public BingoDataFactory()
        {
            this.loadingTab = "Loading BingoData database";
            this.loadedTab = " BingoData loaded.";
            this.databaseName = "BingoData";
            this.FactoryType = FactoryType.CSV;
        }

        public BingoData GetBingoData(byte id)
        {
            var query =
               from data in this.items.Values
               where data.ID == id
               select data;
            if (query.Count() != 0)
                return query.First();
            else
            {
                return null;
            }
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, BingoData item)
        {
            throw new NotImplementedException();
        }

        protected override uint GetKey(BingoData item)
        {
            return item.ID;
        }

        protected override void ParseCSV(BingoData item, string[] paras)
        {
            item.ID = byte.Parse(paras[0]);
            item.Type = byte.Parse(paras[1]);
            item.EventName = paras[2];
            item.EventID = uint.Parse(paras[3]);
            item.AllNum = uint.Parse(paras[4]);
        }
    }
}
