using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Iris;
using SagaDB.Partner;
using SagaMap.Dungeon;
using SagaMap.Manager;
using SagaMap.Tasks.Dungeon;

namespace SagaMap.AncientArks
{
    public class AncientArkWarpFactory : Factory<AncientArkWarpFactory, AncientArkWarp>
    {
        public AncientArkWarpFactory()
        {
            this.loadingTab = "Loading AncientArk database";
            this.loadedTab = " AncientArk loaded.";
            this.databaseName = "AncientArk";
            this.FactoryType = FactoryType.CSV;
        }

        protected override uint GetKey(AncientArkWarp item)
        {
            return item.MapID;
        }

        protected override void ParseCSV(AncientArkWarp item, string[] paras)
        {
            item.MapID = uint.Parse(paras[0]);
            item.Warp_X.Add(AncientArkGateType.Enter, byte.Parse(paras[1]));
            item.Warp_Y.Add(AncientArkGateType.Enter, byte.Parse(paras[2]));
            item.Warp_X.Add(AncientArkGateType.East, byte.Parse(paras[3]));
            item.Warp_Y.Add(AncientArkGateType.East, byte.Parse(paras[4]));
            item.Warp_X.Add(AncientArkGateType.West, byte.Parse(paras[5]));
            item.Warp_Y.Add(AncientArkGateType.West, byte.Parse(paras[6]));
            item.Warp_X.Add(AncientArkGateType.South, byte.Parse(paras[7]));
            item.Warp_Y.Add(AncientArkGateType.South, byte.Parse(paras[8]));
            item.Warp_X.Add(AncientArkGateType.North, byte.Parse(paras[9]));
            item.Warp_Y.Add(AncientArkGateType.North, byte.Parse(paras[10]));
            item.Warp_X.Add(AncientArkGateType.Next, byte.Parse(paras[11]));
            item.Warp_Y.Add(AncientArkGateType.Next, byte.Parse(paras[12]));
            item.Warp_X.Add(AncientArkGateType.End, byte.Parse(paras[13]));
            item.Warp_Y.Add(AncientArkGateType.End, byte.Parse(paras[14]));

        }

        protected override void ParseXML(XmlElement root, XmlElement current, AncientArkWarp item)
        {
            throw new NotImplementedException();
        }
        private bool toBool(string input)
        {
            if (input == "1") return true; else return false;
        }


    }
}
