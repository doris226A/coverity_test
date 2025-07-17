using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Global
{
    public class ElementShield : ISkill
    {
        public Elements element;
        public bool monsteruse;
        public ElementShield(Elements e, bool ismonster = false)
        {
            element = e;
            monsteruse = ismonster;
        }
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (monsteruse)
            {
                level = 5;
                dActor = sActor;
            }

            if (dActor.Status.Additions.ContainsKey("FireShield"))
                SkillHandler.RemoveAddition(dActor, "FireShield");
            if (dActor.Status.Additions.ContainsKey("WaterShield"))
                SkillHandler.RemoveAddition(dActor, "WaterShield");
            if (dActor.Status.Additions.ContainsKey("WindShield"))
                SkillHandler.RemoveAddition(dActor, "WindShield");
            if (dActor.Status.Additions.ContainsKey("EarthShield"))
                SkillHandler.RemoveAddition(dActor, "EarthShield");
            if (dActor.Status.Additions.ContainsKey("HolyShield"))
                SkillHandler.RemoveAddition(dActor, "HolyShield");
            if (dActor.Status.Additions.ContainsKey("DarkShield"))
                SkillHandler.RemoveAddition(dActor, "DarkShield");

            dActor.Buff.FireShield = false;
            dActor.Buff.WaterShield = false;
            dActor.Buff.WindShield = false;
            dActor.Buff.EarthShield = false;
            dActor.Buff.HolyShield = false;
            dActor.Buff.DarkShield = false;

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

            int life = new int[] { 0, 15000, 35000, 60000, 100000, 150000 }[level];
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, element.ToString() + "Shield", life);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int def = skill.skill.Level * 5;

            int atk = 5 + skill.skill.Level * 4;

            if (actor.type == ActorType.MOB)
                def += 100;



            Type type = actor.Buff.GetType();
            System.Reflection.PropertyInfo propertyInfo = null;

            //2-2元素从者额外追加攻击属性值和防御属性值,并且增加元素武器buff
            if (actor.Status.ElementSoul.ContainsKey(element))
            {
                var atk1 = 0;
                int def1 = 0;
                //计算攻击元素属性值额外追加数值
                atk1 += ((25 * (actor.Status.ElementSoul[element] > 1 ? 1 : 0)) + (5 * (actor.Status.ElementSoul[element] > 1 ? 1 : 0) * (actor.Status.ElementSoul[element])));
                //计算防御元素属性值额外追加数值
                def1 += (5 * (actor.Status.ElementSoul[element] - 1));

                if (actor.Status.Additions.ContainsKey(element.ToString() + "Shield"))
                    SkillHandler.RemoveAddition(actor, element.ToString() + "Shield");
                if (actor.Status.Additions.ContainsKey(element.ToString() + "Weapon"))
                    SkillHandler.RemoveAddition(actor, element.ToString() + "Weapon");

                //保存并设置增加元素防御属性值参数
                if (skill.Variable.ContainsKey("ElementShield_Weapon"))
                    skill.Variable.Remove("ElementShield_Weapon");
                skill.Variable.Add("ElementShield_Weapon", atk1);
                actor.Status.attackElements_skill[element] += atk1;
                if (skill.Variable.ContainsKey("ElementShield_Weapon1"))
                    skill.Variable.Remove("ElementShield_Weapon1");
                skill.Variable.Add("ElementShield_Weapon1", atk);
                actor.Status.attackElements_skill[element] += atk;

                if (skill.Variable.ContainsKey("ElementWeapon_Shield"))
                    skill.Variable.Remove("ElementWeapon_Shield");
                skill.Variable.Add("ElementWeapon_Shield", def1);
                actor.Status.elements_skill[element] += def1;

                //反射增加元素盾buff
                propertyInfo = type.GetProperty(element.ToString() + "Shield");
                propertyInfo.SetValue(actor.Buff, true, null);

                //反射增加元素武器buff
                propertyInfo = type.GetProperty(element.ToString() + "Weapon");
                propertyInfo.SetValue(actor.Buff, true, null);
            }
            else
            {
                if (skill.Variable.ContainsKey("ElementWeapon_Shield"))
                    skill.Variable.Remove("ElementWeapon_Shield");
                skill.Variable.Add("ElementWeapon_Shield", def);
                actor.Status.elements_skill[element] += def;

                propertyInfo = type.GetProperty(element.ToString() + "Shield");
                propertyInfo.SetValue(actor.Buff, true, null);
            }
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {


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
                if (skill.Variable.ContainsKey("ElementWeapon_Shield"))
                    actor.Status.elements_skill[element] -= skill.Variable["ElementWeapon_Shield"];
                //反射移除元素盾buff
                propertyInfo = type.GetProperty(element.ToString() + "Shield");
                propertyInfo.SetValue(actor.Buff, false, null);
            }*/



            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
