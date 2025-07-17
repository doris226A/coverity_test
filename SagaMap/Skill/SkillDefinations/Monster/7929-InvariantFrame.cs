using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// インバリデイトフレーム
    /// </summary>
    public class InvariantFrame : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 4.0f;
            
            List<Actor> actors = Manager.MapManager.Instance.GetMap(sActor.MapID).GetActorsArea(dActor, 200, true);
            List<Actor> affected = new List<Actor>();
            foreach (var item in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, item))
                {
                    affected.Add(item);
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
                    
            }
            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, SagaLib.Elements.Dark, factor);
        }
    }
}
