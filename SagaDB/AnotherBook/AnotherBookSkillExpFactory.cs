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
    public class AnotherBookSkillExpFactory : Singleton<AnotherBookSkillExpFactory>
    {
        protected string loadingTab = "";
        protected string loadedTab = "";
        protected string databaseName = "";

        public AnotherBookSkillExpFactory()
        {
            this.loadingTab = "Loading Another Book Skill Exp database";
            this.loadedTab = " Another Book Skill Exp loaded.";
            this.databaseName = "Another Book Skill Exp";
        }

        public Dictionary<string, List<AnotherBookSkillExp>> Items = new Dictionary<string, List<AnotherBookSkillExp>>();
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

                    AnotherBookSkillExp item = new AnotherBookSkillExp();
                    item.ID = byte.Parse(paras[0]);
                    item.SkillID = ushort.Parse(paras[1]);
                    item.Level = byte.Parse(paras[2]);
                    item.Exp = ulong.Parse(paras[3]);

                    string Key = item.ID + "," + item.SkillID;
                    if (!Items.ContainsKey(Key))
                        Items.Add(Key, new List<AnotherBookSkillExp>());
                    Items[Key].Add(item);

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
