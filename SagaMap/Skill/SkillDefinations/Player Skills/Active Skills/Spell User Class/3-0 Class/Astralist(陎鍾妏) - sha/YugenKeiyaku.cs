using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Astralist
{
    /// <summary>
    /// ����֮��Լ����
    /// </summary>
    public class YugenKeiyaku : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.WeaponElement != Elements.Neutral)
            {
                return 0;
            }
            return -12;
        }
        public SkillArg arg = new SkillArg();
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.skill.BaseData.effect5 = 0;
            int lifetime = 150000 + 30000 * level;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "YugenKeiyaku", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.�T��եȪ������C = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.�T��եȪ������C = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

    }
}
