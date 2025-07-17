using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 攻擊．煥發 アタックバースト
    /// </summary>
    public class BRDFATKUpASDEFUpOne : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.dActor = 0;//不显示效果
            int life = 30000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "ATKUp", life);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short value;
            value = 10;

            if (skill.Variable.ContainsKey("ATK"))
                skill.Variable.Remove("ATK");
            skill.Variable.Add("ATK", value);
            actor.Status.max_atk1_skill += value;
            actor.Status.max_atk2_skill += value;
            actor.Status.max_atk3_skill += value;

            actor.Buff.MaxAtkUp = true;
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
            int value = skill.Variable["ATK"];
            actor.Status.max_atk1_skill -= (short)value;
            actor.Status.max_atk2_skill -= (short)value;
            actor.Status.max_atk3_skill -= (short)value;

            actor.Buff.MaxAtkUp = false;
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
