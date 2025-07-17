using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Sorcerer
{
    /// <summary>
    /// 神聖光界（ディバインバリア）
    /// </summary>
    public class TRDevineBarrier : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 125000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "TRDevineBarrier", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short LDef = 0, RDef = 0, LMDef = 0, RMDef = 0;
            int level = skill.skill.Level;
            LDef = 13;
            RDef = 10;
            LMDef = 20;
            RMDef = 15;

            //左防
            if (skill.Variable.ContainsKey("TRDevineBarrier_LDef"))
                skill.Variable.Remove("TRDevineBarrier_LDef");
            skill.Variable.Add("TRDevineBarrier_LDef", LDef);
            //右防
            if (skill.Variable.ContainsKey("TRDevineBarrier_RDef"))
                skill.Variable.Remove("TRDevineBarrier_RDef");
            skill.Variable.Add("TRDevineBarrier_RDef", RDef);
            //左魔防
            if (skill.Variable.ContainsKey("TRDevineBarrier_LMDef"))
                skill.Variable.Remove("TRDevineBarrier_LMDef");
            skill.Variable.Add("TRDevineBarrier_LMDef", LMDef);
            //右魔防
            if (skill.Variable.ContainsKey("TRDevineBarrier_RMDef"))
                skill.Variable.Remove("TRDevineBarrier_RMDef");
            skill.Variable.Add("TRDevineBarrier_RMDef", RMDef);

            //左防
            actor.Status.def_skill += LDef;
            //右防
            actor.Status.def_add_skill += RDef;
            //左魔防
            actor.Status.mdef_skill += LMDef;
            //右魔防
            actor.Status.mdef_add_skill += RMDef;

            actor.Buff.DefUp = true;
            actor.Buff.DefRateUp = true;
            actor.Buff.MagicDefUp = true;
            actor.Buff.MagicDefRateUp = true;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //左防
            actor.Status.def_skill -= (short)skill.Variable["TRDevineBarrier_LDef"];
            //右防
            actor.Status.def_add_skill -= (short)skill.Variable["TRDevineBarrier_RDef"];
            //左魔防
            actor.Status.mdef_skill -= (short)skill.Variable["TRDevineBarrier_LMDef"];
            //右魔防
            actor.Status.mdef_add_skill -= (short)skill.Variable["TRDevineBarrier_RMDef"];
            actor.Buff.DefUp = false;
            actor.Buff.DefRateUp = false;
            actor.Buff.MagicDefUp = false;
            actor.Buff.MagicDefRateUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}