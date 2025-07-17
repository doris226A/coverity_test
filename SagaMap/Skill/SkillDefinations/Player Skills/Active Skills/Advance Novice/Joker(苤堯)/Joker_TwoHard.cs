using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Manager;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// Joker_2雙手
    /// </summary>
    public class Joker_TwoHead : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*float factor = new float[] { 0, 8.5f, 10.5f, 12.5f, 14.5f, 16.5f }[level];

            List<Actor> actors = Manager.MapManager.Instance.GetMap(sActor.MapID).GetActorsArea(dActor, 250, true);
            List<Actor> affected = new List<Actor>();
            int elements;
            if (sActor.WeaponElement != SagaLib.Elements.Neutral)
            {
                elements = sActor.Status.attackElements_item[sActor.WeaponElement]
                                    + sActor.Status.attackElements_skill[sActor.WeaponElement]
                                    + sActor.Status.attackelements_iris[sActor.WeaponElement];
            }
            else
            {
                elements = 0;
            }
            float rete = new float[] { 0, 0.01f, 0.03f, 0.05f, 0.07f, 0.1f }[level];
            foreach (var item in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, item))
                {
                    
                    int dmg = SkillHandler.Instance.CalcDamage(true, sActor, dActor, args, SkillHandler.DefType.Def, sActor.WeaponElement, elements, factor);
                    SkillHandler.Instance.CauseDamage(sActor, dActor, dmg);
                    SkillHandler.Instance.ShowVessel(dActor, dmg);
                }
            }*/
            //args.type = ATTACK_TYPE.SLASH;
            //SkillHandler.Instance.PhysicalAttack(sActor, affected, args, sActor.WeaponElement, factor);

            float factor = new float[] { 0, 8.5f, 10.5f, 12.5f, 14.5f, 16.5f }[level];

            List<Actor> actors = Manager.MapManager.Instance.GetMap(sActor.MapID).GetActorsArea(dActor, 250, true);
            List<Actor> affected = new List<Actor>();
            int elements;
            if (sActor.WeaponElement != SagaLib.Elements.Neutral)
            {
                elements = sActor.Status.attackElements_item[sActor.WeaponElement]
                                    + sActor.Status.attackElements_skill[sActor.WeaponElement]
                                    + sActor.Status.attackelements_iris[sActor.WeaponElement];
            }
            else
            {
                elements = 0;
            }
            float rete = new float[] { 0, 0.01f, 0.03f, 0.05f, 0.07f, 0.1f }[level];
            foreach (var item in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, item))
                {

                    int dmg = SkillHandler.Instance.CalcDamage(true, sActor, item, args, SkillHandler.DefType.Def, sActor.WeaponElement, elements, factor);
                    SkillHandler.Instance.CauseDamage(sActor, item, dmg);
                    SkillHandler.Instance.ShowVessel(item, dmg);

                    int heal = (int)(dmg * rete);
                    if (sActor.MaxHP < (sActor.HP + heal))
                        sActor.HP = sActor.MaxHP;
                    else
                        sActor.HP += (uint)heal;
                    SkillHandler.Instance.ShowVessel(sActor, -heal);
                    //MapManager.Instance.GetMap(dActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, dActor, true);
                }
            }
        }
    }
}
