
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Trader
{
    /// <summary>
    /// 掉宝率倍增
    /// </summary>
    public class TreasureUp : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int num = SagaLib.Global.Random.Next(1, 100);
            if (num <= 40)
            {
                Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(dActor, 8013);
                if (!dActor.Status.SkillRate.ContainsKey(3349))
                {
                    dActor.Status.SkillRate.Add(3349, level);
                }
            }
        }
        #endregion
    }
}