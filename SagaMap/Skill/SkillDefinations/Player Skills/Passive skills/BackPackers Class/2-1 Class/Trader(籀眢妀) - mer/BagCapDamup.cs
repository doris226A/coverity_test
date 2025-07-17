
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
using SagaDB.Item;
namespace SagaMap.Skill.SkillDefinations.Trader
{
    /// <summary>
    /// 筋力修練（ウェイトコントロール）
    /// </summary>
    public class BagCapDamup : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = true;
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "BagCapDamupB", active);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            ActorPC pc = (ActorPC)actor;
            float ATK = 0;
            if (pc.Skills2_1.ContainsKey(906))
            {
                ATK = (float)10.0f / (float)(20.0f - (float)pc.Skills2_1[906].Level - (float)skill.skill.Level);
            }
            else
            {
                ATK = (float)10.0f / (float)(20.0f - (float)skill.skill.Level);
            }

            int BagweightUp = 0;
            int Bodyweight = (int)pc.Inventory.Payload[ContainerType.BODY] / 10;
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND) && pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.weightUp > 0)
                BagweightUp = pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.weightUp / 10;
            int minATK = (int)Math.Floor((float)Math.Min(Bodyweight, BagweightUp) * ATK);

            //最小攻擊
            if (skill.Variable.ContainsKey("BagDamUpB_max_atk1"))
                skill.Variable.Remove("BagDamUpB_max_atk1");
            skill.Variable.Add("BagDamUpB_max_atk1", minATK);
            actor.Status.min_atk1_skill += (short)minATK;

            //最小攻擊
            if (skill.Variable.ContainsKey("BagDamUpB_max_atk2"))
                skill.Variable.Remove("BagDamUpB_max_atk2");
            skill.Variable.Add("BagDamUpB_max_atk2", minATK);
            actor.Status.min_atk2_skill += (short)minATK;

            //最小攻擊
            if (skill.Variable.ContainsKey("BagDamUpB_max_atk3"))
                skill.Variable.Remove("BagDamUpB_max_atk3");
            skill.Variable.Add("BagDamUpB_max_atk3", minATK);
            actor.Status.min_atk3_skill += (short)minATK;
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            //最小攻擊
            actor.Status.min_atk1_skill -= (short)skill.Variable["BagDamUpB_max_atk1"];

            //最小攻擊
            actor.Status.min_atk2_skill -= (short)skill.Variable["BagDamUpB_max_atk2"];

            //最小攻擊
            actor.Status.min_atk3_skill -= (short)skill.Variable["BagDamUpB_max_atk3"];
        }
        #endregion
    }
}

