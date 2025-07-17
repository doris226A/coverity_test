using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;
using SagaMap.Network.Client;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Stryder
{
    /// <summary>
    /// 風行者
    /// </summary>
    public class Strider : ISkill
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
                if (actorPC.Partner != null && actorPC.Partner.BaseData.partnertype != SagaDB.Partner.PartnerType.MACHINE)
                {
                    DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "Strider", true);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(sActor, skill);
                }
                else
                {
                    SkillHandler.RemoveAddition(sActor, "Strider");
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
            ActorPC actorPC = (ActorPC)actor;
            if (actorPC.Partner == null)
                return;
            ActorPartner partner = (ActorPartner)actorPC.Partner;
            //最小攻擊
            partner.Status.min_atk1_skill -= (short)skill.Variable["Strider_min_atk_bs_add"];
            partner.Status.min_atk2_skill -= (short)skill.Variable["Strider_min_atk_bs_add"];
            partner.Status.min_atk3_skill -= (short)skill.Variable["Strider_min_atk_bs_add"];
            //大傷
            partner.Status.max_atk1_skill -= (short)skill.Variable["Strider_max_atk_bs_add"];
            partner.Status.max_atk2_skill -= (short)skill.Variable["Strider_max_atk_bs_add"];
            partner.Status.max_atk3_skill -= (short)skill.Variable["Strider_max_atk_bs_add"];
            //HP MP SP
            partner.Status.hp_skill -= (short)skill.Variable["Strider_hp_add"];
            partner.Status.mp_skill -= (short)skill.Variable["Strider_mp_add"];
            partner.Status.sp_skill -= (short)skill.Variable["Strider_sp_add"];
            //最大魔攻
            partner.Status.max_matk_skill -= (short)skill.Variable["Strider_Max_MAtk"];
            //最小魔攻
            partner.Status.min_matk_skill -= (short)skill.Variable["Strider_Min_MAtk"];
            //近戰迴避
            partner.Status.avoid_melee_skill -= (short)skill.Variable["Strider_avo_melee_add"];
            //遠距迴避
            partner.Status.avoid_ranged_skill -= (short)skill.Variable["Strider_avo_ranged_add"];
            //近戰命中
            partner.Status.hit_melee_skill -= (short)skill.Variable["Strider_hit_melee_add"];
            //遠距命中
            partner.Status.hit_ranged_skill -= (short)skill.Variable["Strider_hit_ranged_add"];
            //右防禦力
            partner.Status.def_add_skill -= (short)skill.Variable["Strider_right_def_add"];
            //右魔法防禦
            partner.Status.mdef_add_skill -= (short)skill.Variable["Strider_right_mdef_add"];

            MapClient.FromActorPC(actorPC).SendPetInfo();
        }
        void SetupSkill(Actor actor, DefaultPassiveSkill skill)
        {
            ActorPC actorPC = (ActorPC)actor;
            if (actorPC.Partner == null)
                return;
            ActorPartner partner = (ActorPartner)actorPC.Partner;

            float bouns = (float)((30 + skill.skill.Level) / 100.0f);

            int max_atk_bs_add1 = 0, min_atk_bs_add1 = 0;
            int max_atk_bs_add2 = 0, min_atk_bs_add2 = 0;
            int max_atk_bs_add3 = 0, min_atk_bs_add3 = 0;
            int max_matk_add = 0, min_matk_add = 0, avo_melee_add = 0, avo_ranged_add = 0, hit_melee_add = 0, hit_ranged_add = 0;
            int left_def_add = 0, right_def_add = 0, left_mdef_add = 0, right_mdef_add = 0, hp_add = 0, mp_add = 0, sp_add = 0;

            hp_add = (int)(actorPC.MaxHP * bouns);
            mp_add = (int)(actorPC.MaxMP * bouns);
            sp_add = (int)(actorPC.MaxSP * bouns);
            max_atk_bs_add1 = (int)(actorPC.Status.max_atk_bs * bouns);
            min_atk_bs_add1 = (int)(actorPC.Status.min_atk_bs * bouns);
            max_atk_bs_add2 = (int)(actorPC.Status.max_atk_bs * bouns);
            min_atk_bs_add2 = (int)(actorPC.Status.min_atk_bs * bouns);
            max_atk_bs_add3 = (int)(actorPC.Status.max_atk_bs * bouns);
            min_atk_bs_add3 = (int)(actorPC.Status.min_atk_bs * bouns);
            max_matk_add = (int)(actorPC.Status.max_matk_bs * bouns);
            min_matk_add = (int)(actorPC.Status.min_matk_bs * bouns);

            avo_melee_add = (int)(actorPC.Status.avoid_ranged * bouns);
            avo_ranged_add = (int)(actorPC.Status.avoid_ranged * bouns);
            hit_melee_add = (int)(actorPC.Status.avoid_ranged * bouns);
            hit_ranged_add = (int)(actorPC.Status.avoid_ranged * bouns);
            right_def_add = (int)(actorPC.Status.def_add * bouns);
            right_mdef_add = (int)(actorPC.Status.mdef_add * bouns);


            //HP
            if (skill.Variable.ContainsKey("Strider_hp_add"))
                skill.Variable.Remove("Strider_hp_add");
            skill.Variable.Add("Strider_hp_add", hp_add);
            partner.Status.hp_skill += (short)hp_add;
            //MP
            if (skill.Variable.ContainsKey("Strider_mp_add"))
                skill.Variable.Remove("Strider_mp_add");
            skill.Variable.Add("Strider_mp_add", mp_add);
            partner.Status.mp_skill += (short)mp_add;
            //SP
            if (skill.Variable.ContainsKey("Strider_sp_add"))
                skill.Variable.Remove("Strider_sp_add");
            skill.Variable.Add("Strider_sp_add", sp_add);
            partner.Status.sp_skill += (short)sp_add;

            //大傷
            if (skill.Variable.ContainsKey("Strider_max_atk_bs_add"))
                skill.Variable.Remove("Strider_max_atk_bs_add");
            skill.Variable.Add("Strider_max_atk_bs_add", max_atk_bs_add1);
            partner.Status.max_atk1_skill += (short)max_atk_bs_add1;

            if (skill.Variable.ContainsKey("Strider_max_atk_bs_add"))
                skill.Variable.Remove("Strider_max_atk_bs_add");
            skill.Variable.Add("Strider_max_atk_bs_add", max_atk_bs_add2);
            partner.Status.max_atk2_skill += (short)max_atk_bs_add2;

            if (skill.Variable.ContainsKey("Strider_max_atk_bs_add"))
                skill.Variable.Remove("Strider_max_atk_bs_add");
            skill.Variable.Add("Strider_max_atk_bs_add", max_atk_bs_add3);
            partner.Status.max_atk3_skill += (short)max_atk_bs_add3;
            //小傷
            if (skill.Variable.ContainsKey("Strider_min_atk_bs_add"))
                skill.Variable.Remove("Strider_min_atk_bs_add");
            skill.Variable.Add("Strider_min_atk_bs_add", min_atk_bs_add1);
            partner.Status.min_atk1_skill += (short)min_atk_bs_add1;

            if (skill.Variable.ContainsKey("Strider_min_atk_bs_add"))
                skill.Variable.Remove("Strider_min_atk_bs_add");
            skill.Variable.Add("Strider_min_atk_bs_add", min_atk_bs_add2);
            partner.Status.min_atk2_skill += (short)min_atk_bs_add2;

            if (skill.Variable.ContainsKey("Strider_min_atk_bs_add"))
                skill.Variable.Remove("Strider_min_atk_bs_add");
            skill.Variable.Add("Strider_min_atk_bs_add", min_atk_bs_add3);
            partner.Status.min_atk3_skill += (short)min_atk_bs_add3;

            //最大魔攻
            if (skill.Variable.ContainsKey("Strider_Max_MAtk"))
                skill.Variable.Remove("Strider_Max_MAtk");
            skill.Variable.Add("Strider_Max_MAtk", max_matk_add);
            partner.Status.max_matk_skill += (short)max_matk_add;
            //最小魔攻
            if (skill.Variable.ContainsKey("Strider_Min_MAtk"))
                skill.Variable.Remove("Strider_Min_MAtk");
            skill.Variable.Add("Strider_Min_MAtk", min_matk_add);
            partner.Status.min_matk_skill += (short)min_matk_add;


            //近戰迴避
            if (skill.Variable.ContainsKey("Strider_avo_melee_add"))
                skill.Variable.Remove("Strider_avo_melee_add");
            skill.Variable.Add("Strider_avo_melee_add", avo_melee_add);
            partner.Status.avoid_melee_skill += (short)avo_melee_add;
            //遠距迴避
            if (skill.Variable.ContainsKey("Strider_avo_ranged_add"))
                skill.Variable.Remove("Strider_avo_ranged_add");
            skill.Variable.Add("Strider_avo_ranged_add", avo_ranged_add);
            partner.Status.avoid_ranged_skill += (short)avo_ranged_add;
            //近戰命中
            if (skill.Variable.ContainsKey("Strider_hit_melee_add"))
                skill.Variable.Remove("Strider_hit_melee_add");
            skill.Variable.Add("Strider_hit_melee_add", hit_melee_add);
            partner.Status.hit_melee_skill += (short)hit_melee_add;
            //遠距命中
            if (skill.Variable.ContainsKey("Strider_hit_ranged_add"))
                skill.Variable.Remove("Strider_hit_ranged_add");
            skill.Variable.Add("Strider_hit_ranged_add", hit_ranged_add);
            partner.Status.hit_ranged_skill += (short)hit_ranged_add;
            //右防禦力
            if (skill.Variable.ContainsKey("Strider_right_def_add"))
                skill.Variable.Remove("Strider_right_def_add");
            skill.Variable.Add("Strider_right_def_add", right_def_add);
            partner.Status.def_add_skill += (short)right_def_add;
            //右魔法防禦
            if (skill.Variable.ContainsKey("Strider_right_mdef_add"))
                skill.Variable.Remove("Strider_right_mdef_add");
            skill.Variable.Add("Strider_right_mdef_add", right_mdef_add);
            partner.Status.mdef_add_skill += (short)right_mdef_add;

            MapClient.FromActorPC(actorPC).SendPetInfo();
        }
        /*void DeleteSkill(Actor actor, DefaultPassiveSkill skill)
        {
            //最小攻擊
            actor.Status.min_atk1_skill -= (short)skill.Variable["Strider_min_atk_bs_add"];
            actor.Status.min_atk2_skill -= (short)skill.Variable["Strider_min_atk_bs_add"];
            actor.Status.min_atk3_skill -= (short)skill.Variable["Strider_min_atk_bs_add"];
            //大傷
            actor.Status.max_atk1_skill -= (short)skill.Variable["Strider_max_atk_bs_add"];
            actor.Status.max_atk2_skill -= (short)skill.Variable["Strider_max_atk_bs_add"];
            actor.Status.max_atk3_skill -= (short)skill.Variable["Strider_max_atk_bs_add"];
            //HP MP SP
            actor.Status.hp_skill -= (short)skill.Variable["Strider_hp_add"];
            actor.Status.mp_skill -= (short)skill.Variable["Strider_mp_add"];
            actor.Status.sp_skill -= (short)skill.Variable["Strider_sp_add"];
            //最大魔攻
            actor.Status.max_matk_skill -= (short)skill.Variable["Strider_Max_MAtk"];
            //最小魔攻
            actor.Status.min_matk_skill -= (short)skill.Variable["Strider_Min_MAtk"];
            //近戰迴避
            actor.Status.avoid_melee_skill -= (short)skill.Variable["Strider_avo_melee_add"];
            //遠距迴避
            actor.Status.avoid_ranged_skill -= (short)skill.Variable["Strider_avo_ranged_add"];
            //近戰命中
            actor.Status.hit_melee_skill -= (short)skill.Variable["Strider_hit_melee_add"];
            //遠距命中
            actor.Status.hit_ranged_skill -= (short)skill.Variable["Strider_hit_ranged_add"];
            //右防禦力
            actor.Status.def_add_skill -= (short)skill.Variable["Strider_right_def_add"];
            //右魔法防禦
            actor.Status.mdef_add_skill -= (short)skill.Variable["Strider_right_mdef_add"];
             
        }
        void SetupSkill(Actor actor, DefaultPassiveSkill skill)
        {
            ActorPC actorPC = (ActorPC)actor;
            if (actorPC.Partner == null)
                return;
            ActorPartner partner = (ActorPartner)actorPC.Partner;

            float bouns = (float)((30 + skill.skill.Level) / 100.0f);

            int max_atk_bs_add1 = 0, min_atk_bs_add1 = 0;
            int max_atk_bs_add2 = 0, min_atk_bs_add2 = 0;
            int max_atk_bs_add3 = 0, min_atk_bs_add3 = 0;
            int max_matk_add = 0, min_matk_add = 0, avo_melee_add = 0, avo_ranged_add = 0, hit_melee_add = 0, hit_ranged_add = 0;
            int left_def_add = 0, right_def_add = 0, left_mdef_add = 0, right_mdef_add = 0, hp_add = 0, mp_add = 0, sp_add = 0;
            
            hp_add = (int)(partner.MaxHP * bouns);
            mp_add = (int)(partner.MaxMP * bouns);
            sp_add = (int)(partner.MaxSP * bouns);
            max_atk_bs_add1 = (int)(partner.Status.max_atk_bs * bouns);
            min_atk_bs_add1 = (int)(partner.Status.min_atk_bs * bouns);
            max_atk_bs_add2 = (int)(partner.Status.max_atk_bs * bouns);
            min_atk_bs_add2 = (int)(partner.Status.min_atk_bs * bouns);
            max_atk_bs_add3 = (int)(partner.Status.max_atk_bs * bouns);
            min_atk_bs_add3 = (int)(partner.Status.min_atk_bs * bouns);
            max_matk_add = (int)(partner.Status.max_matk_bs * bouns);
            min_matk_add = (int)(partner.Status.min_matk_bs * bouns);

            avo_melee_add = (int)(partner.Status.avoid_ranged * bouns);
            avo_ranged_add = (int)(partner.Status.avoid_ranged * bouns);
            hit_melee_add = (int)(partner.Status.avoid_ranged * bouns);
            hit_ranged_add = (int)(partner.Status.avoid_ranged * bouns);
            right_def_add = (int)(partner.Status.def_add * bouns);
            right_mdef_add = (int)(partner.Status.mdef_add * bouns);


            //HP
            if (skill.Variable.ContainsKey("Strider_hp_add"))
                skill.Variable.Remove("Strider_hp_add");
            skill.Variable.Add("Strider_hp_add", hp_add);
            actor.Status.hp_skill += (short)hp_add;
            //MP
            if (skill.Variable.ContainsKey("Strider_mp_add"))
                skill.Variable.Remove("Strider_mp_add");
            skill.Variable.Add("Strider_mp_add", mp_add);
            actor.Status.mp_skill += (short)mp_add;
            //SP
            if (skill.Variable.ContainsKey("Strider_sp_add"))
                skill.Variable.Remove("Strider_sp_add");
            skill.Variable.Add("Strider_sp_add", sp_add);
            actor.Status.sp_skill += (short)sp_add;

            //大傷
            if (skill.Variable.ContainsKey("Strider_max_atk_bs_add"))
                skill.Variable.Remove("Strider_max_atk_bs_add");
            skill.Variable.Add("Strider_max_atk_bs_add", max_atk_bs_add1);
            actor.Status.max_atk1_skill += (short)max_atk_bs_add1;

            if (skill.Variable.ContainsKey("Strider_max_atk_bs_add"))
                skill.Variable.Remove("Strider_max_atk_bs_add");
            skill.Variable.Add("Strider_max_atk_bs_add", max_atk_bs_add2);
            actor.Status.max_atk2_skill += (short)max_atk_bs_add2;

            if (skill.Variable.ContainsKey("Strider_max_atk_bs_add"))
                skill.Variable.Remove("Strider_max_atk_bs_add");
            skill.Variable.Add("Strider_max_atk_bs_add", max_atk_bs_add3);
            actor.Status.max_atk3_skill += (short)max_atk_bs_add3;
            //小傷
            if (skill.Variable.ContainsKey("Strider_min_atk_bs_add"))
                skill.Variable.Remove("Strider_min_atk_bs_add");
            skill.Variable.Add("Strider_min_atk_bs_add", min_atk_bs_add1);
            actor.Status.min_atk1_skill += (short)min_atk_bs_add1;

            if (skill.Variable.ContainsKey("Strider_min_atk_bs_add"))
                skill.Variable.Remove("Strider_min_atk_bs_add");
            skill.Variable.Add("Strider_min_atk_bs_add", min_atk_bs_add2);
            actor.Status.min_atk2_skill += (short)min_atk_bs_add2;

            if (skill.Variable.ContainsKey("Strider_min_atk_bs_add"))
                skill.Variable.Remove("Strider_min_atk_bs_add");
            skill.Variable.Add("Strider_min_atk_bs_add", min_atk_bs_add3);
            actor.Status.min_atk3_skill += (short)min_atk_bs_add3;

            //最大魔攻
            if (skill.Variable.ContainsKey("Strider_Max_MAtk"))
                skill.Variable.Remove("Strider_Max_MAtk");
            skill.Variable.Add("Strider_Max_MAtk", max_matk_add);
            actor.Status.max_matk_skill += (short)max_matk_add;
            //最小魔攻
            if (skill.Variable.ContainsKey("Strider_Min_MAtk"))
                skill.Variable.Remove("Strider_Min_MAtk");
            skill.Variable.Add("Strider_Min_MAtk", min_matk_add);
            actor.Status.min_matk_skill += (short)min_matk_add;


            //近戰迴避
            if (skill.Variable.ContainsKey("Strider_avo_melee_add"))
                skill.Variable.Remove("Strider_avo_melee_add");
            skill.Variable.Add("Strider_avo_melee_add", avo_melee_add);
            actor.Status.avoid_melee_skill += (short)avo_melee_add;
            //遠距迴避
            if (skill.Variable.ContainsKey("Strider_avo_ranged_add"))
                skill.Variable.Remove("Strider_avo_ranged_add");
            skill.Variable.Add("Strider_avo_ranged_add", avo_ranged_add);
            actor.Status.avoid_ranged_skill += (short)avo_ranged_add;
            //近戰命中
            if (skill.Variable.ContainsKey("Strider_hit_melee_add"))
                skill.Variable.Remove("Strider_hit_melee_add");
            skill.Variable.Add("Strider_hit_melee_add", hit_melee_add);
            actor.Status.hit_melee_skill += (short)hit_melee_add;
            //遠距命中
            if (skill.Variable.ContainsKey("Strider_hit_ranged_add"))
                skill.Variable.Remove("Strider_hit_ranged_add");
            skill.Variable.Add("Strider_hit_ranged_add", hit_ranged_add);
            actor.Status.hit_ranged_skill += (short)hit_ranged_add;
            //右防禦力
            if (skill.Variable.ContainsKey("Strider_right_def_add"))
                skill.Variable.Remove("Strider_right_def_add");
            skill.Variable.Add("Strider_right_def_add", right_def_add);
            actor.Status.def_add_skill += (short)right_def_add;
            //右魔法防禦
            if (skill.Variable.ContainsKey("Strider_right_mdef_add"))
                skill.Variable.Remove("Strider_right_mdef_add");
            skill.Variable.Add("Strider_right_mdef_add", right_mdef_add);
            actor.Status.mdef_add_skill += (short)right_mdef_add;
            
        }*/
        #endregion
    }
}
