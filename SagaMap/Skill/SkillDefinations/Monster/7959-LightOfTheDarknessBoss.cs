﻿using SagaDB.Actor;
using SagaMap.Mob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    class LightOfTheDarknessBoss : ISkill
    {
        public Dictionary<SagaMap.Skill.SkillHandler.ActorDirection, List<int>> range = new Dictionary<SkillHandler.ActorDirection, List<int>>();
        public int TryCast(SagaDB.Actor.ActorPC sActor, SagaDB.Actor.Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            actor.MapID = sActor.MapID;
            actor.X = sActor.X;
            actor.Y = sActor.Y;
            Mob.MobAI ai = new SagaMap.Mob.MobAI(actor, true);
            List<MapNode> path = ai.FindPath(SagaLib.Global.PosX16to8(sActor.X, map.Width), SagaLib.Global.PosY16to8(sActor.Y, map.Height), args.x, args.y);
            if (path.Count >= 2)
            {
                //根据现有路径推算一步
                int deltaX = path[path.Count - 1].x - path[path.Count - 2].x;
                int deltaY = path[path.Count - 1].y - path[path.Count - 2].y;
                deltaX = path[path.Count - 1].x + deltaX;
                deltaY = path[path.Count - 1].y + deltaY;
                MapNode node = new MapNode();
                node.x = (byte)deltaX;
                node.y = (byte)deltaY;
                path.Add(node);
            }
            if (path.Count == 1)
            {
                //根据现有路径推算一步
                int deltaX = path[path.Count - 1].x - SagaLib.Global.PosX16to8(sActor.X, map.Width);
                int deltaY = path[path.Count - 1].y - SagaLib.Global.PosY16to8(sActor.Y, map.Height);
                deltaX = path[path.Count - 1].x + deltaX;
                deltaY = path[path.Count - 1].y + deltaY;
                MapNode node = new MapNode();
                node.x = (byte)deltaX;
                node.y = (byte)deltaY;
                path.Add(node);
            }
            short[] pos2 = new short[2];
            List<Actor> affected = new List<Actor>();
            List<Actor> list;
            int count = -1;
            while (path.Count > count + 1)
            {
                pos2[0] = SagaLib.Global.PosX8to16(path[count + 1].x, map.Width);
                pos2[1] = SagaLib.Global.PosY8to16(path[count + 1].y, map.Height);
                //取得当前格子内的Actor
                list = map.GetActorsArea(pos2[0], pos2[1], 100);
                //筛选有效对象
                foreach (Actor i in list)
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i) && !i.Buff.Dead)
                    {
                        bool appear = false;
                        foreach (Actor act in affected)
                        {
                            if (act == i)
                            {
                                appear = true;
                            }
                        }
                        if (appear == false)
                            affected.Add(i);
                    }
                }
                count++;
            }
            float factor = 15.0f;
            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, SagaLib.Elements.Holy, factor);
        }
    }
}
