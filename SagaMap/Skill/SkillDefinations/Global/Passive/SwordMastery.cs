using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    public class SwordMastery : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = false;
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND))
                {
                    if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.SWORD
                        || pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.RAPIER
                        )
                    {
                        active = true;
                    }
                }
                DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "SwordMastery", active);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(sActor, skill);
            }
        }

        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            int value;
            value = skill.skill.Level * 5;
            if (skill.skill.Level == 5)
                value += 5;


            if (skill.Variable.ContainsKey("SwordMastery_max_atk1"))
                skill.Variable.Remove("SwordMastery_max_atk1");
            skill.Variable.Add("SwordMastery_max_atk1", value);
            actor.Status.min_atk2_skill += (short)value;

            if (skill.Variable.ContainsKey("SwordMastery_max_atk3"))
                skill.Variable.Remove("SwordMastery_max_atk3");
            skill.Variable.Add("SwordMastery_max_atk3", value);
            actor.Status.min_atk3_skill += (short)value;

        }

        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            int value = skill.Variable["SwordMastery_max_atk1"];
            actor.Status.min_atk2_skill -= (short)value;

            int value3 = skill.Variable["SwordMastery_max_atk3"];
            actor.Status.min_atk3_skill -= (short)value3;

        }

        #endregion
    }
}
