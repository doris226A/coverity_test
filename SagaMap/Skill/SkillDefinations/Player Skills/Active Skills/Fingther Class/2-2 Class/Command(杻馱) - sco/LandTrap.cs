﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 設置地雷（ランドマイン）J9
    /// </summary>
    public class LandTrap : Trap
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            uint itemID = 10022308;
            if (SkillHandler.Instance.CountItem(sActor, itemID) > 0)
            {
                SkillHandler.Instance.TakeItem(sActor, itemID, 1);
                return 0;
            }
            return -12;
        }

        public LandTrap()
            : base(true, 100, PosType.sActor)
        {

        }
        public override void BeforeProc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.dActor = 0;
            LifeTime = 60000;
        }
        public override void ProcSkill(Actor sActor, Actor mActor, ActorSkill actor, SkillArg args, Map map, int level, float factor)
        {
            factor = 1.5f + 1f * level;
            List<Actor> affected = map.GetActorsArea(actor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            if (!realAffected.Contains(mActor))
            {
                realAffected.Add(mActor);
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factor);
        }
    }
}