
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Trader
{
    /// <summary>
    /// 連續重擊（ビートスマッシュ）
    /// </summary>
    public class BugRand : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.HANDBAG) || sActor.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, dActor))
                {
                    return 0;
                }
                else
                {
                    return -14;
                }
            }
            return -5;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.5f + 0.2f * level;
            int[] min_Times = { 1, 1, 1, 1, 2, 2 };
            int[] max_Times = { 1, 2, 2, 3, 3, 3 };
            int times = SagaLib.Global.Random.Next(0, 1) == 0 ? min_Times[level] : max_Times[level];

            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                if (pc.Skills3.ContainsKey(995))
                {
                    times += pc.Skills3[995].Level;
                    float factorbouns = 1 + (pc.Skills3[995].Level / 10);
                    factor = factor * factorbouns;
                }
            }

            List<Actor> dest = new List<Actor>();
            for (int i = 0; i < times; i++)
            {
                dest.Add(dActor);
            }
            args.argType = SkillArg.ArgType.Attack;
            args.type = ATTACK_TYPE.BLOW;
            args.delayRate = 4.5f;
            SkillHandler.Instance.PhysicalAttack(sActor, dest, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}