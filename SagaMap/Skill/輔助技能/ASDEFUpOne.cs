
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Merchant
{
    /// <summary>
    /// 
    /// </summary>
    public class ASDEFUpOne : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 30000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "DEFUpOne", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int LDef = 5; //左防
            int RDef = 10;  //右防
            int MaxLMDef = 4; //左魔防
            int MaxRMDef = 7; //右魔防


            //左防
            if (skill.Variable.ContainsKey("SoulOfWater_def"))
                skill.Variable.Remove("SoulOfWater_def");
            skill.Variable.Add("SoulOfWater_def", LDef);
            actor.Status.def_skill += (short)LDef;

            //右防
            if (skill.Variable.ContainsKey("SoulOfWater_def_add"))
                skill.Variable.Remove("SoulOfWater_def_add");
            skill.Variable.Add("SoulOfWater_def_add", RDef);
            actor.Status.def_add_skill += (short)RDef;

            //左魔防
            int LMDef = MaxLMDef;
            if (skill.Variable.ContainsKey("SoulOfWater_mdef"))
                skill.Variable.Remove("SoulOfWater_mdef");
            skill.Variable.Add("SoulOfWater_mdef", LMDef);
            actor.Status.mdef_skill += (short)LMDef;

            //右魔防
            int RMDef = MaxRMDef;
            if (skill.Variable.ContainsKey("SoulOfWater_mdef_add"))
                skill.Variable.Remove("SoulOfWater_mdef_add");
            skill.Variable.Add("SoulOfWater_mdef_add", RMDef);
            actor.Status.mdef_add_skill += (short)RMDef;

            actor.Buff.DefUp = true;
            actor.Buff.MagicDefUp = true;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //左防
            actor.Status.def_skill -= (short)skill.Variable["SoulOfWater_def"];

            //右防
            actor.Status.def_add_skill -= (short)skill.Variable["SoulOfWater_def_add"];

            //左魔防
            actor.Status.mdef_skill -= (short)skill.Variable["SoulOfWater_mdef"];

            //右魔防
            actor.Status.mdef_add_skill -= (short)skill.Variable["SoulOfWater_mdef_add"];

            actor.Buff.DefUp = false;
            actor.Buff.MagicDefUp = false;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}
