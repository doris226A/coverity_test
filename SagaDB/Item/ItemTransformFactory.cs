using SagaLib;
using SagaLib.VirtualFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SagaDB.Item
{
    public class ItemTransformFactory : Singleton<ItemTransformFactory>
    {
        public Dictionary<uint, ItemTransform> Items = new Dictionary<uint, ItemTransform>();

        public void Init(string path, Encoding encoding)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path), encoding);
            string[] paras;
            uint TID = 0;
            ItemTransform it;
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
                    if (paras.Length <= 0) continue;
                    if (paras[0] == "") continue;
                    if (paras[0] == "TRANSFORM")
                    {
                        it = new ItemTransform();
                        it.Product = uint.Parse(paras[2]);
                        TID = uint.Parse(paras[1]);
                        it.ChangeID = TID;
                        it.TransformCondition = paras[3];
                        it.Comment = paras[4];
                        Items.Add(TID, it);
                    }
                    else
                    {
                        List<ItemTransformStuffAbility> lst = new List<ItemTransformStuffAbility>();
                        if (paras.Length > 2)
                        {
                            for (int i = 1; i < paras.Length - 2; i += 2)
                            {
                                if (paras[i] != "")
                                {
                                    ItemTransformStuffAbility ability = new ItemTransformStuffAbility();
                                    ability.Ability = paras[i];
                                    ability.Value = uint.Parse(paras[i + 1]);
                                    lst.Add(ability);
                                }
                            }
                        }
                        Items[TID].Stuffs.Add(uint.Parse(paras[0]), lst);
                    }
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
