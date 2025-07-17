using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.Skill.SkillDefinations.Monster;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// イクスパンジアーム
    /// </summary>
    public class IkspiariArmusing : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }



        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*bool cure = false;
            if (SagaLib.Global.Random.Next(0, 99) < 70)
            {
                cure = true;
            }
            if (cure)
            {
                RemoveAddition(sActor, "Poison");
                RemoveAddition(sActor, "MoveSpeedDown");
                RemoveAddition(sActor, "MoveSpeedDown2");
                RemoveAddition(sActor, "Stone");
                RemoveAddition(sActor, "Silence");
                RemoveAddition(sActor, "Stun");
                RemoveAddition(sActor, "Sleep");
                RemoveAddition(sActor, "Frosen");
                RemoveAddition(sActor, "Confuse");
            }
            float factor = 1.4f + 0.3f * level;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);*/

            if (SagaLib.Global.Random.Next(0, 99) < 70)
            {
                string[] no = { "DecreaseShield", "DecreaseWeapon", "FireWeapon", "WindWeapon", "WaterWeapon", "EarthWeapon", "DarkWeapon", "HolyWeapon", "EarthShield", "FireShield", "WaterShield", "WindShield", "DarkShield", "HolyShield" };
                int rate = 30 + 10 * level;
                if (SagaLib.Global.Random.Next(0, 99) < rate)
                {
                    List<string> adds = new List<string>();
                    foreach (System.Collections.Generic.KeyValuePair<string, SagaDB.Actor.Addition> ad in dActor.Status.Additions)
                    {
                        if (!(ad.Value is DefaultPassiveSkill))
                        {
                            adds.Add(ad.Value.Name);
                            foreach (string i in no)
                            {
                                if (ad.Value.Name == i)
                                    adds.Remove(ad.Value.Name);
                            }
                        }
                    }
                    foreach (string adn in adds)
                    {
                        SkillHandler.RemoveAddition(dActor, adn);
                    }
                }
            }


            float factor = 1.4f + 0.3f * level;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
        }


        /*public void RemoveAddition(Actor actor, String additionName)
        {
            if (actor.Status.Additions.ContainsKey(additionName))
            {
                Addition addition = actor.Status.Additions[additionName];
                actor.Status.Additions.TryRemove(additionName, out _);
                if (addition.Activated)
                {
                    addition.AdditionEnd();
                }
                addition.Activated = false;
            }
        }*/
        #endregion
    }
}
