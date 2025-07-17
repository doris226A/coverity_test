
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Machinery
{
    /// <summary>
    /// 超越极限（オーバーチューン）
    /// </summary>
    public class RobotOverTune : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(sActor);
            if (pet != null)
            {
                if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    return 0;
                }
                else
                {
                    return -54;
                }
            }
            return -54;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int life = new int[] { 0, 50000, 70000, 90000 }[level];
            Actor realdActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
            DefaultBuff skill = new DefaultBuff(args.skill, realdActor, "RobotOverTune", life, 1000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            skill.OnUpdate += this.Update;
            SkillHandler.ApplyAddition(realdActor, skill);
        }

        void Update(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (!SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    actor.Status.Additions["RobotOverTune"].AdditionEnd();
                    actor.Status.Additions.TryRemove("RobotOverTune", out _);
                }
            }
            else
            {
                actor.Status.Additions["RobotOverTune"].AdditionEnd();
                actor.Status.Additions.TryRemove("RobotOverTune", out _);
            }

        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int level = skill.skill.Level;
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            //最大攻擊
            int max_atk1_add = (int)(pet.Status.max_atk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_max_atk1"))
                skill.Variable.Remove("RobotAtkUp_max_atk1");
            skill.Variable.Add("RobotAtkUp_max_atk1", max_atk1_add);
            pet.Status.max_atk1 += (ushort)max_atk1_add;

            //最大攻擊
            int max_atk2_add = (int)(pet.Status.max_atk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_max_atk2"))
                skill.Variable.Remove("RobotAtkUp_max_atk2");
            skill.Variable.Add("RobotAtkUp_max_atk2", max_atk2_add);
            pet.Status.max_atk2 += (ushort)max_atk2_add;

            //最大攻擊
            int max_atk3_add = (int)(pet.Status.max_atk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_max_atk3"))
                skill.Variable.Remove("RobotAtkUp_max_atk3");
            skill.Variable.Add("RobotAtkUp_max_atk3", max_atk3_add);
            pet.Status.max_atk3 += (ushort)max_atk3_add;

            //最小攻擊
            int min_atk1_add = (int)(pet.Status.min_atk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_min_atk1"))
                skill.Variable.Remove("RobotAtkUp_min_atk1");
            skill.Variable.Add("RobotAtkUp_min_atk1", min_atk1_add);
            pet.Status.min_atk1 += (ushort)min_atk1_add;

            //最小攻擊
            int min_atk2_add = (int)(pet.Status.min_atk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_min_atk2"))
                skill.Variable.Remove("RobotAtkUp_min_atk2");
            skill.Variable.Add("RobotAtkUp_min_atk2", min_atk2_add);
            pet.Status.min_atk2 += (ushort)min_atk2_add;

            //最小攻擊
            int min_atk3_add = (int)(pet.Status.min_atk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_min_atk3"))
                skill.Variable.Remove("RobotAtkUp_min_atk3");
            skill.Variable.Add("RobotAtkUp_min_atk3", min_atk3_add);
            pet.Status.min_atk3 += (ushort)min_atk3_add;

            //最大魔攻
            int max_matk_add = (int)(pet.Status.max_matk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_max_matk"))
                skill.Variable.Remove("RobotAtkUp_max_matk");
            skill.Variable.Add("RobotAtkUp_max_matk", max_matk_add);
            pet.Status.max_matk += (ushort)max_matk_add;

            //最小魔攻
            int min_matk_add = (int)(pet.Status.min_matk_bs * (0.05f + 0.1f * level));
            if (skill.Variable.ContainsKey("RobotAtkUp_min_matk"))
                skill.Variable.Remove("RobotAtkUp_min_matk");
            skill.Variable.Add("RobotAtkUp_min_matk", min_matk_add);
            pet.Status.min_matk += (ushort)min_matk_add;
            actor.Buff.OverTune = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    //最大攻擊
                    pet.Status.max_atk1 -= (ushort)skill.Variable["RobotAtkUp_max_atk1"];

                    //最大攻擊
                    pet.Status.max_atk2 -= (ushort)skill.Variable["RobotAtkUp_max_atk2"];

                    //最大攻擊
                    pet.Status.max_atk3 -= (ushort)skill.Variable["RobotAtkUp_max_atk3"];

                    //最小攻擊
                    pet.Status.min_atk1 -= (ushort)skill.Variable["RobotAtkUp_min_atk1"];

                    //最小攻擊
                    pet.Status.min_atk2 -= (ushort)skill.Variable["RobotAtkUp_min_atk2"];

                    //最小攻擊
                    pet.Status.min_atk3 -= (ushort)skill.Variable["RobotAtkUp_min_atk3"];

                    //最大魔攻
                    pet.Status.max_matk -= (ushort)skill.Variable["RobotAtkUp_max_matk"];

                    //最小魔攻
                    pet.Status.min_matk -= (ushort)skill.Variable["RobotAtkUp_min_matk"];
                }
            }

            actor.Buff.OverTune = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}

