using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.SkillDefinations;
namespace SagaMap.Skill.Additions.Global
{
    /// <summary>
    /// 溫泉
    /// </summary>
    public class HotSpring : DefaultBuff
    {
        public HotSpring(SagaDB.Skill.Skill skill, Actor actor, int lifetime, int period)
            : base(skill, actor, "HotSpring", lifetime, period, true)
        {
            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {
            actor.Buff.EffectOfHotSpring = true;
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            actor.Buff.EffectOfHotSpring = false;
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
            if (!actor.Buff.NoRegen)
            {
                try
                {
                    if (actor.type == ActorType.PC)
                    {
                        ActorPC pc = (ActorPC)actor;
                        Map map = Manager.MapManager.Instance.GetMap(actor.MapID);

                        pc.HP += (uint)(pc.MaxHP * (100 + ((pc.Vit + pc.Status.vit_item + pc.Status.vit_rev) / 3)) / 2000);
                        if (pc.HP > pc.MaxHP)
                            pc.HP = pc.MaxHP;

                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, pc, true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }

        }
    }
}