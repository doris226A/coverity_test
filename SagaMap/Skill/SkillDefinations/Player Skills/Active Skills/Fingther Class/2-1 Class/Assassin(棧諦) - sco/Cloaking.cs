using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Assassin
{
    /// <summary>
    /// 潛行（クローキング）
    /// </summary>
    public class Cloaking : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            //周圍有敵人時禁止潛行+檢查7x7範圍內是否有怪物或敵對生物
            List<Actor> actorsInRange = Manager.MapManager.Instance.GetMap(sActor.MapID).GetActorsArea(sActor, 7 * 64, true);

            foreach (Actor actorInRange in actorsInRange)
            {
                if (actorInRange != sActor && SkillHandler.Instance.CheckValidAttackTarget(sActor, actorInRange))
                {
                    // 有怪物或敵對生物存在，無法使用潛行技能
                    return -12;
                }
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 3600000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "Cloaking", lifetime, 1000);
            skill.OnAdditionStart += this.StartEvent;
            skill.OnAdditionEnd += this.EndEvent;
            skill.OnUpdate += this.TimerUpdate;
            SkillHandler.ApplyAddition(sActor, skill);

        }
        void StartEvent(Actor actor, DefaultBuff skill)
        {
            //隱藏自己
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                pc.Motion = SagaLib.MotionType.STAND;
            }

            actor.Buff.Transparent = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

            // 隱身技能處理邏輯
            List<ActorMob> actorsInRange = new List<ActorMob>();
            foreach (Actor Mapactor in Manager.MapManager.Instance.GetMap(actor.MapID).Actors.Values)
            {
                if (Mapactor != null && Mapactor.type == ActorType.MOB)
                {
                    actorsInRange.Add((ActorMob)Mapactor);
                }
            }
            foreach (ActorMob actorInRange in actorsInRange)
            {
                if (actorInRange != null && ((ActorEventHandlers.MobEventHandler)actorInRange.e).AI.Hate.ContainsKey(actor.ActorID))
                {
                    ((ActorEventHandlers.MobEventHandler)actorInRange.e).AI.Hate.TryRemove(actor.ActorID, out _);//二哈更改Hate
                }
            }
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {

            //顯示自己
            actor.Buff.Transparent = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void TimerUpdate(Actor actor, DefaultBuff skill)
        {

            if (actor.type != ActorType.PC)
            {
                return;
            }
            ActorPC dActorPC = (ActorPC)actor;


            if (actor.SP <= 0)
            {
                actor.SP = 0;
                actor.Buff.Transparent = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
                actor.Status.Additions["Cloaking"].AdditionEnd();
                actor.Status.Additions.TryRemove("Cloaking", out _);
            }
            else if (dActorPC.Motion == SagaLib.MotionType.SIT)
            {
                dActorPC.Motion = SagaLib.MotionType.STAND;
                actor.Buff.Transparent = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                actor.Status.Additions["Cloaking"].AdditionEnd();
                actor.Status.Additions.TryRemove("Cloaking", out _);
            }
            else
            {
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                actor.SP -= 1;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
            }

        }
        #endregion
    }
}
