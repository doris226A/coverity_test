
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Mob;
using SagaMap.ActorEventHandlers;
namespace SagaMap.Skill.SkillDefinations.Blacksmith
{
    /// <summary>
    /// 回來！（戻れ！）
    /// </summary>
    public class PetBack : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (sActor.Slave.Count != 0)
            {
                foreach (Actor i in sActor.Slave)
                {
                    if (i.type == ActorType.MOB)
                    {
                        MobEventHandler meh = (MobEventHandler)i.e;
                        meh.AI.StopAttacking();
                    }
                    if (i.type == ActorType.SHADOW)
                    {
                        PetEventHandler meh = (PetEventHandler)i.e;
                        meh.AI.StopAttacking();
                        meh.AI.AttackActor(dActor.ActorID);
                    }
                }
            }
            ActorPartner par = SkillHandler.Instance.GetPartner(sActor);
            if (par == null)
            {
                return;
            }
            PartnerEventHandler peh = (PartnerEventHandler)par.e;
            peh.AI.StopAttacking();

            /*MobAI ai = SkillHandler.Instance.GetMobAI(sActor);
            if (ai == null)
            {
                return;
            }
            ai.StopAttacking();*/
        }
        #endregion
    }
}