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
    class RobotASPDUp : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
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
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "RobotASPDUp", lifetime,1000);
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
                    actor.Status.Additions["RobotASPDUp"].AdditionEnd();
                    actor.Status.Additions.TryRemove("RobotASPDUp", out _);
                }
            }
            else
            {
                actor.Status.Additions["RobotASPDUp"].AdditionEnd();
                actor.Status.Additions.TryRemove("RobotASPDUp", out _);
            }

        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            float rank = new float[] { 0, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f }[skill.skill.Level];
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (skill.Variable.ContainsKey("RobotASPDUp"))
                skill.Variable.Remove("RobotASPDUp");
            if ((pet.Status.aspd + pet.Status.aspd * rank) <= 800)
            {
                skill.Variable.Add("RobotASPDUp", (int)(pet.Status.aspd * rank));
                pet.Status.aspd += (short)Math.Max(800 - pet.Status.aspd, 0);
            }
            else if ((pet.Status.aspd + pet.Status.aspd * rank) > 800)
            {
                skill.Variable.Add("RobotASPDUp", (int)Math.Max(800 - pet.Status.aspd, 0));
                pet.Status.aspd += (short)Math.Max(800 - pet.Status.aspd, 0);
            }

            if (skill.Variable.ContainsKey("RobotSHit"))
                skill.Variable.Remove("RobotSHit");
            if ((pet.Status.hit_melee + pet.Status.hit_melee * rank) <= 500)
            {
                skill.Variable.Add("RobotSHit", (int)(pet.Status.hit_melee * rank));
                pet.Status.hit_melee += (ushort)Math.Max(500 - pet.Status.hit_melee, 0);
            }
            else if ((pet.Status.hit_melee + pet.Status.hit_melee * rank) > 500)
            {
                skill.Variable.Add("RobotSHit", (int)Math.Max(500 - pet.Status.hit_melee, 0));
                pet.Status.hit_melee += (ushort)Math.Max(500 - pet.Status.hit_melee, 0);
            }

            if (skill.Variable.ContainsKey("RobotLHit"))
                skill.Variable.Remove("RobotLHit");
            if ((pet.Status.hit_ranged + pet.Status.hit_ranged * rank) <= 500)
            {
                skill.Variable.Add("RobotLHit", (int)(pet.Status.hit_ranged * rank));
                pet.Status.hit_ranged += (ushort)Math.Max(500 - pet.Status.hit_ranged, 0);
            }
            else if ((pet.Status.hit_ranged + pet.Status.hit_ranged * rank) > 500)
            {
                skill.Variable.Add("RobotLHit", (int)Math.Max(500 - pet.Status.hit_ranged, 0));
                pet.Status.hit_ranged += (ushort)Math.Max(500 - pet.Status.hit_ranged, 0);
            }

            if (skill.Variable.ContainsKey("RobotSAVOID"))
                skill.Variable.Remove("RobotSAVOID");
            if ((pet.Status.avoid_melee + pet.Status.avoid_melee * rank) <= 500)
            {
                skill.Variable.Add("RobotSAVOID", (int)(pet.Status.avoid_melee * rank));
                pet.Status.avoid_melee += (ushort)Math.Max(800 - pet.Status.avoid_melee, 0);
            }
            else if ((pet.Status.avoid_melee + pet.Status.avoid_melee * rank) > 500)
            {
                skill.Variable.Add("RobotSAVOID", (int)Math.Max(500 - pet.Status.avoid_melee, 0));
                pet.Status.avoid_melee += (ushort)Math.Max(500 - pet.Status.avoid_melee, 0);
            }

            if (skill.Variable.ContainsKey("RobotLAVOID"))
                skill.Variable.Remove("RobotLAVOID");
            if ((pet.Status.avoid_ranged + pet.Status.avoid_ranged * rank) <= 500)
            {
                skill.Variable.Add("RobotLAVOID", (int)(pet.Status.avoid_ranged * rank));
                pet.Status.avoid_ranged += (ushort)Math.Max(800 - pet.Status.avoid_ranged, 0);
            }
            else if ((pet.Status.avoid_ranged + pet.Status.avoid_ranged * rank) > 500)
            {
                skill.Variable.Add("RobotLAVOID", (int)Math.Max(500 - pet.Status.avoid_ranged, 0));
                pet.Status.avoid_ranged += (ushort)Math.Max(500 - pet.Status.avoid_ranged, 0);
            }

            actor.Buff.三转机器人攻速上升 = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    pet.Status.aspd -= (short)(skill.Variable["RobotASPDUp"]);
                    pet.Status.hit_melee -= (ushort)(skill.Variable["RobotSHit"]);
                    pet.Status.hit_ranged -= (ushort)(skill.Variable["RobotLHit"]);
                    pet.Status.avoid_melee -= (ushort)(skill.Variable["RobotSAVOID"]);
                    pet.Status.avoid_ranged -= (ushort)(skill.Variable["RobotLAVOID"]);
                }
            }
                    
            actor.Buff.三转机器人攻速上升 = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
//if (i.Status.Additions.ContainsKey("イレイザー") 