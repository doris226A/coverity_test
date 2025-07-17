using System;
using SagaDB.Actor;
using SagaLib;

namespace SagaMap.Skill.Additions.Global
{
    public class ExpJobBouns : DefaultBuff
    {
        public ExpJobBouns(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime)
            : base(skill, actor, name, lifetime)
        {
            /*if (SkillHandler.Instance.isBossMob(actor))
                this.Enabled = false;*/


            this.OnAdditionStart += this.StartEvent;
            this.OnAdditionEnd += this.EndEvent;
            this.OnUpdate += this.TimerUpdate;
        }

        public ExpJobBouns(SagaDB.Skill.Skill skill, Actor actor, string name, int lifetime, int period)
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
            if (startTime >= pc.TimerItems["EXP_JOB_BOUNS"].EndTime)
            {
                if (skill.Variable.ContainsKey("EXP_JOB_BOUNS"))
                {
                    MapServer.charDB.DeleteTimerItem(pc.CharID, "EXP_JOB_BOUNS");
                    pc.TimerItems.Remove("EXP_JOB_BOUNS");
                    skill.Variable.Remove("EXP_JOB_BOUNS");
                    Skill.SkillHandler.RemoveAddition(actor, "ExpJobBouns");
                    Skill.SkillHandler.SendSystemMessage(pc, "【職業經驗戒指】效果消失了！");
                }
            }
        }

        void TimerUpdate(Actor actor, DefaultBuff skill)
        {
        }
    }
}
