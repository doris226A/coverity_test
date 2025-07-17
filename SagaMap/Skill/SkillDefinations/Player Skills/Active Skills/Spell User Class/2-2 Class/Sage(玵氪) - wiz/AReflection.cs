

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Sage
{
    /// <summary>
    /// 反射（リフレクション）
    /// </summary>
    public class AReflection : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //MagicReflect skill = new MagicReflect();
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "MagicReflect", 120000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);

        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Reflection = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5023);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Reflection = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}