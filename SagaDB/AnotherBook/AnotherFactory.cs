using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaLib.VirtualFileSystem;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace SagaDB.AnotherBook
{
    public class AnotherFactory : Singleton<AnotherFactory>
    {
        protected string loadingTab = "";
        protected string loadedTab = "";
        protected string databaseName = "";

        public AnotherFactory()
        {
            this.loadingTab = "Loading Another Book Paper database";
            this.loadedTab = " Another Book Paper loaded.";
            this.databaseName = "Another Book Paper";
        }

        public Dictionary<uint, Dictionary<byte, Another>> Items = new Dictionary<uint, Dictionary<byte, Another>>();
        public void Init(string path, System.Text.Encoding encoding)
        {
            Stream baseStream = VirtualFileSystemManager.Instance.FileSystem.OpenFile(path);
            TextFieldParser parser = new TextFieldParser(baseStream, encoding);
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");


            DateTime time = DateTime.Now;
            int count = 0;
            int lines = 0;

            string label = this.loadingTab;
#if !Web
            Logger.ProgressBarShow(0, (uint)baseStream.Length, label);
#endif

            while (!parser.EndOfData)
            {
                lines++;
                string[] paras;

                try
                {
                    paras = parser.ReadFields();

                    if (paras.Length < 2)
                        continue;
                    if (paras[0].IndexOf('#') != -1 || paras[0] == string.Empty)
                        continue;

                    for (int i = 0; i < paras.Length; i++)
                    {
                        if (paras[i] == "" || paras[i].ToLower() == "null")
                            paras[i] = "0";
                    }

                    Another paper = new Another();
                    paper.ID = uint.Parse(paras[0]);
                    paper.Name = paras[1];
                    paper.Type = byte.Parse(paras[2]);
                    paper.Level = byte.Parse(paras[3]);
                    paper.Icon = byte.Parse(paras[4]);
                    for (int i = 0; i < 8; i++)
                    {
                        paper.PaperItems1.Add(uint.Parse(paras[5 + i]));
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        paper.PaperItems2.Add(uint.Parse(paras[13 + i]));
                    }
                    paper.RequestItem1 = uint.Parse(paras[21]);
                    paper.ReqeustItem2 = uint.Parse(paras[22]);
                    paper.AwakeSkillID = uint.Parse(paras[23]);
                    paper.AwakeSkillMaxLV = byte.Parse(paras[24]);
                    for (int i = 0; i < 5; i++)
                    {
                        uint skillid = uint.Parse(paras[25 + i * 2]);
                        byte skillMaxLV = byte.Parse(paras[26 + i * 2]);
                        if (!paper.Skills.ContainsKey(skillid))
                            paper.Skills.Add(skillid, new List<byte>());
                        paper.Skills[skillid].Add(skillMaxLV);
                    }
                    paper.Str = ushort.Parse(paras[35]);
                    paper.Mag = ushort.Parse(paras[36]);
                    paper.Vit = ushort.Parse(paras[37]);
                    paper.Dex = ushort.Parse(paras[38]);
                    paper.Agi = ushort.Parse(paras[39]);
                    paper.Int = ushort.Parse(paras[40]);
                    paper.HP_ADD = short.Parse(paras[41]);
                    paper.MP_ADD = short.Parse(paras[42]);
                    paper.SP_ADD = short.Parse(paras[43]);
                    paper.MIN_ATK_ADD = ushort.Parse(paras[44]);
                    paper.MAX_ATK_ADD = ushort.Parse(paras[45]);
                    paper.MIN_MATK_ADD = ushort.Parse(paras[46]);
                    paper.MAX_MATK_ADD = ushort.Parse(paras[47]);
                    paper.DEF_ADD = ushort.Parse(paras[48]);
                    paper.MDEF_ADD = ushort.Parse(paras[49]);
                    paper.HIT_MELEE_ADD = ushort.Parse(paras[50]);
                    paper.HIT_RANGE_ADD = ushort.Parse(paras[51]);
                    paper.AVOID_MELEE_ADD = ushort.Parse(paras[52]);
                    paper.AVOID_RANGE_ADD = ushort.Parse(paras[53]);
                    if (!Items.ContainsKey(paper.ID))
                        Items.Add(paper.ID, new Dictionary<byte, Another>());
                    Items[paper.ID].Add(paper.Level, paper);

#if !Web
                    if ((DateTime.Now - time).TotalMilliseconds > 10)
                    {
                        time = DateTime.Now;
                        Logger.ProgressBarShow((uint)baseStream.Position, (uint)baseStream.Length, label);
                    }
#endif
                    count++;
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                    Logger.ShowError("Error on parsing " + this.databaseName + " db!\r\n       File:" + path + ":" + lines.ToString());
                }
            }
            Logger.ProgressBarHide(count + this.loadedTab);
            parser.Close();
            baseStream.Close();
        }
        public byte GetPaperLv(ulong value)
        {
            byte lv = 0;
            if (value >= 0xff) lv = 1;
            if (value >= 0xffff) lv = 2;
            if (value >= 0xffffff) lv = 3;
            if (value >= 0xffffffff) lv = 4;
            if (value >= 0xffffffffff) lv = 5;
            return lv;
        }
        public ulong GetPaperValue(byte paperID, byte lv, uint ItemID)
        {
            ulong value = 0;
            if (!Items.ContainsKey(paperID)) return 0;
            if (!Items[paperID].ContainsKey(lv)) return 0;
            if (!Items[paperID][lv].PaperItems1.Contains(ItemID)) return 0;
            int index = Items[paperID][lv].PaperItems1.IndexOf(ItemID);
            switch (index)
            {
                case 0:
                    value = 0x1;
                    break;
                case 1:
                    value = 0x2;
                    break;
                case 2:
                    value = 0x4;
                    break;
                case 3:
                    value = 0x8;
                    break;
                case 4:
                    value = 0x10;
                    break;
                case 5:
                    value = 0x20;
                    break;
                case 6:
                    value = 0x40;
                    break;
                case 7:
                    value = 0x80;
                    break;
            }
            if (lv > 1)
                value = value * (ulong)((lv - 1) * 0x100);
            return value;
        }
    }
}
