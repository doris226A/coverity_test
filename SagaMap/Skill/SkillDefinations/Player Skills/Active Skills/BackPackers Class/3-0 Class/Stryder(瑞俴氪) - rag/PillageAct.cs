using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Stryder
{
    /// <summary>
    /// ピレッジアクト
    /// </summary>
    public class PillageAct : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0;
            factor = new float[] { 0, 0.5f, 3.0f, 8.0f, 13.0f, 18.0f }[level];

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 200, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                    if (act.type == ActorType.MOB)
                    {
                        ActorMob mob = (ActorMob)act;
                        if (!dActor.Status.Additions.ContainsKey("Snatch") && !SkillHandler.Instance.isBossMob(mob))
                        {
                            int rate = 15 - 2 * level;
                            if (sActor.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)sActor;
                                if (pc.Skills2_2.ContainsKey(2372))
                                {
                                    rate += 10;
                                }
                            }
                            if (SagaLib.Global.Random.Next(0, 99) < rate)
                            {
                                int baseValue = 0, maxVlaue = 0;
                                if (mob.BaseData.dropItems.Count > 0)
                                {
                                    maxVlaue = baseValue = 0;
                                    bool oneshotdrop = false;
                                    int denominator = SagaLib.Global.Random.Next(1, mob.BaseData.dropItems.Sum(x => x.Rate));

                                    foreach (SagaDB.Mob.MobData.DropData i in mob.BaseData.dropItems)
                                    {
                                        if (oneshotdrop)
                                            continue;

                                        maxVlaue = baseValue + i.Rate;
                                        if (denominator >= baseValue && denominator < maxVlaue)
                                        {
                                            if (i.ItemID != 10000000)
                                            {
                                                SagaMap.Network.Client.MapClient client = SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)sActor);
                                                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(i.ItemID);
                                                client.AddItem(item, true);

                                                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Snatch", int.MaxValue);
                                                SkillHandler.ApplyAddition(dActor, skill);
                                                break;
                                            }
                                            else
                                            {
                                                foreach (SagaDB.Mob.MobData.DropData j in mob.BaseData.dropItems)
                                                {
                                                    if (i.ItemID != 10000000)
                                                    {
                                                        SagaMap.Network.Client.MapClient client = SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)sActor);
                                                        SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(i.ItemID);
                                                        client.AddItem(item, true);

                                                        DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Snatch", int.MaxValue);
                                                        SkillHandler.ApplyAddition(dActor, skill);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        baseValue = maxVlaue;
                                    }
                                }
                            }
                        }
                    }

                }

            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
