
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.Mob;
using SagaMap.ActorEventHandlers;
namespace SagaMap.Skill.SkillDefinations.Trader
{
    /// <summary>
    /// 召喚傭兵
    /// </summary>
    public class SumMercenary : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            uint[] MobID = { 0, 90010034, 90010035, 90010036 };
            SagaDB.Mob.MobData mobD = SagaDB.Mob.MobFactory.Instance.GetMobData(MobID[level]);
            
            ActorMob mob = map.SpawnMob(MobID[level], (short)(sActor.X + SagaLib.Global.Random.Next(1, 10))
                                    , (short)(sActor.Y + SagaLib.Global.Random.Next(1, 10))
                                    , 2500, sActor);
            ((ActorEventHandlers.MobEventHandler)mob.e).AI.Mode = new Mob.AIMode(0);//跟著主人戰鬥
            ActorPC pc = (ActorPC)sActor;
            //召喚物主人
            mob.Owner = pc;
            sActor.Slave.Add(mob);
            if (level < 3)
            {
                int lifetime = 300000;
                SumDeathBuff skill = new SumDeathBuff(args.skill, sActor, mob, lifetime);
                SkillHandler.ApplyAddition(sActor, skill);
            }*/
            uint[] MobID = { 0, 90010034, 90010035, 90010036 };
            ActorPC pc = (ActorPC)sActor;
            SagaMap.Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
            ActorShadow actor = new ActorShadow(MobID[level]);
            //ActorShadow actor = new ActorShadow(pc);
            actor.MapID = pc.MapID;
            actor.X = pc.X;
            actor.Y = pc.Y;
            actor.Owner = pc;
            actor.MaxHP = actor.MaxHP + pc.MaxHP;
            actor.HP = actor.MaxHP;
            actor.Speed = pc.Speed;
            actor.BaseData.range = 1.0f;

            SagaMap.ActorEventHandlers.PetEventHandler eh = new SagaMap.ActorEventHandlers.PetEventHandler(actor);
            actor.e = eh;
            eh.AI.Mode = new SagaMap.Mob.AIMode(0);

            switch (level)
            {
                case 1:
                    eh.AI.Mode.EventAttacking.Add(6606, 15);   //滅！
                    eh.AI.Mode.EventAttacking.Add(7652, 15);   //取消延遲
                    eh.AI.Mode.EventAttacking.Add(7574, 15);   //提升迴避率
                    eh.AI.Mode.EventAttacking.Add(7529, 30);   //利刃旋风

                    eh.AI.Mode.EventAttacking.Add(0, 20);   //空
                    eh.AI.Mode.EventMasterCombat.Add(0, 30);   //空

                    eh.AI.Mode.EventMasterCombat.Add(7652, 15);   //取消延遲
                    eh.AI.Mode.EventMasterCombat.Add(7574, 15);   //提升迴避率
                    break;
                case 2:
                    eh.AI.Mode.EventAttacking.Add(7516, 40);   //重击
                    eh.AI.Mode.EventAttacking.Add(7791, 10);   //有毒爆炸
                    eh.AI.Mode.EventAttacking.Add(6604, 20);   //全力支援
                    eh.AI.Mode.EventAttacking.Add(6603, 30);   //生命秘藥

                    eh.AI.Mode.EventAttacking.Add(0, 20);   //空
                    eh.AI.Mode.EventMasterCombat.Add(0, 30);   //空

                    eh.AI.Mode.EventMasterCombat.Add(6604, 15);   //全力支援
                    eh.AI.Mode.EventMasterCombat.Add(6603, 30);   //生命秘藥
                    break;
                case 3:
                    eh.AI.Mode.EventAttacking.Add(7742, 52);   //連續攻擊
                    eh.AI.Mode.EventAttacking.Add(6429, 20);   //取消延遲
                    eh.AI.Mode.EventAttacking.Add(7765, 8);   //憤怒
                    eh.AI.Mode.EventAttacking.Add(6605, 20);   //武士的覺悟

                    eh.AI.Mode.EventAttacking.Add(0, 20);   //空
                    eh.AI.Mode.EventMasterCombat.Add(0, 30);   //空

                    eh.AI.Mode.EventMasterCombat.Add(6429, 15);   //取消延遲
                    break;
            }
            eh.AI.Mode.EventAttackingSkillRate = 10;
            eh.AI.Mode.EventMasterCombatSkillRate = 5;

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
            sActor.Slave.Add(actor);

            int lifetime = 300000;
            SumDeathBuff skill = new SumDeathBuff(args.skill, sActor, actor, lifetime);
            SkillHandler.ApplyAddition(sActor, skill);
        }
        #endregion

        public class SumDeathBuff : DefaultBuff
        {
            Actor sActor;
            ActorShadow mob;
            public SumDeathBuff(SagaDB.Skill.Skill skill, Actor sActor, ActorShadow mob, int lifetime)
                : base(skill, sActor, "SumDeath", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.sActor = sActor;
                this.mob = mob;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {

            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                sActor.Slave.Remove(mob);
                mob.ClearTaskAddition();
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                map.DeleteActor(mob);
            }
        }
    }
}