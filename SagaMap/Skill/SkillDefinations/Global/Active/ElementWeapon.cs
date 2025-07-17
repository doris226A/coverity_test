using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Global
{
    public class ElementWeapon : ISkill
    {
        public Elements element;
        public ElementWeapon(Elements e)
        {
            element = e;
        }
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (dActor.Status.Additions.ContainsKey("YugenKeiyaku"))
                return;

            if (dActor.Status.Additions.ContainsKey("FireWeapon"))
                SkillHandler.RemoveAddition(dActor, "FireWeapon");
            if (dActor.Status.Additions.ContainsKey("WaterWeapon"))
                SkillHandler.RemoveAddition(dActor, "WaterWeapon");
            if (dActor.Status.Additions.ContainsKey("WindWeapon"))
                SkillHandler.RemoveAddition(dActor, "WindWeapon");
            if (dActor.Status.Additions.ContainsKey("EarthWeapon"))
                SkillHandler.RemoveAddition(dActor, "EarthWeapon");
            if (dActor.Status.Additions.ContainsKey("DarkWeapon"))
                SkillHandler.RemoveAddition(dActor, "DarkWeapon");
            if (dActor.Status.Additions.ContainsKey("HolyWeapon"))
                SkillHandler.RemoveAddition(dActor, "HolyWeapon");

            dActor.Buff.FireWeapon = false;
            dActor.Buff.WaterWeapon = false;
            dActor.Buff.WindWeapon = false;
            dActor.Buff.EarthWeapon = false;
            dActor.Buff.HolyWeapon = false;
            dActor.Buff.DarkWeapon = false;


            if (dActor.Status.ElementSoul.ContainsKey(element))
            {
                if (dActor.Status.Additions.ContainsKey(element.ToString() + "Weapon"))
                    SkillHandler.RemoveAddition(dActor, element.ToString() + "Weapon");
                if (dActor.Status.Additions.ContainsKey(element.ToString() + "Shield"))
                    SkillHandler.RemoveAddition(dActor, element.ToString() + "Shield");
                if (dActor.Status.Additions.ContainsKey("DarkWeapon"))
                    SkillHandler.RemoveAddition(dActor, "DarkWeapon");
                if (dActor.Status.Additions.ContainsKey("HolyWeapon"))
                    SkillHandler.RemoveAddition(dActor, "HolyWeapon");
                if (dActor.Status.Additions.ContainsKey("HolyShield"))
                    SkillHandler.RemoveAddition(dActor, "HolyShield");
                if (dActor.Status.Additions.ContainsKey("DarkShield"))
                    SkillHandler.RemoveAddition(dActor, "DarkShield");
            }

            DefaultBuff skill = new DefaultBuff(args.skill, dActor, element.ToString() + "Weapon", 50000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            //技能基础增加攻击属性值
            //int atk = 30;
            int atk = 5 + skill.skill.Level * 4;
            //2转元素契约额外追加攻击属性值
            if (actor.Status.ElementContract.ContainsKey(element))
                atk += 30;

            //反射获取buff对象
            Type type = actor.Buff.GetType();
            System.Reflection.PropertyInfo propertyInfo = null;

            //2-2元素从者额外追加攻击属性值和防御属性值,并且增加元素盾buff
            if (actor.Status.ElementSoul.ContainsKey(element))
            {
                var def = 0;
                int atk1 = 0;
                //计算攻击元素属性值额外追加数值
                atk1 += 30;
                //计算防御元素属性值额外追加数值
                def += (5 * (actor.Status.ElementSoul[element] - 1));

                //保存并设置增加元素防御属性值参数
                if (skill.Variable.ContainsKey("ElementWeapon_Shield"))
                    skill.Variable.Remove("ElementWeapon_Shield");
                skill.Variable.Add("ElementWeapon_Shield", def);
                actor.Status.elements_skill[element] += def;

                if (skill.Variable.ContainsKey("ElementShield_Weapon"))
                    skill.Variable.Remove("ElementShield_Weapon");
                skill.Variable.Add("ElementShield_Weapon", atk1);
                actor.Status.attackElements_skill[element] += atk1;
                if (skill.Variable.ContainsKey("ElementShield_Weapon1"))
                    skill.Variable.Remove("ElementShield_Weapon1");
                skill.Variable.Add("ElementShield_Weapon1", atk);
                actor.Status.attackElements_skill[element] += atk;

                //反射增加元素盾buff
                propertyInfo = type.GetProperty(element.ToString() + "Shield");
                propertyInfo.SetValue(actor.Buff, true, null);

                //反射增加元素武器buff
                propertyInfo = type.GetProperty(element.ToString() + "Weapon");
                propertyInfo.SetValue(actor.Buff, true, null);
            }
            else
            {
                //保存并设置增加元素攻击属性值参数
                if (skill.Variable.ContainsKey("ElementShield_Weapon"))
                    skill.Variable.Remove("ElementShield_Weapon");
                skill.Variable.Add("ElementShield_Weapon", atk);
                actor.Status.attackElements_skill[element] += atk;

                //反射增加元素武器buff
                propertyInfo = type.GetProperty(element.ToString() + "Weapon");
                propertyInfo.SetValue(actor.Buff, true, null);
            }

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.Status.Additions.ContainsKey("YugenKeiyaku"))
            {
                SkillHandler.RemoveAddition(actor, "YugenKeiyaku");
            }

            //反射获取buff对象
            Type type = actor.Buff.GetType();
            System.Reflection.PropertyInfo propertyInfo = null;

            //把內容移出來
            if (skill.Variable.ContainsKey("ElementWeapon_Shield"))
            {
                actor.Status.elements_skill[element] -= skill.Variable["ElementWeapon_Shield"];
                //反射移除元素盾buff
                propertyInfo = type.GetProperty(element.ToString() + "Shield");
                propertyInfo.SetValue(actor.Buff, false, null);
            }

            if (skill.Variable.ContainsKey("ElementShield_Weapon"))
            {
                actor.Status.attackElements_skill[element] -= skill.Variable["ElementShield_Weapon"];
                //反射移除元素武器buff
                propertyInfo = type.GetProperty(element.ToString() + "Weapon");
                propertyInfo.SetValue(actor.Buff, false, null);
            }

            if (skill.Variable.ContainsKey("ElementShield_Weapon1"))
            {
                actor.Status.attackElements_skill[element] -= skill.Variable["ElementShield_Weapon1"];
                //反射移除元素武器buff
                propertyInfo = type.GetProperty(element.ToString() + "Weapon");
                propertyInfo.SetValue(actor.Buff, false, null);
            }

            //死亡時ElementSoul會先被刪除，導致讀不到，只要把內容移出來就可以了
            //如果目标已习得对应的元素从者技能, 则移除对应的元素防御属性值和元素盾BUFF图标
            /*if (actor.Status.ElementSoul.ContainsKey(element))
            {
                if (skill.Variable.ContainsKey("ElementWeapon_Shield"))
                    actor.Status.elements_skill[element] -= skill.Variable["ElementWeapon_Shield"];
                if (skill.Variable.ContainsKey("ElementShield_Weapon"))
                    actor.Status.attackElements_skill[element] -= skill.Variable["ElementShield_Weapon"];
                if (skill.Variable.ContainsKey("ElementShield_Weapon1"))
                    actor.Status.attackElements_skill[element] -= skill.Variable["ElementShield_Weapon1"];

                //反射移除元素盾buff
                propertyInfo = type.GetProperty(element.ToString() + "Shield");
                propertyInfo.SetValue(actor.Buff, false, null);


                //反射移除元素武器buff
                propertyInfo = type.GetProperty(element.ToString() + "Weapon");
                propertyInfo.SetValue(actor.Buff, false, null);
            }
            else
            {
                //移除元素武器对应的元素攻击属性值
                if (skill.Variable.ContainsKey("ElementShield_Weapon"))
                    actor.Status.attackElements_skill[element] -= skill.Variable["ElementShield_Weapon"]; ;
                //反射移除元素武器buff
                propertyInfo = type.GetProperty(element.ToString() + "Weapon");
                propertyInfo.SetValue(actor.Buff, false, null);
            }*/


            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
