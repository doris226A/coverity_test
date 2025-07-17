using System;
using SagaDB.Actor;
using SagaLib;

namespace SagaMap.Skill.Additions.Global
{
    public class ItemDropBouns : DefaultBuff
    {
        public ItemDropBouns(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime)
            : base(skill, actor, name, lifetime)
        {
            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
        }

        public ItemDropBouns(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime, int period)
            : base(skill, actor, name, lifetime, period)
        {
            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {

        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            var startTime = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var pc = actor as ActorPC;
            if (startTime >= pc.TimerItems["ITEM_DROP_BOUNS"].EndTime)
            {
                if (skill.Variable.ContainsKey("ITEM_DROP_BOUNS"))
                {
                    MapServer.charDB.DeleteTimerItem(pc.CharID, "ITEM_DROP_BOUNS");
                    pc.TimerItems.Remove("ITEM_DROP_BOUNS");
                    skill.Variable.Remove("ITEM_DROP_BOUNS");
                    Skill.SkillHandler.RemoveAddition(actor, "ItemDropBouns");
                    Skill.SkillHandler.SendSystemMessage(pc, "【富豪的證明】效果消失了！");
                }
            }
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
        }
    }
}
