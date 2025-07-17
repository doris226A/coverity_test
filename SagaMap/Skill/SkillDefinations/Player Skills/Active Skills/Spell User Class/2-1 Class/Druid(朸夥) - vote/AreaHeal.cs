using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Druid
{
    /// <summary>
    /// 範圍治癒（エリアヒール）
    /// </summary>
    public class AreaHeal : ISkill
    {
        bool MobUse;
        public AreaHeal()
        {
            this.MobUse = false;
        }
        public AreaHeal(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (MobUse)
            {
                level = 5;
            }
            float factor = -(1f + 0.4f * level);
            float damagefactor = 1f + 0.4f * level;

            if (sActor.Status.Additions.ContainsKey("Cardinal"))//3转10技提升治疗量
                factor += sActor.Status.Cardinal_Rank;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            /*if(sActor.type==ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                foreach (Actor act in affected)
                {
                    if (act.type == ActorType.PC || act.type == ActorType.PET || act.type == ActorType.SHADOW)
                    {
                        if (pc.PossessionTarget == 0)
                        {
                            realAffected.Add(act);
                        }
                    }
                }
            }*/
            List<Actor> damageaffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (act.Buff.Undead)
                {
                    damageaffected.Add(act);
                    continue;
                }
                if (act.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)act;
                    //為騎士團新增
                    if (sActor.type == ActorType.PC)
                    {
                        ActorPC mypc = (ActorPC)sActor;
                        if (pc.Mode != mypc.Mode)
                            continue;
                    }
                    if (pc.PossessionTarget == 0)
                    {
                        realAffected.Add(act);
                    }
                }
                if (act.type == ActorType.PET || act.type == ActorType.PARTNER || act.type == ActorType.SHADOW)
                {
                    realAffected.Add(act);
                }
                if (act.type == ActorType.MOB)
                {
                    ActorMob m = (ActorMob)act;
                    if (m.BaseData.undead)
                        damageaffected.Add(act);
                }
            }

            SkillHandler.Instance.MagicAttack(sActor, damageaffected, args, SagaLib.Elements.Holy, damagefactor);
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SkillHandler.DefType.IgnoreAll, SagaLib.Elements.Holy, factor);
        }

        #endregion
    }
}
