using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Archer
{
    public class BowMastery: ISkill
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
                    if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType == SagaDB.Item.ItemType.BOW)
                    {
                        active = true;
                    }
                }
                DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "BowMastery", active);
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

           /* if (skill.Variable.ContainsKey("MasteryATK"))
                skill.Variable.Remove("MasteryATK");
            skill.Variable.Add("MasteryATK", value);
            actor.Status.min_atk1_skill += (short)value;

            if (skill.Variable.ContainsKey("MasteryATK1"))
                skill.Variable.Remove("MasteryATK1");
            skill.Variable.Add("MasteryATK1", value);
            actor.Status.min_atk2_skill += (short)value;*/

            if (skill.Variable.ContainsKey("MasteryATK2"))
                skill.Variable.Remove("MasteryATK2");
            skill.Variable.Add("MasteryATK2", value);
            actor.Status.min_atk3_skill += (short)value;
        }

        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
           /* int value = skill.Variable["MasteryATK"];
            actor.Status.min_atk1_skill -= (short)value;

            value = skill.Variable["MasteryATK1"];
            actor.Status.min_atk2_skill -= (short)value;*/

            int value = skill.Variable["MasteryATK2"];
            actor.Status.min_atk3_skill -= (short)value;
        }

        #endregion
    }
}
