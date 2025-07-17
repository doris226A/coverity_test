
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaMap.Skill.Additions.Global;
using SagaLib;
using SagaDB.Item;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 大型地雷（クレイモアトラップ）J30闊刀地雷
    /// </summary>
    public class ClayMore : Trap
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            uint itemID2 = 10021901;//200耐久陷阱工具
            uint itemID = 10022308;//地雷
            SagaDB.Item.Item item2 = sActor.Inventory.GetItem(itemID2, Inventory.SearchType.ITEM_ID);
            if (SkillHandler.Instance.CountItem(sActor, itemID) > 0)
            {
                SkillHandler.Instance.TakeItem(sActor, itemID, 1);
                return 0;
            }
            else if (SkillHandler.Instance.CountItem(sActor, itemID) > 0)
            {
                if (item2.Durability > 0)
                {
                    item2.Durability -= 1;
                    Network.Client.MapClient.FromActorPC((ActorPC)sActor).SendItemInfo(item2);
                    return 0;
                }
            }
            return -2;
        }

        public ClayMore()
            : base(true, 300, PosType.sActor)
        {

        }
        public override void BeforeProc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            LifeTime = 10000 + 10000 * level;
        }
        public override void ProcSkill(Actor sActor, Actor mActor, ActorSkill actor, SkillArg args, Map map, int level, float factor)
        {
            //int lifetime = 1500;

            //ClayMoreBuff skill = new ClayMoreBuff(args, sActor, actor, lifetime);
            //SkillHandler.ApplyAddition(sActor, skill);
            ActivatorA timer = new ActivatorA(sActor, actor, args);
            timer.Activate();
        }

        private class ActivatorA : MultiRunTask
        {
            Map map;
            Actor sActor;
            ActorSkill skillActor;
            SkillArg args;
            public ActivatorA(Actor caster, ActorSkill actor, SkillArg args)
            {
                this.sActor = caster;
                this.skillActor = actor;
                this.args = args;
                map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                this.period = 1500;
                this.dueTime = 1500;
            }

            public override void CallBack()
            {
                try
                {
                    float factor = 2.5f + 0.5f * args.skill.Level;
                    List<Actor> affected = map.GetActorsArea(skillActor, 350, true);
                    List<Actor> realAffected = new List<Actor>();
                    foreach (Actor act in affected)
                    {
                        if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                        {
                            realAffected.Add(act);
                            map.SendEffect(act, 5205);
                        }
                    }

                    SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factor);
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, args, skillActor, false);

                    this.Deactivate();
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                    this.Deactivate();
                }
            }
        }
        /*public class ClayMoreBuff : DefaultBuff
        {
            Actor sActor;
            SkillArg args;
            ActorSkill skillActor;
            public ClayMoreBuff(SkillArg skill, Actor sActor, ActorSkill actor, int lifetime)
                : base(skill.skill , actor, "ClayMore", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.sActor = sActor;
                this.skillActor = actor;
                args = skill.Clone();
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
                SagaLib.Logger.ShowInfo("開始");
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                SagaLib.Logger.ShowInfo("結束");
                float factor = 2.5f + 0.5f * skill.skill.Level;
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                List<Actor> affected = map.GetActorsArea(skillActor, 350, false);
                List<Actor> realAffected = new List<Actor>();
                foreach (Actor act in affected)
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        realAffected.Add(act);
                        map.SendEffect(act, 5205);
                    }
                }
                
                SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factor);
            }
        }*/

    }
}