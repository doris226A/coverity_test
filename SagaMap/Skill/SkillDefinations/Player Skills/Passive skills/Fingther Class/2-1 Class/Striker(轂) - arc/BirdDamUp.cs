
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Striker
{
    public class BirdDamUp : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = false;
            ActorPartner pat = SkillHandler.Instance.GetPartner(sActor);
            if (pat != null)
            {
                if (SkillHandler.Instance.CheckPartnerType(pat, "BIRD"))
                {
                    active = true;
                }
            }
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "BirdDamUp", active);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            int level = skill.skill.Level;
            ActorPartner pat = SkillHandler.Instance.GetPartner(actor);
            int addin = new int[] { 0, 10, 20, 30, 40, 50 }[skill.skill.Level];
            //最小攻擊
            int min_atk1_add =  addin;
            if (skill.Variable.ContainsKey("BirdDamUp_min_atk1"))
                skill.Variable.Remove("BirdDamUp_min_atk1");
            skill.Variable.Add("BirdDamUp_min_atk1", min_atk1_add);
            pat.Status.min_atk1_skill += (short)min_atk1_add;

            //最小攻擊
            int min_atk2_add = (int)(5 * level + addin);
            if (skill.Variable.ContainsKey("BirdDamUp_min_atk2"))
                skill.Variable.Remove("BirdDamUp_min_atk2");
            skill.Variable.Add("BirdDamUp_min_atk2", min_atk2_add);
            pat.Status.min_atk2_skill += (short)min_atk2_add;

            //最小攻擊
            int min_atk3_add = (int)(5 * level + addin);
            if (skill.Variable.ContainsKey("BirdDamUp_min_atk3"))
                skill.Variable.Remove("BirdDamUp_min_atk3");
            skill.Variable.Add("BirdDamUp_min_atk3", min_atk3_add);
            pat.Status.min_atk3_skill += (short)min_atk3_add;
  
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            ActorPartner pat = SkillHandler.Instance.GetPartner(actor);
            //最小攻擊
            pat.Status.min_atk1_skill -= (short)skill.Variable["BirdDamUp_min_atk1"];

            //最小攻擊
            pat.Status.min_atk2_skill -= (short)skill.Variable["BirdDamUp_min_atk2"];

            //最小攻擊
            pat.Status.min_atk3_skill -= (short)skill.Variable["BirdDamUp_min_atk3"];

        }
        #endregion
    }
}

