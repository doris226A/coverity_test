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
using SagaDB.Quests;

namespace SagaMap.AncientArks
{
    public class AncientArkGimmickFactory : Factory<AncientArkGimmickFactory, AncientArkGimmick>
    {
        public Dictionary<int, Dictionary<string, List<string>>> Quest_Ask = new Dictionary<int, Dictionary<string, List<string>>>();
        //public Dictionary<int, QuestAsk> QuestAsk = new Dictionary<int, QuestAsk>();
        public List<QuestAsk> QuestAsk = new List<QuestAsk>();
        public AncientArkGimmickFactory()
        {
            this.loadingTab = "Loading AncientArkGimmickFactory database";
            this.loadedTab = " AncientArkGimmickFactory loaded.";
            this.databaseName = "AncientArkGimmickFactory";
            this.FactoryType = FactoryType.CSV;
        }

        protected override uint GetKey(AncientArkGimmick item)
        {
            return item.id;
        }

        protected override void ParseCSV(AncientArkGimmick item, string[] paras)
        {
            item.id = uint.Parse(paras[0]);
            item.name = paras[1].ToString();
            item.type = (AncientArkType)Enum.Parse(typeof(AncientArkType), paras[2]);
            item.Current_ID1 = uint.Parse(paras[3]);
            item.Current_ID2 = uint.Parse(paras[4]);
            item.Current_ID3 = uint.Parse(paras[5]);
            item.CurrentCount = int.Parse(paras[6]);
            item.Time_penalty = int.Parse(paras[7]);
            if (item.id == 91000000)
            {
                QuestAsk q = new QuestAsk();
                q.id = (int)item.Current_ID1;
                q.question = item.name;
                q.ask = paras[8].ToString().Split('/').ToList();
                QuestAsk.Add(q);
                /*List<string> ask = new List<string>(paras[8].ToString().Split(',')); // 使用 ',' 分割字串
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                dict.Add(item.name, ask);
                Quest_Ask.Add((int)item.Current_ID1, dict);*/
            }
        }
        protected override void ParseXML(XmlElement root, XmlElement current, AncientArkGimmick item)
        {
            throw new NotImplementedException();
        }
        private bool toBool(string input)
        {
            if (input == "1") return true; else return false;
        }

    }
}
