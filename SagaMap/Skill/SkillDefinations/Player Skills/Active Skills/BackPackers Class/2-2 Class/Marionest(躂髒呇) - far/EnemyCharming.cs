
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Network.Client;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Marionest
{
    /// <summary>
    /// 調教馴化（モンスターテイミング）
    /// </summary>
    public class EnemyCharming : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Partner != null)
                {
                    return -13;
                }
                if (SkillHandler.Instance.isBossMob(dActor))
                {
                    return -13;
                }
            }
            else
            {
                return -13;
            }
            if (dActor.type == ActorType.PC)
            {
                return 0;
            }
            else if (dActor.type == ActorType.MOB)
            {
                ActorMob dActorMob = (ActorMob)dActor;
                if (!SkillHandler.Instance.isBossMob(dActorMob) && !dActorMob.BaseData.undead)
                {
                    return 0;
                }
            }
            return -13;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int rate = 0;
            if (dActor.type == ActorType.PC)
            {
                rate = 5 * level;
                if (SagaLib.Global.Random.Next(0, 99) < rate)
                {
                    dActor.HP = 0;
                    dActor.e.OnDie();
                    args.affectedActors.Add(dActor);
                    args.Init();
                    args.flag[0] = SagaLib.AttackFlag.DIE;
                    Manager.MapManager.Instance.GetMap(dActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, dActor, true);
                    ActorPC pc = (ActorPC)dActor;
                    ActorShadow p = SummonMe(pc, 1, sActor);
                    sActor.Slave.Add(p);

                    EnemyCharmingBuff skill = new EnemyCharmingBuff(args.skill, sActor, p, 600000);
                    SkillHandler.ApplyAddition(sActor, skill);
                }
                else
                {
                    args.affectedActors.Add(dActor);
                    args.Init();
                    args.flag[0] = SagaLib.AttackFlag.AVOID;
                }
            }
            else if (dActor.type == ActorType.MOB)
            {
                rate = 40 + 10 * level;
                if (dActor.Level - sActor.Level > 5)
                {
                    rate -= 70;
                }
                if (SagaLib.Global.Random.Next(0, 99) < rate)
                {
                    Map map = Manager.MapManager.Instance.GetMap(dActor.MapID);
                    ActorMob dActorMob = (ActorMob)dActor;
                    uint MobID = dActorMob.BaseData.id;
                    short x = dActor.X;
                    short y = dActor.Y;
                    //map.DeleteActor(dActor);
                    dActorMob.HP = 0;
                    dActorMob.e.OnDie();
                    args.affectedActors.Add(dActorMob);
                    args.Init();
                    args.flag[0] = SagaLib.AttackFlag.DIE;
                    Manager.MapManager.Instance.GetMap(dActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, dActorMob, true);
                    //ActorMob mob = SpawnMob(MobID, x, y, 2500, sActor, map);
                    ActorShadow mob = SpawnMob2(dActorMob, x, y, 2500, sActor, map);
                    sActor.Slave.Add(mob);
                    EnemyCharmingBuff skill = new EnemyCharmingBuff(args.skill, sActor, mob, 600000);
                    SkillHandler.ApplyAddition(sActor, skill);
                }
                else
                {
                    args.affectedActors.Add(dActor);
                    args.Init();
                    args.flag[0] = SagaLib.AttackFlag.AVOID;
                }
            }
        }
        public class EnemyCharmingBuff : DefaultBuff
        {
            private ActorMob mob;
            public EnemyCharmingBuff(SagaDB.Skill.Skill skill, Actor actor, ActorShadow mob, int lifetime)
                : base(skill, actor, "EnemyCharming", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.mob = mob;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                mob.ClearTaskAddition();
                map.DeleteActor(mob);
                actor.Slave.Remove(mob);
            }
        }
        #endregion

        public ActorMob SpawnMob(uint mobID, short x, short y, short moveRange, Actor master, Map map)
        {
            ActorMob mob = new ActorMob(mobID);
            mob.Name = "訓服：" + mob.Name;
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
            return mob;
        }

        public ActorShadow SpawnMob2(ActorMob mob, short x, short y, short moveRange, Actor master, Map map)
        {
            ActorPC pc = (ActorPC)master;
            ActorShadow actor = new ActorShadow(mob.MobID);
            actor.Name = "訓服：" + actor.Name;
            actor.MapID = pc.MapID;
            actor.X = pc.X;
            actor.Y = pc.Y;
            actor.Owner = pc;
            actor.Speed = pc.Speed;
            actor.Level = 1;
            //actor.BaseData.range = 1.0f;
            SagaMap.Mob.AIMode oldai = ((SagaMap.ActorEventHandlers.MobEventHandler)mob.e).AI.Mode;

            SagaMap.ActorEventHandlers.PetEventHandler eh = new SagaMap.ActorEventHandlers.PetEventHandler(actor);
            actor.e = eh;
            eh.AI.Mode = new SagaMap.Mob.AIMode(0);

            eh.AI.Mode.EventAttacking = oldai.EventAttacking;
            eh.AI.Mode.EventAttackingSkillRate = 10;

            eh.AI.MoveRange = 500;
            eh.AI.X_Ori = pc.X;
            eh.AI.Y_Ori = pc.Y;
            eh.AI.X_Spawn = pc.X;
            eh.AI.Y_Spawn = pc.Y;
            actor.sightRange = 1000;

            eh.AI.Master = pc;
            eh.AI.OnAttacked(pc, 1);

            map.RegisterActor(actor);
            actor.invisble = false;
            map.OnActorVisibilityChange(actor);
            map.SendVisibleActorsToActor(actor);
            eh.AI.Start();
            return actor;
        }


        public ActorShadow SummonMe(ActorPC sActor, byte level, Actor master)
        {
            ActorPC mpc = (ActorPC)master;
            ActorPC pc = MapClient.FromActorPC(sActor).Character;
            ActorShadow actor = new ActorShadow(pc);
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            actor.Name = pc.Name;
            actor.MapID = pc.MapID;
            //actor.X = SagaLib.Global.PosX8to16(thisarg.x, map.Width);
            //actor.Y = SagaLib.Global.PosY8to16(thisarg.y, map.Height);
            actor.X = master.X;
            actor.Y = master.Y;
            actor.MaxHP = (uint)(pc.MaxHP * (0.1f + 0.2f * level));
            actor.HP = actor.MaxHP;
            actor.Status.max_matk_skill += (short)(pc.Status.max_matk * 1.5f);
            actor.Status.min_matk_skill += (short)(pc.Status.min_matk * 1.5f);
            actor.Speed = pc.Speed;
            actor.BaseData.range = 1.5f;
            actor.Owner = mpc;

            ActorEventHandlers.PetEventHandler eh = new ActorEventHandlers.PetEventHandler(actor);
            actor.e = eh;

            eh.AI.Mode = new SagaMap.Mob.AIMode(0);

            foreach (uint i in pc.Skills.Keys)
            {
                eh.AI.Mode.EventAttacking.Add(i, 10);
            }
            foreach (uint i in pc.Skills2.Keys)
            {
                eh.AI.Mode.EventAttacking.Add(i, 10);
            }

            eh.AI.Mode.EventAttackingSkillRate = 50;
            eh.AI.Master = mpc;
            map.RegisterActor(actor);
            actor.invisble = false;
            map.OnActorVisibilityChange(actor);
            map.SendVisibleActorsToActor(actor);
            eh.AI.Start();

            return actor;
        }
    }
}