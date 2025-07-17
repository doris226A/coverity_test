
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.ActorEventHandlers;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Alchemist
{
    /// <summary>
    /// 化工廠（ケミカルプラント）
    /// </summary>
    public class SumChemicalPlant : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ActorMob mob = map.SpawnMob(10580004, (short)(sActor.X + SagaLib.Global.Random.Next(1, 10)), (short)(sActor.Y + SagaLib.Global.Random.Next(1, 10)), 2500, sActor);
            mob.Status.max_atk1 = (ushort)(sActor.Status.max_atk1 * 0.16 * level);
            mob.Status.max_atk2 = (ushort)(sActor.Status.max_atk2 * 0.16 * level);
            mob.Status.max_atk3 = (ushort)(sActor.Status.max_atk3 * 0.16 * level);
            mob.Status.min_atk1 = (ushort)(sActor.Status.min_atk1 * 0.16 * level);
            mob.Status.min_atk2 = (ushort)(sActor.Status.min_atk2 * 0.16 * level);
            mob.Status.min_atk3 = (ushort)(sActor.Status.min_atk3 * 0.16 * level);
            mob.Status.hit_melee = (ushort)(sActor.Status.hit_melee * 0.16 * level);
            mob.Status.hit_ranged = (ushort)(sActor.Status.hit_ranged * 0.16 * level);
            mob.Owner = sActor;
            sActor.Slave.Add(mob);

            MobEventHandler meh = (MobEventHandler)mob.e;
            meh.AI.AttackActor(dActor.ActorID);

            SumDeathBuff skill = new SumDeathBuff(args.skill, sActor, mob, 5000);
            SkillHandler.ApplyAddition(sActor, skill);

            AutoCastInfo aci = new AutoCastInfo();
            aci.skillID = 3344;//化工廠[接續技能]
            aci.level = level;
            aci.delay = 0;
            args.autoCast.Add(aci);
        }
        #endregion

        public class SumDeathBuff : DefaultBuff
        {
            Actor sActor;
            ActorMob mob;
            public SumDeathBuff(SagaDB.Skill.Skill skill, Actor sActor, ActorMob mob, int lifetime)
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

        //#region Timer
        //private class Activator : MultiRunTask
        //{
        //    Actor sActor;
        //    ActorMob mob;
        //    SkillArg skill;
        //    float factor;
        //    Map map;
        //    public Activator(Actor _sActor, ActorMob mob, SkillArg _args, byte level)
        //    {
        //        sActor = _sActor;
        //        this.mob = mob;
        //        skill = _args.Clone();
        //        factor = 0.5f + 1.5f * level;
        //        this.dueTime = 2500;
        //        this.period = 0;

        //    }
        //    public override void CallBack()
        //    {
        //        //同步鎖，表示之後的代碼是執行緒安全的，也就是，不允許被第二個執行緒同時訪問
        //        ClientManager.EnterCriticalArea();
        //        try
        //        {

        //            this.Deactivate();
        //            map.DeleteActor(mob);
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.ShowError(ex);
        //        }
        //        //解開同步鎖
        //        ClientManager.LeaveCriticalArea();
        //    }
        //}
        //#endregion
    }
}



