﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaDB.Item;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 露营
    /// </summary>
    public class Camp : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {

            bool tent = false;
            SagaDB.Item.Item item = sActor.Inventory.GetItem(10037900, Inventory.SearchType.ITEM_ID);
            if (item != null)
            {
                tent = false;
            }
            else
            {
                tent = true;
            }

            if (tent)
            {
                return -37;
            }
            if (sActor.TenkActor != null)
            {
                return -18;
            }
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            if (map.Info.walkable[args.x, args.y] != 0 && !map.Info.Flag.Test(SagaDB.Map.MapFlags.Healing))

            //if (map.Info.walkable[args.x, args.y] != 0 && !map.Info.Flag.Test(SagaDB.Map.MapFlags.Healing) && sActor.TenkActor == null)
            {
                if (item.Durability > 0)
                {
                    int[] min = { 10, 100, 90, 80, 70, 60, 50, 40, 30, 20, 10 };
                    int num = SagaLib.Global.Random.Next(0, 100);
                    if (num <= min[args.skill.Level])
                    {
                        item.Durability -= 1;
                    }
                    return 0;
                }
                else
                {
                    return -13;
                }
            }
            else
            {
                //SagaMap.Network.Client.MapClient.FromActorPC(sActor).SendSystemMessage("该地方无法搭建帐篷");
                return -6;
            }


        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {


            ActorPC pc = (ActorPC)sActor;
            ActorEvent actor = new ActorEvent(pc);
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);

            actor.MapID = pc.MapID;
            actor.X = SagaLib.Global.PosX8to16(args.x, map.Width);
            actor.Y = SagaLib.Global.PosY8to16(args.y, map.Height);
            uint eventID = Manager.ScriptManager.Instance.GetFreeIDSince(0xF0001233, 2000);
            actor.EventID = eventID;
            pc.TenkActor = actor;
            if (Manager.ScriptManager.Instance.Events.ContainsKey(0xF0001233))
            {
                Scripting.EventActor pattern = (Scripting.EventActor)Manager.ScriptManager.Instance.Events[0xF0001233];
                Scripting.EventActor newEvent = pattern.Clone();
                newEvent.Actor = actor;
                newEvent.EventID = actor.EventID;
                Manager.ScriptManager.Instance.Events.Add(newEvent.EventID, newEvent);
            }
            actor.Type = ActorEventTypes.TENT;
            actor.Title = string.Format(pc.Name + "的帳篷");
            actor.e = new ActorEventHandlers.ItemEventHandler(actor);
            map.RegisterActor(actor);
            actor.invisble = false;
            map.OnActorVisibilityChange(actor);

        }
        #endregion
    }
}