using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Item;
using SagaLib;

namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 定時炸彈（セットボム）J36
    /// </summary>
    public class SetBomb : ISkill
    {
        #region ISkill Members

        uint itemID = 10021901;//200耐久陷阱工具
        uint itemID2 = 10022307;//定時炸彈
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            SagaDB.Item.Item item1 = sActor.Inventory.GetItem(itemID, Inventory.SearchType.ITEM_ID);
            SagaDB.Item.Item item2 = sActor.Inventory.GetItem(itemID2, Inventory.SearchType.ITEM_ID);
            if (SkillHandler.Instance.CountItem(sActor, itemID2) > 0)
            {
                SkillHandler.Instance.TakeItem(sActor, itemID2, 1);
                return 0;
            }
            else if (SkillHandler.Instance.CountItem(sActor, itemID) > 0)
            {
                if (item1.Durability > 0)
                {
                    item1.Durability -= 1;
                    Network.Client.MapClient.FromActorPC((ActorPC)sActor).SendItemInfo(item1);
                    return 0;
                }
            }
            return -2;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float[] factors = { 0, 1.2f, 1.6f, 2.0f, 2.4f, 2.8f };
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factors[level]);
            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2378, level, 3000));

            ActivatorA timer = new ActivatorA(sActor, dActor, args);
            timer.Activate();
        }
        #endregion


        private class ActivatorA : MultiRunTask
        {
            Map map;
            Actor sActor;
            Actor dActor;
            SkillArg args;
            public ActivatorA(Actor caster, Actor actor, SkillArg args)
            {
                this.sActor = caster;
                this.dActor = actor;
                this.args = args;
                map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                this.period = 3000;
                this.dueTime = 3000;
            }

            public override void CallBack()
            {
                try
                {
                    if (dActor.HP > 0)
                    {
                        float[] factors = { 0, 4.0f, 4.5f, 5.0f, 5.5f, 6.0f };
                        List<Actor> affected = map.GetActorsArea(dActor, 150, true);
                        List<Actor> realAffected = new List<Actor>();
                        foreach (Actor act in affected)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                            {
                                realAffected.Add(act);
                            }
                        }
                        map.SendEffect(dActor, 5206);
                        Logger.ShowInfo("FLAG   " + args.flag.Count + args.flag[0].ToString());


                        SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factors[args.skill.Level]);

                        args.argType = SkillArg.ArgType.Attack;

                        args.flag[0] = AttackFlag.HP_DAMAGE;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, args, dActor, true);

                    }
                    this.Deactivate();
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                    this.Deactivate();
                }
            }
        }
    }
}
