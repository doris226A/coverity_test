
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Breeder
{
    /// <summary>
    ///  ペットディレイキャンセル（ペットディレイキャンセル）寵物攻速上升
    /// </summary>
    public class PetDelayCancel : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            ActorPartner partner = SkillHandler.Instance.GetPartner(sActor);
            if (partner == null)
            {
                return -12;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 20000;
            //ActorPet p = SkillHandler.Instance.GetPet(sActor);

            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Pet != null && pc.Pet.Ride)
                {
                    DefaultBuff skill = new DefaultBuff(args.skill, sActor, "WeaponDC", lifetime);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(sActor, skill);
                    return;
                }
            }
            ActorPartner partner = SkillHandler.Instance.GetPartner(sActor);
            if (partner != null)
            {
                DefaultBuff skill = new DefaultBuff(args.skill, partner, "WeaponDC", lifetime);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(partner, skill);
                Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(partner, 4133);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.aspd_skill_perc += 1.5f;

            actor.Buff.DelayCancel = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            float raspd_skill_perc_restore = 2.5f;
            if (actor.Status.aspd_skill_perc > raspd_skill_perc_restore)
            {
                actor.Status.aspd_skill_perc -= raspd_skill_perc_restore;
            }
            else
            {
                actor.Status.aspd_skill_perc = 0;
            }

            actor.Buff.DelayCancel = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        #endregion
    }
}
