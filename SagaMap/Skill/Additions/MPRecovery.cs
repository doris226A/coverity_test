using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaDB.Skill;
using SagaLib;

namespace SagaMap.Skill.Additions.Global
{
    public class MPRecovery : DefaultBuff
    {
        private bool isMarionette = false;
        public MPRecovery(SagaDB.Skill.Skill skill, Actor actor, int lifetime, int period)
            : base(skill, actor, "MPRecovery", lifetime, period)
        {
            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
        }
        public MPRecovery(SagaDB.Skill.Skill skill, Actor actor, int lifetime, int period, bool isMarionette)
            : base(skill, actor, isMarionette ? "Marionette_MPRecovery" : "MPRecovery", lifetime, period)
        {
            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
            this.isMarionette = isMarionette;
        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {
            // Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            // map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            //Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            //map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                if (!actor.Buff.NoRegen)
                {
                    Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                    uint mpadd = 0;
                    mpadd = (uint)Math.Max(actor.MaxMP * actor.Status.mp_recover / 2000, 1);
                    actor.MP += mpadd;
                    if (actor.MP > actor.MaxMP)
                    {
                        actor.MP = actor.MaxMP;
                    }
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
                }
            }
            else
            {
                if (!actor.Buff.NoRegen)
                {
                    Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                    uint mpadd = 0;
                    if (isMarionette)
                    {
                        ActorPC pc = (ActorPC)actor;
                        if (pc.Marionette == null)
                        {
                            this.AdditionEnd();
                        }
                        /*mpadd = (uint)(pc.MaxMP * (100 + ((pc.Mag +
                         pc.Status.mag_item + pc.Status.mag_mario +
                         pc.Status.mag_rev) / 3)) / 2000);*/
                        mpadd = (uint)(actor.MaxMP * actor.Status.mp_recover / 2000);
                    }
                    else
                    {
                        ActorPC pc = (ActorPC)actor;
                        /*mpadd = mpadd = (uint)((pc.MaxMP * (100 + ((pc.Mag +
                         pc.Status.mag_item + pc.Status.mag_mario +
                         pc.Status.mag_rev) / 3)) / 2000) + pc.Status.mp_recover_skill);*/
                        mpadd = (uint)(actor.MaxMP * actor.Status.mp_recover / 2000);
                    }
                    actor.MP += mpadd;
                    if (actor.MP > actor.MaxMP)
                    {
                        actor.MP = actor.MaxMP;
                    }
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
                }
            }
        }
    }
}
