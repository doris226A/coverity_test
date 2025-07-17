
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.ActorEventHandlers;

using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 幸運數字（スリーセブン）
    /// </summary>
    public class Slot : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 100, true);
            foreach (Actor i in affected)
            {
                if (!i.Buff.Dead && i.type == ActorType.MOB)
                {
                    ActorMob m = (ActorMob)i;
                    if (m.MobID == 30610000 || m.MobID == 30620000 || m.MobID == 30630000 || m.MobID == 30640000 || m.MobID == 30650000)
                        return -12;
                }
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            ////建立設置型技能實體
            //ActorSkill actor = new ActorSkill(args.skill, sActor);
            //Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ////設定技能位置
            //actor.MapID = dActor.MapID;
            //actor.X = SagaLib.Global.PosX8to16(args.x, map.Width);
            //actor.Y = SagaLib.Global.PosY8to16(args.y, map.Height);
            ////設定技能的事件處理器，由於技能體不需要得到消息廣播，因此建立空處理器
            //actor.e = new ActorEventHandlers.NullEventHandler();
            ////在指定地圖註冊技能Actor
            //map.RegisterActor(actor);
            ////設置Actor隱身屬性為False
            //actor.invisble = false;
            ////廣播隱身屬性改變事件，以便讓玩家看到技能實體
            //map.OnActorVisibilityChange(actor);
            ////建立技能效果處理物件
            //Activator timer = new Activator(sActor, actor, args, level);
            //timer.Activate();

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            /*arrX = SagaLib.Global.PosX8to16(args.x, map.Width);
            arrY = SagaLib.Global.PosY8to16(args.y, map.Height);
            arrX2 = SagaLib.Global.PosX8to16(args.x++, map.Width);
            arrY2 = SagaLib.Global.PosY8to16(args.y, map.Height);
            arrX3 = SagaLib.Global.PosX8to16(args.x--, map.Width);
            arrY3 = SagaLib.Global.PosY8to16(args.y, map.Height);*/


            //ActorMob mob = map.SpawnMob(30650000, SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 0, sActor);
            mob = map.SpawnMob(30650000, SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 0, sActor);
            ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)mob.e;
            eh.Dying += Ondie;
            arrX = SagaLib.Global.PosX8to16(args.x, map.Width);
            arrY = SagaLib.Global.PosY8to16(args.y, map.Height);
            arrX2 = SagaLib.Global.PosX8to16((byte)(args.x + 1), map.Width);
            arrY2 = SagaLib.Global.PosY8to16(args.y, map.Height);
            arrX3 = SagaLib.Global.PosX8to16((byte)(args.x - 1), map.Width);
            arrY3 = SagaLib.Global.PosY8to16(args.y, map.Height);
            /*switch (SkillHandler.Instance.GetDirection(mob))
            {
                case SkillHandler.ActorDirection.North:
                case SkillHandler.ActorDirection.NorthEast:
                    SkillHandler.Instance.GetRelatedPos(mob, 1, 0, out arrX2, out arrY2);
                    SkillHandler.Instance.GetRelatedPos(mob, -1, 0, out arrX3, out arrY3);
                    break;
                case SkillHandler.ActorDirection.South:
                case SkillHandler.ActorDirection.SouthEast:
                    SkillHandler.Instance.GetRelatedPos(mob, 1, 0, out arrX2, out arrY2);
                    SkillHandler.Instance.GetRelatedPos(mob, -1, 0, out arrX3, out arrY3);
                    break;
                case SkillHandler.ActorDirection.West:
                case SkillHandler.ActorDirection.NorthWest:
                    SkillHandler.Instance.GetRelatedPos(mob, 0, 1, out arrX2, out arrY2);
                    SkillHandler.Instance.GetRelatedPos(mob, 0, -1, out arrX3, out arrY3);
                    break;
                case SkillHandler.ActorDirection.East:
                case SkillHandler.ActorDirection.SouthWest:
                    SkillHandler.Instance.GetRelatedPos(mob, 0, 1, out arrX2, out arrY2);
                    SkillHandler.Instance.GetRelatedPos(mob, 0, -1, out arrX3, out arrY3);
                   
                    break;
            }*/


            //ActorMob mobA = map.SpawnMob(30650000, SagaLib.Global.PosX8to16(args.x++, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 0, sActor);
            //ActorMob mobA = map.SpawnMob(30650000, arrX2, arrY2, 0, sActor);
            mobA = map.SpawnMob(30650000, arrX2, arrY2, 0, sActor);
            ActorEventHandlers.MobEventHandler ehA = (ActorEventHandlers.MobEventHandler)mobA.e;
            ehA.Dying += Ondie;

            //ActorMob mobB = map.SpawnMob(30650000, SagaLib.Global.PosX8to16(args.x--, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 0, sActor);
            //ActorMob mobB = map.SpawnMob(30650000, arrX3, arrY3, 0, sActor);
            mobB = map.SpawnMob(30650000, arrX3, arrY3, 0, sActor);
            ActorEventHandlers.MobEventHandler ehB = (ActorEventHandlers.MobEventHandler)mobB.e;
            ehB.Dying += Ondie;


            timer = new Activator(sActor, args, arrX, arrY, level);
            timer.Activate();
        }
        #endregion
        short arrX = 0;
        short arrY = 0;
        short arrX2 = 0;
        short arrY2 = 0;
        short arrX3 = 0;
        short arrY3 = 0;
        ActorMob mob;
        ActorMob mobA;
        ActorMob mobB;
        Activator timer;
        void Ondie(MobEventHandler e, ActorPC pc)
        {
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            ActorMob mob = null;
            switch (SagaLib.Global.Random.Next(1, 4))
            {
                case 1:
                    mob = map.SpawnMob(30610000, e.mob.X, e.mob.Y, 0, null);
                    break;
                case 2:
                    mob = map.SpawnMob(30620000, e.mob.X, e.mob.Y, 0, null);
                    break;
                case 3:
                    mob = map.SpawnMob(30630000, e.mob.X, e.mob.Y, 0, null);
                    break;
                case 4:
                    mob = map.SpawnMob(30640000, e.mob.X, e.mob.Y, 0, null);
                    break;
            }
            List<Actor> affected = map.GetActorsArea(arrX, arrY, 100, true);
            int A = 0;
            int B = 0;
            int C = 0;
            int D = 0;
            foreach (Actor i in affected)
            {
                if (!i.Buff.Dead && i.type == ActorType.MOB)
                {
                    ActorMob m = (ActorMob)i;
                    if (m.MobID == 30610000)
                        A++;
                    if (m.MobID == 30620000)
                        B++;
                    if (m.MobID == 30630000)
                        C++;
                    if (m.MobID == 30640000)
                        D++;
                }
            }
            if (A == 3)
            {
                switch (SagaLib.Global.Random.Next(1, 10))
                {
                    case 1:
                        skillM(mob, Elements.Earth, 1.3f, 5259, pc);
                        break;
                    case 2:
                        skillM(mob, Elements.Fire, 1.3f, 5002, pc);
                        break;
                    case 3:
                        skillM(mob, Elements.Water, 1.3f, 5260, pc);
                        break;
                    case 4:
                        skillM(mob, Elements.Wind, 1.3f, 5258, pc);
                        break;
                    case 5:
                        skillM(mob, Elements.Dark, 1.3f, 5142, pc);
                        break;
                    case 6:
                        skillL(mob, 1.5f, pc);
                        break;
                    case 7:
                        //ジャイアントコッケー像
                        SlotSpawnMob(30490000, arrX, arrY, 0, null, map);
                        break;
                    case 8:
                        //プチメタリカ
                        SlotSpawnMob(10001002, arrX, arrY, 2500, null, map);
                        break;
                    case 9:
                        //女王
                        SlotSpawnMob(10320001, arrX, arrY, 2500, null, map);
                        break;
                    case 10:
                        //笨重
                        SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10032810);
                        ActorItem actor = new ActorItem(item);
                        actor.e = new ActorEventHandlers.ItemEventHandler(actor);
                        actor.Owner = pc;
                        actor.Party = false;
                        actor.MapID = pc.MapID;
                        actor.X = pc.X;
                        actor.Y = pc.Y;
                        map.RegisterActor(actor);
                        actor.invisble = false;
                        map.OnActorVisibilityChange(actor);
                        Tasks.Item.DeleteItem task = new Tasks.Item.DeleteItem(actor);
                        task.Activate();
                        actor.Tasks.Add("DeleteItem", task);
                        break;
                }
                clear(pc);
                return;
            }
            else if (B == 3)
            {
                switch (SagaLib.Global.Random.Next(1, 10))
                {
                    case 1:
                        skillM(mob, Elements.Earth, 1.3f, 5259, pc);
                        break;
                    case 2:
                        skillM(mob, Elements.Fire, 1.3f, 5002, pc);
                        break;
                    case 3:
                        skillM(mob, Elements.Water, 1.3f, 5260, pc);
                        break;
                    case 4:
                        skillM(mob, Elements.Wind, 1.3f, 5258, pc);
                        break;
                    case 5:
                        skillM(mob, Elements.Dark, 1.3f, 5142, pc);
                        break;
                    case 6:
                        skillL(mob, 1.5f, pc);
                        break;
                    case 7:
                        //ジャイアントコッケー像
                        SlotSpawnMob(30490000, arrX, arrY, 0, null, map);
                        break;
                    case 8:
                        //プチメタリカ
                        SlotSpawnMob(10001002, arrX, arrY, 2500, null, map);
                        break;
                    case 9:
                        //女王
                        SlotSpawnMob(10320001, arrX, arrY, 2500, null, map);
                        break;
                    case 10:
                        //笨重
                        SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10032810);
                        ActorItem actor = new ActorItem(item);
                        actor.e = new ActorEventHandlers.ItemEventHandler(actor);
                        actor.Owner = pc;
                        actor.Party = false;
                        actor.MapID = pc.MapID;
                        actor.X = pc.X;
                        actor.Y = pc.Y;
                        map.RegisterActor(actor);
                        actor.invisble = false;
                        map.OnActorVisibilityChange(actor);
                        Tasks.Item.DeleteItem task = new Tasks.Item.DeleteItem(actor);
                        task.Activate();
                        actor.Tasks.Add("DeleteItem", task);
                        break;
                }
                clear(pc);
                return;
            }
            else if (C == 3)
            {
                switch (SagaLib.Global.Random.Next(1, 10))
                {
                    case 1:
                        skillM(mob, Elements.Earth, 1.3f, 5259, pc);
                        break;
                    case 2:
                        skillM(mob, Elements.Fire, 1.3f, 5002, pc);
                        break;
                    case 3:
                        skillM(mob, Elements.Water, 1.3f, 5260, pc);
                        break;
                    case 4:
                        skillM(mob, Elements.Wind, 1.3f, 5258, pc);
                        break;
                    case 5:
                        skillM(mob, Elements.Dark, 1.3f, 5142, pc);
                        break;
                    case 6:
                        skillL(mob, 1.5f, pc);
                        break;
                    case 7:
                        //ジャイアントコッケー像
                        SlotSpawnMob(30490000, arrX, arrY, 0, null, map);
                        break;
                    case 8:
                        //プチメタリカ
                        SlotSpawnMob(10001002, arrX, arrY, 2500, null, map);
                        break;
                    case 9:
                        //女王
                        SlotSpawnMob(10320001, arrX, arrY, 2500, null, map);
                        break;
                    case 10:
                        //笨重
                        SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10032810);
                        ActorItem actor = new ActorItem(item);
                        actor.e = new ActorEventHandlers.ItemEventHandler(actor);
                        actor.Owner = pc;
                        actor.Party = false;
                        actor.MapID = pc.MapID;
                        actor.X = pc.X;
                        actor.Y = pc.Y;
                        map.RegisterActor(actor);
                        actor.invisble = false;
                        map.OnActorVisibilityChange(actor);
                        Tasks.Item.DeleteItem task = new Tasks.Item.DeleteItem(actor);
                        task.Activate();
                        actor.Tasks.Add("DeleteItem", task);
                        break;
                }
                clear(pc);
                return;
            }
            else if (D == 3)
            {
                switch (SagaLib.Global.Random.Next(1, 10))
                {
                    case 1:
                        skillM(mob, Elements.Earth, 1.3f, 5259, pc);
                        break;
                    case 2:
                        skillM(mob, Elements.Fire, 1.3f, 5002, pc);
                        break;
                    case 3:
                        skillM(mob, Elements.Water, 1.3f, 5260, pc);
                        break;
                    case 4:
                        skillM(mob, Elements.Wind, 1.3f, 5258, pc);
                        break;
                    case 5:
                        skillM(mob, Elements.Dark, 1.3f, 5142, pc);
                        break;
                    case 6:
                        skillL(mob, 1.5f, pc);
                        break;
                    case 7:
                        //ジャイアントコッケー像
                        SlotSpawnMob(30490000, arrX, arrY, 0, null, map);
                        break;
                    case 8:
                        //プチメタリカ
                        SlotSpawnMob(10001002, arrX, arrY, 2500, null, map);
                        break;
                    case 9:
                        //女王
                        SlotSpawnMob(10320001, arrX, arrY, 2500, null, map);
                        break;
                    case 10:
                        //笨重
                        SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10032810);
                        ActorItem actor = new ActorItem(item);
                        actor.e = new ActorEventHandlers.ItemEventHandler(actor);
                        actor.Owner = pc;
                        actor.Party = false;
                        actor.MapID = pc.MapID;
                        actor.X = pc.X;
                        actor.Y = pc.Y;
                        map.RegisterActor(actor);
                        actor.invisble = false;
                        map.OnActorVisibilityChange(actor);
                        Tasks.Item.DeleteItem task = new Tasks.Item.DeleteItem(actor);
                        task.Activate();
                        actor.Tasks.Add("DeleteItem", task);
                        break;
                }
                clear(pc);
                return;
            }
            else
            {
                ActorEventHandlers.MobEventHandler eh2 = (ActorEventHandlers.MobEventHandler)mob.e;
                eh2.Dying += OndieA;
            }
        }
        void OndieA(MobEventHandler e, ActorPC pc)
        {
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            ActorMob mob = map.SpawnMob(30650000, e.mob.X, e.mob.Y, 0, null);
            ActorEventHandlers.MobEventHandler eh2 = (ActorEventHandlers.MobEventHandler)mob.e;
            eh2.Dying += Ondie;
        }

        void clear(Actor pc)
        {
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            List<Actor> affected = map.GetActorsArea(arrX, arrY, 100, true);
            foreach (Actor i in affected)
            {
                if (!i.Buff.Dead && i.type == ActorType.MOB)
                {
                    ActorMob m = (ActorMob)i;
                    if (m.MobID == 30610000 || m.MobID == 30620000 || m.MobID == 30630000 || m.MobID == 30640000 || m.MobID == 30650000)
                    {
                        m.ClearTaskAddition();
                        ActorEventHandlers.MobEventHandler eh2 = (ActorEventHandlers.MobEventHandler)m.e;
                        eh2.Dying -= Ondie;
                        eh2.OnDie(false);
                        //map.DeleteActor(m);
                    }
                }
            }
            if (timer != null && timer.Activated)
            {
                timer.Deactivate();
            }

        }

        void skillM(Actor sActor, Elements Element, float factor, uint effectid, Actor pc)
        {
            //float factor = 1.8f;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(pc, act))
                {
                    realAffected.Add(act);
                }
            }
            map.SendEffect(sActor, effectid);
            SkillArg arg = new SkillArg();
            SkillHandler.Instance.MagicAttack(sActor, realAffected, arg, Element, factor);
        }
        void skillL(Actor sActor, float factor, Actor pc)
        {
            //float factor = 1.8f;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 550, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(pc, act))
                {
                    realAffected.Add(act);
                }
            }
            map.SendEffect(sActor, 5159);
            SkillArg arg = new SkillArg();
            SkillHandler.Instance.MagicAttack(sActor, realAffected, arg, Elements.Dark, factor);
        }



        #region Timer
        private class Activator : MultiRunTask
        {
            Actor sActor;
            SkillArg skill;
            float factor;
            Map map;
            short arrX = 0;
            short arrY = 0;
            ActorMob mob;
            public Activator(Actor _sActor, SkillArg _args, short arrX, short arrY, byte level)
            {
                sActor = _sActor;
                this.arrX = arrX;
                this.arrY = arrY;
                skill = _args.Clone();
                factor = 0.1f * level;
                this.dueTime = 30000;
                this.period = 1000;
                map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            }
            public override void CallBack()
            {
                //同步鎖，表示之後的代碼是執行緒安全的，也就是，不允許被第二個執行緒同時訪問
                //测试去除技能同步锁ClientManager.EnterCriticalArea();
                try
                {
                    List<Actor> affected = map.GetActorsArea(arrX, arrY, 100, true);
                    foreach (Actor i in affected)
                    {
                        if (!i.Buff.Dead && i.type == ActorType.MOB)
                        {
                            ActorMob m = (ActorMob)i;
                            if (m.MobID == 30610000 || m.MobID == 30620000 || m.MobID == 30630000 || m.MobID == 30640000 || m.MobID == 30650000)
                            {
                                m.ClearTaskAddition();
                                map.DeleteActor(m);
                            }
                        }
                    }
                    switch (SagaLib.Global.Random.Next(1, 10))
                    {
                        case 1:
                            //コッコー5匹
                            SlotSpawnMob(10010000, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10010000, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10010000, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10010000, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10010000, arrX, arrY, 2500, null, map);
                            break;
                        case 2:
                            //ゼリコ
                            //SlotSpawnMob(30650000, arrX, arrY, 2500, null);
                            SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10032800);
                            ActorItem actor = new ActorItem(item);
                            actor.e = new ActorEventHandlers.ItemEventHandler(actor);
                            actor.Owner = sActor;
                            actor.Party = false;
                            actor.MapID = sActor.MapID;
                            actor.X = arrX;
                            actor.Y = arrY;
                            map.RegisterActor(actor);
                            actor.invisble = false;
                            map.OnActorVisibilityChange(actor);
                            Tasks.Item.DeleteItem task = new Tasks.Item.DeleteItem(actor);
                            task.Activate();
                            actor.Tasks.Add("DeleteItem", task);
                            break;
                        case 3:
                            //パンサーゴースト3匹
                            SlotSpawnMob(10681500, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10681500, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10681500, arrX, arrY, 2500, null, map);
                            break;
                        case 4:
                            //幼虫1匹
                            SlotSpawnMob(10300000, arrX, arrY, 2500, null, map);
                            break;
                        case 5:
                            //でか蜘蛛
                            SlotSpawnMob(10210001, arrX, arrY, 2500, null, map);
                            break;
                        case 6:
                            //でかクローラー
                            SlotSpawnMob(10050001, arrX, arrY, 2500, null, map);
                            break;
                        case 7:
                            //スケルトン
                            SlotSpawnMob(10190000, arrX, arrY, 2500, null, map);
                            break;
                        case 8:
                            //アルカナハート3匹
                            SlotSpawnMob(10100100, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10100100, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10100100, arrX, arrY, 2500, null, map);
                            break;
                        case 9:
                            //パンサーデッド3匹
                            SlotSpawnMob(10181000, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10181000, arrX, arrY, 2500, null, map);
                            SlotSpawnMob(10181000, arrX, arrY, 2500, null, map);
                            break;
                        case 10:
                            //デス1匹
                            SlotSpawnMob(10250000, arrX, arrY, 2500, null, map);
                            break;
                    }
                    this.Deactivate();
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
                //解開同步鎖
                //测试去除技能同步锁ClientManager.LeaveCriticalArea();
            }



            #region TimerClear
            public ActorMob SlotSpawnMob(uint mobID, short x, short y, short moveRange, Actor master, Map map)
            {
                mob = new ActorMob(mobID);
                mob.MapID = map.ID;
                mob.X = x;
                mob.Y = y;
                ActorEventHandlers.MobEventHandler eh = new ActorEventHandlers.MobEventHandler(mob);
                mob.e = eh;
                eh.AI.MoveRange = moveRange;
                if (Mob.MobAIFactory.Instance.Items.ContainsKey(mob.MobID))
                    eh.AI.Mode = Mob.MobAIFactory.Instance.Items[mob.MobID];
                else
                    eh.AI.Mode = new Mob.AIMode(0);
                eh.AI.Master = master;
                eh.AI.X_Ori = x;
                eh.AI.Y_Ori = y;
                eh.AI.X_Spawn = x;
                eh.AI.Y_Spawn = y;
                if (eh.AI.Master != null)
                {
                    eh.AI.OnAttacked(master, 1);
                }
                map.RegisterActor(mob);
                mob.invisble = false;
                mob.sightRange = 2000;
                map.SendVisibleActorsToActor(mob);
                map.OnActorVisibilityChange(mob);
                eh.AI.Start();
                ActivatorClear Cleartimer = new ActivatorClear(mob);
                Cleartimer.Activate();
                return mob;
            }

            private class ActivatorClear : MultiRunTask
            {
                ActorMob moba;
                public ActivatorClear(ActorMob mob)
                {
                    moba = mob;
                    this.dueTime = 300000;
                    this.period = 1000;
                }
                public override void CallBack()
                {
                    try
                    {
                        moba.ClearTaskAddition();
                        Map map = Manager.MapManager.Instance.GetMap(moba.MapID);
                        map.DeleteActor(moba);
                        this.Deactivate();
                    }
                    catch (Exception ex)
                    {
                        this.Deactivate();
                        Logger.ShowError(ex);
                    }
                }
            }
            #endregion
        }
        #endregion

        #region TimerClear
        public ActorMob SlotSpawnMob(uint mobID, short x, short y, short moveRange, Actor master, Map map)
        {
            ActorMob mob = new ActorMob(mobID);
            mob.MapID = map.ID;
            mob.X = x;
            mob.Y = y;
            ActorEventHandlers.MobEventHandler eh = new ActorEventHandlers.MobEventHandler(mob);
            mob.e = eh;
            eh.AI.MoveRange = moveRange;
            if (Mob.MobAIFactory.Instance.Items.ContainsKey(mob.MobID))
                eh.AI.Mode = Mob.MobAIFactory.Instance.Items[mob.MobID];
            else
                eh.AI.Mode = new Mob.AIMode(0);
            eh.AI.Master = master;
            eh.AI.X_Ori = x;
            eh.AI.Y_Ori = y;
            eh.AI.X_Spawn = x;
            eh.AI.Y_Spawn = y;
            if (eh.AI.Master != null)
            {
                eh.AI.OnAttacked(master, 1);
            }
            map.RegisterActor(mob);
            mob.invisble = false;
            mob.sightRange = 2000;
            map.SendVisibleActorsToActor(mob);
            map.OnActorVisibilityChange(mob);
            eh.AI.Start();
            ActivatorClear Cleartimer = new ActivatorClear(mob);
            Cleartimer.Activate();
            return mob;
        }

        private class ActivatorClear : MultiRunTask
        {
            ActorMob mob;
            public ActivatorClear(ActorMob mob)
            {
                this.dueTime = 300000;
                this.period = 1000;
            }
            public override void CallBack()
            {
                try
                {
                    mob.ClearTaskAddition();
                    Map map = Manager.MapManager.Instance.GetMap(mob.MapID);
                    map.DeleteActor(mob);
                    this.Deactivate();
                }
                catch (Exception ex)
                {
                    this.Deactivate();
                    Logger.ShowError(ex);
                }
            }
        }
        #endregion

    }
}



