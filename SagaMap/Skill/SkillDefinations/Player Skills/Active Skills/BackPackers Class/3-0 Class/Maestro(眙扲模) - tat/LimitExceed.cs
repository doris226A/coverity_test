
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Maestro
{
    /// <summary>
    /// レールガン
    /// </summary>
    public class LimitExceed : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.Status.Additions.ContainsKey("RobotAtkUp") ||
                sActor.Status.Additions.ContainsKey("RobotDefUp"))
            {
                return -12;
            }
            ActorPet pet = SkillHandler.Instance.GetPet(sActor);
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
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "LimitExceed", lifetime, 1000);
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
                    actor.Status.Additions["LimitExceed"].AdditionEnd();
                    actor.Status.Additions.TryRemove("LimitExceed", out _);
                }
            }
            else
            {
                actor.Status.Additions["LimitExceed"].AdditionEnd();
                actor.Status.Additions.TryRemove("LimitExceed", out _);
            }

        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {

            float trans_am_atk_up = 0.3f + 0.1f * skill.skill.Level;
            float trans_am_defadd_up = 0.3f + 0.1f * skill.skill.Level;
            float trans_am_def_up = 0.3f + 0.1f * skill.skill.Level;
            float defendup = 0;
            if (actor is ActorPC)
            {
                ActorPC pc = actor as ActorPC;

                if (pc.Skills3.ContainsKey(2489) || pc.DualJobSkill.Exists(x => x.ID == 2489))
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 2489))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 2489).Level;

                    var mainlv = 0;
                    if (pc.Skills3.ContainsKey(2489))
                        mainlv = pc.Skills3[2489].Level;

                    int maxlv = Math.Max(duallv, mainlv);
                    float atkendup = 0;
                    switch (skill.skill.Level)
                    {
                        case 1:
                            atkendup = 0.8f + 0.1f * maxlv;
                            trans_am_atk_up += atkendup;
                            break;
                        case 2:
                            atkendup = 1.05f + 0.1f * maxlv;
                            trans_am_atk_up += atkendup;
                            break;
                        case 3:
                            atkendup = 1.3f + 0.1f * maxlv;
                            trans_am_atk_up += atkendup;
                            break;
                        case 4:
                            atkendup = 1.8f + 0.1f * maxlv;
                            trans_am_atk_up += atkendup;
                            break;
                        case 5:
                            atkendup = 2.3f + 0.1f * maxlv;
                            trans_am_atk_up += atkendup;
                            break;
                    }
                }

                if (pc.Skills3.ContainsKey(2500) || pc.DualJobSkill.Exists(x => x.ID == 2500))
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 2500))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 2500).Level;

                    var mainlv = 0;
                    if (pc.Skills3.ContainsKey(2500))
                        mainlv = pc.Skills3[2500].Level;

                    int maxlv = Math.Max(duallv, mainlv);
                    float defaddendup = 0;
                    switch (skill.skill.Level)
                    {
                        case 1:
                            defaddendup = 0.8f + 0.1f * maxlv;
                            defendup = 0.32f + 0.1f * maxlv;
                            trans_am_defadd_up += defaddendup;
                            break;
                        case 2:
                            defaddendup = 1.05f + 0.1f * maxlv;
                            defendup = 0.34f + 0.1f * maxlv;
                            trans_am_defadd_up += defaddendup;
                            break;
                        case 3:
                            defaddendup = 1.3f + 0.1f * maxlv;
                            defendup = 0.36f + 0.1f * maxlv;
                            trans_am_defadd_up += defaddendup;
                            break;
                        case 4:
                            defaddendup = 1.8f + 0.1f * maxlv;
                            defendup = 0.38f + 0.1f * maxlv;
                            trans_am_defadd_up += defaddendup;
                            break;
                        case 5:
                            defaddendup = 2.3f + 0.1f * maxlv;
                            defendup = 0.4f + 0.1f * maxlv;
                            trans_am_defadd_up += defaddendup;
                            break;
                    }
                }


            }
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            int minatk1up = (int)(pet.Status.min_atk_bs * trans_am_atk_up);
            int minatk2up = (int)(pet.Status.min_atk_bs * trans_am_atk_up);
            int minatk3up = (int)(pet.Status.min_atk_bs * trans_am_atk_up);
            int maxatk1up = (int)(pet.Status.max_atk_bs * trans_am_atk_up);
            int maxatk2up = (int)(pet.Status.max_atk_bs * trans_am_atk_up);
            int maxatk3up = (int)(pet.Status.max_atk_bs * trans_am_atk_up);
            int defaddup = (int)(pet.Status.def_add * trans_am_defadd_up);
            int defup = (int)(pet.Status.def * defendup);
            if (skill.Variable.ContainsKey("Robotminatk1Up"))
                skill.Variable.Remove("Robotminatk1Up");
            skill.Variable.Add("Robotminatk1Up", minatk1up);
            pet.Status.min_atk1 += (ushort)minatk1up;
            if (skill.Variable.ContainsKey("Robotminatk2Up"))
                skill.Variable.Remove("Robotminatk2Up");
            skill.Variable.Add("Robotminatk2Up", minatk2up);
            pet.Status.min_atk2 += (ushort)minatk2up;
            if (skill.Variable.ContainsKey("Robotminatk3Up"))
                skill.Variable.Remove("Robotminatk3Up");
            skill.Variable.Add("Robotminatk3Up", minatk3up);
            pet.Status.min_atk3 += (ushort)minatk3up;

            if (skill.Variable.ContainsKey("Robotmaxatk1Up"))
                skill.Variable.Remove("Robotmaxatk1Up");
            skill.Variable.Add("Robotmaxatk1Up", maxatk1up);
            pet.Status.max_atk1 += (ushort)maxatk1up;
            if (skill.Variable.ContainsKey("Robotmaxatk2Up"))
                skill.Variable.Remove("Robotmaxatk2Up");
            skill.Variable.Add("Robotmaxatk2Up", maxatk2up);
            pet.Status.max_atk2 += (ushort)maxatk2up;
            if (skill.Variable.ContainsKey("Robotmaxatk3Up"))
                skill.Variable.Remove("Robotmaxatk3Up");
            skill.Variable.Add("Robotmaxatk3Up", maxatk3up);
            pet.Status.max_atk3 += (ushort)maxatk3up;

            if (skill.Variable.ContainsKey("RobotdefaddUp"))
                skill.Variable.Remove("RobotdefaddUp");
            skill.Variable.Add("RobotdefaddUp", defaddup);
            pet.Status.def_add += (short)defaddup;

            if (skill.Variable.ContainsKey("RobotdefUp"))
                skill.Variable.Remove("RobotdefUp");
            if ((pet.Status.def + pet.Status.def * defup) <= 90)
            {
                skill.Variable.Add("RobotdefUp", (int)(pet.Status.def * defup));
                pet.Status.def += (ushort)(pet.Status.def * defup);
            }
            else if ((pet.Status.def + pet.Status.def * defup) > 90)
            {
                skill.Variable.Add("RobotdefUp", (int)Math.Max(90 - pet.Status.def, 0));
                pet.Status.def += (ushort)Math.Max(90 - pet.Status.def, 0);
            }

            actor.Buff.三转2足ATKUP = true;
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
                    pet.Status.min_atk1 -= (ushort)(skill.Variable["Robotminatk1Up"]);
                    pet.Status.min_atk2 -= (ushort)(skill.Variable["Robotminatk2Up"]);
                    pet.Status.min_atk3 -= (ushort)(skill.Variable["Robotminatk3Up"]);
                    pet.Status.max_atk1 -= (ushort)(skill.Variable["Robotmaxatk1Up"]);
                    pet.Status.max_atk2 -= (ushort)(skill.Variable["Robotmaxatk2Up"]);
                    pet.Status.max_atk3 -= (ushort)(skill.Variable["Robotmaxatk3Up"]);
                    pet.Status.def_add -= (short)(skill.Variable["RobotdefaddUp"]);
                    pet.Status.def_skill -= (short)(skill.Variable["RobotdefUp"]);
                }
            }




            actor.Buff.三转2足ATKUP = false;
            actor.Buff.三转铁匠2足DEFUP = false;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
    }
}