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
    class RobotDefUp : ISkill
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
            int lifetime = 1800000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "RobotDefUp", lifetime, 1000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            skill.OnUpdate += this.Update;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        #endregion

        void Update(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (!SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    actor.Status.Additions["RobotDefUp"].AdditionEnd();
                    actor.Status.Additions.TryRemove("RobotDefUp", out _);
                }
            }
            else
            {
                actor.Status.Additions["RobotDefUp"].AdditionEnd();
                actor.Status.Additions.TryRemove("RobotDefUp", out _);
            }

        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            float rank = new float[] { 0, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f }[skill.skill.Level];
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (skill.Variable.ContainsKey("RobotDefUp"))
                skill.Variable.Remove("RobotDefUp");
            skill.Variable.Add("RobotDefUp", (int)(pet.Status.def_add * rank));
            pet.Status.def_add += (short)(pet.Status.def_add * rank);

            float rank2 = new float[] { 0, 0.02f, 0.04f, 0.06f, 0.08f, 0.1f }[skill.skill.Level];
            if (skill.Variable.ContainsKey("RobotDefUp2"))
                skill.Variable.Remove("RobotDefUp2");
            if ((pet.Status.def + pet.Status.def * rank2) <= 90)
            {
                skill.Variable.Add("RobotDefUp2", (int)(pet.Status.def * rank2));
                pet.Status.def += (ushort)(pet.Status.def * rank2);
            }
            else if ((pet.Status.def + pet.Status.def * rank2) > 90)
            {
                skill.Variable.Add("RobotDefUp2", (int)Math.Max(90 - pet.Status.def, 0));
                pet.Status.def += (ushort)Math.Max(90 - pet.Status.def, 0);
            }
                

            


            actor.Buff.三转铁匠2足DEFUP = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    pet.Status.def_add -= (short)(skill.Variable["RobotDefUp"]);
                    pet.Status.def -= (ushort)(skill.Variable["RobotDefUp2"]);
                }
            }


            actor.Buff.三转铁匠2足DEFUP = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
//if (i.Status.Additions.ContainsKey("イレイザー") 