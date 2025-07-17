
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Machinery
{
    /// <summary>
    /// 提升機器人的迴避率（ロボット回避力上昇）
    /// </summary>
    public class RobotAvoUp : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;//封印
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //bool active = false;
            //ActorPet pet = SkillHandler.Instance.GetPet(sActor);
            //if (pet != null)
            //{
            //    if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
            //    {
            //        active = true;
            //    }
            //    DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "RobotAvoUp", active);
            //    skill.OnAdditionStart += this.StartEventHandler;
            //    skill.OnAdditionEnd += this.EndEventHandler;
            //    SkillHandler.ApplyAddition(sActor, skill);
            //}
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            //int level = skill.skill.Level;
            //ActorPet pet = SkillHandler.Instance.GetPet(actor);
            //float avoid_add = new float[] { 0, 0.1f, 0.13f, 0.16f, 0.19f, 0.22f }[skill.skill.Level];
            ////近戰迴避
            //int avoid_melee_add = (int)(pet.Status.avoid_melee * avoid_add);
            //if (skill.Variable.ContainsKey("RobotAvoUp_avoid_melee"))
            //    skill.Variable.Remove("RobotAvoUp_avoid_melee");
            //skill.Variable.Add("RobotAvoUp_avoid_melee", avoid_melee_add);
            //pet.Status.avoid_melee += (ushort)avoid_melee_add;

            ////遠距迴避
            //int avoid_ranged_add = (int)(pet.Status.avoid_ranged * avoid_add);
            //if (skill.Variable.ContainsKey("RobotAvoUp_avoid_ranged"))
            //    skill.Variable.Remove("RobotAvoUp_avoid_ranged");
            //skill.Variable.Add("RobotAvoUp_avoid_ranged", avoid_ranged_add);
            //pet.Status.avoid_ranged += (ushort)avoid_ranged_add;

        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            //ActorPet pet = SkillHandler.Instance.GetPet(actor);
            ////近戰迴避
            //pet.Status.avoid_melee -= (ushort)skill.Variable["RobotAvoUp_avoid_melee"];

            ////遠距迴避
            //pet.Status.avoid_ranged -= (ushort)skill.Variable["RobotAvoUp_avoid_ranged"];

        }
        #endregion
    }
}
