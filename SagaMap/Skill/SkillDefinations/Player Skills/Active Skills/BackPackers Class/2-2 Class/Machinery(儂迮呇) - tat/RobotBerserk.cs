
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Machinery
{
    /// <summary>
    /// 備戰疾走（オーバーラン）
    /// </summary>
    public class RobotBerserk : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(sActor);
            if (pet == null)
            {
                return -54;//需回傳"需裝備寵物"
            }
            if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
            {
                //return 0;
                return 0;
            }
            return -54;//需回傳"需裝備寵物"
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*int lifetime=10000*level;
            Berserk skill = new Berserk(args.skill, sActor, lifetime);
            SkillHandler.ApplyAddition(sActor, skill);*/
            int lifetime = 10000 * level;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "RobotBerserk", lifetime, 1000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            skill.OnUpdate += this.Update;
            SkillHandler.ApplyAddition(sActor, skill);
        }

        void Update(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (!SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    actor.Status.Additions["RobotBerserk"].AdditionEnd();
                    actor.Status.Additions.TryRemove("RobotBerserk", out _);
                }
            }
            else
            {
                actor.Status.Additions["RobotBerserk"].AdditionEnd();
                actor.Status.Additions.TryRemove("RobotBerserk", out _);
            }

        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int level = skill.skill.Level;
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            //最大攻擊
            int max_atk1_add = (int)(pet.Status.max_atk_bs);
            if (skill.Variable.ContainsKey("Berserk_max_atk1"))
                skill.Variable.Remove("Berserk_max_atk1");
            skill.Variable.Add("Berserk_max_atk1", max_atk1_add);
            actor.Status.max_atk1_skill += (short)max_atk1_add;

            //最大攻擊
            int max_atk2_add = (int)(pet.Status.max_atk_bs);
            if (skill.Variable.ContainsKey("Berserk_max_atk2"))
                skill.Variable.Remove("Berserk_max_atk2");
            skill.Variable.Add("Berserk_max_atk2", max_atk2_add);
            actor.Status.max_atk2_skill += (short)max_atk2_add;

            //最大攻擊
            int max_atk3_add = (int)(pet.Status.max_atk_bs);
            if (skill.Variable.ContainsKey("Berserk_max_atk3"))
                skill.Variable.Remove("Berserk_max_atk3");
            skill.Variable.Add("Berserk_max_atk3", max_atk3_add);
            actor.Status.max_atk3_skill += (short)max_atk3_add;

            //最小攻擊
            int min_atk1_add = (int)(pet.Status.min_atk_bs);
            if (skill.Variable.ContainsKey("Berserk_min_atk1"))
                skill.Variable.Remove("Berserk_min_atk1");
            skill.Variable.Add("Berserk_min_atk1", min_atk1_add);
            actor.Status.min_atk1_skill += (short)min_atk1_add;

            //最小攻擊
            int min_atk2_add = (int)(pet.Status.min_atk_bs);
            if (skill.Variable.ContainsKey("Berserk_min_atk2"))
                skill.Variable.Remove("Berserk_min_atk2");
            skill.Variable.Add("Berserk_min_atk2", min_atk2_add);
            actor.Status.min_atk2_skill += (short)min_atk2_add;

            //最小攻擊
            int min_atk3_add = (int)(pet.Status.min_atk_bs);
            if (skill.Variable.ContainsKey("Berserk_min_atk3"))
                skill.Variable.Remove("Berserk_min_atk3");
            skill.Variable.Add("Berserk_min_atk3", min_atk3_add);
            actor.Status.min_atk3_skill += (short)min_atk3_add;


            //右防禦
            int def_add_add = -(int)(pet.Status.def_add);
            if (skill.Variable.ContainsKey("Berserk_def_add"))
                skill.Variable.Remove("Berserk_def_add");
            skill.Variable.Add("Berserk_def_add", def_add_add);
            actor.Status.def_add_skill += (short)def_add_add;

            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.Berserker = true;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            if (pet != null)
            {
                if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                {
                    //最大攻擊
                    actor.Status.max_atk1_skill -= (short)skill.Variable["Berserk_max_atk1"];

                    //最大攻擊
                    actor.Status.max_atk2_skill -= (short)skill.Variable["Berserk_max_atk2"];

                    //最大攻擊
                    actor.Status.max_atk3_skill -= (short)skill.Variable["Berserk_max_atk3"];

                    //最小攻擊
                    actor.Status.min_atk1_skill -= (short)skill.Variable["Berserk_min_atk1"];

                    //最小攻擊
                    actor.Status.min_atk2_skill -= (short)skill.Variable["Berserk_min_atk2"];

                    //最小攻擊
                    actor.Status.min_atk3_skill -= (short)skill.Variable["Berserk_min_atk3"];

                    //右防禦
                    actor.Status.def_add_skill -= (short)skill.Variable["Berserk_def_add"];


                }
            }
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.Berserker = false;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}