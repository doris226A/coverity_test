/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaLib.VirtualFileSystem;
using SagaDB.Actor;

namespace SagaDB.Navi
{
    public class NaviFactory : Singleton<NaviFactory>
    {
        Navi navi;
        public Navi Navi
        {
            get
            {
                return this.navi;
            }
            set
            {
                this.navi = value;
            }
        }
        public void Init(string path, System.Text.Encoding encoding)
        {

            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);
            string[] paras;
            navi = new Navi();
            uint eventId = 0, categoryId = 0, stepId = 0, stepUniqueId = 0;
            while (!sr.EndOfStream)
            {
                string line;
                line = sr.ReadLine();
                try
                {
                    if (line == "") continue;
                    if (line.Substring(0, 1) == "#")
                        continue;
                    paras = line.Split(',');
                    stepUniqueId = uint.Parse(paras[0]);
                    categoryId = uint.Parse(paras[1]);
                    eventId = uint.Parse(paras[2]);
                    stepId = uint.Parse(paras[3]);
                    if (!navi.Categories.ContainsKey(categoryId))
                    {
                        navi.Categories.Add(categoryId, new Category(categoryId));
                    }
                    Category c = navi.Categories[categoryId];
                    if (!c.Events.ContainsKey(eventId))
                    {
                        c.Events.Add(eventId, new Event(eventId));
                    }
                    Event e = c.Events[eventId];
                    Step s = new Step(stepId, stepUniqueId, e);
                    e.Steps.Add(stepId, s);
                    navi.UniqueSteps.Add(stepUniqueId, s);
                }
                catch (Exception ex)
                {
                    Logger.ShowError("Error on parsing Navi db!\r\nat line:" + line);
                    Logger.ShowError(ex);
                }
            }
        }
    }
}*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaLib;
using SagaDB.Actor;
using SagaLib.VirtualFileSystem;
using System.Xml;
namespace SagaDB.Navi
{
    public class NaviFactory : Factory<NaviFactory, Navi>
    {
        public NaviFactory()
        {
            this.loadingTab = "Loading navi database";
            this.loadedTab = " navis loaded.";
            this.databaseName = "navi";
            this.FactoryType = FactoryType.CSV;
        }
        Navi navi;
        public Navi Navi
        {
            get
            {
                return this.navi;
            }
            set
            {
                this.navi = value;
            }
        }

        uint i;
        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, Navi item)
        {
            throw new NotImplementedException();
        }

        protected override uint GetKey(Navi item)
        {
            return i;
        }

        protected override void ParseCSV(Navi item, string[] paras)
        {
            uint stepUniqueId = uint.Parse(paras[0]);
            uint categoryId = uint.Parse(paras[1]);
            uint eventId = uint.Parse(paras[2]);
            uint stepId = uint.Parse(paras[3]);
            if (!item.Categories.ContainsKey(categoryId))
            {
                item.Categories.Add(categoryId, new Category(categoryId));
            }
            Category c = item.Categories[categoryId];
            if (!c.Events.ContainsKey(eventId))
            {
                c.Events.Add(eventId, new Event(eventId));
            }
            Event e = c.Events[eventId];

            Step s = new Step(stepId, stepUniqueId, e);
            e.Steps.Add(stepId, s);
            item.UniqueSteps.Add(stepUniqueId, s);
            i++;
        }
    }
}
