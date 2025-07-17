using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Eraser
{
    /// <summary>
    /// マーシレスシャドウ
    /// </summary>
    public class Demacia : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.Status.Additions.ContainsKey("Cloaking"))
                return -30;
            else
                return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int[] lifetime = { 0, 10000, 15000, 15000, 15000, 20000 };
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "Cloaking", lifetime[level], 1000);
            skill.OnAdditionStart += this.StartEvent;
            skill.OnAdditionEnd += this.EndEvent;
            skill.OnUpdate += this.TimerUpdate;
            SkillHandler.ApplyAddition(sActor, skill);
            float factor = 1.3f + 0.7f * level;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(dActor, 150, true);
            List<Actor> affected = new List<Actor>();
            foreach (Actor i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                    affected.Add(i);
                if (SkillHandler.Instance.CanAdditionApply(sActor, i, SkillHandler.DefaultAdditions.Confuse, 10))
                {
                    Additions.Global.Stiff skill2 = new SagaMap.Skill.Additions.Global.Stiff(args.skill, i, 2500);
                    SkillHandler.ApplyAddition(i, skill);
                }
            }

            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, sActor.WeaponElement, factor);
            sActor.Buff.Transparent = true;
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, sActor, true);
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
