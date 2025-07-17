using Microsoft.VisualBasic.FileIO;
using SagaLib;
using SagaLib.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.AnotherBook
{
    public class AnotherTransformFactory : Singleton<AnotherTransformFactory>
    {
        protected string loadingTab = "";
        protected string loadedTab = "";
        protected string databaseName = "";

        public Dictionary<string, AnotherTransform> Items = new  Dictionary<string, AnotherTransform>();

        public AnotherTransformFactory()
        {
            this.loadingTab = "Loading Another Skill Transform database";
            this.loadedTab = " Another Skill Transform loaded.";
            this.databaseName = "Another Skill Transform";
        }

        public void Init(string path, Encoding encoding)
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

                    AnotherTransform item = new AnotherTransform();
                    item.ID = byte.Parse(paras[0]);
                    item.Level = byte.Parse(paras[1]);
                    item.PVPMode = byte.Parse(paras[2]);
                    item.EffectID = ushort.Parse(paras[3]);
                    item.MaxValueFlag = byte.Parse(paras[4]);
                    item.Str = ushort.Parse(paras[5]);
                    item.Mag = ushort.Parse(paras[6]);
                    item.Vit = ushort.Parse(paras[7]);
                    item.Dex = ushort.Parse(paras[8]);
                    item.Agi = ushort.Parse(paras[9]);
                    item.Int = ushort.Parse(paras[10]);
                    item.HP_ADD = ushort.Parse(paras[11]);
                    item.MP_ADD = ushort.Parse(paras[12]);
                    item.SP_ADD = ushort.Parse(paras[13]);
                    item.MIN_ATK_ADD = ushort.Parse(paras[14]);
                    item.MAX_ATK_ADD = ushort.Parse(paras[15]);
                    item.MIN_MATK_ADD = ushort.Parse(paras[16]);
                    item.MAX_MATK_ADD = ushort.Parse(paras[17]);
                    item.DEF_ADD = ushort.Parse(paras[18]);
                    item.MDEF_ADD = ushort.Parse(paras[19]);
                    item.HIT_MELEE_ADD = ushort.Parse(paras[20]);
                    item.HIT_RANGE_ADD = ushort.Parse(paras[21]);
                    item.AVOID_MELEE_ADD = ushort.Parse(paras[22]);
                    item.AVOID_RANGE_ADD = ushort.Parse(paras[23]);
                    item.APSD_ADD = ushort.Parse(paras[24]);
                    item.CSPD_ADD = ushort.Parse(paras[25]);
                    string Key = item.ID + "," + item.Level + "," + item.MaxValueFlag;
                    if (!Items.ContainsKey(Key))
                        Items.Add(Key, item);
                    else
                        Items[Key] = item;

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
    }
}
