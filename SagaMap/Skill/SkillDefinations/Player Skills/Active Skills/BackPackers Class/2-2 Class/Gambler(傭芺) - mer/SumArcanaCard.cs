
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.ActorEventHandlers;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 艾卡卡（アルカナカード）
    /// </summary>
    public class SumArcanaCard : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            uint itemID = 10035700;
            if (SkillHandler.Instance.CountItem(sActor, itemID) > 0)
            {
                SkillHandler.Instance.TakeItem(sActor, itemID, 1);
                return 0;
            }
            return -2;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //uint[] NextSkillIDs = { 2432, 2431, 2434, 2435 };
            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(NextSkillIDs[SagaLib.Global.Random.Next (0, NextSkillIDs.Length - 1)], level, 0));

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            short x = SagaLib.Global.PosX8to16(args.x, map.Width);
            short y = SagaLib.Global.PosY8to16(args.y, map.Height);
            switch (SagaLib.Global.Random.Next(1, 4))
            {
                case 1:
                    int lifetime = 6000;
                    uint MobID = 10100101;//紅心艾卡納
                    ActorMob mob = map.SpawnMob(MobID, x, y, 2500, sActor);
                    MobEventHandler mh = (MobEventHandler)mob.e;
                    mh.AI.Mode = new SagaMap.Mob.AIMode(1);
                    SumArcanaCardBuff skill = new SumArcanaCardBuff(args.skill, sActor, mob, lifetime);
                    SkillHandler.ApplyAddition(sActor, skill);
                    break;
                case 2:
                    int lifetimeA = 8000;
                    uint MobIDA = 10310006;//艾卡納J牌
                    ActorMob mobA = map.SpawnMob(MobIDA, x, y, 2500, sActor);
                    MobEventHandler mhA = (MobEventHandler)mobA.e;
                    mhA.AI.Mode = new SagaMap.Mob.AIMode(1);
                    SumArcanaCardBuff skillA = new SumArcanaCardBuff(args.skill, sActor, mobA, lifetimeA);
                    SkillHandler.ApplyAddition(sActor, skillA);
                    break;
                case 3:
                    //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2436, level, 0));
                    int lifetimeB = 5000;
                    uint MobIDB = 10320006;//艾卡納王后
                    ActorMob mobB = map.SpawnMob(MobIDB, x, y, 2500, sActor);
                    MobEventHandler mhB = (MobEventHandler)mobB.e;
                    mhB.AI.Mode = new SagaMap.Mob.AIMode(0);
                    mhB.AI.CastSkill(2436, level, sActor);

                    SumArcanaCardBuff skillB = new SumArcanaCardBuff(args.skill, sActor, mobB, lifetimeB);
                    SkillHandler.ApplyAddition(sActor, skillB);
                    break;
                case 4:
                    //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2437, level, 0));

                    int lifetimeC = 12000;
                    uint MobIDC = 10330006;//艾卡納王
                    ActorMob mobC = map.SpawnMob(MobIDC, x, y, 2500, sActor);
                    MobEventHandler mhC = (MobEventHandler)mobC.e;
                    mhC.AI.Mode = new SagaMap.Mob.AIMode(0);
                    mhC.AI.CastSkill(2437, level, mobC.X, mobC.Y);

                    SumArcanaCardBuff skillC = new SumArcanaCardBuff(args.skill, sActor, mobC, lifetimeC);
                    SkillHandler.ApplyAddition(sActor, skillC);

                    Activator timer = new Activator(mobC);
                    timer.Activate();
                    break;
            }
        }

        public class SumArcanaCardBuff : DefaultBuff
        {
            ActorMob mob;
            public SumArcanaCardBuff(SagaDB.Skill.Skill skill, Actor actor, ActorMob mob, int lifetime)
                : base(skill, actor, "SumArcanaCard" + SagaLib.Global.Random.Next(0, 99).ToString(), lifetime)
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
            }
        }
        #endregion

        private class Activator : MultiRunTask
        {
            ActorMob MobActor;
            public Activator(ActorMob Actor)
            {
                this.MobActor = Actor;
                this.period = 2000;
                this.dueTime = 6000;
            }

            public override void CallBack()
            {
                try
                {
                    MobEventHandler mhC = (MobEventHandler)MobActor.e;
                    mhC.AI.CastSkill(2437, 1, MobActor.X, MobActor.Y);
                    this.Deactivate();
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                    this.Deactivate();
                }
            }
        }
    }
}