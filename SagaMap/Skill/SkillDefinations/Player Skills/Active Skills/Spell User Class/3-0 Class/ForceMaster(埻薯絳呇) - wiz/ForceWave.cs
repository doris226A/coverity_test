using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB;
using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.ForceMaster
{
    public class ForceWave : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            /*Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            if (map.CheckActorSkillInRange(dActor.X, dActor.Y, 200))
            {
                return -17;
            }*/
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*//创建设置型技能技能体
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //设定技能体位置
            actor.MapID = dActor.MapID;
            actor.X = dActor.X;
            actor.Y = dActor.Y;
            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            //在指定地图注册技能体Actor
            map.RegisterActor(actor);
            //设置Actor隐身属性为非
            actor.invisble = false;
            //广播隐身属性改变事件，以便让玩家看到技能体
            map.OnActorVisibilityChange(actor);
            //設置系
            actor.Stackable = false;
            //创建技能效果处理对象
            Activator timer = new Activator(sActor, dActor, actor, args, level);
            timer.Activate();*/

            ActorPC pc = (ActorPC)sActor;
            SagaMap.Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
            ActorShadow actor = new ActorShadow(90010045);
            //ActorShadow actor = new ActorShadow(pc);
            actor.MapID = pc.MapID;
            actor.Status.max_matk = pc.Status.max_matk;
            actor.Status.min_matk = pc.Status.min_matk;
            actor.X = pc.X;
            actor.Y = pc.Y;
            actor.Owner = pc;
            actor.MaxHP = actor.MaxHP + pc.MaxHP;
            actor.HP = actor.MaxHP;
            actor.Speed = pc.Speed;
            actor.BaseData.range = 1.0f;

            SagaMap.ActorEventHandlers.PetEventHandler eh = new SagaMap.ActorEventHandlers.PetEventHandler(actor);
            actor.e = eh;
            eh.AI.Mode = new SagaMap.Mob.AIMode(2);

            eh.AI.Mode.EventAttackingSkillRate = 100;
            eh.AI.Mode.EventMasterCombatSkillRate = 100;

            eh.AI.MoveRange = 500;
            eh.AI.X_Ori = pc.X;
            eh.AI.Y_Ori = pc.Y;
            eh.AI.X_Spawn = pc.X;
            eh.AI.Y_Spawn = pc.Y;
            actor.sightRange = 1000;
            eh.AI.Mode.EventAttacking.Add(3291, 100);
            eh.AI.Master = pc;
            eh.AI.OnAttacked(pc, 1);

            map.RegisterActor(actor);
            actor.invisble = false;
            map.OnActorVisibilityChange(actor);
            map.SendVisibleActorsToActor(actor);
            eh.AI.Start();

            eh.AI.Hate.TryAdd(dActor.ActorID, dActor.MaxHP);//二哈更改Hate
            int lifetime = 10000;
            SumDeathBuff skill = new SumDeathBuff(args.skill, actor, dActor, lifetime);
            SkillHandler.ApplyAddition(actor, skill);
        }
        #endregion

        public class SumDeathBuff : DefaultBuff
        {
            Actor sActor;
            Actor mob;
            Map map;
            public SumDeathBuff(SagaDB.Skill.Skill skill, Actor sActor, Actor mob, int lifetime)
                : base(skill, sActor, "SumDeath", lifetime, 3000)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.OnUpdate += this.TimerUpdate;
                this.sActor = sActor;
                this.mob = mob;
                this.map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {

            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                sActor.ClearTaskAddition();
                map.DeleteActor(sActor);
            }
            void TimerUpdate(Actor sActor, DefaultBuff skill)
            {
                if (sActor == null)
                {
                    sActor.ClearTaskAddition();
                    this.map.DeleteActor(sActor);
                }
                if (this.mob == null || this.mob.Buff.Dead)
                {
                    sActor.ClearTaskAddition();
                    this.map.DeleteActor(sActor);
                }

            }
        }
    }
}
