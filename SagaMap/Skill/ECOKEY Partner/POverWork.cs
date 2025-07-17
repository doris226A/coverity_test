using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 迅捷
    /// </summary>
    public class POverWork : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (dActor.type == ActorType.PC || dActor.type == ActorType.PARTNER)
            {
                int life = 10000;
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "WeaponDC", life);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.aspd_skill_perc += (float)(2.0f);

            actor.Buff.OverWork = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            float raspd_skill_perc_restore = (float)(2.0f);
            if (actor.Status.aspd_skill_perc > raspd_skill_perc_restore + 1)
            {
                actor.Status.aspd_skill_perc -= raspd_skill_perc_restore;
            }
            else
            {
                actor.Status.aspd_skill_perc = 1;
            }

            actor.Buff.OverWork = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        #endregion
    }
}
