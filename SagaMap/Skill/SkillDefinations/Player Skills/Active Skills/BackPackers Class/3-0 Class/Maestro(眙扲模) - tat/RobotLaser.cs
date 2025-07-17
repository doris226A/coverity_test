
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Maestro
{
    /// <summary>
    /// レールガン
    /// </summary>
    public class RobotLaser : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(sActor);
            if (!sActor.Status.Additions.ContainsKey("RobotLaser"))
            {
                if (pet == null)
                {
                    return -54;//需回傳"需裝備寵物"
                }
                if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    if (sActor.type == ActorType.PC)
                    {
                        uint itemID = 10122000;//レールガン専用弾
                        ActorPC pc = sActor as ActorPC;
                        if (SkillHandler.Instance.CountItem(pc, itemID) > 0)
                        {
                            SkillHandler.Instance.TakeItem(pc, itemID, 1);
                            return 0;
                        }
                        else
                        {
                            return -2;
                        }
                    }
                    return 0;
                }
                return -54;//需回傳"需裝備寵物"
            }
            else return -30;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 8f * level;
            int lifetime = 35000 - 5000 * level;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "RobotLaser", lifetime);
            SkillHandler.ApplyAddition(sActor, skill);
        }
        #endregion
    }
}