using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 劍之達人
    /// </summary>
    public class TOverWork : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
            /*  if (CheckPossible(pc))
                  return 0;
              else
                  return -5;*/
        }

        bool CheckPossible(Actor sActor)
        {
                return true;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
           // args.dActor = 0;//不显示效果
                            // Actor realdActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
                            //  if (CheckPossible(realdActor))
                            // {
            if (dActor.type == ActorType.PC || dActor.type == ActorType.PARTNER)
            {
                int life = 15000;
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "WeaponDC", life);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                skill.OnCheckValid += this.ValidCheck;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }

        void ValidCheck(ActorPC pc, Actor dActor, out int result)
        {
            result = TryCast(pc, dActor, null);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.aspd_skill_perc += (float)(1.5f);

            actor.Buff.OverWork = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            float raspd_skill_perc_restore = (float)(1.5f);
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
