using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;
using SagaDB.Item;


namespace SagaMap.Skill.SkillDefinations.Harvest
{
    public class TwineSleep : ISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            /*uint itemID = 10088701;
            if (SkillHandler.Instance.CountItem(pc, itemID) > 0)
            {
                SkillHandler.Instance.TakeItem(pc, itemID, 1);
                return 0;
            }*/
            uint itemID = 10088701;//200耐久陷阱工具
            SagaDB.Item.Item item2 = pc.Inventory.GetItem(itemID, Inventory.SearchType.ITEM_ID);
            if (SkillHandler.Instance.CountItem(pc, itemID) > 0)
            {
                if (item2.Durability > 0)
                {
                    item2.Durability -= 1;
                    Network.Client.MapClient.FromActorPC(pc).SendItemInfo(item2);
                    return 0;
                }
            }
            return -2;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 40000;
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = sActor as ActorPC;
                if (pc.Skills3.ContainsKey(993) || pc.DualJobSkill.Exists(x => x.ID == 993))
                {
                    lifetime = 60000;
                }
            }
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //获取设置中心9*9范围的怪物
            List<Actor> actors = map.GetActorsArea(sActor, 400, true);
            List<Actor> affected = new List<Actor>();
            args.affectedActors.Clear();
            foreach (Actor i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i) && i.Status.Additions.ContainsKey("Twine"))
                {
                    if (SkillHandler.Instance.isBossMob(i) || i.type == ActorType.PC)
                    {
                        switch (level)
                        {
                            case 1:
                                Additions.Global.Sleep skill = new SagaMap.Skill.Additions.Global.Sleep(args.skill, i, 4000);
                                SkillHandler.ApplyAddition(i, skill);
                                break;
                            case 2:
                                Additions.Global.Silence skill2 = new SagaMap.Skill.Additions.Global.Silence(args.skill, i, 4000);
                                SkillHandler.ApplyAddition(i, skill2);
                                break;
                            case 3:
                                DefaultBuff skill3 = new DefaultBuff(args.skill, i, "TwineSleep_aspddown", 20000);
                                skill3.OnAdditionStart += this.StartEventHandler;
                                skill3.OnAdditionEnd += this.EndEventHandler;
                                SkillHandler.ApplyAddition(i, skill3);
                                break;
                            case 4:
                                Additions.Global.Confuse skill4 = new SagaMap.Skill.Additions.Global.Confuse(args.skill, i, 4000);
                                SkillHandler.ApplyAddition(i, skill4);
                                break;
                            case 5:
                                Additions.Global.鈍足 skill5 = new SagaMap.Skill.Additions.Global.鈍足(args.skill, i, 4000);
                                SkillHandler.ApplyAddition(i, skill5);
                                break;
                        }
                    }
                    else
                    {
                        switch (level)
                        {
                            case 1:
                                Additions.Global.Sleep skill = new SagaMap.Skill.Additions.Global.Sleep(args.skill, i, lifetime);
                                SkillHandler.ApplyAddition(i, skill);
                                break;
                            case 2:
                                Additions.Global.Silence skill2 = new SagaMap.Skill.Additions.Global.Silence(args.skill, i, lifetime);
                                SkillHandler.ApplyAddition(i, skill2);
                                break;
                            case 3:
                                DefaultBuff skill3 = new DefaultBuff(args.skill, i, "TwineSleep_aspddown", lifetime);
                                skill3.OnAdditionStart += this.StartEventHandler;
                                skill3.OnAdditionEnd += this.EndEventHandler;
                                SkillHandler.ApplyAddition(i, skill3);
                                break;
                            case 4:
                                Additions.Global.Confuse skill4 = new SagaMap.Skill.Additions.Global.Confuse(args.skill, i, lifetime);
                                SkillHandler.ApplyAddition(i, skill4);
                                break;
                            case 5:
                                Additions.Global.鈍足 skill5 = new SagaMap.Skill.Additions.Global.鈍足(args.skill, i, lifetime);
                                SkillHandler.ApplyAddition(i, skill5);
                                break;
                        }
                    }

                }
            }
        }


        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int aspddown = (int)(actor.Status.aspd / 2.0f);

            //大傷
            if (skill.Variable.ContainsKey("TwineSleep_Speed_down"))
                skill.Variable.Remove("TwineSleep_Speed_down");
            skill.Variable.Add("TwineSleep_Speed_down", aspddown);
            actor.Status.aspd_skill -= (short)aspddown;
            actor.Buff.AttackSpeedDown = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.aspd_skill += (short)skill.Variable["TwineSleep_Speed_down"];

            actor.Buff.AttackSpeedDown = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
