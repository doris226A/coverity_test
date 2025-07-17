
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaDB.Item;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Royaldealer
{
    /// <summary>
    /// 鞄の達人//995
    /// </summary>
    public class BagMaster : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = false;
            if (SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.LEFT_HANDBAG, SagaDB.Item.ItemType.HANDBAG))
            {
                active = true;
            }
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "BagMaster", active);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            int level = skill.skill.Level;
            float[] ATK = { 0, 0.05f, 0.15f, 0.25f };
            float[] CAPA = { 0, 0.15f, 0.30f, 0.50f };

            int CAPAadd = (int)(((ActorPC)actor).Inventory.MaxVolume[ContainerType.BODY] * CAPA[level]);
            if (skill.Variable.ContainsKey("BagMaster_CAPACommunion"))
                skill.Variable.Remove("BagMaster_CAPACommunion");
            skill.Variable.Add("BagMaster_CAPACommunion", CAPAadd);
            actor.Status.CAPA_skill += (short)CAPAadd;


            int PAYLadd = (int)(((ActorPC)actor).Inventory.MaxPayload[ContainerType.BODY] * CAPA[level]);
            if (skill.Variable.ContainsKey("BagMaster_PAYLCommunion"))
                skill.Variable.Remove("BagMaster_PAYLCommunion");
            skill.Variable.Add("BagMaster_PAYLCommunion", PAYLadd);
            actor.Status.PAYL_skill += (short)PAYLadd;

            //最大攻擊
            int max_atk1_add = (int)(actor.Status.max_atk1 * ATK[level]);
            if (skill.Variable.ContainsKey("BagDamUp_max_atk1"))
                skill.Variable.Remove("BagDamUp_max_atk1");
            skill.Variable.Add("BagDamUp_max_atk1", max_atk1_add);
            actor.Status.max_atk1_skill += (short)max_atk1_add;

            //最大攻擊
            int max_atk2_add = (int)(actor.Status.max_atk2 * ATK[level]);
            if (skill.Variable.ContainsKey("BagDamUp_max_atk2"))
                skill.Variable.Remove("BagDamUp_max_atk2");
            skill.Variable.Add("BagDamUp_max_atk2", max_atk2_add);
            actor.Status.max_atk2_skill += (short)max_atk2_add;

            //最大攻擊
            int max_atk3_add = (int)(actor.Status.max_atk3 * ATK[level]);
            if (skill.Variable.ContainsKey("BagDamUp_max_atk3"))
                skill.Variable.Remove("BagDamUp_max_atk3");
            skill.Variable.Add("BagDamUp_max_atk3", max_atk3_add);
            actor.Status.max_atk3_skill += (short)max_atk3_add;

            //最小攻擊
            int min_atk1_add = (int)(actor.Status.min_atk1 * ATK[level]);
            if (skill.Variable.ContainsKey("BagDamUp_min_atk1"))
                skill.Variable.Remove("BagDamUp_min_atk1");
            skill.Variable.Add("BagDamUp_min_atk1", min_atk1_add);
            actor.Status.min_atk1_skill += (short)min_atk1_add;

            //最小攻擊
            int min_atk2_add = (int)(actor.Status.min_atk2 * ATK[level]);
            if (skill.Variable.ContainsKey("BagDamUp_min_atk2"))
                skill.Variable.Remove("BagDamUp_min_atk2");
            skill.Variable.Add("BagDamUp_min_atk2", min_atk2_add);
            actor.Status.min_atk2_skill += (short)min_atk2_add;

            //最小攻擊
            int min_atk3_add = (int)(actor.Status.min_atk3 * ATK[level]);
            if (skill.Variable.ContainsKey("BagDamUp_min_atk3"))
                skill.Variable.Remove("BagDamUp_min_atk3");
            skill.Variable.Add("BagDamUp_min_atk3", min_atk3_add);
            actor.Status.min_atk3_skill += (short)min_atk3_add;

        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            //最大攻擊
            actor.Status.max_atk1_skill -= (short)skill.Variable["BagDamUp_max_atk1"];

            //最大攻擊
            actor.Status.max_atk2_skill -= (short)skill.Variable["BagDamUp_max_atk2"];

            //最大攻擊
            actor.Status.max_atk3_skill -= (short)skill.Variable["BagDamUp_max_atk3"];

            //最小攻擊
            actor.Status.min_atk1_skill -= (short)skill.Variable["BagDamUp_min_atk1"];

            //最小攻擊
            actor.Status.min_atk2_skill -= (short)skill.Variable["BagDamUp_min_atk2"];

            //最小攻擊
            actor.Status.min_atk3_skill -= (short)skill.Variable["BagDamUp_min_atk3"];

            actor.Status.CAPA_skill -= (short)skill.Variable["BagMaster_CAPACommunion"];

            actor.Status.PAYL_skill -= (short)skill.Variable["BagMaster_PAYLCommunion"];
        }
        #endregion
    }
}

