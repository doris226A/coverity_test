using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Iris;
using SagaDB.Partner;
using SagaLib.VirtualFileSystem;
using SagaDB.Item;
using SagaMap.Manager;
using SagaMap.Tasks.Partner;

namespace SagaMap.AncientArks
{
    public class AncientArkMode3Factory : Factory<AncientArkMode3Factory, AncientArkInfo>
    {
        uint count = 0;
        public AncientArkMode3Factory()
        {
            this.loadingTab = "Loading AncientArk database";
            this.loadedTab = " AncientArk loaded.";
            this.databaseName = "AncientArk";
            this.FactoryType = FactoryType.CSV;
        }

        protected override uint GetKey(AncientArkInfo item)
        {
            return item.ID;
        }

        protected override void ParseCSV(AncientArkInfo item, string[] paras)
        {
            item.ID = uint.Parse(paras[0]);
            item.Name = paras[1].ToString();
            item.TimeLimit = int.Parse(paras[2]);
            item.Level = byte.Parse(paras[3]);
            item.Rebirth = toBool(paras[4]);
            //item.spawnfile = paras[21].ToString();
            item.Treasurefile = paras[22].ToString();
        }
        protected override void ParseXML(XmlElement root, XmlElement current, AncientArkInfo item)
        {
            throw new NotImplementedException();
        }
        private bool toBool(string input)
        {
            if (input == "1") return true; else return false;
        }
    }
}
