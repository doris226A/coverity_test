using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;


namespace SagaMap.Skill.SkillDefinations.Maestro
{
    class RobotAtkUp : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (pc.Status.Additions.ContainsKey("LimitExceed"))
            {
                return -12;
            }
            ActorPet pet = SkillHandler.Instance.GetPet(pc);
            if (pet == null)
            {
                return -54;//需回傳"需裝備寵物"
            }
            if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
            {
                return 0;
            }
            return -54;//需回傳"需裝備寵物"
        }


        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 180000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "RobotAtkUp", lifetime, 1000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            skill.OnUpdate += this.Update;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        #endregion


        void Update(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (!SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    actor.Status.Additions["RobotAtkUp"].AdditionEnd();
                    actor.Status.Additions.TryRemove("RobotAtkUp", out _);
                }
            }
            else
            {
                actor.Status.Additions["RobotAtkUp"].AdditionEnd();
                actor.Status.Additions.TryRemove("RobotAtkUp", out _);
            }

        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            float rank = new float[] { 0, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f }[skill.skill.Level];
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (skill.Variable.ContainsKey("RobotAtkUp"))
                skill.Variable.Remove("RobotAtkUp");
            skill.Variable.Add("RobotAtkUp1", (int)(pet.Status.max_atk1 * rank));
            pet.Status.max_atk1 += (ushort)(pet.Status.max_atk_bs * rank);

            if (skill.Variable.ContainsKey("RobotAtkUp2"))
                skill.Variable.Remove("RobotAtkUp2");
            skill.Variable.Add("RobotAtkUp2", (int)(pet.Status.max_atk2 * rank));
            pet.Status.max_atk2 += (ushort)(pet.Status.max_atk_bs * rank);

            if (skill.Variable.ContainsKey("RobotAtkUp3"))
                skill.Variable.Remove("RobotAtkUp3");
            skill.Variable.Add("RobotAtkUp3", (int)(pet.Status.max_atk3 * rank));
            pet.Status.max_atk3 += (ushort)(pet.Status.max_atk_bs * rank);


            if (skill.Variable.ContainsKey("RobotAtkUp4"))
                skill.Variable.Remove("RobotAtkUp4");
            skill.Variable.Add("RobotAtkUp4", (int)(pet.Status.min_atk1 * rank));
            pet.Status.min_atk1 += (ushort)(pet.Status.min_atk_bs * rank);

            if (skill.Variable.ContainsKey("RobotAtkUp5"))
                skill.Variable.Remove("RobotAtkUp5");
            skill.Variable.Add("RobotAtkUp5", (int)(pet.Status.min_atk2 * rank));
            pet.Status.min_atk2 += (ushort)(pet.Status.min_atk_bs * rank);

            if (skill.Variable.ContainsKey("RobotAtkUp6"))
                skill.Variable.Remove("RobotAtkUp6");
            skill.Variable.Add("RobotAtkUp6", (int)(pet.Status.min_atk3 * rank));
            pet.Status.min_atk3 += (ushort)(pet.Status.min_atk_bs * rank);
            actor.Buff.三转2足ATKUP = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    pet.Status.max_atk1 -= (ushort)(skill.Variable["RobotAtkUp1"]);
                    pet.Status.max_atk2 -= (ushort)(skill.Variable["RobotAtkUp2"]);
                    pet.Status.max_atk3 -= (ushort)(skill.Variable["RobotAtkUp3"]);

                    pet.Status.min_atk1 -= (ushort)(skill.Variable["RobotAtkUp4"]);
                    pet.Status.min_atk2 -= (ushort)(skill.Variable["RobotAtkUp5"]);
                    pet.Status.min_atk3 -= (ushort)(skill.Variable["RobotAtkUp6"]);
                }
            }
            actor.Buff.三转2足ATKUP = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
//if (i.Status.Additions.ContainsKey("イレイザー") 