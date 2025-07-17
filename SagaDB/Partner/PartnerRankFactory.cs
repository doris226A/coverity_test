/*using SagaLib;
using SagaLib.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.Partner
{
    public class PartnerRankFactory : Singleton<PartnerRankFactory>
    {
        Dictionary<byte, PartnerRank> Items = new Dictionary<byte, PartnerRank>();
        public void InitPartnerRankDB(string path, System.Text.Encoding encoding)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);
#if !Web
            string label = "Loading partner Rank database";
            Logger.ProgressBarShow(0, (uint)sr.BaseStream.Length, label);
#endif
            DateTime time = DateTime.Now;
            string[] paras;
            while (!sr.EndOfStream)
            {
                string line;
                line = sr.ReadLine();
                try
                {
                    if (line == "")
                        continue;

                    if (line.Substring(0, 1) == "#")
                        continue;

                    paras = line.Split(',');
                    PartnerRank rnk = new PartnerRank();
                    rnk.RankLevel = byte.Parse(paras[0]);
                    rnk.Rank = paras[1];
                    rnk.Exp = int.Parse(paras[2]);
                    rnk.GetPoint = byte.Parse(paras[3]);
                    if (Items.ContainsKey(rnk.RankLevel))
                        Items[rnk.RankLevel] = rnk;
                    else
                        Items.Add(rnk.RankLevel, rnk);
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }

        }
    }
}
*/