﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;

namespace SagaMap.Skill.SkillDefinations.BladeMaster
{
    public class Sinkuha : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (CheckPossible(pc))
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(pc, dActor))
                {
                    return 0;
                }
                else
                {
                    return -14;
                }
            }
            else
            {
                return -5;
            }
        }

        bool CheckPossible(Actor sActor)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND) || pc.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0;
            if (CheckPossible(sActor))
            {
                args.type = ATTACK_TYPE.BLOW;
                //args.argType = SkillArg.ArgType.Attack;
                factor = 1.0f + 0.2f * level;
                List<Actor> actors = new List<Actor>();

                if (level == 6)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        actors.Add(dActor);
                        
                    }
                }
                else
                    actors.Add(dActor);
                SkillHandler.Instance.PhysicalAttack(sActor, actors, args, sActor.WeaponElement, factor);

            }
        }

        #endregion
    }
}

