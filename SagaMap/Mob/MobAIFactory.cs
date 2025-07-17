using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaLib.VirtualFileSystem;
using SagaMap.AncientArks;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Mob
{
    public class MobAIFactory : Factory<MobAIFactory, AIMode>
    {
        public MobAIFactory()
        {
            this.loadingTab = "Loading MobAI database";
            this.loadedTab = " AIs loaded.";
            this.databaseName = "MobAI";
            this.FactoryType = FactoryType.XML;
        }

        protected override uint GetKey(AIMode item)
        {
            return item.MobID;
        }

        protected override void ParseCSV(AIMode item, string[] paras)
        {
            throw new NotImplementedException();
        }


        public void InitMobAI(string path, System.Text.Encoding encoding)//ECOKEY 根據怪物csv補技能
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);
            int count = 0;
            uint logger = 0;
            string[] paras;
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

                    for (int i = 0; i < paras.Length; i++)
                    {
                        if (paras[i] == "" || paras[i].ToLower() == "null")
                            paras[i] = "0";
                    }
                    uint id = uint.Parse(paras[0]);
                    if (!MobAIFactory.Instance.Items.ContainsKey(id))
                    {
                        AIMode item = new AIMode();
                        item.MobID = uint.Parse(paras[0]);
                        item.AI = 1;
                        item.EventAttackingSkillRate = 20;
                        uint skillid1 = uint.Parse(paras[94]);
                        logger = skillid1;
                        if (skillid1 != 0)
                        {
                            int rate1 = int.Parse(paras[95]);
                            item.EventAttacking.Add(skillid1, rate1);

                            uint skillid2 = uint.Parse(paras[96]);
                            if (skillid2 != 0)
                            {
                                int rate2 = int.Parse(paras[97]);
                                item.EventAttacking.Add(skillid2, rate2);
                            }
                            uint skillid3 = uint.Parse(paras[98]);
                            if (skillid3 != 0)
                            {
                                int rate3 = int.Parse(paras[99]);
                                item.EventAttacking.Add(skillid3, rate3);
                            }
                            MobAIFactory.Instance.Items.Add(id, item);
                        }
                    }

                    count++;
                }
                catch (Exception ex)
                {
#if !Web
                    Logger.ShowError("Error on parsing MobAI db!\r\nat line:" + line);
                    Logger.ShowError(ex);
#endif
                }
            }
            sr.Close();
        }

        protected override void ParseXML(System.Xml.XmlElement root, System.Xml.XmlElement current, AIMode item)
        {
            switch (root.Name.ToLower())
            {
                case "mob":
                    switch (current.Name.ToLower())
                    {
                        case "id":
                            item.MobID = uint.Parse(current.InnerText);
                            break;
                        case "aimode":
                            item.AI = int.Parse(current.InnerText);
                            break;
                        case "eventattackingonskillcast":
                            if (current.Attributes.Count > 0)
                            {
                                item.EventAttackingSkillRate = int.Parse(current.Attributes["Rate"].InnerText);
                            }
                            else
                                item.EventAttackingSkillRate = 50;
                            break;
                        case "eventmastercombatonskillcast":
                            if (current.Attributes.Count > 0)
                            {
                                item.EventMasterCombatSkillRate = int.Parse(current.Attributes["Rate"].InnerText);
                            }
                            else
                                item.EventMasterCombatSkillRate = 50;
                            break;
                        case "useskillofshortrange":
                            item.Distance = int.Parse(current.Attributes["Distance"].InnerText);
                            item.ShortCD = int.Parse(current.Attributes["CD"].InnerText);
                            item.isNewAI = true;
                            break;
                        case "useskilloflongrange":
                            item.LongCD = int.Parse(current.Attributes["CD"].InnerText);
                            item.isNewAI = true;
                            break;
                        case "useskillofhp":
                            item.isNewAI = true;
                            break;
                        case "useskilllist":
                            item.isAnAI = true;
                            uint ID = uint.Parse(current.Attributes["ID"].InnerText);
                            int MaxHP = int.Parse(current.Attributes["MaxHP"].InnerText);
                            int MinHP = int.Parse(current.Attributes["MinHP"].InnerText);
                            int Rate = int.Parse(current.Attributes["Rate"].InnerText);
                            AIMode.SkillList sl = new AIMode.SkillList();
                            sl.MaxHP = MaxHP;
                            sl.MinHP = MinHP;
                            sl.Rate = Rate;
                            if (!item.AnAI_SkillAssemblage.ContainsKey(ID))
                                item.AnAI_SkillAssemblage.Add(ID, sl);
                            else
                                item.AnAI_SkillAssemblage[ID] = sl;
                            switch (current.Name.ToLower())
                            {
                                case "skill":
                                    break;
                            }
                            break;
                    }
                    break;
                case "useskilllist":
                    switch (current.Name.ToLower())
                    {
                        case "skill":
                            uint listid = uint.Parse(current.Attributes["ListID"].InnerText);
                            AIMode.SkillsInfo si = new AIMode.SkillsInfo();
                            si.Delay = int.Parse(current.Attributes["Delay"].InnerText);
                            si.SkillID = uint.Parse(current.InnerText);
                            uint Sequence = uint.Parse(current.Attributes["Sequence"].InnerText);
                            if (!item.AnAI_SkillAssemblage[listid].AnAI_SkillList.ContainsKey(Sequence))
                                item.AnAI_SkillAssemblage[listid].AnAI_SkillList.Add(Sequence, si);
                            else
                                item.AnAI_SkillAssemblage[listid].AnAI_SkillList[Sequence] = si;
                            break;
                    }
                    break;
                case "eventattackingonskillcast":
                    switch (current.Name.ToLower())
                    {
                        case "skill":
                            uint id = uint.Parse(current.InnerText);
                            int rate = int.Parse(current.Attributes["Rate"].InnerText);
                            item.EventAttacking.Add(id, rate);
                            break;
                    }
                    break;
                case "eventmastercombatonskillcast":
                    switch (current.Name.ToLower())
                    {
                        case "skill":
                            uint id = uint.Parse(current.InnerText);
                            int rate = int.Parse(current.Attributes["Rate"].InnerText);
                            item.EventMasterCombat.Add(id, rate);
                            break;
                    }
                    break;

                case "useskillofshortrange":
                    switch (current.Name.ToLower())
                    {
                        case "skill":
                            uint id = uint.Parse(current.InnerText);
                            AIMode.SkilInfo si = new AIMode.SkilInfo();
                            si.CD = int.Parse(current.Attributes["CD"].InnerText);
                            si.Rate = int.Parse(current.Attributes["Rate"].InnerText);
                            si.MaxHP = int.Parse(current.Attributes["MaxHP"].InnerText);
                            si.MinHP = int.Parse(current.Attributes["MinHP"].InnerText);
                            item.SkillOfShort.Add(id, si);
                            break;
                    }
                    break;
                case "useskilloflongrange":
                    switch (current.Name.ToLower())
                    {
                        case "skill":
                            uint id = uint.Parse(current.InnerText);
                            AIMode.SkilInfo si = new AIMode.SkilInfo();
                            si.CD = int.Parse(current.Attributes["CD"].InnerText);
                            si.Rate = int.Parse(current.Attributes["Rate"].InnerText);
                            si.MaxHP = int.Parse(current.Attributes["MaxHP"].InnerText);
                            si.MinHP = int.Parse(current.Attributes["MinHP"].InnerText);
                            item.SkillOfLong.Add(id, si);
                            break;
                    }
                    break;
                case "useskillofhp":
                    switch (current.Name.ToLower())
                    {
                        case "skill":
                            uint id = uint.Parse(current.InnerText);
                            item.SkillOfHP.Add(int.Parse(current.Attributes["HP"].InnerText), id);
                            break;
                    }
                    break;

                case "eventdieskill"://ECOKEY 新增怪物死亡時放技能
                    switch (current.Name.ToLower())
                    {
                        case "skill":
                            uint id = uint.Parse(current.InnerText);
                            int rate = int.Parse(current.Attributes["Rate"].InnerText);
                            item.EventDieSkillRate = rate;
                            item.EventDieSkill = id;
                            break;
                    }
                    break;
            }
        }


    }
}
