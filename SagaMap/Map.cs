﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;//二哈更改測試

using SagaDB.Actor;
using SagaDB.Item;
using SagaDB.Map;
using SagaLib;
using SagaMap.Manager;
using System.Linq;

namespace SagaMap
{
    public partial class Map
    {
        public bool returnori = false;
        public uint OriID;


        private string name;
        private uint id;

        private ushort width, height;
        public uint ID { get { return this.id; } set { this.id = value; } }
        public string Name { get { return this.name; } }

        public ushort Width { get { return this.width; } }
        public ushort Height { get { return this.height; } }

        private ConcurrentDictionary<uint, Actor> actorsByID;
        //private Dictionary<uint, Actor> actorsByID;//二哈更改測試
        private ConcurrentDictionary<uint, List<Actor>> actorsByRegion;//二哈更改1015
        private Dictionary<string, ActorPC> pcByName;

        private const uint ID_BORDER_MOB = 10000;
        private const uint ID_BORDER_PET = 20000;
        private const uint ID_BORDER_GOLEM = 40000;
        private const uint ID_BORDER_ITEM = 50000;
        private const uint ID_BORDER_EVENT = 60000;
        private const uint ID_BORDER_ANOMOB = 110000;
        private const uint ID_BORDER_SKILL = 120000;
        private const uint ID_BORDER2 = 0x3B9ACA00;//border for possession items

        private static uint nextPcId;
        private uint nextMobId;
        private uint nextItemId;
        private uint nextPetId;
        private uint nextEventId;
        private uint nextGolemId;
        private uint nextAnoMobID;
        private uint nextSkillID;

        private static readonly object registerlock = new object(); //注册对象时的锁

        public MapInfo Info;

        public enum MOVE_TYPE { START, STOP };
        public enum EVENT_TYPE
        {
            APPEAR, DISAPPEAR, MOTION, EMOTION, CHAT, SKILL, CHANGE_EQUIP, CHANGE_STATUS, BUFF_CHANGE,
            ACTOR_SELECTION, YAW_UPDATE, CHAR_INFO_UPDATE, PLAYER_SIZE_UPDATE, ATTACK, HPMPSP_UPDATE,
            LEVEL_UP, PLAYER_MODE, SHOW_EFFECT, POSSESSION, PARTY_NAME_UPDATE,
            SPEED_UPDATE, SIGN_UPDATE, RING_NAME_UPDATE, WRP_RANKING_UPDATE, ATTACK_TYPE_CHANGE, PLAYERSHOP_CHANGE, PLAYERSHOP_CHANGE_CLOSE,
            WAITTYPE, FURNITURE_SIT, PAPER_CHANGE, TELEPORT, SKILL_CANCEL,
        }

        public enum TOALL_EVENT_TYPE { CHAT };

        public Map(MapInfo info)
        {
            this.id = info.id;
            this.name = info.name;
            this.width = info.width;
            this.height = info.height;
            this.Info = info;

            //this.actorsByID = new Dictionary<uint, Actor>();
            this.actorsByID = new ConcurrentDictionary<uint, Actor>();//二哈更改測試
            this.actorsByRegion = new ConcurrentDictionary<uint, List<Actor>>();//二哈更改1015
            this.pcByName = new Dictionary<string, ActorPC>();
            if (nextPcId == 0)
                nextPcId = 0x10;
            this.nextMobId = ID_BORDER_MOB + 1;
            this.nextItemId = ID_BORDER_ITEM + 1;
            this.nextPetId = ID_BORDER_PET + 1;
            this.nextEventId = ID_BORDER_EVENT + 1;
            this.nextGolemId = ID_BORDER_GOLEM + 1;
            this.nextAnoMobID = ID_BORDER_ANOMOB + 1;
            this.nextSkillID = ID_BORDER_SKILL + 1;
        }


        public short[] GetRandomPos()
        {
            short[] ret = new short[2];

            ret[0] = (short)Global.Random.Next(-12700, +12700);
            ret[1] = (short)Global.Random.Next(-12700, +12700);

            return ret;
        }

        public short[] GetRandomPosAroundActor(Actor actor)
        {
            short[] ret = new short[2];

            ret[0] = (short)Global.Random.Next(actor.X - 100, actor.X + 100);
            ret[1] = (short)Global.Random.Next(actor.Y - 100, actor.Y + 100);

            return ret;
        }

        public short[] GetRandomPosAroundActor2(Actor actor)
        {
            short[] ret = new short[2];

            ret[0] = (short)Global.Random.Next(actor.X - 600, actor.X + 600);
            ret[1] = (short)Global.Random.Next(actor.Y - 600, actor.Y + 600);

            return ret;
        }

        public short[] GetRandomPosAroundPos(short x, short y, int range)
        {
            short[] ret = new short[2];
            byte new_x, new_y;
            int count = 0;
            do
            {
                if (count >= 1000)
                {
                    ret[0] = x;
                    ret[1] = y;
                    return ret;
                }
                ret[0] = (short)Global.Random.Next(x - range, x + range);
                ret[1] = (short)Global.Random.Next(y - range, y + range);
                new_x = Global.PosX16to8(ret[0], this.width);
                new_y = Global.PosY16to8(ret[1], this.height);
                count++;
                if (new_x >= this.width)
                    new_x = (byte)(this.width - 1);
                if (new_y >= this.height)
                    new_y = (byte)(this.height - 1);
            } while (this.Info.walkable[new_x, new_y] != 2);
            return ret;
        }

        // public Dictionary<uint, Actor> Actors { get { return this.actorsByID; } }
        public ConcurrentDictionary<uint, Actor> Actors { get { return this.actorsByID; } }//二哈更改測試

        public Actor GetActor(uint id)
        {
            try
            {
                //return actorsByID[id];
                return actorsByID.TryGetValue(id, out Actor actor) ? actor : null;//二哈更改測試
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActorPC GetPC(string name)
        {
            try
            {
                return pcByName[name];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActorPC GetPC(uint charID)
        {
            try
            {
                var chr = from c in pcByName.Values
                          where c.CharID == charID
                          select c;
                return chr.First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private uint GetNewActorID(ActorType type)
        {
            uint newID = 0;
            uint startID = 0;

            if (type == ActorType.PC)
            {
                newID = nextPcId;
                startID = nextPcId;
            }
            else
            {
                if (type == ActorType.MOB)
                {
                    newID = this.nextMobId;
                    startID = this.nextMobId;
                }
                else if (type == ActorType.PET || type == ActorType.SHADOW || type == ActorType.PARTNER)
                {
                    newID = this.nextPetId;
                    startID = this.nextPetId;
                }
                else if (type == ActorType.EVENT || type == ActorType.FURNITURE)
                {
                    newID = this.nextEventId;
                    startID = this.nextEventId;
                }
                else if (type == ActorType.GOLEM)
                {
                    newID = this.nextGolemId;
                    startID = this.nextGolemId;
                }
                else if (type == ActorType.ANOTHERMOB)
                {
                    newID = this.nextAnoMobID;
                    startID = this.nextAnoMobID;
                }
                else if (type == ActorType.SKILL)
                {
                    newID = this.nextSkillID;
                    startID = this.nextSkillID;
                }
                else
                {
                    newID = this.nextItemId;
                    startID = this.nextItemId;
                }
            }

            if (newID >= 10000 && type == ActorType.PC)
                newID = 16;

            if (newID >= 20000 && type == ActorType.MOB)
                newID = ID_BORDER_MOB + 1;
            if (newID >= 30000 && (type == ActorType.PET || type == ActorType.PARTNER))
                newID = ID_BORDER_PET + 1;
            if (newID >= 50000 && type == ActorType.GOLEM)
                newID = ID_BORDER_GOLEM + 1;
            if (newID >= 60000 && type == ActorType.ITEM)
                newID = ID_BORDER_ITEM + 1;
            if (newID >= 70000 && type == ActorType.EVENT)
                newID = ID_BORDER_EVENT + 1;
            if (newID >= 120000 && type == ActorType.ANOTHERMOB)
                newID = ID_BORDER_ANOMOB + 1;
            if (newID >= 140000 && type == ActorType.SKILL)
                newID = ID_BORDER_SKILL + 1;
            if (newID >= UInt32.MaxValue)
                newID = 1;

            while (this.actorsByID.ContainsKey(newID))
            {
                newID++;

                if (newID >= 10000 && type == ActorType.PC)
                    newID = 16;

                if (newID >= 20000 && type == ActorType.MOB)
                    newID = ID_BORDER_MOB + 1;
                if (newID >= 30000 && (type == ActorType.PET || type == ActorType.PARTNER))
                    newID = ID_BORDER_PET + 1;
                if (newID >= 50000 && type == ActorType.GOLEM)
                    newID = ID_BORDER_GOLEM + 1;
                if (newID >= 60000 && type == ActorType.ITEM)
                    newID = ID_BORDER_ITEM + 1;
                if (newID >= 70000 && type == ActorType.EVENT)
                    newID = ID_BORDER_EVENT + 1;
                if (newID >= 120000 && type == ActorType.ANOTHERMOB)
                    newID = ID_BORDER_ANOMOB + 1;
                if (newID >= 140000 && type == ActorType.SKILL)
                    newID = ID_BORDER_SKILL + 1;
                if (newID >= UInt32.MaxValue)
                    newID = 1;

                if (newID == startID) return 0;
            }

            if (type == ActorType.PC)
                nextPcId = newID + 1;
            else
                if (type == ActorType.MOB)
                this.nextMobId = newID + 1;
            else if ((type == ActorType.PET || type == ActorType.PARTNER))
                this.nextPetId = newID + 1;
            else if (type == ActorType.FURNITURE || type == ActorType.EVENT)
                this.nextEventId = newID + 1;
            else if (type == ActorType.GOLEM)
                this.nextGolemId = newID + 1;
            else if (type == ActorType.ANOTHERMOB)
                this.nextAnoMobID = newID + 1;
            else if (type == ActorType.SKILL)
                this.nextSkillID = newID + 1;
            else
                this.nextItemId = newID + 1;


            return newID;
        }

        public bool RegisterActor(Actor nActor)
        {
            // default: no success
            bool succes = false;

            // set the actorID and the actor's region on this map
            uint newID = 0;
            if (Global.clientMananger != null)
                ClientManager.EnterCriticalArea();
            if (nActor.type == ActorType.MOB && ((ActorMob)nActor).AnotherID != 0)
                newID = this.GetNewActorID(ActorType.ANOTHERMOB);
            else
                newID = this.GetNewActorID(nActor.type);
            if (Global.clientMananger != null)
                ClientManager.LeaveCriticalArea();
            if (nActor.type == ActorType.ITEM)
            {
                ActorItem item = (ActorItem)nActor;
                if (item.PossessionItem)
                    newID += ID_BORDER2;
            }

            if (newID != 0)
            {
                nActor.ActorID = newID;
                nActor.region = this.GetRegion(nActor.X, nActor.Y);

                if (GetRegionPlayerCount(nActor.region) == 0 && nActor.type == ActorType.PC)
                {
                    MobAIToggle(nActor.region, true);
                }
                // make the actor invisible (when the actor is ready: set it to false & call OnActorVisibilityChange)
                nActor.invisble = true;

                // add the new actor to the tables
                //DateTime time = DateTime.Now;
                if (Global.clientMananger != null)
                    ClientManager.EnterCriticalArea();
                try
                {
                    while (actorsByID.ContainsKey(nActor.ActorID))
                    {
                        if (nActor.type == ActorType.MOB && ((ActorMob)nActor).AnotherID != 0)
                            nActor.ActorID = this.GetNewActorID(ActorType.ANOTHERMOB);
                        else
                            nActor.ActorID = this.GetNewActorID(nActor.type);
                        if (nActor.type == ActorType.ITEM && ((ActorItem)nActor).PossessionItem)
                            nActor.ActorID += ID_BORDER2;
                    }
                    //actorsByID.Add(nActor.ActorID, nActor);
                    actorsByID.TryAdd(nActor.ActorID, nActor);//二哈更改測試
                }
                catch (Exception ex)
                {
                    SagaLib.Logger.ShowError(ex);
                    SagaLib.Logger.ShowError("oh,fuck!");
                }
                if (Global.clientMananger != null)
                    ClientManager.LeaveCriticalArea();
                //double usedtime = (DateTime.Now - time).TotalMilliseconds;
                //if (usedtime > 0)
                //    Logger.ShowError("在地图:" + ID + " 注册ID: " + nActor.ActorID + " 花费时间:" + usedtime + "ms");

                if (nActor.type == ActorType.PC && !this.pcByName.ContainsKey(nActor.Name))
                    this.pcByName.Add(nActor.Name, (ActorPC)nActor);

                if (!this.actorsByRegion.ContainsKey(nActor.region))
                    this.actorsByRegion.TryAdd(nActor.region, new List<Actor>());//二哈更改1015

                this.actorsByRegion[nActor.region].Add(nActor);

                succes = true;
            }
            nActor.MapID = this.ID;
            if (nActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)nActor;
                /*if (this.Info.Flag.Test(MapFlags.Wrp))//ECOKEY 攻防戰 - 暫時關閉wrp
                {
                    pc.Mode = PlayerMode.WRP;
                }
                else */
                if (pc.Mode == PlayerMode.KNIGHT_EAST || pc.Mode == PlayerMode.KNIGHT_FLOWER || pc.Mode == PlayerMode.KNIGHT_NORTH || pc.Mode == PlayerMode.KNIGHT_ROCK || pc.Mode == PlayerMode.KNIGHT_SOUTH || pc.Mode == PlayerMode.KNIGHT_WEST)
                {
                    pc.Mode = pc.Mode;
                }
                else
                {
                    pc.Mode = PlayerMode.NORMAL;
                }
            }
            nActor.e.OnCreate(succes);
            return succes;
        }

        public bool RegisterActor(Actor nActor, uint SessionID)
        {
            // default: no success
            bool succes = false;

            // set the actorID and the actor's region on this map
            uint newID = SessionID;

            if (newID != 0)
            {
                nActor.ActorID = newID;
                nActor.region = this.GetRegion(nActor.X, nActor.Y);
                if (GetRegionPlayerCount(nActor.region) == 0 && nActor.type == ActorType.PC)
                {
                    MobAIToggle(nActor.region, true);
                }

                // make the actor invisible (when the actor is ready: set it to false & call OnActorVisibilityChange)
                nActor.invisble = true;

                // add the new actor to the tables
                // if (!this.actorsByID.ContainsKey(nActor.ActorID)) this.actorsByID.Add(nActor.ActorID, nActor);
                if (!this.actorsByID.ContainsKey(nActor.ActorID)) this.actorsByID.TryAdd(nActor.ActorID, nActor);//二哈更改測試


                if (nActor.type == ActorType.PC && !this.pcByName.ContainsKey(nActor.Name))
                    this.pcByName.Add(nActor.Name, (ActorPC)nActor);

                if (!this.actorsByRegion.ContainsKey(nActor.region))
                    this.actorsByRegion.TryAdd(nActor.region, new List<Actor>());//二哈更改1015

                this.actorsByRegion[nActor.region].Add(nActor);

                succes = true;
            }
            if (nActor.type == ActorType.PC)
            {
                ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)nActor.e;
                if (eh.Client.state != SagaMap.Network.Client.MapClient.SESSION_STATE.DISCONNECTED)
                    eh.Client.state = SagaMap.Network.Client.MapClient.SESSION_STATE.LOADING;
                else
                {
                    MapServer.charDB.SaveChar((ActorPC)nActor, false, false);
                    MapServer.accountDB.WriteUser(((ActorPC)nActor).Account);

                }
            }
            nActor.MapID = this.ID;
            if (nActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)nActor;
                /*if (this.Info.Flag.Test(MapFlags.Wrp))//ECOKEY 攻防戰 - 暫時關閉wrp
                {
                    pc.Mode = PlayerMode.WRP;
                }
                else */
                if (pc.Mode == PlayerMode.KNIGHT_EAST || pc.Mode == PlayerMode.KNIGHT_FLOWER || pc.Mode == PlayerMode.KNIGHT_NORTH || pc.Mode == PlayerMode.KNIGHT_ROCK || pc.Mode == PlayerMode.KNIGHT_SOUTH || pc.Mode == PlayerMode.KNIGHT_WEST)
                {
                    pc.Mode = pc.Mode;
                }
                else
                {
                    pc.Mode = PlayerMode.NORMAL;
                }
            }
            nActor.e.OnCreate(succes);
            return succes;
        }

        public void OnActorVisibilityChange(Actor dActor)
        {
            if (dActor.invisble)
            {
                dActor.invisble = false;
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.DISAPPEAR, null, dActor, false);
                dActor.invisble = true;
            }

            else
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.APPEAR, null, dActor, false);
        }

        public void DeleteActor(Actor dActor)
        {
            this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.DISAPPEAR, null, dActor, false);

            if (dActor.type == ActorType.PC && this.pcByName.ContainsKey(dActor.Name))
                this.pcByName.Remove(dActor.Name);
            //ClientManager.EnterCriticalArea();
            //this.actorsByID.Remove(dActor.ActorID);
            this.actorsByID.TryRemove(dActor.ActorID, out _);//二哈更改測試


            //ECOKEY 四重奏error
            lock (this.actorsByRegion)
            {
                if (this.actorsByRegion.ContainsKey(dActor.region))
                {
                    this.actorsByRegion[dActor.region].Remove(dActor);
                    if (GetRegionPlayerCount(dActor.region) == 0)
                    {
                        MobAIToggle(dActor.region, false);
                    }
                }
            }


            //ClientManager.LeaveCriticalArea();
            dActor.e.OnDelete();
            if (this.IsDungeon)
            {
                if (this.DungeonMap.MapType == SagaMap.Dungeon.MapType.End)
                {
                    int count = 0;
                    foreach (Actor i in actorsByID.Values)
                    {
                        if (i.type == ActorType.MOB)
                            count++;
                    }
                    if (count == 0)
                    {
                        Dungeon.DungeonFactory.Instance.GetDungeon(this.creator.DungeonID).Destory(SagaMap.Dungeon.DestroyType.BossDown);
                    }
                }
            }
        }

        public class BuffChangeEventArgs : MapEventArgs
        {
            public ActorPC Character { get; }

            public BuffChangeEventArgs(ActorPC character)
            {
                Character = character;
            }
        }

        public void MoveActor(MOVE_TYPE mType, Actor mActor, short[] pos, ushort dir, ushort speed)
        {
            MoveActor(mType, mActor, pos, dir, speed, false);
        }
        public void MoveActor(MOVE_TYPE mType, Actor mActor, short[] pos, ushort dir, ushort speed, bool sendToSelf)
        {
            MoveActor(mType, mActor, pos, dir, speed, sendToSelf, MoveType.RUN);
        }
        // make sure only 1 thread at a time is executing this method
        public void MoveActor(MOVE_TYPE mType, Actor mActor, short[] pos, ushort dir, ushort speed, bool sendToSelf, MoveType moveType)
        {
            try
            {
                bool knockBack = false;
                if (mActor.Status != null)
                {
                    if (mActor.Status.Additions.ContainsKey("Meditatioon"))
                    {
                        mActor.Status.Additions["Meditatioon"].AdditionEnd();
                        mActor.Status.Additions.TryRemove("Meditatioon", out _);
                    }
                    if (mActor.Status.Additions.ContainsKey("Hiding"))
                    {
                        mActor.Status.Additions["Hiding"].AdditionEnd();
                        mActor.Status.Additions.TryRemove("Hiding", out _);
                    }
                    if (mActor.Status.Additions.ContainsKey("fish"))
                    {
                        mActor.Status.Additions["fish"].AdditionEnd();
                        mActor.Status.Additions.TryRemove("fish", out _);
                    }
                    if (mActor.Status.Additions.ContainsKey("IAmTree"))
                    {
                        mActor.Status.Additions["IAmTree"].AdditionEnd();
                        mActor.Status.Additions.TryRemove("IAmTree", out _);
                    }

                }
                // check wheter the destination is in range, if not kick the client
                if (mActor.HP == 0 && mActor.type != ActorType.GOLEM && mActor.type != ActorType.SKILL && mActor.type != ActorType.FURNITURE)//ECOKEY 家具重放BUG修復
                {
                    pos = new short[2] { mActor.X, mActor.Y };
                    dir = 600;
                    knockBack = true;
                    sendToSelf = true;
                }
                if (mActor.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)mActor;
                    if (pc.CInt["WaitEventID"] != 0)
                    {
                        SagaMap.Network.Client.MapClient.FromActorPC(pc).EventActivate((uint)pc.CInt["WaitEventID"]);
                        pc.CInt["WaitEventID"] = 0;
                    }

                    if (pc.Motion == MotionType.STAND)
                    {
                        pc.Buff.Sit = false;
                        if (pc.Tasks.ContainsKey("Regeneration"))
                        {
                            pc.Tasks["Regeneration"].Deactivate();
                            pc.Tasks.Remove("Regeneration");
                            BuffChangeEventArgs buffChangeEventArgs = new BuffChangeEventArgs(pc);
                            this.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, buffChangeEventArgs, pc, true);
                        }
                    }
                    pc.Motion = MotionType.STAND;
                    pc.MotionLoop = false;
                }
                if (mActor.type == ActorType.PC && !knockBack)
                {
                    ActorPC pc = (ActorPC)mActor;
                    List<ActorPC> possessioned = pc.PossesionedActors;
                    foreach (ActorPC i in possessioned)
                    {
                        if (i == pc) continue;
                        if (i.MapID == mActor.MapID)
                            MoveActor(mType, i, pos, dir, speed);
                    }
                    if (pc.Online)
                    {
                        Network.Client.MapClient client = Network.Client.MapClient.FromActorPC(pc);
                        if ((DateTime.Now - client.moveStamp).TotalSeconds >= 2)
                        {
                            if (client.Character.Party != null)
                            {
                                PartyManager.Instance.UpdateMemberPosition(client.Character.Party, client.Character);
                            }
                            client.moveStamp = DateTime.Now;

                        }
                    }
                }


                //scroll through all actors that "could" see the mActor at "from"
                //or are going "to see" mActor, or are still seeing mActor
                if (!knockBack)
                {

                    for (short deltaY = -1; deltaY <= 1; deltaY++)
                    {
                        for (short deltaX = -1; deltaX <= 1; deltaX++)
                        {
                            uint region = (uint)(mActor.region + (deltaX * 10000) + deltaY);
                            if (!this.actorsByRegion.ContainsKey(region)) continue;

                            //ClientManager.EnterCriticalArea();
                            Actor[] list = actorsByRegion[region].ToArray();
                            //ClientManager.LeaveCriticalArea();

                            foreach (Actor actor in list)
                            {
                                if (actor.ActorID == mActor.ActorID && !sendToSelf) continue;
                                if (!this.actorsByRegion[region].Contains(actor))
                                    continue;
                                if (actor.Status == null)
                                {
                                    this.DeleteActor(actor);
                                    continue;
                                }
                                // A) INFORM OTHER ACTORS

                                //actor "could" see mActor at its "from" position
                                if (this.ACanSeeB(actor, mActor))
                                {
                                    //actor will still be able to see mActor
                                    if (this.ACanSeeB(actor, mActor, pos[0], pos[1]))
                                    {
                                        if (mType == MOVE_TYPE.START)
                                        {


                                            if (moveType != MoveType.RUN)
                                                actor.e.OnActorStartsMoving(mActor, pos, dir, speed, moveType);
                                            else
                                                actor.e.OnActorStartsMoving(mActor, pos, dir, speed);
                                        }
                                        else
                                            actor.e.OnActorStopsMoving(mActor, pos, dir, speed);
                                    }
                                    //actor won't be able to see mActor anymore
                                    else actor.e.OnActorDisappears(mActor);
                                }
                                //actor "could not" see mActor, but will be able to see him now
                                else if (this.ACanSeeB(actor, mActor, pos[0], pos[1]))
                                {
                                    actor.e.OnActorAppears(mActor);

                                    //send move / move stop
                                    if (mType == MOVE_TYPE.START)
                                    {
                                        if (moveType != MoveType.RUN)
                                            actor.e.OnActorStartsMoving(mActor, pos, dir, speed, moveType);
                                        else
                                            actor.e.OnActorStartsMoving(mActor, pos, dir, speed);
                                    }
                                    else
                                        actor.e.OnActorStopsMoving(mActor, pos, dir, speed);
                                }

                                // B) INFORM mActor
                                //mActor "could" see actor on its "from" position
                                if (this.ACanSeeB(mActor, actor))
                                {
                                    //mActor won't be able to see actor anymore
                                    if (!this.ACanSeeB(mActor, pos[0], pos[1], actor))
                                        mActor.e.OnActorDisappears(actor);
                                    //mAactor will still be able to see actor
                                    else { }
                                }

                                else if (this.ACanSeeB(mActor, pos[0], pos[1], actor))
                                {
                                    //mActor "could not" see actor, but will be able to see him now
                                    //send pcinfo
                                    mActor.e.OnActorAppears(actor);
                                }
                            }
                        }
                    }
                }
                else
                    mActor.e.OnActorStopsMoving(mActor, pos, dir, speed);

                //update x/y/z/yaw of the actor    
                mActor.LastX = mActor.X;
                mActor.LastY = mActor.Y;
                mActor.X = pos[0];
                mActor.Y = pos[1];
                if (mActor.type == ActorType.FURNITURE)
                {
                    ((ActorFurniture)mActor).Z = pos[2];
                }
                if (dir <= 360)
                    mActor.Dir = dir;

                //update the region of the actor
                uint newRegion = this.GetRegion(pos[0], pos[1]);
                if (mActor.region != newRegion)
                {
                    this.actorsByRegion[mActor.region].Remove(mActor);
                    //turn off all the ai if the old region has no player on it
                    if (GetRegionPlayerCount(mActor.region) == 0)
                    {
                        MobAIToggle(mActor.region, false);
                    }
                    mActor.region = newRegion;
                    if (GetRegionPlayerCount(mActor.region) == 0 && mActor.type == ActorType.PC)
                    {
                        MobAIToggle(mActor.region, true);
                    }

                    if (!this.actorsByRegion.ContainsKey(newRegion))
                        this.actorsByRegion.TryAdd(newRegion, new List<Actor>());//二哈更改1015

                    this.actorsByRegion[newRegion].Add(mActor);
                }
            }

            catch (Exception ex)
            { Logger.ShowError(ex); }
            //moveCounter--;
        }

        public int GetRegionPlayerCount(uint region)
        {
            List<Actor> actors;
            int count = 0;
            if (!this.actorsByRegion.ContainsKey(region)) return 0;
            actors = this.actorsByRegion[region];
            List<int> removelist = new List<int>();
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor;
                if (actors[i] == null)
                {
                    removelist.Add(i);
                    continue;
                }
                actor = actors[i];
                if (actor.type == ActorType.PC) count++;
            }
            foreach (int i in removelist)
            {
                actors.RemoveAt(i);
            }
            return count;
        }

        public void MobAIToggle(uint region, bool toggle)
        {
           
        }

        public bool MoveStepIsInRange(Actor mActor, short[] to)
        {
            if (mActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                Network.Client.MapClient client = Network.Client.MapClient.FromActorPC(pc);
                if (client.AI != null)
                {
                    if (client.AI.Activated)
                        return true;
                }
                TimeSpan span = DateTime.Now - client.moveCheckStamp;
                //if (span.TotalMilliseconds > 50)
                {

                    double maximal;
                    if (span.TotalMilliseconds > 1000)
                        maximal = mActor.Speed * 2f;
                    else if (span.TotalMilliseconds > 100)
                        maximal = mActor.Speed * (span.TotalMilliseconds / 1000) * 5f;
                    else
                        maximal = mActor.Speed * 0.5f;
                    // Disabled, until we have something better
                    if (System.Math.Abs(mActor.X - to[0]) > maximal)
                        return false;
                    if (System.Math.Abs(mActor.Y - to[1]) > maximal)
                        return false;
                    //we don't check for z , yet, to allow falling from great hight
                    //if (System.Math.Abs(mActor.z - to[2]) > mActor.maxMoveRange) return false;
                }
            }
            return true;
        }


        public uint GetRegion(float x, float y)
        {

            uint REGION_DIAMETER = Global.MAX_SIGHT_RANGE * 2;

            // best case we should now load the size of the map from a config file, however that's not
            // possible yet, so we just create a region code off the values x/y

         
            // init nx,ny
            bool nx = false;
            bool ny = false;
            // make x,y positive
            if (x < 0) { x = x - (2 * x); nx = true; }
            if (y < 0) { y = y - (2 * y); ny = true; }
            // convert x,y to uints
            uint ux = (uint)x;
            uint uy = (uint)y;
            // divide through REGION_DIAMETER
            ux = (uint)(ux / REGION_DIAMETER);
            uy = (uint)(uy / REGION_DIAMETER);
            // calc ux
            if (ux > 4999) ux = 4999;
            if (!nx) ux = ux + 5000;
            else ux = 5000 - ux;
            // calc uy
            if (uy > 4999) uy = 4999;
            if (!ny) uy = uy + 5000;
            else uy = 5000 - uy;
            // finally generate the region code and return it
            return (uint)((ux * 10000) + uy);
        }

        public bool ACanSeeB(Actor A, Actor B)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(A.X - B.X) > A.sightRange) return false;
            if (System.Math.Abs(A.Y - B.Y) > A.sightRange) return false;
            return true;
        }

        public bool ACanSeeB(Actor A, Actor B, float bx, float by)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(A.X - bx) > A.sightRange) return false;
            if (System.Math.Abs(A.Y - by) > A.sightRange) return false;
            return true;
        }

        public bool ACanSeeB(Actor A, float ax, float ay, Actor B)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(ax - B.X) > A.sightRange) return false;
            if (System.Math.Abs(ay - B.Y) > A.sightRange) return false;
            return true;
        }

        public bool ACanSeeB(Actor A, Actor B, float sightrange)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(A.X - B.X) > sightrange) return false;
            if (System.Math.Abs(A.Y - B.Y) > sightrange) return false;
            return true;
        }

     
        public void SendVisibleActorsToActor(Actor jActor)
        {
            //search all actors which can be seen by jActor and tell jActor about them
            for (short deltaY = -1; deltaY <= 1; deltaY++)
            {
                for (short deltaX = -1; deltaX <= 1; deltaX++)
                {
                    uint region = (uint)(jActor.region + (deltaX * 10000) + deltaY);
                    if (!this.actorsByRegion.ContainsKey(region)) continue;
                    Actor[] list = this.actorsByRegion[region].ToArray();
                    List<Actor> listAF = new List<Actor>();
                    foreach (Actor actor in list)
                    {
                        try
                        {
                            if (actor.ActorID == jActor.ActorID) continue;
                            if (actor.Status == null)
                            {
                                this.DeleteActor(actor);
                                continue;
                            }
                            //check wheter jActor can see actor, if yes: inform jActor
                            if (this.ACanSeeB(jActor, actor))
                            {
                                if (actor.type == ActorType.FURNITURE && ItemFactory.Instance.GetItem(((ActorFurniture)actor).ItemID).BaseData.itemType != ItemType.FF_CASTLE && id > 90001000)
                                {
                                    listAF.Add(actor);
                                }
                                else
                                    jActor.e.OnActorAppears(actor);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.ShowError(ex);
                        }
                    }
                    if (listAF.Count > 0)
                    {
                        List<ActorFurniture> afs = new List<ActorFurniture>();
                        int index = 0;
                        foreach (Actor i in listAF)
                        {
                            if (index >= 40)
                            {
                                jActor.e.OnActorFurnitureList(afs);
                                afs.Clear();
                                index = 0;
                            }
                            afs.Add((ActorFurniture)i);
                            index++;
                        }
                        if (afs.Count > 0)
                            jActor.e.OnActorFurnitureList(afs);
                    }
                }
            }
        }

        public void TeleportActor(Actor sActor, short x, short y)
        {
            if (sActor.Status.Additions.ContainsKey("Meditatioon"))
            {
                sActor.Status.Additions["Meditatioon"].AdditionEnd();
                sActor.Status.Additions.TryRemove("Meditatioon", out _);
            }
            if (sActor.Status.Additions.ContainsKey("Hiding"))
            {
                sActor.Status.Additions["Hiding"].AdditionEnd();
                sActor.Status.Additions.TryRemove("Hiding", out _);
            }
            if (sActor.Status.Additions.ContainsKey("fish"))
            {
                sActor.Status.Additions["fish"].AdditionEnd();
                sActor.Status.Additions.TryRemove("fish", out _);
            }
            if (sActor.Status.Additions.ContainsKey("Cloaking"))
            {
                sActor.Status.Additions["Cloaking"].AdditionEnd();
                sActor.Status.Additions.TryRemove("Cloaking", out _);
            }
            if (sActor.Status.Additions.ContainsKey("IAmTree"))
            {
                sActor.Status.Additions["IAmTree"].AdditionEnd();
                sActor.Status.Additions.TryRemove("IAmTree", out _);
            }
            if (sActor.Status.Additions.ContainsKey("Invisible"))
            {
                sActor.Status.Additions["Invisible"].AdditionEnd();
                sActor.Status.Additions.TryRemove("Invisible", out _);
            }

            if (sActor.HP == 0)
                return;
            if (sActor.type != ActorType.PC)
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.DISAPPEAR, null, sActor, false);

            this.actorsByRegion[sActor.region].Remove(sActor);
            if (GetRegionPlayerCount(sActor.region) == 0)
            {
                MobAIToggle(sActor.region, false);
            }

            sActor.X = x;
            sActor.Y = y;
            sActor.region = this.GetRegion(x, y);
            if (GetRegionPlayerCount(sActor.region) == 0 && sActor.type == ActorType.PC)
            {
                MobAIToggle(sActor.region, true);
            }

            if (!this.actorsByRegion.ContainsKey(sActor.region)) this.actorsByRegion.TryAdd(sActor.region, new List<Actor>());//二哈更改1015
            this.actorsByRegion[sActor.region].Add(sActor);

            sActor.e.OnTeleport(x, y);
            if (sActor.type != ActorType.PC)
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.APPEAR, null, sActor, false);
            else
            {
                short[] pos = new short[2];
                pos[0] = x;
                pos[1] = y;
                MoveActor(MOVE_TYPE.START, sActor, pos, sActor.Dir, sActor.Speed, false, MoveType.VANISH2);
            }
            this.SendVisibleActorsToActor(sActor);
        }

        public void SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE etype, MapEventArgs args, Actor sActor, bool sendToSourceActor)
        {
            try
            {
                for (short deltaY = -1; deltaY <= 1; deltaY++)
                {
                    for (short deltaX = -1; deltaX <= 1; deltaX++)
                    {
                        uint region = (uint)(sActor.region + (deltaX * 10000) + deltaY);
                        if (!this.actorsByRegion.ContainsKey(region)) continue;
                        //   Actor[] actors = this.actorsByRegion[region].ToArray();
                        //   foreach (Actor actor in actors)
                        //ECOKEY 10/17/2023
                      
                        // 確保在多執行緒環境中安全訪問
                        List<Actor> actors = new List<Actor>(this.actorsByRegion[region]);

                        if (actors.Count > 0)
                        {
                            // 現在可以安全地複製到目的陣列
                            Actor[] destinationArray = new Actor[actors.Count];
                            Array.Copy(actors.ToArray(), destinationArray, actors.Count);

                            foreach (Actor actor in destinationArray)
                            {

                                try
                                {
                                    if (!sendToSourceActor && (actor.ActorID == sActor.ActorID)) continue;
                                    if (actor.Status == null)
                                    {
                                        if (etype != EVENT_TYPE.DISAPPEAR)
                                            this.DeleteActor(actor);
                                        continue;
                                    }
                                    if (this.ACanSeeB(actor, sActor))
                                    {
                                        switch (etype)
                                        {
                                            case EVENT_TYPE.PLAYERSHOP_CHANGE:
                                                if (sActor.type == ActorType.PC || sActor.type == ActorType.EVENT)
                                                {
                                                    actor.e.OnPlayerShopChange(sActor);
                                                }
                                                break;
                                            case EVENT_TYPE.PLAYERSHOP_CHANGE_CLOSE:
                                                if (sActor.type == ActorType.PC || sActor.type == ActorType.EVENT)
                                                {
                                                    actor.e.OnPlayerShopChangeClose(sActor);
                                                }
                                                break;

                                            case EVENT_TYPE.PLAYER_SIZE_UPDATE:
                                                actor.e.OnPlayerSizeChange(sActor);
                                                break;


                                            case EVENT_TYPE.CHAR_INFO_UPDATE:
                                                actor.e.OnCharInfoUpdate(sActor);
                                                break;

                                            case EVENT_TYPE.CHANGE_STATUS:
                                                if (sActor.type != ActorType.PC)
                                                    break;

                                                actor.e.OnPlayerChangeStatus((ActorPC)sActor);
                                                break;

                                            case EVENT_TYPE.APPEAR:
                                                actor.e.OnActorAppears(sActor);
                                                break;

                                            case EVENT_TYPE.DISAPPEAR:
                                                actor.e.OnActorDisappears(sActor);
                                                break;

                                            case EVENT_TYPE.EMOTION:
                                                actor.e.OnActorChangeEmotion(sActor, args);
                                                break;

                                            case EVENT_TYPE.MOTION:
                                                actor.e.OnActorChangeMotion(sActor, args);
                                                break;

                                            case EVENT_TYPE.WAITTYPE:
                                                actor.e.OnActorChangeWaitType(sActor);
                                                break;

                                            case EVENT_TYPE.CHAT:
                                                actor.e.OnActorChat(sActor, args);
                                                break;

                                            case EVENT_TYPE.SKILL:
                                                actor.e.OnActorSkillUse(sActor, args);
                                                break;

                                            case EVENT_TYPE.CHANGE_EQUIP:
                                                actor.e.OnActorChangeEquip(sActor, args);
                                                break;
                                            case EVENT_TYPE.ATTACK:
                                                actor.e.OnAttack(sActor, args);
                                                break;
                                            case EVENT_TYPE.HPMPSP_UPDATE:
                                                actor.e.OnHPMPSPUpdate(sActor);
                                                break;
                                            case EVENT_TYPE.BUFF_CHANGE:
                                                actor.e.OnActorChangeBuff(sActor);
                                                break;
                                            case EVENT_TYPE.LEVEL_UP:
                                                actor.e.OnLevelUp(sActor, args);
                                                break;
                                            case EVENT_TYPE.PLAYER_MODE:
                                                actor.e.OnPlayerMode(sActor);
                                                break;
                                            case EVENT_TYPE.SHOW_EFFECT:
                                                actor.e.OnShowEffect(sActor, args);
                                                break;
                                            case EVENT_TYPE.POSSESSION:
                                                actor.e.OnActorPossession(sActor, args);
                                                break;
                                            case EVENT_TYPE.PARTY_NAME_UPDATE:
                                                if (sActor.type != ActorType.PC)
                                                    break;
                                                actor.e.OnActorPartyUpdate((ActorPC)sActor);
                                                break;
                                            case EVENT_TYPE.SPEED_UPDATE:
                                                actor.e.OnActorSpeedChange(sActor);
                                                break;
                                            case EVENT_TYPE.SIGN_UPDATE:
                                                if (sActor.type == ActorType.PC || sActor.type == ActorType.EVENT)
                                                {
                                                    actor.e.OnSignUpdate(sActor);
                                                }
                                                break;
                                            case EVENT_TYPE.RING_NAME_UPDATE:
                                                if (sActor.type != ActorType.PC)
                                                    break;
                                                actor.e.OnActorRingUpdate((ActorPC)sActor);
                                                break;
                                            case EVENT_TYPE.WRP_RANKING_UPDATE:
                                                if (sActor.type != ActorType.PC)
                                                    break;
                                                actor.e.OnActorWRPRankingUpdate((ActorPC)sActor);
                                                break;
                                            case EVENT_TYPE.ATTACK_TYPE_CHANGE:
                                                if (sActor.type != ActorType.PC)
                                                    break;
                                                actor.e.OnActorChangeAttackType((ActorPC)sActor);
                                                break;
                                            case EVENT_TYPE.FURNITURE_SIT:
                                                if (sActor.type != ActorType.PC)
                                                    break;
                                                actor.e.OnActorFurnitureSit((ActorPC)sActor);
                                                break;
                                            case EVENT_TYPE.PAPER_CHANGE:
                                                if (sActor.type != ActorType.PC)
                                                    break;
                                                actor.e.OnActorPaperChange((ActorPC)sActor);
                                                break;
                                            case EVENT_TYPE.SKILL_CANCEL:
                                                actor.e.OnActorSkillCancel(sActor);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }

                                catch (Exception ex)
                                {
                                    Logger.ShowError(ex);
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void SendEventToAllActors(TOALL_EVENT_TYPE etype, MapEventArgs args, Actor sActor, bool sendToSourceActor)
        {
            foreach (Actor actor in this.actorsByID.Values)
            {
                if (sActor != null) if (!sendToSourceActor && (actor.ActorID == sActor.ActorID)) continue;

                switch (etype)
                {
                    case TOALL_EVENT_TYPE.CHAT:
                        actor.e.OnActorChat(sActor, args);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SendActorToMap(Actor mActor, Map newMap, short x, short y)
        {
            SendActorToMap(mActor, newMap, x, y, false);
        }

        public void SendActorToMap(Actor mActor, Map newMap, short x, short y, bool possession)
        {
            // todo: add support for multiple map servers
            if (mActor.Status.Additions.ContainsKey("Meditatioon"))
            {
                mActor.Status.Additions["Meditatioon"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Meditatioon", out _);
            }
            if (mActor.Status.Additions.ContainsKey("Hiding"))
            {
                mActor.Status.Additions["Hiding"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Hiding", out _);
            }
            if (mActor.Status.Additions.ContainsKey("fish"))
            {
                mActor.Status.Additions["fish"].AdditionEnd();
                mActor.Status.Additions.TryRemove("fish", out _);
            }
            if (mActor.Status.Additions.ContainsKey("Cloaking"))
            {
                mActor.Status.Additions["Cloaking"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Cloaking", out _);
            }
            if (mActor.Status.Additions.ContainsKey("IAmTree"))
            {
                mActor.Status.Additions["IAmTree"].AdditionEnd();
                mActor.Status.Additions.TryRemove("IAmTree", out _);
            }
            if (mActor.Status.Additions.ContainsKey("Invisible"))
            {
                mActor.Status.Additions["Invisible"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Invisible", out _);
            }

            if (mActor.HP == 0)
                return;
            //send also the possessioned actors to the same map
            if (mActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                if (pc.PossessionTarget != 0 && !possession)
                    return;
                List<ActorPC> possessioned = pc.PossesionedActors;
                foreach (ActorPC i in possessioned)
                {
                    if (i == pc) continue;
                    SendActorToMap(i, newMap, x, y);
                }
            }

            // obtain the new map
            byte mapid = (byte)newMap.id;
            if (mapid == mActor.MapID)
            {
                TeleportActor(mActor, x, y);
                return;
            }

            // delete the actor from this map
            this.DeleteActor(mActor);

            // update the actor
            mActor.MapID = mapid;
            mActor.X = x;
            mActor.Y = y;
            mActor.Buff.Dead = false;

            // register the actor in the new map
            if (mActor.type != ActorType.PC)
            {
                newMap.RegisterActor(mActor);
            }
            else
            {
                ((ActorPC)mActor).Motion = MotionType.STAND;
                newMap.RegisterActor(mActor, mActor.ActorID);
                Network.Client.MapClient client = Network.Client.MapClient.FromActorPC((ActorPC)mActor);
                if (client.AI != null)
                {
                    client.AI.map = newMap;
                }
            }
        }

        public void SendActorToMap(Actor mActor, uint mapid, short x, short y)
        {
            SendActorToMap(mActor, mapid, x, y, false);
        }

        public void SendActorToMap(Actor mActor, uint mapid, short x, short y, bool possession)
        {
            // todo: add support for multiple map servers
            if (mActor.Status.Additions.ContainsKey("Meditatioon"))
            {
                mActor.Status.Additions["Meditatioon"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Meditatioon", out _);
            }
            if (mActor.Status.Additions.ContainsKey("Hiding"))
            {
                mActor.Status.Additions["Hiding"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Hiding", out _);
            }
            if (mActor.Status.Additions.ContainsKey("fish"))
            {
                mActor.Status.Additions["fish"].AdditionEnd();
                mActor.Status.Additions.TryRemove("fish", out _);
            }
            if (mActor.Status.Additions.ContainsKey("Cloaking"))
            {
                mActor.Status.Additions["Cloaking"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Cloaking", out _);
            }
            if (mActor.Status.Additions.ContainsKey("IAmTree"))
            {
                mActor.Status.Additions["IAmTree"].AdditionEnd();
                mActor.Status.Additions.TryRemove("IAmTree", out _);
            }
            if (mActor.Status.Additions.ContainsKey("Invisible"))
            {
                mActor.Status.Additions["Invisible"].AdditionEnd();
                mActor.Status.Additions.TryRemove("Invisible", out _);
            }

            if (mActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                if (pc.PossessionTarget != 0 && !possession)
                    return;
                List<ActorPC> possessioned = pc.PossesionedActors;
                foreach (ActorPC i in possessioned)
                {
                    if (i == pc) continue;
                    SendActorToMap(i, mapid, x, y, true);
                }
            }

            // obtain the new map
            Map newMap;
            if (mapid == mActor.MapID)
            {
                TeleportActor(mActor, x, y);
                return;
            }
            newMap = MapManager.Instance.GetMap(mapid);
            if (newMap == null)
                return;
            // delete the actor from this map
            this.DeleteActor(mActor);
            if (x == 0f && y == 0f)
            {
                short[] pos = newMap.GetRandomPos();
                x = pos[0];
                y = pos[1];
            }
            // update the actor
            mActor.MapID = mapid;
            mActor.X = x;
            mActor.Y = y;
            if (mActor.Buff.Dead || mActor.HP == 0)
            {
                mActor.HP = 1;
                mActor.Buff.Dead = false;
            }

            // register the actor in the new map
            if (mActor.type != ActorType.PC)
            {
                newMap.RegisterActor(mActor);
            }
            else
            {
                ((ActorPC)mActor).Motion = MotionType.STAND;
                newMap.RegisterActor(mActor, mActor.ActorID);
                Network.Client.MapClient client = Network.Client.MapClient.FromActorPC((ActorPC)mActor);
                if (client.AI != null)
                {
                    client.AI.map = newMap;
                }
            }
        }

        private void SendActorToActor(Actor mActor, Actor tActor)
        {
            if (mActor.MapID == tActor.MapID)
                this.TeleportActor(mActor, tActor.X, tActor.Y);
            else
                this.SendActorToMap(mActor, tActor.MapID, tActor.X, tActor.Y);
        }



    }
}

/*using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;//二哈更改測試

using SagaDB.Actor;
using SagaDB.Item;
using SagaDB.Map;
using SagaLib;
using SagaMap.Manager;
using System.Linq;

namespace SagaMap
{
    public partial class Map
    {

        public bool returnori = false;
        public uint OriID;


        private string name;
        private uint id;

        private ushort width, height;
        public uint ID { get { return this.id; } set { this.id = value; } }
        public string Name { get { return this.name; } }

        public ushort Width { get { return this.width; } }
        public ushort Height { get { return this.height; } }

        private ConcurrentDictionary<uint, Actor> actorsByID;
        //private Dictionary<uint, Actor> actorsByID;//二哈更改測試
        private Dictionary<uint, List<Actor>> actorsByRegion;
        private Dictionary<string, ActorPC> pcByName;

        private const uint ID_BORDER_MOB = 10000;
        private const uint ID_BORDER_PET = 20000;
        private const uint ID_BORDER_GOLEM = 40000;
        private const uint ID_BORDER_ITEM = 50000;
        private const uint ID_BORDER_EVENT = 60000;
        private const uint ID_BORDER_ANOMOB = 110000;
        private const uint ID_BORDER_SKILL = 120000;
        private const uint ID_BORDER2 = 0x3B9ACA00;//border for possession items

        private static uint nextPcId;
        private uint nextMobId;
        private uint nextItemId;
        private uint nextPetId;
        private uint nextEventId;
        private uint nextGolemId;
        private uint nextAnoMobID;
        private uint nextSkillID;

        private static readonly object registerlock = new object(); //注册对象时的锁

        public MapInfo Info;

        public enum MOVE_TYPE { START, STOP };
        public enum EVENT_TYPE
        {
            APPEAR, DISAPPEAR, MOTION, EMOTION, CHAT, SKILL, CHANGE_EQUIP, CHANGE_STATUS, BUFF_CHANGE,
            ACTOR_SELECTION, YAW_UPDATE, CHAR_INFO_UPDATE, PLAYER_SIZE_UPDATE, ATTACK, HPMPSP_UPDATE,
            LEVEL_UP, PLAYER_MODE, SHOW_EFFECT, POSSESSION, PARTY_NAME_UPDATE,
            SPEED_UPDATE, SIGN_UPDATE, RING_NAME_UPDATE, WRP_RANKING_UPDATE, ATTACK_TYPE_CHANGE, PLAYERSHOP_CHANGE, PLAYERSHOP_CHANGE_CLOSE,
            WAITTYPE, FURNITURE_SIT, PAPER_CHANGE, TELEPORT, SKILL_CANCEL,
        }

        public enum TOALL_EVENT_TYPE { CHAT };

        public Map(MapInfo info)
        {
            this.id = info.id;
            this.name = info.name;
            this.width = info.width;
            this.height = info.height;
            this.Info = info;

            //this.actorsByID = new Dictionary<uint, Actor>();
            this.actorsByID = new ConcurrentDictionary<uint, Actor>();//二哈更改測試
            this.actorsByRegion = new Dictionary<uint, List<Actor>>();
            this.pcByName = new Dictionary<string, ActorPC>();
            if (nextPcId == 0)
                nextPcId = 0x10;
            this.nextMobId = ID_BORDER_MOB + 1;
            this.nextItemId = ID_BORDER_ITEM + 1;
            this.nextPetId = ID_BORDER_PET + 1;
            this.nextEventId = ID_BORDER_EVENT + 1;
            this.nextGolemId = ID_BORDER_GOLEM + 1;
            this.nextAnoMobID = ID_BORDER_ANOMOB + 1;
            this.nextSkillID = ID_BORDER_SKILL + 1;
        }


        public short[] GetRandomPos()
        {
            short[] ret = new short[2];

            ret[0] = (short)Global.Random.Next(-12700, +12700);
            ret[1] = (short)Global.Random.Next(-12700, +12700);

            return ret;
        }

        public short[] GetRandomPosAroundActor(Actor actor)
        {
            short[] ret = new short[2];

            ret[0] = (short)Global.Random.Next(actor.X - 100, actor.X + 100);
            ret[1] = (short)Global.Random.Next(actor.Y - 100, actor.Y + 100);

            return ret;
        }

        public short[] GetRandomPosAroundActor2(Actor actor)
        {
            short[] ret = new short[2];

            ret[0] = (short)Global.Random.Next(actor.X - 600, actor.X + 600);
            ret[1] = (short)Global.Random.Next(actor.Y - 600, actor.Y + 600);

            return ret;
        }

        public short[] GetRandomPosAroundPos(short x, short y, int range)
        {
            short[] ret = new short[2];
            byte new_x, new_y;
            int count = 0;
            do
            {
                if (count >= 1000)
                {
                    ret[0] = x;
                    ret[1] = y;
                    return ret;
                }
                ret[0] = (short)Global.Random.Next(x - range, x + range);
                ret[1] = (short)Global.Random.Next(y - range, y + range);
                new_x = Global.PosX16to8(ret[0], this.width);
                new_y = Global.PosY16to8(ret[1], this.height);
                count++;
                if (new_x >= this.width)
                    new_x = (byte)(this.width - 1);
                if (new_y >= this.height)
                    new_y = (byte)(this.height - 1);
            } while (this.Info.walkable[new_x, new_y] != 2);
            return ret;
        }

        // public Dictionary<uint, Actor> Actors { get { return this.actorsByID; } }
        public ConcurrentDictionary<uint, Actor> Actors { get { return this.actorsByID; } }//二哈更改測試

        public Actor GetActor(uint id)
        {
            try
            {
                //return actorsByID[id];
                return actorsByID.TryGetValue(id, out Actor actor) ? actor : null;//二哈更改測試
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActorPC GetPC(string name)
        {
            try
            {
                return pcByName[name];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActorPC GetPC(uint charID)
        {
            try
            {
                var chr = from c in pcByName.Values
                          where c.CharID == charID
                          select c;
                return chr.First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private uint GetNewActorID(ActorType type)
        {
            uint newID = 0;
            uint startID = 0;

            if (type == ActorType.PC)
            {
                newID = nextPcId;
                startID = nextPcId;
            }
            else
            {
                if (type == ActorType.MOB)
                {
                    newID = this.nextMobId;
                    startID = this.nextMobId;
                }
                else if (type == ActorType.PET || type == ActorType.SHADOW || type == ActorType.PARTNER)
                {
                    newID = this.nextPetId;
                    startID = this.nextPetId;
                }
                else if (type == ActorType.EVENT || type == ActorType.FURNITURE)
                {
                    newID = this.nextEventId;
                    startID = this.nextEventId;
                }
                else if (type == ActorType.GOLEM)
                {
                    newID = this.nextGolemId;
                    startID = this.nextGolemId;
                }
                else if (type == ActorType.ANOTHERMOB)
                {
                    newID = this.nextAnoMobID;
                    startID = this.nextAnoMobID;
                }
                else if (type == ActorType.SKILL)
                {
                    newID = this.nextSkillID;
                    startID = this.nextSkillID;
                }
                else
                {
                    newID = this.nextItemId;
                    startID = this.nextItemId;
                }
            }

            if (newID >= 10000 && type == ActorType.PC)
                newID = 16;

            if (newID >= 20000 && type == ActorType.MOB)
                newID = ID_BORDER_MOB + 1;
            if (newID >= 30000 && (type == ActorType.PET || type == ActorType.PARTNER))
                newID = ID_BORDER_PET + 1;
            if (newID >= 50000 && type == ActorType.GOLEM)
                newID = ID_BORDER_GOLEM + 1;
            if (newID >= 60000 && type == ActorType.ITEM)
                newID = ID_BORDER_ITEM + 1;
            if (newID >= 70000 && type == ActorType.EVENT)
                newID = ID_BORDER_EVENT + 1;
            if (newID >= 120000 && type == ActorType.ANOTHERMOB)
                newID = ID_BORDER_ANOMOB + 1;
            if (newID >= 140000 && type == ActorType.SKILL)
                newID = ID_BORDER_SKILL + 1;
            if (newID >= UInt32.MaxValue)
                newID = 1;

            while (this.actorsByID.ContainsKey(newID))
            {
                newID++;

                if (newID >= 10000 && type == ActorType.PC)
                    newID = 16;

                if (newID >= 20000 && type == ActorType.MOB)
                    newID = ID_BORDER_MOB + 1;
                if (newID >= 30000 && (type == ActorType.PET || type == ActorType.PARTNER))
                    newID = ID_BORDER_PET + 1;
                if (newID >= 50000 && type == ActorType.GOLEM)
                    newID = ID_BORDER_GOLEM + 1;
                if (newID >= 60000 && type == ActorType.ITEM)
                    newID = ID_BORDER_ITEM + 1;
                if (newID >= 70000 && type == ActorType.EVENT)
                    newID = ID_BORDER_EVENT + 1;
                if (newID >= 120000 && type == ActorType.ANOTHERMOB)
                    newID = ID_BORDER_ANOMOB + 1;
                if (newID >= 140000 && type == ActorType.SKILL)
                    newID = ID_BORDER_SKILL + 1;
                if (newID >= UInt32.MaxValue)
                    newID = 1;

                if (newID == startID) return 0;
            }

            if (type == ActorType.PC)
                nextPcId = newID + 1;
            else
                if (type == ActorType.MOB)
                this.nextMobId = newID + 1;
            else if ((type == ActorType.PET || type == ActorType.PARTNER))
                this.nextPetId = newID + 1;
            else if (type == ActorType.FURNITURE || type == ActorType.EVENT)
                this.nextEventId = newID + 1;
            else if (type == ActorType.GOLEM)
                this.nextGolemId = newID + 1;
            else if (type == ActorType.ANOTHERMOB)
                this.nextAnoMobID = newID + 1;
            else if (type == ActorType.SKILL)
                this.nextSkillID = newID + 1;
            else
                this.nextItemId = newID + 1;


            return newID;
        }

        public bool RegisterActor(Actor nActor)
        {
            // default: no success
            bool succes = false;

            // set the actorID and the actor's region on this map
            uint newID = 0;
            if (Global.clientMananger != null)
                ClientManager.EnterCriticalArea();
            if (nActor.type == ActorType.MOB && ((ActorMob)nActor).AnotherID != 0)
                newID = this.GetNewActorID(ActorType.ANOTHERMOB);
            else
                newID = this.GetNewActorID(nActor.type);
            if (Global.clientMananger != null)
                ClientManager.LeaveCriticalArea();
            if (nActor.type == ActorType.ITEM)
            {
                ActorItem item = (ActorItem)nActor;
                if (item.PossessionItem)
                    newID += ID_BORDER2;
            }

            if (newID != 0)
            {
                nActor.ActorID = newID;
                nActor.region = this.GetRegion(nActor.X, nActor.Y);

                if (GetRegionPlayerCount(nActor.region) == 0 && nActor.type == ActorType.PC)
                {
                    MobAIToggle(nActor.region, true);
                }
                // make the actor invisible (when the actor is ready: set it to false & call OnActorVisibilityChange)
                nActor.invisble = true;

                // add the new actor to the tables
                //DateTime time = DateTime.Now;
                if (Global.clientMananger != null)
                    ClientManager.EnterCriticalArea();
                    try
                    {
                        while (actorsByID.ContainsKey(nActor.ActorID))
                        {
                            if (nActor.type == ActorType.MOB && ((ActorMob)nActor).AnotherID != 0)
                                nActor.ActorID = this.GetNewActorID(ActorType.ANOTHERMOB);
                            else
                                nActor.ActorID = this.GetNewActorID(nActor.type);
                            if (nActor.type == ActorType.ITEM && ((ActorItem)nActor).PossessionItem)
                                nActor.ActorID += ID_BORDER2;
                        }
                    //actorsByID.Add(nActor.ActorID, nActor);
                    actorsByID.TryAdd(nActor.ActorID, nActor);//二哈更改測試
                }
                    catch (Exception ex)
                    {
                        SagaLib.Logger.ShowError(ex);
                        SagaLib.Logger.ShowError("oh,fuck!");
                    }
                if (Global.clientMananger != null)
                    ClientManager.LeaveCriticalArea();
                //double usedtime = (DateTime.Now - time).TotalMilliseconds;
                //if (usedtime > 0)
                //    Logger.ShowError("在地图:" + ID + " 注册ID: " + nActor.ActorID + " 花费时间:" + usedtime + "ms");

                if (nActor.type == ActorType.PC && !this.pcByName.ContainsKey(nActor.Name))
                    this.pcByName.Add(nActor.Name, (ActorPC)nActor);

                if (!this.actorsByRegion.ContainsKey(nActor.region))
                    this.actorsByRegion.Add(nActor.region, new List<Actor>());

                this.actorsByRegion[nActor.region].Add(nActor);

                succes = true;
            }
            nActor.MapID = this.ID;
            if (nActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)nActor;
                if (this.Info.Flag.Test(MapFlags.Wrp))
                {
                    pc.Mode = PlayerMode.WRP;
                }
                else if (pc.Mode == PlayerMode.KNIGHT_EAST || pc.Mode == PlayerMode.KNIGHT_FLOWER || pc.Mode == PlayerMode.KNIGHT_NORTH || pc.Mode == PlayerMode.KNIGHT_ROCK || pc.Mode == PlayerMode.KNIGHT_SOUTH || pc.Mode == PlayerMode.KNIGHT_WEST)
                {
                    pc.Mode = pc.Mode;
                }
                else
                {
                    pc.Mode = PlayerMode.NORMAL;
                }
            }
            nActor.e.OnCreate(succes);
            return succes;
        }

        public bool RegisterActor(Actor nActor, uint SessionID)
        {
            // default: no success
            bool succes = false;

            // set the actorID and the actor's region on this map
            uint newID = SessionID;

            if (newID != 0)
            {
                nActor.ActorID = newID;
                nActor.region = this.GetRegion(nActor.X, nActor.Y);
                if (GetRegionPlayerCount(nActor.region) == 0 && nActor.type == ActorType.PC)
                {
                    MobAIToggle(nActor.region, true);
                }

                // make the actor invisible (when the actor is ready: set it to false & call OnActorVisibilityChange)
                nActor.invisble = true;

                // add the new actor to the tables
                // if (!this.actorsByID.ContainsKey(nActor.ActorID)) this.actorsByID.Add(nActor.ActorID, nActor);
                if (!this.actorsByID.ContainsKey(nActor.ActorID)) this.actorsByID.TryAdd(nActor.ActorID, nActor);//二哈更改測試


                if (nActor.type == ActorType.PC && !this.pcByName.ContainsKey(nActor.Name))
                    this.pcByName.Add(nActor.Name, (ActorPC)nActor);

                if (!this.actorsByRegion.ContainsKey(nActor.region))
                    this.actorsByRegion.Add(nActor.region, new List<Actor>());

                this.actorsByRegion[nActor.region].Add(nActor);

                succes = true;
            }
            if (nActor.type == ActorType.PC)
            {
                ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)nActor.e;
                if (eh.Client.state != SagaMap.Network.Client.MapClient.SESSION_STATE.DISCONNECTED)
                    eh.Client.state = SagaMap.Network.Client.MapClient.SESSION_STATE.LOADING;
                else
                {
                    MapServer.charDB.SaveChar((ActorPC)nActor, false, false);
                    MapServer.accountDB.WriteUser(((ActorPC)nActor).Account);

                }
            }
            nActor.MapID = this.ID;
            if (nActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)nActor;
                if (this.Info.Flag.Test(MapFlags.Wrp))
                {
                    pc.Mode = PlayerMode.WRP;
                }
                else if (pc.Mode == PlayerMode.KNIGHT_EAST || pc.Mode == PlayerMode.KNIGHT_FLOWER || pc.Mode == PlayerMode.KNIGHT_NORTH || pc.Mode == PlayerMode.KNIGHT_ROCK || pc.Mode == PlayerMode.KNIGHT_SOUTH || pc.Mode == PlayerMode.KNIGHT_WEST)
                {
                    pc.Mode = pc.Mode;
                }
                else
                {
                    pc.Mode = PlayerMode.NORMAL;
                }
            }
            nActor.e.OnCreate(succes);
            return succes;
        }

        public void OnActorVisibilityChange(Actor dActor)
        {
            if (dActor.invisble)
            {
                dActor.invisble = false;
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.DISAPPEAR, null, dActor, false);
                dActor.invisble = true;
            }

            else
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.APPEAR, null, dActor, false);
        }

        public void DeleteActor(Actor dActor)
        {
            this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.DISAPPEAR, null, dActor, false);

            if (dActor.type == ActorType.PC && this.pcByName.ContainsKey(dActor.Name))
                this.pcByName.Remove(dActor.Name);
            //ClientManager.EnterCriticalArea();
            //this.actorsByID.Remove(dActor.ActorID);
            this.actorsByID.TryRemove(dActor.ActorID, out _);//二哈更改測試 


            //ECOKEY 四重奏error
            lock (this.actorsByRegion)
            {
                if (this.actorsByRegion.ContainsKey(dActor.region))
                {
                    this.actorsByRegion[dActor.region].Remove(dActor);
                    if (GetRegionPlayerCount(dActor.region) == 0)
                    {
                        MobAIToggle(dActor.region, false);
                    }
                }
            }

            //ClientManager.LeaveCriticalArea();
            dActor.e.OnDelete();
            if (this.IsDungeon)
            {
                if (this.DungeonMap.MapType == SagaMap.Dungeon.MapType.End)
                {
                    int count = 0;
                    foreach (Actor i in actorsByID.Values)
                    {
                        if (i.type == ActorType.MOB)
                            count++;
                    }
                    if (count == 0)
                    {
                        Dungeon.DungeonFactory.Instance.GetDungeon(this.creator.DungeonID).Destory(SagaMap.Dungeon.DestroyType.BossDown);
                    }
                }
            }
        }

        public class BuffChangeEventArgs : MapEventArgs
        {
            public ActorPC Character { get; }

            public BuffChangeEventArgs(ActorPC character)
            {
                Character = character;
            }
        }

        public void MoveActor(MOVE_TYPE mType, Actor mActor, short[] pos, ushort dir, ushort speed)
        {
            MoveActor(mType, mActor, pos, dir, speed, false);
        }
        public void MoveActor(MOVE_TYPE mType, Actor mActor, short[] pos, ushort dir, ushort speed, bool sendToSelf)
        {
            MoveActor(mType, mActor, pos, dir, speed, sendToSelf, MoveType.RUN);
        }
        // make sure only 1 thread at a time is executing this method
        public void MoveActor(MOVE_TYPE mType, Actor mActor, short[] pos, ushort dir, ushort speed, bool sendToSelf, MoveType moveType)
        {
            try
            {
                bool knockBack = false;
                if (mActor.Status != null)
                {
                    if (mActor.Status.Additions.ContainsKey("Meditatioon"))
                    {
                        mActor.Status.Additions["Meditatioon"].AdditionEnd();
                        mActor.Status.Additions.Remove("Meditatioon");
                    }
                    if (mActor.Status.Additions.ContainsKey("Hiding"))
                    {
                        mActor.Status.Additions["Hiding"].AdditionEnd();
                        mActor.Status.Additions.Remove("Hiding");
                    }
                    if (mActor.Status.Additions.ContainsKey("fish"))
                    {
                        mActor.Status.Additions["fish"].AdditionEnd();
                        mActor.Status.Additions.Remove("fish");
                    }
                    if (mActor.Status.Additions.ContainsKey("IAmTree"))
                    {
                        mActor.Status.Additions["IAmTree"].AdditionEnd();
                        mActor.Status.Additions.Remove("IAmTree");
                    }

                }
                // check wheter the destination is in range, if not kick the client
                if (mActor.HP == 0 && mActor.type != ActorType.GOLEM && mActor.type != ActorType.SKILL && mActor.type != ActorType.FURNITURE)//ECOKEY 家具重放BUG修復
                {
                    pos = new short[2] { mActor.X, mActor.Y };
                    dir = 600;
                    knockBack = true;
                    sendToSelf = true;
                }
                if (mActor.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)mActor;
                    if (pc.CInt["WaitEventID"] != 0)
                    {
                        SagaMap.Network.Client.MapClient.FromActorPC(pc).EventActivate((uint)pc.CInt["WaitEventID"]);
                        pc.CInt["WaitEventID"] = 0;
                    }

                    if (pc.Motion == MotionType.STAND)
                    {
                        pc.Buff.Sit = false;
                        if (pc.Tasks.ContainsKey("Regeneration"))
                        {
                            pc.Tasks["Regeneration"].Deactivate();
                            pc.Tasks.Remove("Regeneration");
                            BuffChangeEventArgs buffChangeEventArgs = new BuffChangeEventArgs(pc);
                            this.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, buffChangeEventArgs, pc, true);
                        }
                    }
                    pc.Motion = MotionType.STAND;
                    pc.MotionLoop = false;
                }
                if (mActor.type == ActorType.PC && !knockBack)
                {
                    ActorPC pc = (ActorPC)mActor;
                    List<ActorPC> possessioned = pc.PossesionedActors;
                    foreach (ActorPC i in possessioned)
                    {
                        if (i == pc) continue;
                        if (i.MapID == mActor.MapID)
                            MoveActor(mType, i, pos, dir, speed);
                    }
                    if (pc.Online)
                    {
                        Network.Client.MapClient client = Network.Client.MapClient.FromActorPC(pc);
                        if ((DateTime.Now - client.moveStamp).TotalSeconds >= 2)
                        {
                            if (client.Character.Party != null)
                            {
                                PartyManager.Instance.UpdateMemberPosition(client.Character.Party, client.Character);
                            }
                            client.moveStamp = DateTime.Now;

                        }
                    }
                }


                //scroll through all actors that "could" see the mActor at "from"
                //or are going "to see" mActor, or are still seeing mActor
                if (!knockBack)
                {

                    for (short deltaY = -1; deltaY <= 1; deltaY++)
                    {
                        for (short deltaX = -1; deltaX <= 1; deltaX++)
                        {
                            uint region = (uint)(mActor.region + (deltaX * 10000) + deltaY);
                            if (!this.actorsByRegion.ContainsKey(region)) continue;

                            //ClientManager.EnterCriticalArea();
                            Actor[] list = actorsByRegion[region].ToArray();
                            //ClientManager.LeaveCriticalArea();

                            foreach (Actor actor in list)
                            {
                                if (actor.ActorID == mActor.ActorID && !sendToSelf) continue;
                                if (!this.actorsByRegion[region].Contains(actor))
                                    continue;
                                if (actor.Status == null)
                                {
                                    this.DeleteActor(actor);
                                    continue;
                                }
                                // A) INFORM OTHER ACTORS

                                //actor "could" see mActor at its "from" position
                                if (this.ACanSeeB(actor, mActor))
                                {
                                    //actor will still be able to see mActor
                                    if (this.ACanSeeB(actor, mActor, pos[0], pos[1]))
                                    {
                                        if (mType == MOVE_TYPE.START)
                                        {


                                            if (moveType != MoveType.RUN)
                                                actor.e.OnActorStartsMoving(mActor, pos, dir, speed, moveType);
                                            else
                                                actor.e.OnActorStartsMoving(mActor, pos, dir, speed);
                                        }
                                        else
                                            actor.e.OnActorStopsMoving(mActor, pos, dir, speed);
                                    }
                                    //actor won't be able to see mActor anymore
                                    else actor.e.OnActorDisappears(mActor);
                                }
                                //actor "could not" see mActor, but will be able to see him now
                                else if (this.ACanSeeB(actor, mActor, pos[0], pos[1]))
                                {
                                    actor.e.OnActorAppears(mActor);

                                    //send move / move stop
                                    if (mType == MOVE_TYPE.START)
                                    {
                                        if (moveType != MoveType.RUN)
                                            actor.e.OnActorStartsMoving(mActor, pos, dir, speed, moveType);
                                        else
                                            actor.e.OnActorStartsMoving(mActor, pos, dir, speed);
                                    }
                                    else
                                        actor.e.OnActorStopsMoving(mActor, pos, dir, speed);
                                }

                                // B) INFORM mActor
                                //mActor "could" see actor on its "from" position
                                if (this.ACanSeeB(mActor, actor))
                                {
                                    //mActor won't be able to see actor anymore
                                    if (!this.ACanSeeB(mActor, pos[0], pos[1], actor))
                                        mActor.e.OnActorDisappears(actor);
                                    //mAactor will still be able to see actor
                                    else { }
                                }

                                else if (this.ACanSeeB(mActor, pos[0], pos[1], actor))
                                {
                                    //mActor "could not" see actor, but will be able to see him now
                                    //send pcinfo
                                    mActor.e.OnActorAppears(actor);
                                }
                            }
                        }
                    }
                }
                else
                    mActor.e.OnActorStopsMoving(mActor, pos, dir, speed);

                //update x/y/z/yaw of the actor    
                mActor.LastX = mActor.X;
                mActor.LastY = mActor.Y;
                mActor.X = pos[0];
                mActor.Y = pos[1];
                if (mActor.type == ActorType.FURNITURE)
                {
                    ((ActorFurniture)mActor).Z = pos[2];
                }
                if (dir <= 360)
                    mActor.Dir = dir;

                //update the region of the actor
                uint newRegion = this.GetRegion(pos[0], pos[1]);
                if (mActor.region != newRegion)
                {
                    this.actorsByRegion[mActor.region].Remove(mActor);
                    //turn off all the ai if the old region has no player on it
                    if (GetRegionPlayerCount(mActor.region) == 0)
                    {
                        MobAIToggle(mActor.region, false);
                    }
                    mActor.region = newRegion;
                    if (GetRegionPlayerCount(mActor.region) == 0 && mActor.type == ActorType.PC)
                    {
                        MobAIToggle(mActor.region, true);
                    }

                    if (!this.actorsByRegion.ContainsKey(newRegion))
                        this.actorsByRegion.Add(newRegion, new List<Actor>());

                    this.actorsByRegion[newRegion].Add(mActor);
                }
            }

            catch (Exception ex)
            { Logger.ShowError(ex); }
            //moveCounter--;
        }

        public int GetRegionPlayerCount(uint region)
        {
            List<Actor> actors;
            int count = 0;
            if (!this.actorsByRegion.ContainsKey(region)) return 0;
            actors = this.actorsByRegion[region];
            List<int> removelist = new List<int>();
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor;
                if (actors[i] == null)
                {
                    removelist.Add(i);
                    continue;
                }
                actor = actors[i];
                if (actor.type == ActorType.PC) count++;
            }
            foreach (int i in removelist)
            {
                actors.RemoveAt(i);
            }
            return count;
        }

        public void MobAIToggle(uint region, bool toggle)
        {
           
        }

        public bool MoveStepIsInRange(Actor mActor, short[] to)
        {
            if (mActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                Network.Client.MapClient client = Network.Client.MapClient.FromActorPC(pc);
                if (client.AI != null)
                {
                    if (client.AI.Activated)
                        return true;
                }
                TimeSpan span = DateTime.Now - client.moveCheckStamp;
                //if (span.TotalMilliseconds > 50)
                {

                    double maximal;
                    if (span.TotalMilliseconds > 1000)
                        maximal = mActor.Speed * 2f;
                    else if (span.TotalMilliseconds > 100)
                        maximal = mActor.Speed * (span.TotalMilliseconds / 1000) * 5f;
                    else
                        maximal = mActor.Speed * 0.5f;
                    // Disabled, until we have something better
                    if (System.Math.Abs(mActor.X - to[0]) > maximal)
                        return false;
                    if (System.Math.Abs(mActor.Y - to[1]) > maximal)
                        return false;
                    //we don't check for z , yet, to allow falling from great hight
                    //if (System.Math.Abs(mActor.z - to[2]) > mActor.maxMoveRange) return false;
                }
            }
            return true;
        }


        public uint GetRegion(float x, float y)
        {

            uint REGION_DIAMETER = Global.MAX_SIGHT_RANGE * 2;

            // best case we should now load the size of the map from a config file, however that's not
            // possible yet, so we just create a region code off the values x/y

           
            // init nx,ny
            bool nx = false;
            bool ny = false;
            // make x,y positive
            if (x < 0) { x = x - (2 * x); nx = true; }
            if (y < 0) { y = y - (2 * y); ny = true; }
            // convert x,y to uints
            uint ux = (uint)x;
            uint uy = (uint)y;
            // divide through REGION_DIAMETER
            ux = (uint)(ux / REGION_DIAMETER);
            uy = (uint)(uy / REGION_DIAMETER);
            // calc ux
            if (ux > 4999) ux = 4999;
            if (!nx) ux = ux + 5000;
            else ux = 5000 - ux;
            // calc uy
            if (uy > 4999) uy = 4999;
            if (!ny) uy = uy + 5000;
            else uy = 5000 - uy;
            // finally generate the region code and return it
            return (uint)((ux * 10000) + uy);
        }

        public bool ACanSeeB(Actor A, Actor B)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(A.X - B.X) > A.sightRange) return false;
            if (System.Math.Abs(A.Y - B.Y) > A.sightRange) return false;
            return true;
        }

        public bool ACanSeeB(Actor A, Actor B, float bx, float by)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(A.X - bx) > A.sightRange) return false;
            if (System.Math.Abs(A.Y - by) > A.sightRange) return false;
            return true;
        }

        public bool ACanSeeB(Actor A, float ax, float ay, Actor B)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(ax - B.X) > A.sightRange) return false;
            if (System.Math.Abs(ay - B.Y) > A.sightRange) return false;
            return true;
        }

        public bool ACanSeeB(Actor A, Actor B, float sightrange)
        {
            if (A == null || B == null)
                return false;
            if (B.invisble) return false;
            if (System.Math.Abs(A.X - B.X) > sightrange) return false;
            if (System.Math.Abs(A.Y - B.Y) > sightrange) return false; 
            return true;
        }

     
          public void SendVisibleActorsToActor(Actor jActor)
          {
              //search all actors which can be seen by jActor and tell jActor about them
              for (short deltaY = -1; deltaY <= 1; deltaY++)
              {
                  for (short deltaX = -1; deltaX <= 1; deltaX++)
                  {
                      uint region = (uint)(jActor.region + (deltaX * 10000) + deltaY);
                      if (!this.actorsByRegion.ContainsKey(region)) continue;
                      Actor[] list = this.actorsByRegion[region].ToArray();
                      List<Actor> listAF = new List<Actor>();
                      foreach (Actor actor in list)
                      {
                          try
                          {
                              if (actor.ActorID == jActor.ActorID) continue;
                              if (actor.Status == null)
                              {
                                  this.DeleteActor(actor);
                                  continue;
                              }
                              //check wheter jActor can see actor, if yes: inform jActor
                              if (this.ACanSeeB(jActor, actor))
                              {
                                  if (actor.type == ActorType.FURNITURE && ItemFactory.Instance.GetItem(((ActorFurniture)actor).ItemID).BaseData.itemType != ItemType.FF_CASTLE && id > 90001000)
                                  {
                                      listAF.Add(actor);
                                  }
                                  else
                                      jActor.e.OnActorAppears(actor);
                              }
                          }
                          catch (Exception ex)
                          {
                              Logger.ShowError(ex);
                          }
                      }
                      if (listAF.Count > 0)
                      {
                          List<ActorFurniture> afs = new List<ActorFurniture>();
                          int index = 0;
                          foreach (Actor i in listAF)
                          {
                              if (index >= 40)
                              {
                                  jActor.e.OnActorFurnitureList(afs);
                                  afs.Clear();
                                  index = 0;
                              }
                              afs.Add((ActorFurniture)i);
                              index++;
                          }
                          if (afs.Count > 0)
                              jActor.e.OnActorFurnitureList(afs);
                      }
                  }
              }
          }

        public void TeleportActor(Actor sActor, short x, short y)
        {
            if (sActor.Status.Additions.ContainsKey("Meditatioon"))
            {
                sActor.Status.Additions["Meditatioon"].AdditionEnd();
                sActor.Status.Additions.Remove("Meditatioon");
            }
            if (sActor.Status.Additions.ContainsKey("Hiding"))
            {
                sActor.Status.Additions["Hiding"].AdditionEnd();
                sActor.Status.Additions.Remove("Hiding");
            }
            if (sActor.Status.Additions.ContainsKey("fish"))
            {
                sActor.Status.Additions["fish"].AdditionEnd();
                sActor.Status.Additions.Remove("fish");
            }
            if (sActor.Status.Additions.ContainsKey("Cloaking"))
            {
                sActor.Status.Additions["Cloaking"].AdditionEnd();
                sActor.Status.Additions.Remove("Cloaking");
            }
            if (sActor.Status.Additions.ContainsKey("IAmTree"))
            {
                sActor.Status.Additions["IAmTree"].AdditionEnd();
                sActor.Status.Additions.Remove("IAmTree");
            }
            if (sActor.Status.Additions.ContainsKey("Invisible"))
            {
                sActor.Status.Additions["Invisible"].AdditionEnd();
                sActor.Status.Additions.Remove("Invisible");
            }

            if (sActor.HP == 0)
                return;
            if (sActor.type != ActorType.PC)
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.DISAPPEAR, null, sActor, false);

            this.actorsByRegion[sActor.region].Remove(sActor);
            if (GetRegionPlayerCount(sActor.region) == 0)
            {
                MobAIToggle(sActor.region, false);
            }

            sActor.X = x;
            sActor.Y = y;
            sActor.region = this.GetRegion(x, y);
            if (GetRegionPlayerCount(sActor.region) == 0 && sActor.type == ActorType.PC)
            {
                MobAIToggle(sActor.region, true);
            }

            if (!this.actorsByRegion.ContainsKey(sActor.region)) this.actorsByRegion.Add(sActor.region, new List<Actor>());
            this.actorsByRegion[sActor.region].Add(sActor);

            sActor.e.OnTeleport(x, y);
            if (sActor.type != ActorType.PC)
                this.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.APPEAR, null, sActor, false);
            else
            {
                short[] pos = new short[2];
                pos[0] = x;
                pos[1] = y;
                MoveActor(MOVE_TYPE.START, sActor, pos, sActor.Dir, sActor.Speed, false, MoveType.VANISH2);
            }
            this.SendVisibleActorsToActor(sActor);
        }

        public void SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE etype, MapEventArgs args, Actor sActor, bool sendToSourceActor)
        {
            try
            {
                for (short deltaY = -1; deltaY <= 1; deltaY++)
                {
                    for (short deltaX = -1; deltaX <= 1; deltaX++)
                    {
                        uint region = (uint)(sActor.region + (deltaX * 10000) + deltaY);
                        if (!this.actorsByRegion.ContainsKey(region)) continue;
                        //   Actor[] actors = this.actorsByRegion[region].ToArray();
                        //   foreach (Actor actor in actors)
                        //ECOKEY 10/17/2023
                    
                       // 確保在多執行緒環境中安全訪問
                List<Actor> actors = new List<Actor>(this.actorsByRegion[region]);

                if (actors.Count > 0)
                {
                    // 現在可以安全地複製到目的陣列
                    Actor[] destinationArray = new Actor[actors.Count];
                    Array.Copy(actors.ToArray(), destinationArray, actors.Count);

                            foreach (Actor actor in destinationArray)
                            {
                                
                            try
                            {
                                if (!sendToSourceActor && (actor.ActorID == sActor.ActorID)) continue;
                                if (actor.Status == null)
                                {
                                    if (etype != EVENT_TYPE.DISAPPEAR)
                                        this.DeleteActor(actor);
                                    continue;
                                }
                                if (this.ACanSeeB(actor, sActor))
                                {
                                    switch (etype)
                                    {
                                        case EVENT_TYPE.PLAYERSHOP_CHANGE:
                                            if (sActor.type == ActorType.PC || sActor.type == ActorType.EVENT)
                                            {
                                                actor.e.OnPlayerShopChange(sActor);
                                            }
                                            break;
                                        case EVENT_TYPE.PLAYERSHOP_CHANGE_CLOSE:
                                            if (sActor.type == ActorType.PC || sActor.type == ActorType.EVENT)
                                            {
                                                actor.e.OnPlayerShopChangeClose(sActor);
                                            }
                                            break;

                                        case EVENT_TYPE.PLAYER_SIZE_UPDATE:
                                            actor.e.OnPlayerSizeChange(sActor);
                                            break;


                                        case EVENT_TYPE.CHAR_INFO_UPDATE:
                                            actor.e.OnCharInfoUpdate(sActor);
                                            break;

                                        case EVENT_TYPE.CHANGE_STATUS:
                                            if (sActor.type != ActorType.PC)
                                                break;

                                            actor.e.OnPlayerChangeStatus((ActorPC)sActor);
                                            break;

                                        case EVENT_TYPE.APPEAR:
                                            actor.e.OnActorAppears(sActor);
                                            break;

                                        case EVENT_TYPE.DISAPPEAR:
                                            actor.e.OnActorDisappears(sActor);
                                            break;

                                        case EVENT_TYPE.EMOTION:
                                            actor.e.OnActorChangeEmotion(sActor, args);
                                            break;

                                        case EVENT_TYPE.MOTION:
                                            actor.e.OnActorChangeMotion(sActor, args);
                                            break;

                                        case EVENT_TYPE.WAITTYPE:
                                            actor.e.OnActorChangeWaitType(sActor);
                                            break;

                                        case EVENT_TYPE.CHAT:
                                            actor.e.OnActorChat(sActor, args);
                                            break;

                                        case EVENT_TYPE.SKILL:
                                            actor.e.OnActorSkillUse(sActor, args);
                                            break;

                                        case EVENT_TYPE.CHANGE_EQUIP:
                                            actor.e.OnActorChangeEquip(sActor, args);
                                            break;
                                        case EVENT_TYPE.ATTACK:
                                            actor.e.OnAttack(sActor, args);
                                            break;
                                        case EVENT_TYPE.HPMPSP_UPDATE:
                                            actor.e.OnHPMPSPUpdate(sActor);
                                            break;
                                        case EVENT_TYPE.BUFF_CHANGE:
                                            actor.e.OnActorChangeBuff(sActor);
                                            break;
                                        case EVENT_TYPE.LEVEL_UP:
                                            actor.e.OnLevelUp(sActor, args);
                                            break;
                                        case EVENT_TYPE.PLAYER_MODE:
                                            actor.e.OnPlayerMode(sActor);
                                            break;
                                        case EVENT_TYPE.SHOW_EFFECT:
                                            actor.e.OnShowEffect(sActor, args);
                                            break;
                                        case EVENT_TYPE.POSSESSION:
                                            actor.e.OnActorPossession(sActor, args);
                                            break;
                                        case EVENT_TYPE.PARTY_NAME_UPDATE:
                                            if (sActor.type != ActorType.PC)
                                                break;
                                            actor.e.OnActorPartyUpdate((ActorPC)sActor);
                                            break;
                                        case EVENT_TYPE.SPEED_UPDATE:
                                            actor.e.OnActorSpeedChange(sActor);
                                            break;
                                        case EVENT_TYPE.SIGN_UPDATE:
                                            if (sActor.type == ActorType.PC || sActor.type == ActorType.EVENT)
                                            {
                                                actor.e.OnSignUpdate(sActor);
                                            }
                                            break;
                                        case EVENT_TYPE.RING_NAME_UPDATE:
                                            if (sActor.type != ActorType.PC)
                                                break;
                                            actor.e.OnActorRingUpdate((ActorPC)sActor);
                                            break;
                                        case EVENT_TYPE.WRP_RANKING_UPDATE:
                                            if (sActor.type != ActorType.PC)
                                                break;
                                            actor.e.OnActorWRPRankingUpdate((ActorPC)sActor);
                                            break;
                                        case EVENT_TYPE.ATTACK_TYPE_CHANGE:
                                            if (sActor.type != ActorType.PC)
                                                break;
                                            actor.e.OnActorChangeAttackType((ActorPC)sActor);
                                            break;
                                        case EVENT_TYPE.FURNITURE_SIT:
                                            if (sActor.type != ActorType.PC)
                                                break;
                                            actor.e.OnActorFurnitureSit((ActorPC)sActor);
                                            break;
                                        case EVENT_TYPE.PAPER_CHANGE:
                                            if (sActor.type != ActorType.PC)
                                                break;
                                            actor.e.OnActorPaperChange((ActorPC)sActor);
                                            break;
                                        case EVENT_TYPE.SKILL_CANCEL:
                                            actor.e.OnActorSkillCancel(sActor);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                           
                            catch (Exception ex)
                            {
                                Logger.ShowError(ex);
                            }
                        
                            }
                        }
                    
                }
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void SendEventToAllActors(TOALL_EVENT_TYPE etype, MapEventArgs args, Actor sActor, bool sendToSourceActor)
        {
            foreach (Actor actor in this.actorsByID.Values)
            {
                if (sActor != null) if (!sendToSourceActor && (actor.ActorID == sActor.ActorID)) continue;

                switch (etype)
                {
                    case TOALL_EVENT_TYPE.CHAT:
                        actor.e.OnActorChat(sActor, args);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SendActorToMap(Actor mActor, Map newMap, short x, short y)
        {
            SendActorToMap(mActor, newMap, x, y, false);
        }

        public void SendActorToMap(Actor mActor, Map newMap, short x, short y, bool possession)
        {
            // todo: add support for multiple map servers
            if (mActor.Status.Additions.ContainsKey("Meditatioon"))
            {
                mActor.Status.Additions["Meditatioon"].AdditionEnd();
                mActor.Status.Additions.Remove("Meditatioon");
            }
            if (mActor.Status.Additions.ContainsKey("Hiding"))
            {
                mActor.Status.Additions["Hiding"].AdditionEnd();
                mActor.Status.Additions.Remove("Hiding");
            }
            if (mActor.Status.Additions.ContainsKey("fish"))
            {
                mActor.Status.Additions["fish"].AdditionEnd();
                mActor.Status.Additions.Remove("fish");
            }
            if (mActor.Status.Additions.ContainsKey("Cloaking"))
            {
                mActor.Status.Additions["Cloaking"].AdditionEnd();
                mActor.Status.Additions.Remove("Cloaking");
            }
            if (mActor.Status.Additions.ContainsKey("IAmTree"))
            {
                mActor.Status.Additions["IAmTree"].AdditionEnd();
                mActor.Status.Additions.Remove("IAmTree");
            }
            if (mActor.Status.Additions.ContainsKey("Invisible"))
            {
                mActor.Status.Additions["Invisible"].AdditionEnd();
                mActor.Status.Additions.Remove("Invisible");
            }

            if (mActor.HP == 0)
                return;
            //send also the possessioned actors to the same map
            if (mActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                if (pc.PossessionTarget != 0 && !possession)
                    return;
                List<ActorPC> possessioned = pc.PossesionedActors;
                foreach (ActorPC i in possessioned)
                {
                    if (i == pc) continue;
                    SendActorToMap(i, newMap, x, y);
                }
            }

            // obtain the new map
            byte mapid = (byte)newMap.id;
            if (mapid == mActor.MapID)
            {
                TeleportActor(mActor, x, y);
                return;
            }

            // delete the actor from this map
            this.DeleteActor(mActor);

            // update the actor
            mActor.MapID = mapid;
            mActor.X = x;
            mActor.Y = y;
            mActor.Buff.Dead = false;

            // register the actor in the new map
            if (mActor.type != ActorType.PC)
            {
                newMap.RegisterActor(mActor);
            }
            else
            {
                ((ActorPC)mActor).Motion = MotionType.STAND;
                newMap.RegisterActor(mActor, mActor.ActorID);
                Network.Client.MapClient client = Network.Client.MapClient.FromActorPC((ActorPC)mActor);
                if (client.AI != null)
                {
                    client.AI.map = newMap;
                }
            }
        }

        public void SendActorToMap(Actor mActor, uint mapid, short x, short y)
        {
            SendActorToMap(mActor, mapid, x, y, false);
        }

        public void SendActorToMap(Actor mActor, uint mapid, short x, short y, bool possession)
        {
            // todo: add support for multiple map servers
            if (mActor.Status.Additions.ContainsKey("Meditatioon"))
            {
                mActor.Status.Additions["Meditatioon"].AdditionEnd();
                mActor.Status.Additions.Remove("Meditatioon");
            }
            if (mActor.Status.Additions.ContainsKey("Hiding"))
            {
                mActor.Status.Additions["Hiding"].AdditionEnd();
                mActor.Status.Additions.Remove("Hiding");
            }
            if (mActor.Status.Additions.ContainsKey("fish"))
            {
                mActor.Status.Additions["fish"].AdditionEnd();
                mActor.Status.Additions.Remove("fish");
            }
            if (mActor.Status.Additions.ContainsKey("Cloaking"))
            {
                mActor.Status.Additions["Cloaking"].AdditionEnd();
                mActor.Status.Additions.Remove("Cloaking");
            }
            if (mActor.Status.Additions.ContainsKey("IAmTree"))
            {
                mActor.Status.Additions["IAmTree"].AdditionEnd();
                mActor.Status.Additions.Remove("IAmTree");
            }
            if (mActor.Status.Additions.ContainsKey("Invisible"))
            {
                mActor.Status.Additions["Invisible"].AdditionEnd();
                mActor.Status.Additions.Remove("Invisible");
            }

            if (mActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)mActor;
                if (pc.PossessionTarget != 0 && !possession)
                    return;
                List<ActorPC> possessioned = pc.PossesionedActors;
                foreach (ActorPC i in possessioned)
                {
                    if (i == pc) continue;
                    SendActorToMap(i, mapid, x, y, true);
                }
            }

            // obtain the new map
            Map newMap;
            if (mapid == mActor.MapID)
            {
                TeleportActor(mActor, x, y);
                return;
            }
            newMap = MapManager.Instance.GetMap(mapid);
            if (newMap == null)
                return;
            // delete the actor from this map
            this.DeleteActor(mActor);
            if (x == 0f && y == 0f)
            {
                short[] pos = newMap.GetRandomPos();
                x = pos[0];
                y = pos[1];
            }
            // update the actor
            mActor.MapID = mapid;
            mActor.X = x;
            mActor.Y = y;
            if (mActor.Buff.Dead || mActor.HP == 0)
            {
                mActor.HP = 1;
                mActor.Buff.Dead = false;
            }

            // register the actor in the new map
            if (mActor.type != ActorType.PC)
            {
                newMap.RegisterActor(mActor);
            }
            else
            {
                ((ActorPC)mActor).Motion = MotionType.STAND;
                newMap.RegisterActor(mActor, mActor.ActorID);
                Network.Client.MapClient client = Network.Client.MapClient.FromActorPC((ActorPC)mActor);
                if (client.AI != null)
                {
                    client.AI.map = newMap;
                }
            }
        }

        private void SendActorToActor(Actor mActor, Actor tActor)
        {
            if (mActor.MapID == tActor.MapID)
                this.TeleportActor(mActor, tActor.X, tActor.Y);
            else
                this.SendActorToMap(mActor, tActor.MapID, tActor.X, tActor.Y);
        }



    }
}
*/