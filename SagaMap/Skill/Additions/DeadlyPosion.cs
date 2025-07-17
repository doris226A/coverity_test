using System;
using SagaDB.Actor;
using SagaLib;

namespace SagaMap.Skill.Additions.Global
{
    public class DeadlyPosion : DefaultBuff
    {
        public DeadlyPosion(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime)
            : base(skill, actor, name, lifetime)
        {
            if (SkillHandler.Instance.isBossMob(actor))
                this.Enabled = false;

            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
        }

        public DeadlyPosion(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime, int period)
            : base(skill, actor, name, lifetime, period)
        {
            if (SkillHandler.Instance.isBossMob(actor))
                this.Enabled = false;

            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {
            actor.Buff.DeadlyPoison = true;
            if (actor.type == ActorType.PC)
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            else
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, false);
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            actor.Buff.DeadlyPoison = false;
            if (actor.type == ActorType.PC)
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            else
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, false);
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
            //测试去除技能同步锁ClientManager.EnterCriticalArea();
            try
            {
                if (actor.HP > 0 && !actor.Buff.Dead)
                {
                    Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                    int amount = (int)(actor.MaxHP / 100) * 10;
                    if (amount < 1)
                        amount = 1;
                    if (actor.HP > amount)
                        actor.HP = (uint)(actor.HP - amount);
                    else
                        actor.HP = 1;
                    actor.e.OnHPMPSPUpdate(actor);
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
            //测试去除技能同步锁ClientManager.LeaveCriticalArea();
        }
    }
}
