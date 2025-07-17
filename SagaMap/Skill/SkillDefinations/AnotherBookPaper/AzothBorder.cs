
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    /// <summary>
    /// アゾットブレイバー
    /// </summary>
    public class AzothBorder : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2593, level, 200));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2594, level, 200));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2595, level, 200));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2596, level, 200));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2597, level, 200));
        }
        #endregion

    }
}
