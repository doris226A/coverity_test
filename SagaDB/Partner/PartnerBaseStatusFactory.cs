using SagaDB.Actor;
using SagaLib;
using SagaLib.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SagaDB.Partner
{
    public class PartnerBaseStatusFactory : Singleton<PartnerBaseStatusFactory>
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

                    PartnerData partner = PartnerFactory.Instance.Partners[uint.Parse(paras[0])];

                    if (partner == null)
                    {
                        Logger.ShowError("Partner: " + paras[0] + " is not exists. skip.");
                        continue;
                    }

                    partner.hp_in = ushort.Parse(paras[2]);
                    partner.mp_in = ushort.Parse(paras[3]);
                    partner.sp_in = ushort.Parse(paras[4]);
                    partner.atk_min_in = ushort.Parse(paras[5]);
                    partner.atk_max_in = ushort.Parse(paras[6]);
                    partner.matk_min_in = ushort.Parse(paras[7]);
                    partner.matk_max_in = ushort.Parse(paras[8]);
                    partner.hit_melee_in = ushort.Parse(paras[9]);
                    partner.hit_ranged_in = ushort.Parse(paras[10]);

                    partner.def_add_in = ushort.Parse(paras[11]);
                    partner.def_in = ushort.Parse(paras[12]);
                    partner.mdef_add_in = ushort.Parse(paras[13]);
                    partner.mdef_in = ushort.Parse(paras[14]);

                    partner.avoid_melee_in = ushort.Parse(paras[15]);
                    partner.avoid_ranged_in = ushort.Parse(paras[16]);

                    partner.aspd_in = short.Parse(paras[17]);
                    partner.cspd_in = short.Parse(paras[18]);

                    partner.hp_rec_in = uint.Parse(paras[19]);
                    partner.mp_rec_in = uint.Parse(paras[20]);
                    partner.sp_rec_in = uint.Parse(paras[21]);

                    partner.hp_fn = ushort.Parse(paras[22]);
                    partner.mp_fn = ushort.Parse(paras[23]);
                    partner.sp_fn = ushort.Parse(paras[24]);
                    partner.atk_min_fn = ushort.Parse(paras[25]);
                    partner.atk_max_fn = ushort.Parse(paras[26]);
                    partner.matk_min_fn = ushort.Parse(paras[27]);
                    partner.matk_max_fn = ushort.Parse(paras[28]);
                    partner.hit_melee_fn = ushort.Parse(paras[29]);
                    partner.hit_ranged_fn = ushort.Parse(paras[30]);

                    partner.def_add_fn = ushort.Parse(paras[31]);
                    partner.def_fn = ushort.Parse(paras[32]);
                    partner.mdef_add_fn = ushort.Parse(paras[33]);
                    partner.mdef_fn = ushort.Parse(paras[34]);

                    partner.avoid_melee_fn = ushort.Parse(paras[35]);
                    partner.avoid_ranged_fn = ushort.Parse(paras[36]);

                    partner.aspd_fn = short.Parse(paras[37]);
                    partner.cspd_fn = short.Parse(paras[38]);

                    partner.hp_rec_fn = uint.Parse(paras[39]);
                    partner.mp_rec_fn = uint.Parse(paras[40]);
                    partner.sp_rec_fn = uint.Parse(paras[41]);

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
