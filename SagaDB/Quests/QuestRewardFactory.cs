using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;

namespace SagaDB.Quests
{
    public class QuestRewardFactory : Factory<QuestRewardFactory, QuestRewardInfo>
    {
        public QuestRewardFactory()
        {
            this.loadingTab = "Loading QuestRewardFactory database";
            this.loadedTab = " QuestReward loaded.";
            this.databaseName = "QuestReward";
            this.FactoryType = FactoryType.CSV;
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, QuestRewardInfo item)
        {
            throw new NotImplementedException();
        }

        protected override uint GetKey(QuestRewardInfo item)
        {
            return item.ID;
        }

        protected override void ParseCSV(QuestRewardInfo item, string[] paras)
        {
            item.ID = uint.Parse(paras[0]);
            item.EXP = uint.Parse(paras[1]);
            item.JEXP = uint.Parse(paras[2]);
            item.Gold = uint.Parse(paras[3]);
            item.EP = uint.Parse(paras[4]);
        }
    }
}
