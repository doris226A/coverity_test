using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    /// <summary>
    /// 槲寄生射击(ヤドリギショット)
    /// </summary>
    public class YHHMistletoeShooting : ISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float damagefactor = 3.0f;
            int dmgend = 0;
            float healcut = 0.1f;

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(dActor.X, dActor.Y, 300, null);
            List<Actor> recoveraffected = new List<Actor>();//治疗集合
            foreach (Actor act in affected)
            {
                if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act) && !act.Buff.NoRegen)
                {
                    recoveraffected.Add(act);
                }
            }
            SkillHandler.Instance.ShowEffectByActor(sActor, 3096);
            int healend = 0;
            dmgend = (int)(SkillHandler.Instance.CalcDamage(true, sActor, dActor, args, SkillHandler.DefType.MDef, sActor.WeaponElement, 0, damagefactor));
            healend = (int)(dmgend * healcut);
            SkillHandler.Instance.FixAttack(sActor, recoveraffected, args, SagaLib.Elements.Holy, -healend);

            foreach (Actor i in recoveraffected)
            {
                if (i.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)i;
                    if (pc.PossessionTarget > 0)
                        continue;
                }
                i.HP += (uint)healend;
                if (i.HP >= i.MaxHP)
                {
                    i.HP = i.MaxHP;
                }
                SkillHandler.Instance.ShowVessel(i, -healend);
            }
            SkillHandler.Instance.ShowEffectByActor(dActor, 4392);
            SkillHandler.Instance.FixAttackNoref(sActor, dActor, args, sActor.WeaponElement, dmgend);
        }

    }
}
