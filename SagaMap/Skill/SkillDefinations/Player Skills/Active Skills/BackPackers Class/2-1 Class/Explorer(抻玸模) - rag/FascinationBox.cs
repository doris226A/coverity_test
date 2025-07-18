﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.ActorEventHandlers;
using SagaLib;
using SagaMap.Skill.Additions.Global;
using static SagaMap.Skill.SkillHandler;
using SagaMap.Scripting;
using SagaMap.Mob;

namespace SagaMap.Skill.SkillDefinations.Explorer
{
    /// <summary>
    /// 魅惑之箱（ファシネイションボックス）
    /// </summary>
    public class FascinationBox : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        SkillArg argQ;

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            float factor = new float[] { 0, 2.0f, 3.5f, 5.0f }[level];
            if (sActor.Slave.Count > 4)
            {
                Map mapA = Manager.MapManager.Instance.GetMap(sActor.MapID);
                EffectArg arg2 = new EffectArg();
                arg2.effectID = 5345;
                arg2.actorID = sActor.Slave[0].ActorID;
                mapA.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, sActor.Slave[0], true);

                List<Actor> affectedA = mapA.GetActorsArea(sActor.Slave[0], 200, false);
                List<Actor> realAffected = new List<Actor>();
                foreach (Actor act in affectedA)
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        //realAffected.Add(act);
                        int demage = SkillHandler.Instance.CalcDamage(false, sActor.Slave[0], act, args, DefType.MDef, Elements.Neutral, 0, factor);
                        SkillHandler.Instance.FixAttack(sActor.Slave[0], act, args, SagaLib.Elements.Neutral, demage);
                        SkillHandler.Instance.ShowVessel(act, demage);
                    }
                }
                realAffected.Add(sActor.Slave[0]);
                mapA.DeleteActor(sActor.Slave[0]);
                sActor.Slave[0].HP = 0;
                sActor.Slave[0].e.OnDie();
                sActor.Slave.Remove(sActor.Slave[0]);
            }

            ActorMob m = new ActorMob();
            m = map.SpawnMob(90010037, dActor.X, dActor.Y, 0, sActor);
            //m.BaseData.mobType = SagaDB.Mob.MobType.NONE_NOTOUCH;

            argQ = args;
            ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)m.e;

            m.Owner = sActor;//召喚物主人
            m.MaxHP = 99999;
            m.HP = 99999;
            m.Status.min_matk = 2000;
            m.Status.max_matk = 2800;
            sActor.Slave.Add(m);
            int LifeTime = 4000;
            SkillArg arg = new SkillArg();
            arg = args;
            Activator timer = new Activator(sActor, m, arg, level, LifeTime, factor);
            timer.Activate();

            int damage = 2500 + 500 * level;
            List<Actor> affected = map.GetActorsArea(sActor, 250, false);
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    SkillHandler.Instance.AttractMob(m, act, damage);
                }
            }


        }
        #endregion

        #region Timer
        public class Activator : MultiRunTask
        {
            public Actor sActor;
            public ActorMob actor;
            public SkillArg skill;
            public Map map;
            public int lifetime;
            public int level;
            public bool OneTimes;
            public int State = 0;
            public float factor;
            public event ProcSkillHandler ProcSkill;
            public delegate void ProcSkillHandler(Actor sActor, Actor mActor, Actor actor, SkillArg args, Map map, int level, float factor);
            public event OnTimerHandler OnTimer;
            public delegate void OnTimerHandler(Activator timer);

            public Activator(Actor _sActor, ActorMob _dActor, SkillArg _args, byte level, int lifetime, float _factor)
            {
                sActor = _sActor;
                actor = _dActor;
                skill = _args;
                this.dueTime = 0;
                this.period = 1;
                this.lifetime = lifetime;
                this.level = level;
                this.factor = _factor;
                map = Manager.MapManager.Instance.GetMap(sActor.MapID);

            }
            public override void CallBack()
            {
                //同步鎖，表示之後的代碼是執行緒安全的，也就是，不允許被第二個執行緒同時訪問
                //测试去除技能同步锁
                //ClientManager.EnterCriticalArea();
                try
                {

                    if (lifetime > 0)
                    {
                        if (OnTimer != null)
                        {
                            OnTimer.Invoke(this);
                        }
                        lifetime -= this.period;
                    }
                    else
                    {
                        Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                        EffectArg arg2 = new EffectArg();
                        arg2.effectID = 5345;
                        arg2.actorID = actor.ActorID;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, actor, true);

                        List<Actor> affected = map.GetActorsArea(actor, 200, false);
                        List<Actor> realAffected = new List<Actor>();
                        foreach (Actor act in affected)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(actor, act))
                            {
                                //realAffected.Add(act);
                                int demage = SkillHandler.Instance.CalcDamage(false, actor, act, skill, DefType.MDef, Elements.Neutral, 0, factor);
                                SkillHandler.Instance.FixAttack(actor, act, skill, SagaLib.Elements.Neutral, demage);
                                SkillHandler.Instance.ShowVessel(act, demage);
                            }
                        }
                        this.Deactivate();
                        map.DeleteActor(actor);
                        actor.HP = 0;
                        actor.e.OnDie();
                        //0121 error修正
                        if (sActor.Slave != null)
                            sActor.Slave.Remove(actor);
                    }

                    if (actor.HP < actor.MaxHP)
                    {
                        Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                        EffectArg arg2 = new EffectArg();
                        arg2.effectID = 5345;
                        arg2.actorID = actor.ActorID;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, actor, true);

                        List<Actor> affected = map.GetActorsArea(actor, 200, false);
                        List<Actor> realAffected = new List<Actor>();
                        foreach (Actor act in affected)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                            {
                                //realAffected.Add(act);
                                int demage = SkillHandler.Instance.CalcDamage(false, actor, act, skill, DefType.MDef, Elements.Neutral, 0, factor);
                                SkillHandler.Instance.FixAttack(actor, act, skill, SagaLib.Elements.Neutral, demage);
                                SkillHandler.Instance.ShowVessel(act, demage);
                            }
                        }
                        realAffected.Add(actor);
                        this.Deactivate();
                        map.DeleteActor(actor);
                        actor.HP = 0;
                        actor.e.OnDie();
                        //0121 error修正
                        if (sActor.Slave != null)
                            sActor.Slave.Remove(actor);
                    }

                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
                //解開同步鎖
                //测试去除技能同步锁
                //ClientManager.LeaveCriticalArea();
            }
        }
        #endregion
    }
}
