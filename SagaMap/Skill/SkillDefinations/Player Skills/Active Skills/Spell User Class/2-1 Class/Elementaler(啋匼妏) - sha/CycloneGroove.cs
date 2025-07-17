using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;

using SagaLib;

using SagaMap.Mob;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Elementaler
{
    /// <summary>
    /// 元素四重奏（エレメンタルカルテット）
    /// </summary>
    public class CycloneGroove : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
          //  args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3303, level, 0));
          //  args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3304, level, 100));
           // args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3305, level, 100));
            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3306, level, 100));
        }
        #endregion
    }
}