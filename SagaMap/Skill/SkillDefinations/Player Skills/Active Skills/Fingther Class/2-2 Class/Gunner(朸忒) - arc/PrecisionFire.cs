
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gunner
{
    /// <summary>
    /// 精密射擊（精密射撃）
    /// </summary>
    public class PrecisionFire : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.PossessionTarget != 0)
            {
                ActorPC realDActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
                if (!SkillHandler.Instance.CheckSkillCanCastForWeapon(realDActor, args))
                {
                    return -5;
                }
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            ActorPC realDActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
            int lifetime = 5000 * level;
            DefaultBuff skill = new DefaultBuff(args.skill, realDActor, "PrecisionFire", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(realDActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {

            //一定時間內讓自己可以100％命中目標
            //效果期間不會發生致命攻擊，也不會發生AVOID、GUARD狀態
            actor.Buff.PrecisionFire = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.PrecisionFire = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
