using SagaLib;
using SagaLib.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaDB.Partner
{
    public class PartnerAdvanceStatusFactory : Singleton<PartnerAdvanceStatusFactory>
    {
        public void InitPartnerBaseStatus(string path, Encoding encoding)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);
            int count = 0;
#if !Web
            string label = "Loading partner info";
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
                    if (line == "") continue;
                    if (line.Substring(0, 1) == "#")
                        continue;
                    paras = line.Split(',');
                    for (int i = 0; i < paras.Length; i++)
                    {
                        if (paras[i] == "" || paras[i].ToLower() == "null")
                            paras[i] = "0";
                    }

                    if (!PartnerFactory.Instance.Partners.ContainsKey(uint.Parse(paras[0])))
                        continue;

                    PartnerData partner = PartnerFactory.Instance.GetPartnerData(uint.Parse(paras[0]));

                    PartnerAdvanceStatus partneradvance = new PartnerAdvanceStatus();
                    partneradvance.id = uint.Parse(paras[0]);
                    partneradvance.name = paras[1];
                    partneradvance.level = byte.Parse(paras[2]);

                    partneradvance.hp = ushort.Parse(paras[3]);
                    partneradvance.mp = ushort.Parse(paras[4]);
                    partneradvance.sp = ushort.Parse(paras[5]);
                    partneradvance.atk_min = ushort.Parse(paras[6]);
                    partneradvance.atk_max = ushort.Parse(paras[7]);
                    partneradvance.matk_min = ushort.Parse(paras[8]);
                    partneradvance.matk_max = ushort.Parse(paras[9]);
                    partneradvance.hit_melee = ushort.Parse(paras[10]);
                    partneradvance.hit_ranged = ushort.Parse(paras[11]);

                    partneradvance.def_add = ushort.Parse(paras[12]);
                    partneradvance.def = ushort.Parse(paras[13]);
                    partneradvance.mdef_add = ushort.Parse(paras[14]);
                    partneradvance.mdef = ushort.Parse(paras[15]);

                    partneradvance.avoid_melee = ushort.Parse(paras[16]);
                    partneradvance.avoid_ranged = ushort.Parse(paras[17]);

                    partneradvance.aspd = short.Parse(paras[18]);
                    partneradvance.cspd = short.Parse(paras[19]);

                    if (partner == null)
                    {
                        Logger.ShowError("Partner: "+partneradvance.id+"["+partneradvance.name+"] is not exists. skip.");
                        continue;
                    }

                    if (partner.AdvanceStatus.ContainsKey(partneradvance.level))
                        partner.AdvanceStatus[partneradvance.level] = partneradvance;
                    else
                        partner.AdvanceStatus.Add(partneradvance.level, partneradvance);

                    count++;

#if !Web
                    if ((DateTime.Now - time).TotalMilliseconds > 40)
                    {
                        time = DateTime.Now;
                        Logger.ProgressBarShow((uint)sr.BaseStream.Position, (uint)sr.BaseStream.Length, label);
                    }
#endif                   
                }
                catch (Exception ex)
                {
#if !Web
                    Logger.ShowError("Error on parsing Partner Base Info!\r\nat line:" + line);
                    Logger.ShowError(ex);
#endif
                }
            }
#if !Web
            Logger.ProgressBarHide(count + " Partners Base Infos Loaded.");
#endif
            sr.Close();
        }

        private bool toBool(string input)
        {
            if (input == "1") return true; else return false;
        }
    }
}
