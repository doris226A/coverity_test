using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// ホーリーフェザー
    /// </summary>
    public class DivineProtection : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> realAffected = new List<Actor>();
            ActorPC sPC = (ActorPC)sActor;
            if (sPC.Party != null)
            {
                foreach (ActorPC act in sPC.Party.Members.Values)
                {
                    if (act.Online)
                    {
                        if (sPC.Party != null)
                        {
                            if (act.Party.ID != 0 && !act.Buff.Dead && act.MapID == sActor.MapID)
                            {
                                realAffected.Add(act);
                            }
                        }

                    }
                }
            }
            else
            {
                realAffected.Add(sActor);
            }
            args.affectedActors = realAffected;
            args.Init();
            foreach (Actor rAct in realAffected)
            {
                if (rAct.Status.Additions.ContainsKey("StyleChange"))
                    continue;
                DefaultBuff skill = new DefaultBuff(args.skill, rAct, "DivineProtection", 300000);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(rAct, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short status_add = new short[] { 0, 1, 2, 3, 5, 8 }[skill.skill.Level];
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                if (pc.Job == PC_JOB.JOKER)
                {
                    pc.Status.str_skill += status_add;
                    pc.Status.vit_skill += status_add;
                    pc.Status.dex_skill += status_add;
                    pc.Status.mag_skill += status_add;
                    pc.Status.int_skill += status_add;
                    pc.Status.agi_skill += status_add;
                    pc.Buff.STRUp = true;
                    pc.Buff.VITUp = true;
                    pc.Buff.DEXUp = true;
                    pc.Buff.MagUp = true;
                    pc.Buff.INTUp = true;
                    pc.Buff.AGIUp = true;
                }
                else if (pc.JobBasic == PC_JOB.SWORDMAN ||
                        pc.JobBasic == PC_JOB.FENCER ||
                        pc.JobBasic == PC_JOB.SCOUT ||
                        pc.JobBasic == PC_JOB.ARCHER)
                {
                    pc.Status.str_skill += status_add;
                    pc.Status.vit_skill += status_add;
                    pc.Buff.STRUp = true;
                    pc.Buff.VITUp = true;
                }
                else if (pc.JobBasic == PC_JOB.WIZARD ||
                        pc.JobBasic == PC_JOB.SHAMAN ||
                        pc.JobBasic == PC_JOB.VATES ||
                        pc.JobBasic == PC_JOB.WARLOCK)
                {
                    pc.Status.mag_skill += status_add;
                    pc.Status.int_skill += status_add;
                    pc.Buff.MagUp = true;
                    pc.Buff.INTUp = true;
                }
                else if (pc.JobBasic == PC_JOB.TATARABE ||
                        pc.JobBasic == PC_JOB.FARMASIST ||
                        pc.JobBasic == PC_JOB.RANGER ||
                        pc.JobBasic == PC_JOB.MERCHANT ||
                        pc.Race == PC_RACE.DEM)
                {
                    pc.Status.str_skill += status_add;
                    pc.Status.mag_skill += status_add;
                    pc.Buff.STRUp = true;
                    pc.Buff.MagUp = true;
                }
            }
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_STATUS, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            short status_add = new short[] { 0, 1, 2, 3, 5, 8 }[skill.skill.Level];
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                if (pc.Job == PC_JOB.JOKER)
                {
                    pc.Status.str_skill -= status_add;
                    pc.Status.vit_skill -= status_add;
                    pc.Status.dex_skill -= status_add;
                    pc.Status.mag_skill -= status_add;
                    pc.Status.int_skill -= status_add;
                    pc.Status.agi_skill -= status_add;
                    pc.Buff.STRUp = false;
                    pc.Buff.VITUp = false;
                    pc.Buff.DEXUp = false;
                    pc.Buff.MagUp = false;
                    pc.Buff.INTUp = false;
                    pc.Buff.AGIUp = false;
                }
                else if (pc.JobBasic == PC_JOB.SWORDMAN ||
                        pc.JobBasic == PC_JOB.FENCER ||
                        pc.JobBasic == PC_JOB.SCOUT ||
                        pc.JobBasic == PC_JOB.ARCHER)
                {
                    pc.Status.str_skill -= status_add;
                    pc.Status.vit_skill -= status_add;
                    pc.Buff.STRUp = false;
                    pc.Buff.VITUp = false;
                }
                else if (pc.JobBasic == PC_JOB.WIZARD ||
                        pc.JobBasic == PC_JOB.SHAMAN ||
                        pc.JobBasic == PC_JOB.VATES ||
                        pc.JobBasic == PC_JOB.WARLOCK)
                {
                    pc.Status.mag_skill -= status_add;
                    pc.Status.int_skill -= status_add;
                    pc.Buff.MagUp = false;
                    pc.Buff.INTUp = false;
                }
                else if (pc.JobBasic == PC_JOB.TATARABE ||
                        pc.JobBasic == PC_JOB.FARMASIST ||
                        pc.JobBasic == PC_JOB.RANGER ||
                        pc.JobBasic == PC_JOB.MERCHANT ||
                        pc.Race == PC_RACE.DEM)
                {
                    pc.Status.str_skill -= status_add;
                    pc.Status.mag_skill -= status_add;
                    pc.Buff.STRUp = false;
                    pc.Buff.MagUp = false;
                }
            }
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_STATUS, null, actor, true);
        }
        #endregion
    }
}
