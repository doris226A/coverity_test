using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.SunFlowerAdditions
{
    /// <summary>
    /// 神聖光界（ディバインバリア）
    /// </summary>
    public class EquipChange : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 10000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "EquipChange", lifetime);

            /*SkillHandler.RemoveAddition(sActor, "MagicShield");
            SkillHandler.RemoveAddition(sActor, "EnergyShield");
            SkillHandler.RemoveAddition(sActor, "MagicBarrier");
            SkillHandler.RemoveAddition(sActor, "EnergyBarrier");*/

            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short LDef = 0, RDef = 0, LMDef = 0, RMDef = 0;
            int level = skill.skill.Level;
            switch (level)
            {
                case 1:
                    LDef = 15;
                    RDef = 20;
                    LMDef = 25;
                    RMDef = 20;
                    break;
                case 2:
                    LDef = 15;
                    RDef = 20;
                    LMDef = 25;
                    RMDef = 20;
                    break;
                case 3:
                    LDef = 15;
                    RDef = 20;
                    LMDef = 25;
                    RMDef = 20;
                    break;
                case 4:
                    LDef = 15;
                    RDef = 20;
                    LMDef = 25;
                    RMDef = 20;
                    break;
                case 5:
                    LDef = 15;
                    RDef = 20;
                    LMDef = 25;
                    RMDef = 20;
                    break;
            }
            if (actor.Status.Additions.ContainsKey("SoulOfEarth"))
            {
                actor.Status.Additions["SoulOfEarth"].AdditionEnd();
            }
            if (actor.Status.Additions.ContainsKey("SoulOfWater"))
            {
                actor.Status.Additions["SoulOfWater"].AdditionEnd();
            }
            if (actor.Status.Additions.ContainsKey("MagicBarrier"))
            {
                actor.Status.Additions["MagicBarrier"].AdditionEnd();
            }
            if (actor.Status.Additions.ContainsKey("EnergyBarrier"))
            {
                actor.Status.Additions["EnergyBarrier"].AdditionEnd();
            }
            if (actor.Status.Additions.ContainsKey("AtkUp_DefUp_SpdDown_AvoDown"))
            {
                actor.Status.Additions["AtkUp_DefUp_SpdDown_AvoDown"].AdditionEnd();
            }



            //RemoveAddition(actor, "SoulOfEarth");
            //RemoveAddition(actor, "SoulOfWater");
            //RemoveAddition(actor, "MagicBarrier");
            //RemoveAddition(actor, "EnergyBarrier");
            //左防
            if (skill.Variable.ContainsKey("DevineBarrier_LDef"))
                skill.Variable.Remove("DevineBarrier_LDef");
            skill.Variable.Add("DevineBarrier_LDef", LDef);
            //右防
            if (skill.Variable.ContainsKey("DevineBarrier_RDef"))
                skill.Variable.Remove("DevineBarrier_RDef");
            skill.Variable.Add("DevineBarrier_RDef", RDef);
            //左魔防
            if (skill.Variable.ContainsKey("DevineBarrier_LMDef"))
                skill.Variable.Remove("DevineBarrier_LMDef");
            skill.Variable.Add("DevineBarrier_LMDef", LMDef);
            //右魔防
            if (skill.Variable.ContainsKey("DevineBarrier_RMDef"))
                skill.Variable.Remove("DevineBarrier_RMDef");
            skill.Variable.Add("DevineBarrier_RMDef", RMDef);

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

            EffectArg arg = new EffectArg();
            arg.effectID = 4019;
            arg.actorID = actor.ActorID;


            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, arg, actor, true);





        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //左防
            actor.Status.def_skill -= (short)skill.Variable["DevineBarrier_LDef"];
            //右防
            actor.Status.def_add_skill -= (short)skill.Variable["DevineBarrier_RDef"];
            //左魔防
            actor.Status.mdef_skill -= (short)skill.Variable["DevineBarrier_LMDef"];
            //右魔防
            actor.Status.mdef_add_skill -= (short)skill.Variable["DevineBarrier_RMDef"];
            actor.Buff.DefUp = false;
            actor.Buff.DefRateUp = false;
            actor.Buff.MagicDefUp = false;
            actor.Buff.MagicDefRateUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        public void RemoveAddition(Actor actor, String additionName)
        {
            if (actor.Status.Additions.ContainsKey(additionName))
            {
                Addition addition = actor.Status.Additions[additionName];
                actor.Status.Additions.TryRemove(additionName, out _);
                if (addition.Activated)
                {
                    addition.AdditionEnd();
                }
                addition.Activated = false;
            }
        }
        #endregion
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.SunFlowerAdditions
{
    /// <summary>
    /// 巫婆长袍切换（Ragnarok）
    /// </summary>
    public class EquipChange : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 10000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "EquipChange", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Elements[SagaLib.Elements.Holy] -= 100;
            actor.Elements[SagaLib.Elements.Dark] += 100;
            actor.Buff.DarkShield = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            SkillHandler.Instance.ShowEffectByActor(actor, 5241);
            actor.Elements[SagaLib.Elements.Holy] += 100;
            actor.Elements[SagaLib.Elements.Dark] -= 100;
            actor.Buff.DarkShield = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        #endregion
    }
}
*/