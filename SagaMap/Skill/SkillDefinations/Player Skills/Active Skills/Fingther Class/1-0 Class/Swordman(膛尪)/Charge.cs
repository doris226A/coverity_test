using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 彈飛
    /// </summary>
    public class Charge:ISkill
    {
        #region ISkill Members
        bool MobUse;
        public Charge()
        {
            this.MobUse = false;
        }
        public Charge(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        bool CheckPossible(Actor sActor)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND) || pc.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0;
            args.dActor = sActor.ActorID;
            if (MobUse)
            {
                level = 3;
            }
            if (CheckPossible(sActor))
            {
                args.type = ATTACK_TYPE.BLOW;
                factor = 1.0f + 0.3f * level;

                SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);

                //推开1格
                SkillHandler.Instance.PushBack(sActor, dActor, 2);
            }
        }

        #endregion
    }
}
