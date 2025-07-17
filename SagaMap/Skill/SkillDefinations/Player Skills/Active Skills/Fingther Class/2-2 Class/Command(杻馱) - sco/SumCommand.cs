
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 應援要請（応援要請）
    /// </summary>
    public class SumCommand : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            uint itemID = 10053301;//訊號彈
            if (SkillHandler.Instance.CountItem(pc, itemID) > 0)
            {
                SkillHandler.Instance.TakeItem(pc, itemID, 1);
                return 0;
            }
            return -2;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*int lifetime = 10000;
            List<Actor> sumMob = new List<Actor>();
            List<uint> sumMobID = new List<uint>();
            switch (level)
            {
                case 1:
                    sumMobID.Add(19070401);
                    break;
                case 2:
                    sumMobID.Add(19070501);
                    break;
                case 3:
                    sumMobID.Add(19070401);
                    sumMobID.Add(19070501);
                    break;
            }
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            foreach (uint i in sumMobID)
            {
                sumMob.Add(map.SpawnMob(i, (short)(sActor.X + SagaLib.Global.Random.Next(-100, 100)), (short)(sActor.Y + SagaLib.Global.Random.Next(-100, 100)), 2500, sActor));
            }
            foreach (Actor j in sumMob)
            {
                SagaMap.ActorEventHandlers.MobEventHandler meh = (SagaMap.ActorEventHandlers.MobEventHandler)j.e;
                meh.AI.AttackActor(dActor.ActorID);
            }
            SumCommandBuff skill = new SumCommandBuff(args.skill, sActor, sumMob, lifetime);
            SkillHandler.ApplyAddition(dActor, skill);*/

            int lifetime = 10000;
            List<Actor> sumMob = new List<Actor>();
            List<uint> sumMobID = new List<uint>();
            switch (level)
            {
                case 1:
                    sumMobID.Add(19070401);
                    break;
                case 2:
                    sumMobID.Add(19070501);
                    break;
                case 3:
                    sumMobID.Add(19070401);
                    sumMobID.Add(19070501);
                    break;
            }
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            foreach (uint i in sumMobID)
            {
                sumMob.Add(SpawnMob2(i, sActor.X, sActor.Y, 2500, sActor, map));
            }
            foreach (Actor j in sumMob)
            {
                SagaMap.ActorEventHandlers.PetEventHandler meh = (SagaMap.ActorEventHandlers.PetEventHandler)j.e;
                meh.AI.OnAttacked(dActor, 10);
            }
            SumCommandBuff skill = new SumCommandBuff(args.skill, sActor, sumMob, lifetime);
            SkillHandler.ApplyAddition(sActor, skill);
        }
        public class SumCommandBuff : DefaultBuff
        {
            List<Actor> sumMob;
            public SumCommandBuff(SagaDB.Skill.Skill skill, Actor actor, List<Actor> sumMob, int lifetime)
                : base(skill, actor, "SumCommand", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.sumMob = sumMob;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                foreach (Actor act in sumMob)
                {
                    act.ClearTaskAddition();
                    map.DeleteActor(act);
                }
            }
        }

        public ActorShadow SpawnMob2(uint mob, short x, short y, short moveRange, Actor master, Map map)
        {
            ActorPC pc = (ActorPC)master;
            ActorShadow actor = new ActorShadow(mob);
            actor.MapID = pc.MapID;
            actor.X = pc.X;
            actor.Y = pc.Y;
            actor.Owner = pc;
            actor.Speed = pc.Speed;

            actor.BaseData.level = pc.Level;
            actor.Status.attackType = pc.Status.attackType;
            actor.Status.aspd = pc.Status.aspd;
            actor.Status.def = pc.Status.def;
            actor.Status.def_add = pc.Status.def_add;
            actor.Status.mdef = pc.Status.mdef;
            actor.Status.mdef_add = pc.Status.mdef_add;
            actor.Status.min_atk1 = pc.Status.min_atk1;
            actor.Status.max_atk1 = pc.Status.max_atk1;
            actor.Status.min_atk2 = pc.Status.min_atk2;
            actor.Status.max_atk2 = pc.Status.max_atk2;
            actor.Status.min_atk3 = pc.Status.min_atk3;
            actor.Status.max_atk3 = pc.Status.max_atk3;
            actor.Status.min_matk = pc.Status.min_matk;
            actor.Status.max_matk = pc.Status.max_matk;

            actor.Status.hit_melee = pc.Status.hit_melee;
            actor.Status.hit_ranged = pc.Status.hit_ranged;
            actor.Status.avoid_melee = pc.Status.avoid_melee;
            actor.Status.avoid_ranged = pc.Status.avoid_ranged;
            actor.MaxHP = pc.MaxHP;
            actor.HP = pc.MaxHP;


            SagaMap.ActorEventHandlers.PetEventHandler eh = new SagaMap.ActorEventHandlers.PetEventHandler(actor);
            actor.e = eh;
            if (mob == 19070401)
            {
                eh.AI.Mode = new SagaMap.Mob.AIMode(2);
            }
            else
            {
                eh.AI.Mode = new SagaMap.Mob.AIMode(1);
            }


            eh.AI.MoveRange = 500;
            eh.AI.X_Ori = pc.X;
            eh.AI.Y_Ori = pc.Y;
            eh.AI.X_Spawn = pc.X;
            eh.AI.Y_Spawn = pc.Y;
            actor.sightRange = 1000;

            eh.AI.Master = pc;

            map.RegisterActor(actor);
            actor.invisble = false;
            map.OnActorVisibilityChange(actor);
            map.SendVisibleActorsToActor(actor);
            eh.AI.Start();
            return actor;
        }
        #endregion
    }
}
