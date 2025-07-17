using System;
using SagaDB.Actor;
using SagaLib;

namespace SagaMap.Skill.Additions.Global
{
    public class PartnerGetExp : DefaultBuff
    {
        public PartnerGetExp(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime)
            : base(skill, actor, name, lifetime)
        {

        }

        public PartnerGetExp(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime, int period)
            : base(skill, actor, name, lifetime, period)
        {
            var pc = actor as ActorPC;
            pc.TInt["DogHead"] = 1;
            this.OnAdditionEnd += this.EndEvent;
            this.OnAdditionStart += this.StartEvent;
            this.OnUpdate += this.TimerUpdate;

        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            var startTime = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var pc = actor as ActorPC;
            if (startTime >= pc.TimerItems["PARTNER_GET_EXP"].EndTime)
            {
                if (skill.Variable.ContainsKey("MasterShareExpFromPartner"))
                {
                    MapServer.charDB.DeleteTimerItem(pc.CharID, "PARTNER_GET_EXP");
                    pc.TimerItems.Remove("PARTNER_GET_EXP");
                    pc.TInt["DogHead"] = 0;
                    skill.Variable.Remove("MasterShareExpFromPartner");
                    Skill.SkillHandler.RemoveAddition(actor, "PartnerGetExp");
                    Skill.SkillHandler.SendSystemMessage(pc, "【寵物的協助】效果消失了！");
                }
            }
        }
        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
        }
    }
}
