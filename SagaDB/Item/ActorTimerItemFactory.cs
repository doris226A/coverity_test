
using SagaLib;
using SagaLib.VirtualFileSystem;
using System;
using System.Collections.Generic;

namespace SagaDB.Item
{
    public class ActorTimerItemFactory : Singleton<ActorTimerItemFactory>
    {
        public Dictionary<uint, ActorTimerItem> Items = new Dictionary<uint, ActorTimerItem>();

        public void Init(string path, System.Text.Encoding encoding)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);

            DateTime time = DateTime.Now;

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
                    var timeritem = new ActorTimerItem();
                    timeritem.ItemID = uint.Parse(paras[0]);
                    timeritem.ItemName = paras[1];
                    timeritem.BuffName = paras[2];
                    timeritem.BuffCodeName = paras[3];
                    timeritem.BuffValues = paras[4];
                    timeritem.DurationType = Convert.ToBoolean(int.Parse(paras[5]));
                    timeritem.Duration = uint.Parse(paras[6]);
                    timeritem.LogoutCount = Convert.ToBoolean(int.Parse(paras[7]));

                    if (!Items.ContainsKey(timeritem.ItemID))
                        Items.Add(timeritem.ItemID, timeritem);
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }

            sr.Close();
        }
    }
}
