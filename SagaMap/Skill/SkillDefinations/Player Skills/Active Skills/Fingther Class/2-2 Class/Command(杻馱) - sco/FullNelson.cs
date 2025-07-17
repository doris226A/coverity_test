
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaLib;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 雙絞壓制（羽交い絞め）
    /// </summary>
    public class FullNelson : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        Activator timer = null;
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int[] p = { 0, 80, 60, 40 };
            int point = SagaLib.Global.Random.Next(0, 99);
            if (point <= p[level])
            {
                int[] lifetime = { 0, 300000, 450000, 600000 };
                /*DefaultBuff skill = new DefaultBuff(args.skill, dActor, "FullNelson", lifetime[level]);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                //skill.OnUpdate += this.TimerUpdate;
                SkillHandler.ApplyAddition(dActor, skill);*/

                DefaultBuff skill2 = new DefaultBuff(args.skill, sActor, "Meditatioon", lifetime[level], 1000);
                skill2.OnAdditionStart += this.StartEventHandler;
                skill2.OnAdditionEnd += this.EndEventHandler;
                //skill2.OnUpdate += this.TimerUpdate;
                SkillHandler.ApplyAddition(sActor, skill2);

                sActor.TInt["FullNelson"] = (int)dActor.ActorID;
                sActor.TInt["FullNelson_level"] = level;
                dActor.TInt["d_FullNelson"] = (int)sActor.ActorID;

                dActor.Buff.Stiff = true;
                Manager.MapManager.Instance.GetMap(dActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, dActor, true);
                timer = new Activator((ActorPC)sActor, dActor, args);
                timer.Activate();
            }
            else
            {
                SagaMap.Skill.SkillHandler.Instance.ShowVessel(dActor, 0, 0, 0, SkillHandler.AttackResult.Avoid);

                SkillArg arg = new SkillArg();
                arg.affectedActors.Add(dActor);
                arg.Init();
                arg.sActor = dActor.ActorID;
                arg.argType = SkillArg.ArgType.Attack;
                arg.flag[0] = AttackFlag.AVOID;
                Manager.MapManager.Instance.GetMap(dActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, dActor, true);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.羽交い絞め = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.羽交い絞め = false;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

            Actor dActor = map.GetActor((uint)actor.TInt["FullNelson"]);
            if (dActor != null)
            {
                dActor.TInt["d_FullNelson"] = 0;
                dActor.Buff.Stiff = false;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, dActor, true);
            }
            if (timer != null && timer.Activated)
            {
                timer.Deactivate();
            }
        }

        /*void TimerUpdate(Actor actor, DefaultBuff skill)
        {
            if (actor.type != ActorType.PC)
            {
                return;
            }
            ActorPC dActorPC = (ActorPC)actor;
            if (actor.SP <= 0)
            {
                actor.SP = 0;
                actor.Buff.羽交い絞め = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
                actor.Status.Additions["Meditatioon"].AdditionEnd();
                actor.Status.Additions.Remove("Meditatioon");
            }
            else if (dActorPC.Motion == SagaLib.MotionType.SIT)
            {
                dActorPC.Motion = SagaLib.MotionType.STAND;
                actor.Buff.羽交い絞め = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                actor.Status.Additions["Meditatioon"].AdditionEnd();
                actor.Status.Additions.Remove("Meditatioon");
            }
            else
            {
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                actor.SP -= 1;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
            }

        }*/
        #endregion

        private class Activator : MultiRunTask
        {
            Map map;
            ActorPC actor;
            Actor dActor;
            SkillArg args;
            public Activator(ActorPC sActor, Actor dActor, SkillArg args)
            {
                this.actor = sActor;
                this.dActor = dActor;
                this.args = args;
                map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                this.period = 1000;
                this.dueTime = 1000;
            }

            public override void CallBack()
            {
                try
                {
                    if (actor.SP <= 0)
                    {
                        actor.SP = 0;
                        actor.Buff.羽交い絞め = false;
                        dActor.Buff.Stiff = false;
                        dActor.TInt["d_FullNelson"] = 0;
                        Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                        Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
                        actor.Status.Additions["Meditatioon"].AdditionEnd();
                        actor.Status.Additions.TryRemove("Meditatioon", out _);
                        this.Deactivate();
                    }
                    else if (actor.Motion == SagaLib.MotionType.SIT)
                    {
                        actor.Motion = SagaLib.MotionType.STAND;
                        actor.Buff.羽交い絞め = false;
                        dActor.Buff.Stiff = false;
                        dActor.TInt["d_FullNelson"] = 0;
                        Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                        actor.Status.Additions["Meditatioon"].AdditionEnd();
                        actor.Status.Additions.TryRemove("Meditatioon", out _);
                        this.Deactivate();
                    }
                    else if (dActor.HP <= 0)
                    {
                        actor.Motion = SagaLib.MotionType.STAND;
                        actor.Buff.羽交い絞め = false;
                        actor.Status.Additions["Meditatioon"].AdditionEnd();
                        actor.Status.Additions.TryRemove("Meditatioon", out _);
                        this.Deactivate();
                    }
                    else
                    {
                        Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                        actor.SP -= args.skill.Level;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
                    }

                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }
        }
    }
}
