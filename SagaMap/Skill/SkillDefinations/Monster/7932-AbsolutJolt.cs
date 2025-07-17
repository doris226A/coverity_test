using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// アブソリューションジョルト
    /// </summary>
    public class AbsolutJolt : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            List<Actor> actors = Manager.MapManager.Instance.GetMap(sActor.MapID).GetActorsArea(dActor, 200, true);
            List<Actor> affected = new List<Actor>();
            foreach (var item in actors)
            {
                
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, item))
                {
                    
                    if (item.type == ActorType.PC)
                    {
                        List<string> adds = new List<string>();
                        foreach (System.Collections.Generic.KeyValuePair<string, SagaDB.Actor.Addition> ad in item.Status.Additions)
                        {
                            if (!(ad.Value is DefaultPassiveSkill))
                                adds.Add(ad.Value.Name);
                        }
                        foreach (string adn in adds)
                        {
                            SkillHandler.RemoveAddition(item, adn);
                        }
                    }
                    if (item.ActorID != dActor.ActorID)
                    {
                        SkillHandler.Instance.PushBack(dActor, item, 3);
                    }
                    else
                    {
                        SkillHandler.Instance.PushBack(sActor, dActor, 3);
                    }
                    if (SkillHandler.Instance.CanAdditionApply(sActor, item, SkillHandler.DefaultAdditions.鈍足, 80))
                    {
                        //这里并不知道顿足的持续时间, 先暂时设定为本技能1级时持续1秒, 每级增加0.25秒 满级顿足 2.25秒
                        Additions.Global.鈍足 skill = new SagaMap.Skill.Additions.Global.鈍足(args.skill, item, (int)(750 + 250 * level));
                        SkillHandler.ApplyAddition(item, skill);
                    }
                }
            }
        }
    }
}
