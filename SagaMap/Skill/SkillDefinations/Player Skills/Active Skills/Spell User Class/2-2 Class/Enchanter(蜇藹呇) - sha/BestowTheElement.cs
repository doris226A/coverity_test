using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Enchanter
{
    public class BestowTheElement : ISkill
    {
        public Elements Weaponelement, Shieldelement;
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 400, true);
            Weaponelement = sActor.WeaponElement;
            Shieldelement = sActor.ShieldElement;
            foreach (Actor i in affected)
            {
                if (i.Buff.Dead == true) continue;

                if (i.ActorID == sActor.ActorID)
                    continue;
                if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {
                    if (Shieldelement != Elements.Neutral)//属性防护盾部分
                    {
                        if (i.Status.Additions.ContainsKey("HolyShield"))
                            SkillHandler.RemoveAddition(i, "HolyShield");
                        if (i.Status.Additions.ContainsKey("DarkShield"))
                            SkillHandler.RemoveAddition(i, "DarkShield");
                        if (i.Status.Additions.ContainsKey("FireShield"))
                            SkillHandler.RemoveAddition(i, "FireShield");
                        if (i.Status.Additions.ContainsKey("WaterShield"))
                            SkillHandler.RemoveAddition(i, "WaterShield");
                        if (i.Status.Additions.ContainsKey("WindShield"))
                            SkillHandler.RemoveAddition(i, "WindShield");
                        if (i.Status.Additions.ContainsKey("EarthShield"))
                            SkillHandler.RemoveAddition(i, "EarthShield");
                        i.Buff.DarkShield = false;
                        i.Buff.EarthShield = false;
                        i.Buff.FireShield = false;
                        i.Buff.WaterShield = false;
                        i.Buff.WindShield = false;
                        i.Buff.HolyShield = false;
                        int life = 60000;
                        DefaultBuff skill1 = new DefaultBuff(args.skill, i, Shieldelement.ToString() + "Shield", life);
                        skill1.OnAdditionStart += this.StartEventHandler1;
                        skill1.OnAdditionEnd += this.EndEventHandler1;
                        SkillHandler.ApplyAddition(i, skill1);
                    }
                    if(Weaponelement != Elements.Neutral)//属性武器部分
                    {
                        if (!i.Status.Additions.ContainsKey("YugenKeiyaku"))
                        {
                            if (i.Status.Additions.ContainsKey("FireWeapon"))
                                SkillHandler.RemoveAddition(i, "FireWeapon");
                            if (i.Status.Additions.ContainsKey("WaterWeapon"))
                                SkillHandler.RemoveAddition(i, "WaterWeapon");
                            if (i.Status.Additions.ContainsKey("WindWeapon"))
                                SkillHandler.RemoveAddition(i, "WindWeapon");
                            if (i.Status.Additions.ContainsKey("EarthWeapon"))
                                SkillHandler.RemoveAddition(i, "EarthWeapon");
                            if (i.Status.Additions.ContainsKey("DarkWeapon"))
                                SkillHandler.RemoveAddition(i, "DarkWeapon");
                            if (i.Status.Additions.ContainsKey("HolyWeapon"))
                                SkillHandler.RemoveAddition(i, "HolyWeapon");
                            i.Buff.DarkWeapon = false;
                            i.Buff.EarthWeapon = false;
                            i.Buff.FireWeapon = false;
                            i.Buff.HolyWeapon = false;
                            i.Buff.WaterWeapon = false;
                            i.Buff.WindWeapon = false;
                            DefaultBuff skill2 = new DefaultBuff(args.skill, i, Weaponelement.ToString() + "Weapon", 60000);
                            skill2.OnAdditionStart += this.StartEventHandler2;
                            skill2.OnAdditionEnd += this.EndEventHandler2;
                            SkillHandler.ApplyAddition(i, skill2);
                        }
                        
                    }
                    
                }
            }


        }
        void StartEventHandler2(Actor actor, DefaultBuff skill)
        {
            int atk = 25;
            uint SkillID = 0;
            if (actor.type == ActorType.PC)
            {
                switch (Weaponelement)
                {
                    case Elements.Earth:
                        SkillID = 939;
                        break;
                    case Elements.Water:
                        SkillID = 937;
                        break;
                    case Elements.Fire:
                        SkillID = 936;
                        break;
                    case Elements.Wind:
                        SkillID = 938;
                        break;
                    case Elements.Holy:
                        SkillID = 940;
                        break;
                    case Elements.Dark:
                        SkillID = 941;
                        break;
                    default:
                        SkillID = 0;
                        break;
                }
                ActorPC pc = (ActorPC)actor;
                if ((pc.Skills2_1.ContainsKey(934) || pc.DualJobSkill.Exists(x => x.ID == 934)) && Weaponelement == Elements.Holy)//GU2-1光明之魂
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 934))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 934).Level;


                    var mainlv = 0;
                    if (pc.Skills2_1.ContainsKey(934))
                        mainlv = pc.Skills2_1[934].Level;

                    atk += 25 + 5 * Math.Max(duallv, mainlv);
                }
                else if ((pc.Skills2_2.ContainsKey(935) || pc.DualJobSkill.Exists(x => x.ID == 935)) && Weaponelement == Elements.Dark)//GU2-2黑暗之魂
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 935))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 935).Level;


                    var mainlv = 0;
                    if (pc.Skills2_2.ContainsKey(935))
                        mainlv = pc.Skills2_2[935].Level;

                    atk += 25 + 5 * Math.Max(duallv, mainlv);
                }
                else if (pc.Skills2_1.ContainsKey(SkillID) || pc.DualJobSkill.Exists(x => x.ID == SkillID))
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == SkillID))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == SkillID).Level;


                    var mainlv = 0;
                    if (pc.Skills2_1.ContainsKey(SkillID))
                        mainlv = pc.Skills2_1[SkillID].Level;

                    atk += 5 * Math.Max(duallv, mainlv);
                }
            }







            if (skill.Variable.ContainsKey("ElementWeapon"))
                skill.Variable.Remove("ElementWeapon");
            skill.Variable.Add("ElementWeapon", atk);
            actor.Status.attackElements_skill[Weaponelement] += atk;
            Type type = actor.Buff.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(Weaponelement.ToString() + "Weapon");
            propertyInfo.SetValue(actor.Buff, true, null);
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler2(Actor actor, DefaultBuff skill)
        {
            if (actor.Status.Additions.ContainsKey("YugenKeiyaku"))
            {
                SkillHandler.RemoveAddition(actor, "YugenKeiyaku");
            }
            int value = skill.Variable["ElementWeapon"];
            actor.Status.attackElements_skill[Weaponelement] -= value;
            Type type = actor.Buff.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(Weaponelement.ToString() + "Weapon");
            propertyInfo.SetValue(actor.Buff, false, null);
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void StartEventHandler1(Actor actor, DefaultBuff skill)
        {
            int atk1 = 25;
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                if ((pc.Skills2_1.ContainsKey(934) || pc.DualJobSkill.Exists(x => x.ID == 934)) && Shieldelement == Elements.Holy)//GU2-1光明之魂
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 934))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 934).Level;


                    var mainlv = 0;
                    if (pc.Skills2_1.ContainsKey(934))
                        mainlv = pc.Skills2_1[934].Level;

                    atk1 += 5 * (Math.Max(duallv, mainlv) - 1);
                }
                else if ((pc.Skills2_2.ContainsKey(935) || pc.DualJobSkill.Exists(x => x.ID == 935)) && Shieldelement == Elements.Dark)//GU2-2黑暗之魂
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 935))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 935).Level;


                    var mainlv = 0;
                    if (pc.Skills2_2.ContainsKey(935))
                        mainlv = pc.Skills2_2[935].Level;

                    atk1 += 5 * (Math.Max(duallv, mainlv) - 1);
                }
            }
            if (skill.Variable.ContainsKey("ElementShield"))
                skill.Variable.Remove("ElementShield");
            skill.Variable.Add("ElementShield", atk1);
            actor.Status.elements_skill[Shieldelement] += atk1;

            Type type = actor.Buff.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(Shieldelement.ToString() + "Shield");
            propertyInfo.SetValue(actor.Buff, true, null);
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler1(Actor actor, DefaultBuff skill)
        {
            int value = skill.Variable["ElementShield"];
            actor.Status.elements_skill[Shieldelement] -= (short)value;
            if (actor.Status.Additions.ContainsKey("Amplement"))
            {
                if (Shieldelement != Elements.Dark && Shieldelement != Elements.Holy)
                    actor.Status.Additions["Amplement"].AdditionEnd();
            }
            Type type = actor.Buff.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(Shieldelement.ToString() + "Shield");
            propertyInfo.SetValue(actor.Buff, false, null);
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

    }


}
