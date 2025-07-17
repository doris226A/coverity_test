using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaDB.Skill;
using SagaLib;
using SagaMap.Mob;
using SagaMap.Skill.SkillDefinations.Cardinal;
namespace SagaMap.Skill.SkillDefinations.Monster
{
    public class A7627 : ISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (sActor.TTime["skill_time"] + new TimeSpan(0, 3, 0) > DateTime.Now)
            {
                return;
            }
            sActor.TTime["skill_time"] = DateTime.Now;
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
            test2(sActor, args);
        }

        void test2(Actor pc, SkillArg autoheal)
        {
            ActorSkill actor = new ActorSkill(autoheal.skill, pc);
            Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
            //设定技能体位置    
            actor.MapID = pc.MapID;
            actor.X = pc.X;
            actor.Y = pc.Y;
            actor.Speed = 200;
            //创建AI类
            SagaMap.Mob.MobAI ai = new SagaMap.Mob.MobAI(actor, true);
            short a = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short b = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short c = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short d = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short e = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short f = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short g = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short h = (short)SagaLib.Global.Random.Next(-2000, 2000);
            short sActorX = pc.X;
            short sActorY = pc.Y;
            sActorX += a;
            sActorY += b;
            short sActorX2 = pc.X;
            short sActorY2 = pc.Y;
            sActorX2 += c;
            sActorY2 += d;
            short sActorX3 = pc.X;
            short sActorY3 = pc.Y;
            sActorX3 += e;
            sActorY3 += f;
            short sActorX4 = pc.X;
            short sActorY4 = pc.Y;
            sActorX4 += g;
            sActorY4 += h;


            short sActorX5 = pc.X;
            short sActorY5 = pc.Y;
            sActorX5 += a;
            sActorY5 += c;

            short sActorX6 = pc.X;
            short sActorY6 = pc.Y;
            sActorX6 += b;
            sActorY6 += d;

            short sActorX7 = pc.X;
            short sActorY7 = pc.Y;
            sActorX7 += e;
            sActorY7 += g;

            short sActorX8 = pc.X;
            short sActorY8 = pc.Y;
            sActorX8 += f;
            sActorY8 += h;
            List<MapNode> path = ai.FindPath(SagaLib.Global.PosX16to8(actor.X, map.Width), SagaLib.Global.PosY16to8(actor.Y, map.Height), SagaLib.Global.PosX16to8(sActorX, map.Width), SagaLib.Global.PosY16to8(sActorY, map.Height));
            List<MapNode> pathA = ai.FindPath(SagaLib.Global.PosX16to8(sActorX, map.Width), SagaLib.Global.PosY16to8(sActorY, map.Height), SagaLib.Global.PosX16to8(sActorX2, map.Width), SagaLib.Global.PosY16to8(sActorY2, map.Height));
            List<MapNode> pathB = ai.FindPath(SagaLib.Global.PosX16to8(sActorX2, map.Width), SagaLib.Global.PosY16to8(sActorY2, map.Height), SagaLib.Global.PosX16to8(sActorX3, map.Width), SagaLib.Global.PosY16to8(sActorY3, map.Height));
            List<MapNode> pathC = ai.FindPath(SagaLib.Global.PosX16to8(sActorX3, map.Width), SagaLib.Global.PosY16to8(sActorY3, map.Height), SagaLib.Global.PosX16to8(sActorX4, map.Width), SagaLib.Global.PosY16to8(sActorY4, map.Height));
            List<MapNode> pathD = ai.FindPath(SagaLib.Global.PosX16to8(sActorX4, map.Width), SagaLib.Global.PosY16to8(sActorY4, map.Height), SagaLib.Global.PosX16to8(sActorX5, map.Width), SagaLib.Global.PosY16to8(sActorY5, map.Height));
            List<MapNode> pathE = ai.FindPath(SagaLib.Global.PosX16to8(sActorX5, map.Width), SagaLib.Global.PosY16to8(sActorY5, map.Height), SagaLib.Global.PosX16to8(sActorX6, map.Width), SagaLib.Global.PosY16to8(sActorY6, map.Height));
            List<MapNode> pathF = ai.FindPath(SagaLib.Global.PosX16to8(sActorX6, map.Width), SagaLib.Global.PosY16to8(sActorY6, map.Height), SagaLib.Global.PosX16to8(sActorX7, map.Width), SagaLib.Global.PosY16to8(sActorY7, map.Height));
            List<MapNode> pathG = ai.FindPath(SagaLib.Global.PosX16to8(sActorX7, map.Width), SagaLib.Global.PosY16to8(sActorY7, map.Height), SagaLib.Global.PosX16to8(sActorX8, map.Width), SagaLib.Global.PosY16to8(sActorY8, map.Height));


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
            foreach (MapNode i in pathD)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathE)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathF)
            {
                path.Add(i);
            }
            foreach (MapNode i in pathG)
            {
                path.Add(i);
            }
            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new SagaMap.ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //创建技能效果处理对象


            Activator timer = new Activator(pc, actor, autoheal, path, SagaLib.Elements.Dark);
            timer.Activate();
        }


        private class Activator : MultiRunTask
        {
            ActorSkill actor;
            Actor caster;
            SkillArg skill;
            Map map;
            List<MapNode> path;
            int count = 0;
            int countA = 0;
            float factor = 1f;
            Elements element;
            bool stop = false;

            public Activator(Actor caster, ActorSkill actor, SkillArg args, List<MapNode> path, Elements element)
            {
                this.actor = actor;
                this.caster = caster;
                this.skill = args.Clone();
                map = SagaMap.Manager.MapManager.Instance.GetMap(actor.MapID);
                this.period = 300;
                this.dueTime = 200;
                this.path = path;
                factor = 2.25f;
                this.element = element;
            }

            public override void CallBack()
            {
                try
                {
                    if ((path.Count <= count + 1))// || (count > skill.skill.Level + 2))
                    {
                        this.Deactivate();
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
                            map.MoveActor(Map.MOVE_TYPE.START, actor, pos, 0, 400);

                            //取得当前格子内的Actor
                            //List<Actor> list = map.GetActorsArea(actor, 50, false);
                            List<Actor> list = new List<Actor>();
                            List<Actor> affected = new List<Actor>();

                            //map.SendEffect(caster, SagaLib.Global.PosX16to8(actor.X, map.Width), SagaLib.Global.PosY16to8(actor.Y, map.Height), 4004);
                            if (count >= 2)
                            {
                                list = map.GetActorsArea(SagaLib.Global.PosX8to16(path[count - 2].x, map.Width), SagaLib.Global.PosY8to16(path[count - 2].y, map.Height), 50, null);
                                map.SendEffect(caster, path[count - 2].x, path[count - 2].y, 4003);
                            }
                            else
                            {
                                list = map.GetActorsArea(SagaLib.Global.PosX8to16(path[count].x, map.Width), SagaLib.Global.PosY8to16(path[count].y, map.Height), 50, null);
                                map.SendEffect(caster, path[count].x, path[count].y, 4003);
                            }
                            //筛选有效对象
                            foreach (Actor i in list)
                            {
                                // 检查是否是玩家，并排除玩家作为攻击目标
                                if (i.type == ActorType.PC || i.type == ActorType.PARTNER)
                                {
                                    affected.Add(i);
                                }
                            }

                            skill.affectedActors.Clear();
                            SkillHandler.Instance.MagicAttack(caster, affected, skill, element, factor);
                            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, skill, actor, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.ShowError(ex);
                            this.Deactivate();
                            map.DeleteActor(actor);
                        }
                        countA++;
                        if (countA % 2 == 0)
                        {
                            count++;
                        }
                        //count++;
                    }


                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                    this.Deactivate();
                    map.DeleteActor(actor);
                }

                //解开同步锁
                //测试去除技能同步锁ClientManager.LeaveCriticalArea();
            }
        }
    }
}
