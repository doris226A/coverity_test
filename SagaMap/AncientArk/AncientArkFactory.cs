using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Iris;
using SagaDB.Partner;
using SagaLib.VirtualFileSystem;
using SagaDB.Item;
using SagaMap.Manager;
using SagaMap.Tasks.Partner;

namespace SagaMap.AncientArks
{
    public class AncientArkFactory : Factory<AncientArkFactory, AncientArkInfo>
    {
        public Dictionary<uint, AncientArk> AncientArks = new Dictionary<uint, AncientArk>();
        uint count = 0;
        public AncientArkFactory()
        {
            this.loadingTab = "Loading AncientArk database";
            this.loadedTab = " AncientArk loaded.";
            this.databaseName = "AncientArk";
            this.FactoryType = FactoryType.CSV;
        }
        public void RemoveAncientArk(uint id)
        {
            if (AncientArks.ContainsKey(id))
                AncientArks.Remove(id);
        }

        protected override uint GetKey(AncientArkInfo item)
        {
            return item.ID;
        }

        protected override void ParseCSV(AncientArkInfo item, string[] paras)
        {
            item.ID = uint.Parse(paras[0]);
            item.Name = paras[1].ToString();
            item.TimeLimit = int.Parse(paras[2]);
            item.Level = byte.Parse(paras[3]);
            item.Rebirth = toBool(paras[4]);
            //item.spawnfile = paras[21].ToString();
            item.Treasurefile = paras[22].ToString();
        }
        protected override void ParseXML(XmlElement root, XmlElement current, AncientArkInfo item)
        {
            throw new NotImplementedException();
        }
        private bool toBool(string input)
        {
            if (input == "1") return true; else return false;
        }



        public AncientArk CreateAncientArk(uint id, ActorPC creator, uint exitMap, byte exitX, byte exitY)
        {
            if (items.ContainsKey(id))
            {
                count++;
                AncientArk ark = new AncientArk(id);
                ark.Creator = creator;
                ark.Creator.AncientArkID = count;
                ark.AncientArkID = count;
                ark.Time = ark.Detail.TimeLimit;
                foreach (byte i in ark.Detail.Rooms.Keys)
                {
                    Dictionary<byte, Map> Temp_LayerMaps = new Dictionary<byte, Map>();
                    foreach (AncientArkRoom j in ark.Detail.Rooms[i].Values)
                    {
                        SagaMap.Map map = MapManager.Instance.GetMap(MapManager.Instance.CreateMapInstance(creator, j.mapID, exitMap, exitX, exitY));
                        map.IsAncientArk = true;
                        map.IsMapInstance = true;
                        AncientArkRoom_Maps aa_map = new AncientArkRoom_Maps();
                        aa_map.AncientArkID = count;
                        aa_map.Layer = i;
                        aa_map.Room_ID = j.Room_ID;
                        if (ark.Detail.Rooms[i][j.Room_ID].gimmick_room_id.Count != 0)
                        {
                            aa_map.complete = false;
                            aa_map.Gimmick_ID = ark.Detail.Rooms[i][j.Room_ID].gimmick_room_id.FirstOrDefault();
                        }
                        else
                        {
                            aa_map.complete = true;
                            if(ark.Detail.Rooms[i][j.Room_ID].gimmick_layer_id.FirstOrDefault() != 0)
                            {
                                aa_map.complete_layer = false;
                                aa_map.Gimmick_ID_layer = ark.Detail.Rooms[i][j.Room_ID].gimmick_layer_id.FirstOrDefault();
                                Mob.MobSpawnManager.Instance.LoadOne(j.spawnfile, map.ID, true, false);
                            }
                            else
                            {
                                aa_map.complete_layer = true;
                            }
                        }
                        map.AncientArk = aa_map;
                        Temp_LayerMaps.Add(j.Room_ID, map);
                        if (aa_map.Gimmick_ID != 0)
                        {
                            
                            //Mob.MobSpawnManager.Instance.LoadOne(ark.Detail.spawnfile, map.ID, true, false);
                            Mob.MobSpawnManager.Instance.LoadOne(j.spawnfile, map.ID, true, false);
                            SagaMap.AncientArks.AncientArkGimmick gimmick_layer = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[aa_map.Gimmick_ID];
                            switch (gimmick_layer.type)
                            {
                                case AncientArkType.CANDLE_1:
                                    SagaMap.Manager.AncientArkManager.Instance.蠟燭_點燃所有蠟燭(map);
                                    break;
                                case AncientArkType.CANDLE_2:
                                    SagaMap.Manager.AncientArkManager.Instance.蠟燭_點燃正確的蠟燭(map);
                                    break;
                                case AncientArkType.CANDLE_3:
                                    SagaMap.Manager.AncientArkManager.Instance.蠟燭_點燃特定職業的蠟燭(map);
                                    break;
                                case AncientArkType.CANDLE_4:
                                    SagaMap.Manager.AncientArkManager.Instance.蠟燭_點燃特定種族的蠟燭(map);
                                    break;
                                /*case AncientArkType.CANDLE_5:
                                    SagaMap.Manager.AncientArkManager.Instance.蠟燭_消滅所有蠟燭(map);
                                    break;*/
                                case AncientArkType.CANDLE_6:
                                    SagaMap.Manager.AncientArkManager.Instance.蠟燭_全員摸蠟燭(map);
                                    break;
                                case AncientArkType.ASK:
                                    //int randomKey = AncientArkGimmickFactory.Instance.Quest_Ask.Keys.ElementAt(SagaLib.Global.Random.Next(1,AncientArkGimmickFactory.Instance.Quest_Ask.Count) - 1);
                                    QuestAsk randomKey = AncientArkGimmickFactory.Instance.QuestAsk.ElementAt(SagaLib.Global.Random.Next(1, AncientArkGimmickFactory.Instance.QuestAsk.Count) - 1);
                                    aa_map.Ask = randomKey;
                                    break;
                            }
                        }
                        
                    }
                    ark.Layer_Rooms.Add(i, Temp_LayerMaps);
                }
                /*
                 
                    if (ifEnd)
                        Mob.MobSpawnManager.Instance.LoadOne(dungeon.SpawnFile, nowMap.Map.ID, false, true);
                    else
                        Mob.MobSpawnManager.Instance.LoadOne(dungeon.SpawnFile, nowMap.Map.ID, true, false);
                 */

                /*if (creator.Party != null)
                {
                    foreach (ActorPC i in creator.Party.Members.Values)
                    {
                        if (i.Online)
                        {
                            ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)i.e;
                            //eh.Client.SendSystemMessage(creator.Name + LocalManager.Instance.Strings.ITD_CREATED);
                        }
                    }
                }
                else
                {
                    ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)creator.e;
                    //eh.Client.SendSystemMessage(creator.Name + LocalManager.Instance.Strings.ITD_CREATED);
                }*/


                ark.DestroyTask = new Tasks.AncientArk.AncientArk(ark, ark.Detail.TimeLimit);
                ark.DestroyTask.Activate();
                AncientArks.Add(ark.AncientArkID, ark);
                //Logger.ShowInfo("玩家: {" + creator.Name + "} 成功创建地下城[" + dungeon.Name + "](" + id.ToString() + ") ");//改一下這句，不然會導致無隊伍的狀態下進遺跡會卡一下
                return ark;
            }
            else
                return null;
        }
    }
}
