using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Iris;
using SagaDB.Partner;
using SagaMap.Dungeon;
using SagaMap.Manager;
using SagaMap.Tasks.Dungeon;

namespace SagaMap.AncientArks
{
    public class AncientArkRoomsFactory : Factory<AncientArkRoomsFactory, AncientArkRoom>
    {
        public AncientArkRoomsFactory()
        {
            this.loadingTab = "Loading AncientArk database";
            this.loadedTab = " AncientArk loaded.";
            this.databaseName = "AncientArk";
            this.FactoryType = FactoryType.CSV;
        }

        protected override uint GetKey(AncientArkRoom item)
        {
            return item.Layer;
        }

        protected override void ParseCSV(AncientArkRoom item, string[] paras)
        {
            uint id = uint.Parse(paras[0]);
            byte layer = byte.Parse(paras[5]);
            byte room = byte.Parse(paras[6]);
            if (SagaMap.AncientArks.AncientArkFactory.Instance.Items.ContainsKey(id))
            {
                if (SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms.ContainsKey(layer))
                {
                    if (SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer].ContainsKey(room))
                    {
                        SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].Layer = byte.Parse(paras[5]);
                        SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].Room_ID = byte.Parse(paras[6]);
                        SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].mapID = uint.Parse(paras[7]);
                        if (uint.Parse(paras[8]) != 0) SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].gimmick_room_id.Add(uint.Parse(paras[8]));
                        if (uint.Parse(paras[9]) != 0) SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].gimmick_room_id.Add(uint.Parse(paras[9]));
                        if (uint.Parse(paras[10]) != 0) SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].gimmick_room_id.Add(uint.Parse(paras[10]));
                        if (uint.Parse(paras[11]) != 0) SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].gimmick_room_id.Add(uint.Parse(paras[11]));
                        if (uint.Parse(paras[12]) != 0) SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].gimmick_room_id.Add(uint.Parse(paras[12]));
                        SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].gimmick_layer_id.Add(uint.Parse(paras[13]));
                        SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].gimmick_layer_id.Add(uint.Parse(paras[14]));
                        SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].spawnfile = paras[21].ToString();
                        if (paras[15] != "0") SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[15]), byte.Parse(paras[16]));
                        if (paras[17] != "0") SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[17]), byte.Parse(paras[18]));
                        if (paras[19] != "0") SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer][room].Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[19]), byte.Parse(paras[20]));
                    }
                    else
                    {
                        AncientArkRoom ancientArkRoom = new AncientArkRoom();
                        ancientArkRoom.Layer = byte.Parse(paras[5]);
                        ancientArkRoom.Room_ID = byte.Parse(paras[6]);
                        ancientArkRoom.mapID = uint.Parse(paras[7]);
                        if (uint.Parse(paras[8]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[8]));
                        if (uint.Parse(paras[9]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[9]));
                        if (uint.Parse(paras[10]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[10]));
                        if (uint.Parse(paras[11]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[11]));
                        if (uint.Parse(paras[12]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[12]));
                        ancientArkRoom.gimmick_layer_id.Add(uint.Parse(paras[13]));
                        ancientArkRoom.gimmick_layer_id.Add(uint.Parse(paras[14]));
                        ancientArkRoom.spawnfile = paras[21].ToString();
                        if (paras[15] != "0") ancientArkRoom.Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[15]), byte.Parse(paras[16]));
                        if (paras[17] != "0") ancientArkRoom.Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[17]), byte.Parse(paras[18]));
                        if (paras[19] != "0") ancientArkRoom.Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[19]), byte.Parse(paras[20]));

                        SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms[layer].Add(room, ancientArkRoom);

                    }
                }
                else
                {
                    Dictionary<byte, AncientArkRoom> aar = new Dictionary<byte, AncientArkRoom>();
                    AncientArkRoom ancientArkRoom = new AncientArkRoom();
                    ancientArkRoom.Layer = byte.Parse(paras[5]);
                    ancientArkRoom.Room_ID = byte.Parse(paras[6]);
                    ancientArkRoom.mapID = uint.Parse(paras[7]);
                    if (uint.Parse(paras[8]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[8]));
                    if (uint.Parse(paras[9]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[9]));
                    if (uint.Parse(paras[10]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[10]));
                    if (uint.Parse(paras[11]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[11]));
                    if (uint.Parse(paras[12]) != 0) ancientArkRoom.gimmick_room_id.Add(uint.Parse(paras[12]));
                    ancientArkRoom.gimmick_layer_id.Add(uint.Parse(paras[13]));
                    ancientArkRoom.gimmick_layer_id.Add(uint.Parse(paras[14]));
                    ancientArkRoom.spawnfile = paras[21].ToString();
                    if (paras[15] != "0") ancientArkRoom.Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[15]), byte.Parse(paras[16]));
                    if (paras[17] != "0") ancientArkRoom.Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[17]), byte.Parse(paras[18]));
                    if (paras[19] != "0") ancientArkRoom.Rooms_warp.Add((AncientArkGateType)Enum.Parse(typeof(AncientArkGateType), paras[19]), byte.Parse(paras[20]));

                    aar.Add(room, ancientArkRoom);
                    SagaMap.AncientArks.AncientArkFactory.Instance.Items[id].Rooms.Add(ancientArkRoom.Layer, aar);
                }
            }
        }

        protected override void ParseXML(XmlElement root, XmlElement current, AncientArkRoom item)
        {
            throw new NotImplementedException();
        }
        private bool toBool(string input)
        {
            if (input == "1") return true; else return false;
        }


    }
}
