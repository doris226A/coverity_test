using System;
using SagaDB.Actor;
using SagaLib;

namespace SagaMap.Skill.Additions.Global
{
    public class ExpAllBouns : DefaultBuff
    {
        public ExpAllBouns(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime)
            : base(skill, actor, name, lifetime)
        {
            /*if (SkillHandler.Instance.isBossMob(actor))
                this.Enabled = false;*/


            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
        }

        public ExpAllBouns(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime, int period)
            : base(skill, actor, name, lifetime, period)
        {
            /*if (SkillHandler.Instance.isBossMob(actor))
                this.Enabled = false;*/

            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {

        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            /*if (skill.Variable.ContainsKey("EXP_ALL_BOUNS"))
                skill.Variable.Remove("EXP_ALL_BOUNS");*/

            var startTime = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var pc = actor as ActorPC;
            if (startTime >= pc.TimerItems["EXP_ALL_BOUNS"].EndTime)
            {
                if (skill.Variable.ContainsKey("EXP_ALL_BOUNS"))
                {
                    MapServer.charDB.DeleteTimerItem(pc.CharID, "EXP_ALL_BOUNS");
                    pc.TimerItems.Remove("EXP_ALL_BOUNS");
                    skill.Variable.Remove("EXP_ALL_BOUNS");
                    Skill.SkillHandler.RemoveAddition(actor, "ExpAllBonus");
                    Skill.SkillHandler.SendSystemMessage(pc, "【經驗戒指】效果消失了！");
                }
            }
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
        }
    }
}
