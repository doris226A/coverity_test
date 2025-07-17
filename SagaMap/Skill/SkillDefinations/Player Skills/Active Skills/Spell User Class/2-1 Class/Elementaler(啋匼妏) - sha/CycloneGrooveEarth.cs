using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;

using SagaLib;
using SagaDB.Actor;
using SagaMap.Mob;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Elementaler
{
    /// <summary>
    /// 元素四重奏地（エレメンタルカルテット）
    /// </summary>
    public class CycloneGrooveEarth : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置    
            actor.MapID = sActor.MapID;
            actor.X = sActor.X;
            actor.Y = sActor.Y;
            actor.X += 300;
            actor.Y += 300;
            actor.Speed = 500;
            //创建AI类
            Mob.MobAI ai = new SagaMap.Mob.MobAI(actor, true);

            short sActorX = sActor.X;
            short sActorY = sActor.Y;
            sActorX += 300;
            sActorY -= 200;
            short sActorX2 = sActor.X;
            short sActorY2 = sActor.Y;
            sActorX2 -= 100;
            sActorY2 -= 200;
            short sActorX3 = sActor.X;
            short sActorY3 = sActor.Y;
            sActorX3 -= 100;
            //sActorY3 += 50;
            short sActorX4 = sActor.X;
            short sActorY4 = sActor.Y;
            sActorX4 += 100;
            //sActorY4 += 300;
            List<MapNode> path = ai.FindPath(PosX16to8W(actor.X, map.Width), PosY16to8H(actor.Y, map.Height), PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height));
            List<MapNode> pathA = ai.FindPath(PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height), PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height));
            List<MapNode> pathB = ai.FindPath(PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height), PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height));
            List<MapNode> pathC = ai.FindPath(PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height), PosX16to8W(sActorX4, map.Width), PosY16to8H(sActorY4, map.Height));

            foreach (MapNode i in pathA)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathB)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathC)
            {
                path.Add(i);
            }

            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //创建技能效果处理对象


            Activator timer = new Activator(sActor, actor, args, path, SagaLib.Elements.Earth);
            timer.Activate();
            ballA(sActor, dActor, args, level);
            ballB(sActor, dActor, args, level);
            ballC(sActor, dActor, args, level);
        }
        #endregion

        byte PosX16to8W(short pos, ushort width)
        {
            double val = ((double)width / 2);
            double tmp = (((float)(pos - 50) / 100) + val);
            if (tmp < 0)
                tmp = 0;
            return (byte)tmp;
        }

        byte PosY16to8H(short pos, ushort height)
        {
            double val = ((double)height / 2);
            double tmp = (((float)-(pos + 50) / 100) + val);
            if (tmp < 0)
                tmp = 0;
            return (byte)tmp;
        }

        void ballA(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置    
            actor.MapID = sActor.MapID;
            actor.X = sActor.X;
            actor.Y = sActor.Y;
            actor.X += 300;
            actor.Y -= 300;
            actor.Speed = 500;
            //创建AI类
            Mob.MobAI ai = new SagaMap.Mob.MobAI(actor, true);

            short sActorX = sActor.X;
            short sActorY = sActor.Y;
            sActorX -= 200;
            sActorY -= 300;
            short sActorX2 = sActor.X;
            short sActorY2 = sActor.Y;
            sActorX2 -= 200;
            sActorY2 += 100;
            short sActorX3 = sActor.X;
            short sActorY3 = sActor.Y;
            //sActorX3 -= 100;
            sActorY3 += 100;
            short sActorX4 = sActor.X;
            short sActorY4 = sActor.Y;
            //sActorX4 += 100;
            sActorY4 -= 100;
            List<MapNode> path = ai.FindPath(PosX16to8W(actor.X, map.Width), PosY16to8H(actor.Y, map.Height), PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height));
            List<MapNode> pathA = ai.FindPath(PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height), PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height));
            List<MapNode> pathB = ai.FindPath(PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height), PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height));
            List<MapNode> pathC = ai.FindPath(PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height), PosX16to8W(sActorX4, map.Width), PosY16to8H(sActorY4, map.Height));

            foreach (MapNode i in pathA)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathB)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathC)
            {
                path.Add(i);
            }

            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //创建技能效果处理对象

            Activator timer = new Activator(sActor, actor, args, path, SagaLib.Elements.Earth);
            timer.Activate();
        }

        void ballB(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置    
            actor.MapID = sActor.MapID;
            actor.X = sActor.X;
            actor.Y = sActor.Y;
            actor.X -= 300;
            actor.Y += 300;
            actor.Speed = 500;
            //创建AI类
            Mob.MobAI ai = new SagaMap.Mob.MobAI(actor, true);

            short sActorX = sActor.X;
            short sActorY = sActor.Y;
            sActorX += 200;
            sActorY += 300;
            short sActorX2 = sActor.X;
            short sActorY2 = sActor.Y;
            sActorX2 += 200;
            sActorY2 -= 100;
            short sActorX3 = sActor.X;
            short sActorY3 = sActor.Y;
            //sActorX3 -= 100;
            sActorY3 -= 100;
            short sActorX4 = sActor.X;
            short sActorY4 = sActor.Y;
            //sActorX4 += 100;
            sActorY4 += 100;
            List<MapNode> path = ai.FindPath(PosX16to8W(actor.X, map.Width), PosY16to8H(actor.Y, map.Height), PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height));
            List<MapNode> pathA = ai.FindPath(PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height), PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height));
            List<MapNode> pathB = ai.FindPath(PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height), PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height));
            List<MapNode> pathC = ai.FindPath(PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height), PosX16to8W(sActorX4, map.Width), PosY16to8H(sActorY4, map.Height));

            foreach (MapNode i in pathA)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathB)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathC)
            {
                path.Add(i);
            }

            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //创建技能效果处理对象



            Activator timer = new Activator(sActor, actor, args, path, SagaLib.Elements.Earth);
            timer.Activate();
        }

        void ballC(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置    
            actor.MapID = sActor.MapID;
            actor.X = sActor.X;
            actor.Y = sActor.Y;
            actor.X -= 300;
            actor.Y -= 300;
            actor.Speed = 500;
            //创建AI类
            Mob.MobAI ai = new SagaMap.Mob.MobAI(actor, true);

            short sActorX = sActor.X;
            short sActorY = sActor.Y;
            sActorX -= 300;
            sActorY += 200;
            short sActorX2 = sActor.X;
            short sActorY2 = sActor.Y;
            sActorX2 += 100;
            sActorY2 += 200;
            short sActorX3 = sActor.X;
            short sActorY3 = sActor.Y;
            sActorX3 += 100;
            //sActorY3 -= 100;
            short sActorX4 = sActor.X;
            short sActorY4 = sActor.Y;
            sActorX4 -= 100;
            //sActorY4 += 100;
            List<MapNode> path = ai.FindPath(PosX16to8W(actor.X, map.Width), PosY16to8H(actor.Y, map.Height), PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height));
            List<MapNode> pathA = ai.FindPath(PosX16to8W(sActorX, map.Width), PosY16to8H(sActorY, map.Height), PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height));
            List<MapNode> pathB = ai.FindPath(PosX16to8W(sActorX2, map.Width), PosY16to8H(sActorY2, map.Height), PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height));
            List<MapNode> pathC = ai.FindPath(PosX16to8W(sActorX3, map.Width), PosY16to8H(sActorY3, map.Height), PosX16to8W(sActorX4, map.Width), PosY16to8H(sActorY4, map.Height));

            foreach (MapNode i in pathA)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathB)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathC)
            {
                path.Add(i);
            }

            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //创建技能效果处理对象



            Activator timer = new Activator(sActor, actor, args, path, SagaLib.Elements.Earth);
            timer.Activate();
        }

        #region Timer

        private class Activator : MultiRunTask
        {
            ActorSkill actor;
            Actor caster;
            SkillArg skill;
            Map map;
            List<MapNode> path;
            int count = 0;
            float factor = 1f;
            Elements element;
            bool stop = false;

            public Activator(Actor caster, ActorSkill actor, SkillArg args, List<MapNode> path, Elements element)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
                this.period = 200;
                this.dueTime = 200;
                this.path = path;
                factor = CalcFactor(args.skill.Level);
                this.element = element;
            }

            /// <summary>
            /// 计算伤害加成
            /// </summary>
            /// <param name="level">技能等级</param>
            /// <returns>伤害加成</returns>
            float CalcFactor(byte level)
            {
                switch (level)
                {
                    case 1:
                        return 1.9f;
                    case 2:
                        return 2.1f;
                    case 3:
                        return 2.25f;
                    default:
                        return 1f;
                }
            }

            public override void CallBack()
            {
                try
                {
                    if ((path.Count <= count + 1))// || (count > skill.skill.Level + 2))
                    {
                        this.Deactivate();
                        //在指定地图删除技能体（技能效果结束）
                        map.DeleteActor(actor);
                    }
                    else
                    {
                        try
                        {
                            short[] pos = new short[2];
                            short[] pos2 = new short[2];
                            pos[0] = SagaLib.Global.PosX8to16(path[count].x, map.Width);
                            pos[1] = SagaLib.Global.PosY8to16(path[count].y, map.Height);
                            pos2[0] = SagaLib.Global.PosX8to16(path[count + 1].x, map.Width);
                            pos2[1] = SagaLib.Global.PosY8to16(path[count + 1].y, map.Height);
                            map.MoveActor(Map.MOVE_TYPE.START, actor, pos, 0, 200);

                            //取得当前格子内的Actor
                            List<Actor> list = map.GetActorsArea(actor, 50, false);
                            List<Actor> affected = new List<Actor>();

                            //筛选有效对象
                            foreach (Actor i in list)
                            {
                                // 检查是否是玩家，并排除玩家作为攻击目标
                                if (i.type == ActorType.PC)
                                {
                                    continue; // 如果是玩家，跳过不作为攻击目标
                                }
                                if (SkillHandler.Instance.CheckValidAttackTarget(caster, i))
                                {
                                    affected.Add(i);
                                }

                            }
                       /*     if (map.GetActorsArea(pos2[0], pos2[1], 50).Count != 0 || map.Info.walkable[path[count + 1].x, path[count + 1].y] != 2)
                            {
                                if (stop)
                                {
                                    this.Deactivate();
                                    //在指定地图删除技能体（技能效果结束）
                                    map.DeleteActor(actor);
                                    //return 前必须解锁
                                    ClientManager.LeaveCriticalArea();
                                    return;
                                }
                                else
                                    stop = true;
                            }*/

                            foreach (Actor i in affected)
                            {
                                if (!i.Status.Additions.ContainsKey("FortressCircleSEQ") &&
                                    !i.Status.Additions.ContainsKey("SolidBody"))
                                {
                                    //CannotMove addition = new CannotMove(skill.skill, i, 400);
                                   // Additions.Global.Stiff addition = new Stiff(skill.skill, i, 400);
                                   // SkillHandler.ApplyAddition(i, addition);
                                   // map.MoveActor(Map.MOVE_TYPE.START, i, pos2, 500, 500, true);


                                   /* if (i.type == ActorType.MOB || i.type == ActorType.PET || i.type == ActorType.SHADOW || i.type == ActorType.PARTNER)
                                    {
                                        ActorEventHandlers.MobEventHandler mob = (ActorEventHandlers.MobEventHandler)i.e;
                                        mob.AI.OnPathInterupt();
                                    }
                                    if (i.type == ActorType.PC)
                                    {
                                        skill.affectedActors.Clear();
                                        return;
                                    }*/
                                }

                            }

                            skill.affectedActors.Clear();
                            SkillHandler.Instance.MagicAttack(caster, affected, skill, element, factor);
                            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, skill, actor, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.ShowError(ex);
                        }

                        count++;
                    }


                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }

                //解开同步锁
                //测试去除技能同步锁ClientManager.LeaveCriticalArea();
            }
        }
        #endregion
    }
}