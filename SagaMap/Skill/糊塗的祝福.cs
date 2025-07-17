using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 糊塗的祝福
    /// </summary>
    public class 糊塗的祝福 : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC actorPC = (ActorPC)sActor;
                if (actorPC.Partner != null)
                {
                    DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "PartnerSoul", true);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(sActor, skill);
                }
                else
                {
                    SkillHandler.RemoveAddition(sActor, "PartnerSoul");
                }
            }
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            SetupSkill(actor, skill);
        }

        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            DeleteSkill(actor, skill);
        }
        void DeleteSkill(Actor actor, DefaultPassiveSkill skill)
        {
            //最小攻擊
            actor.Status.min_atk1_skill -= (short)skill.Variable["PartnerSoul_min_atk_bs_add"];
            actor.Status.min_atk2_skill -= (short)skill.Variable["PartnerSoul_min_atk_bs_add"];
            actor.Status.min_atk3_skill -= (short)skill.Variable["PartnerSoul_min_atk_bs_add"];
            //大傷
            actor.Status.max_atk1_skill -= (short)skill.Variable["PartnerSoul_max_atk_bs_add"];
            actor.Status.max_atk2_skill -= (short)skill.Variable["PartnerSoul_max_atk_bs_add"];
            actor.Status.max_atk3_skill -= (short)skill.Variable["PartnerSoul_max_atk_bs_add"];
            //HP MP SP
            actor.Status.hp_skill -= (short)skill.Variable["PartnerSoul_hp_add"];
            actor.Status.mp_skill -= (short)skill.Variable["PartnerSoul_mp_add"];
            actor.Status.sp_skill -= (short)skill.Variable["PartnerSoul_sp_add"];
            //最大魔攻
            actor.Status.max_matk_skill -= (short)skill.Variable["PartnerSoul_Max_MAtk"];
            //最小魔攻
            actor.Status.min_matk_skill -= (short)skill.Variable["PartnerSoul_Min_MAtk"];
            //近戰迴避
            actor.Status.avoid_melee_skill -= (short)skill.Variable["PartnerSoul_avo_melee_add"];
            //遠距迴避
            actor.Status.avoid_ranged_skill -= (short)skill.Variable["PartnerSoul_avo_ranged_add"];
            //近戰命中
            actor.Status.hit_melee_skill -= (short)skill.Variable["PartnerSoul_hit_melee_add"];
            //遠距命中
            actor.Status.hit_ranged_skill -= (short)skill.Variable["PartnerSoul_hit_ranged_add"];
            //左防禦力
            actor.Status.def_skill -= (short)skill.Variable["PartnerSoul_left_def_add"];
            //右防禦力
            actor.Status.def_add_skill -= (short)skill.Variable["PartnerSoul_right_def_add"];
            //左魔法防禦
            actor.Status.mdef_skill -= (short)skill.Variable["PartnerSoul_left_mdef_add"];
            //右魔法防禦
            actor.Status.mdef_add_skill -= (short)skill.Variable["PartnerSoul_right_mdef_add"];

        }
        void SetupSkill(Actor actor, DefaultPassiveSkill skill)
        {
            ActorPC actorPC = (ActorPC)actor;
            if (actorPC.Partner == null)
                return;
            ActorPartner partner = (ActorPartner)actorPC.Partner;

            int max_atk_bs_add1 = 0, min_atk_bs_add1 = 0;
            int max_atk_bs_add2 = 0, min_atk_bs_add2 = 0;
            int max_atk_bs_add3 = 0, min_atk_bs_add3 = 0;
            int max_matk_add = 0, min_matk_add = 0, avo_melee_add = 0, avo_ranged_add = 0, hit_melee_add = 0, hit_ranged_add = 0;
            int left_def_add = 0, right_def_add = 0, left_mdef_add = 0, right_mdef_add = 0, hp_add = 0, mp_add = 0, sp_add = 0;

            max_atk_bs_add1 = (int)(partner.Status.max_atk1 * (0.10));
            min_atk_bs_add1 = (int)(partner.Status.min_atk1 * (0.10));
            max_atk_bs_add2 = (int)(partner.Status.max_atk2 * (0.10));
            min_atk_bs_add2 = (int)(partner.Status.min_atk2 * (0.10));
            max_atk_bs_add3 = (int)(partner.Status.max_atk3 * (0.10));
            min_atk_bs_add3 = (int)(partner.Status.min_atk3 * (0.10));
            max_matk_add = (int)(partner.Status.max_matk * 0.10);
            min_matk_add = (int)(partner.Status.min_matk * 0.10);
            avo_melee_add = (int)(partner.Status.avoid_ranged * 0.06);
            avo_ranged_add = (int)(partner.Status.avoid_ranged * 0.06);
            hit_melee_add = (int)(partner.Status.avoid_ranged * 0.06);
            hit_ranged_add = (int)(partner.Status.avoid_ranged * 0.06);
            left_def_add = (int)(partner.Status.def * 0.05);
            left_mdef_add = (int)(partner.Status.mdef * 0.05);
            right_def_add = (int)(partner.Status.def_add * 0.05);
            right_mdef_add = (int)(partner.Status.mdef_add * 0.05);
            hp_add = (int)(partner.MaxHP * 0.05);
            mp_add = (int)(partner.MaxMP * 0.05);
            sp_add = (int)(partner.MaxSP * 0.05);


            //最大魔攻
            if (skill.Variable.ContainsKey("PartnerSoul_Max_MAtk"))
                skill.Variable.Remove("PartnerSoul_Max_MAtk");
            skill.Variable.Add("PartnerSoul_Max_MAtk", max_matk_add);
            actor.Status.max_matk_skill += (short)max_matk_add;
            //最小魔攻
            if (skill.Variable.ContainsKey("PartnerSoul_Min_MAtk"))
                skill.Variable.Remove("PartnerSoul_Min_MAtk");
            skill.Variable.Add("PartnerSoul_Min_MAtk", min_matk_add);
            actor.Status.min_matk_skill += (short)min_matk_add;
            //近戰迴避
            if (skill.Variable.ContainsKey("PartnerSoul_avo_melee_add"))
                skill.Variable.Remove("PartnerSoul_avo_melee_add");
            skill.Variable.Add("PartnerSoul_avo_melee_add", avo_melee_add);
            actor.Status.avoid_melee_skill += (short)avo_melee_add;
            //遠距迴避
            if (skill.Variable.ContainsKey("PartnerSoul_avo_ranged_add"))
                skill.Variable.Remove("PartnerSoul_avo_ranged_add");
            skill.Variable.Add("PartnerSoul_avo_ranged_add", avo_ranged_add);
            actor.Status.avoid_ranged_skill += (short)avo_ranged_add;
            //近戰命中
            if (skill.Variable.ContainsKey("PartnerSoul_hit_melee_add"))
                skill.Variable.Remove("PartnerSoul_hit_melee_add");
            skill.Variable.Add("PartnerSoul_hit_melee_add", hit_melee_add);
            actor.Status.hit_melee_skill += (short)hit_melee_add;
            //遠距命中
            if (skill.Variable.ContainsKey("PartnerSoul_hit_ranged_add"))
                skill.Variable.Remove("PartnerSoul_hit_ranged_add");
            skill.Variable.Add("PartnerSoul_hit_ranged_add", hit_ranged_add);
            actor.Status.hit_ranged_skill += (short)hit_ranged_add;
            //左防禦力
            if (skill.Variable.ContainsKey("PartnerSoul_left_def_add)"))
                skill.Variable.Remove("PartnerSoul_left_def_add");
            skill.Variable.Add("PartnerSoul_left_def_add", left_def_add);
            actor.Status.def_skill += (short)left_def_add;
            //右防禦力
            if (skill.Variable.ContainsKey("PartnerSoul_right_def_add"))
                skill.Variable.Remove("PartnerSoul_right_def_add");
            skill.Variable.Add("PartnerSoul_right_def_add", right_def_add);
            actor.Status.def_add_skill += (short)right_def_add;
            //左魔法防禦
            if (skill.Variable.ContainsKey("PartnerSoul_left_mdef_add"))
                skill.Variable.Remove("PartnerSoul_left_mdef_add");
            skill.Variable.Add("PartnerSoul_left_mdef_add", left_mdef_add);
            actor.Status.mdef_skill += (short)left_mdef_add;
            //右魔法防禦
            if (skill.Variable.ContainsKey("PartnerSoul_right_mdef_add"))
                skill.Variable.Remove("PartnerSoul_right_mdef_add");
            skill.Variable.Add("PartnerSoul_right_mdef_add", right_mdef_add);
            actor.Status.mdef_add_skill += (short)right_mdef_add;
            //HP
            if (skill.Variable.ContainsKey("PartnerSoul_hp_add"))
                skill.Variable.Remove("PartnerSoul_hp_add");
            skill.Variable.Add("PartnerSoul_hp_add", hp_add);
            actor.Status.hp_skill += (short)hp_add;
            //MP
            if (skill.Variable.ContainsKey("PartnerSoul_mp_add"))
                skill.Variable.Remove("PartnerSoul_mp_add");
            skill.Variable.Add("PartnerSoul_mp_add", mp_add);
            actor.Status.mp_skill += (short)mp_add;
            //SP
            if (skill.Variable.ContainsKey("PartnerSoul_sp_add"))
                skill.Variable.Remove("PartnerSoul_sp_add");
            skill.Variable.Add("PartnerSoul_sp_add", sp_add);
            actor.Status.sp_skill += (short)sp_add;
            //大傷
            if (skill.Variable.ContainsKey("PartnerSoul_max_atk_bs_add"))
                skill.Variable.Remove("PartnerSoul_max_atk_bs_add");
            skill.Variable.Add("PartnerSoul_max_atk_bs_add", max_atk_bs_add1);
            actor.Status.max_atk1_skill += (short)max_atk_bs_add1;

            if (skill.Variable.ContainsKey("PartnerSoul_max_atk_bs_add"))
                skill.Variable.Remove("PartnerSoul_max_atk_bs_add");
            skill.Variable.Add("PartnerSoul_max_atk_bs_add", max_atk_bs_add2);
            actor.Status.max_atk2_skill += (short)max_atk_bs_add2;

            if (skill.Variable.ContainsKey("PartnerSoul_max_atk_bs_add"))
                skill.Variable.Remove("PartnerSoul_max_atk_bs_add");
            skill.Variable.Add("PartnerSoul_max_atk_bs_add", max_atk_bs_add3);
            actor.Status.max_atk3_skill += (short)max_atk_bs_add3;
            //小傷
            if (skill.Variable.ContainsKey("PartnerSoul_min_atk_bs_add"))
                skill.Variable.Remove("PartnerSoul_min_atk_bs_add");
            skill.Variable.Add("PartnerSoul_min_atk_bs_add", min_atk_bs_add1);
            actor.Status.min_atk1_skill += (short)min_atk_bs_add1;

            if (skill.Variable.ContainsKey("PartnerSoul_min_atk_bs_add"))
                skill.Variable.Remove("PartnerSoul_min_atk_bs_add");
            skill.Variable.Add("PartnerSoul_min_atk_bs_add", min_atk_bs_add2);
            actor.Status.min_atk2_skill += (short)min_atk_bs_add2;

            if (skill.Variable.ContainsKey("PartnerSoul_min_atk_bs_add"))
                skill.Variable.Remove("PartnerSoul_min_atk_bs_add");
            skill.Variable.Add("PartnerSoul_min_atk_bs_add", min_atk_bs_add3);
            actor.Status.min_atk3_skill += (short)min_atk_bs_add3;
        }
        #endregion
    }
}
