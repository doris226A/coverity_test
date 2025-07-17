﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Mob;
namespace SagaMap.Skill.SkillDefinations.DarkStalker
{
    /// <summary>
    /// 黑暗騎士（ダークネスオブナイト）
    /// </summary>
    public class DarknessOfNight : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*for (int i = 0; i < 5; i++)
            {
                args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2407, level, 560 * i, args.x, args.y));
            }*/
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2407, level, 800, args.x, args.y));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2407, level, 600, args.x, args.y));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2407, level, 600, args.x, args.y));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2407, level, 600, args.x, args.y));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2408, level, 800, args.x, args.y));
        }
        #endregion
    }
}