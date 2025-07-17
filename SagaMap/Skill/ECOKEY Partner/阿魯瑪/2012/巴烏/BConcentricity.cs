using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Assassin
{
    /// <summary>
    /// アサルト
    /// </summary>
    public class BConcentricity : ISkill
    {
        bool MobUse;
        public BConcentricity()
        {
            this.MobUse = false;
        }
        public BConcentricity(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        bool CheckPossible(Actor sActor)
        {
            return true;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 10000;
            if (sActor.type == ActorType.PC)
            {
                args.dActor = 0;
                Actor realdActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
                if (CheckPossible(realdActor))
                {
                    DefaultBuff skill = new DefaultBuff(args.skill, realdActor, "Concentricity", lifetime);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(realdActor, skill);
                }
            }
           else
            {
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Concentricity", lifetime);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short def_rate = (short)((float)actor.Status.def * 0.5f);
            if (skill.Variable.ContainsKey("Concentricity_def"))
                skill.Variable.Remove("Concentricity_def");
            skill.Variable.Add("Concentricity_def", def_rate);
            actor.Status.def_skill -= def_rate;

            actor.Status.cri_skill += 50;
            actor.Buff.DefDown = true;
            actor.Buff.CriticalRateUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (skill.Variable.ContainsKey("Concentricity_def"))
            {
                short def_rate = (short)skill.Variable["Concentricity_def"];
                actor.Buff.DefDown = false;
                actor.Buff.CriticalRateUp = false;

                actor.Status.cri_skill -= 50;
                actor.Status.def_skill += def_rate;

                skill.Variable.Remove("Concentricity_def"); // 從字典中移除該索引鍵

                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
            /* short def_rate = (short)skill.Variable["Concentricity_def"];//這句
             actor.Buff.DefDown = false;
             actor.Buff.CriticalRateUp = false;

             actor.Status.cri_skill -= 50;
             actor.Status.def_skill += def_rate;

             if (skill.Variable.ContainsKey("Concentricity_def"))
                 skill.Variable.Remove("Concentricity_def");

             Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);*/
        }
        #endregion
    }
}
