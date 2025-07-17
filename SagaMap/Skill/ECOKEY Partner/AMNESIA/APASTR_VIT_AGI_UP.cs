using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Druid
{
    /// <summary>
    /// 強身健體（ラウズボディ）
    /// </summary>
    public class APASTR_VIT_AGI_UP : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //SkillArg SkillFireBolt = new SkillArg();
            //int[] lifetime ={15, 20, 25, 27, 30};
            //DefaultBuff skill = new DefaultBuff(args.skill, dActor, "STR_VIT_AGI_UP", lifetime[level - 1] * 1000);
            int lifetime = 25000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "STR_VIT_AGI_UP", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            if (dActor == sActor)
            {
                Map map = Manager.MapManager.Instance.GetMap(dActor.MapID);
                EffectArg arg2 = new EffectArg();
                arg2.effectID = 5177;
                arg2.actorID = dActor.ActorID;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, dActor, true);
            }
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short STR = 3;
            short VIT = 4;
            short AGI = 7;
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                short minatk = (short)Math.Floor(STR + Math.Pow(Math.Floor((double)(STR / 9)), 2));
                short maxatk = (short)(STR + Math.Pow(Math.Floor((double)((float)(STR + 14) / 5.0f)), 2));
                actor.Status.min_atk1_skill += minatk;
                actor.Status.min_atk2_skill += minatk;
                actor.Status.min_atk3_skill += minatk;
                actor.Status.max_atk1_skill += maxatk;
                actor.Status.max_atk2_skill += maxatk;
                actor.Status.max_atk3_skill += maxatk;

                short def = (short)((float)(VIT / 3.0f) + (float)(VIT / 4.5f));
                short mdef = (short)((float)(VIT / 3.0f) + (float)(VIT / 4.0f));
                actor.Status.def_skill += def;
                actor.Status.mdef_skill += mdef;
                short hp = (short)(actor.MaxHP * (float)(VIT / 130.0f));
                short sp = (short)(actor.MaxSP * (float)(VIT / 1000.0f * 5.0f));
                actor.Status.hp_skill += hp;
                actor.Status.sp_skill += sp;

                short avoid_melee = (short)(AGI + Math.Pow(Math.Floor((float)(AGI + 18) / 9.0f), 2) + Math.Floor((float)actor.Level / 3.0f) - 1);
                short avoid_ranged = (short)(Math.Floor(AGI + Math.Floor((float)actor.Level / 3.0f) + 3));
                actor.Status.avoid_melee_skill += avoid_melee;
                actor.Status.avoid_ranged_skill += avoid_ranged;

                short aspd = (short)(AGI * 3 + Math.Floor(Math.Pow((short)((float)(AGI + 63) / 9.0f), 2)));
                actor.Status.aspd_skill += aspd;

                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_minatk"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_minatk");
                skill.Variable.Add("STR_VIT_AGI_UP_minatk", minatk);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_maxatk"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_maxatk");
                skill.Variable.Add("STR_VIT_AGI_UP_maxatk", maxatk);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_def"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_def");
                skill.Variable.Add("STR_VIT_AGI_UP_def", def);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_mdef"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_mdef");
                skill.Variable.Add("STR_VIT_AGI_UP_mdef", mdef);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_hp"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_hp");
                skill.Variable.Add("STR_VIT_AGI_UP_hp", hp);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_sp"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_sp");
                skill.Variable.Add("STR_VIT_AGI_UP_sp", sp);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_avoid_melee"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_avoid_melee");
                skill.Variable.Add("STR_VIT_AGI_UP_avoid_melee", avoid_melee);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_avoid_ranged"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_avoid_ranged");
                skill.Variable.Add("STR_VIT_AGI_UP_avoid_ranged", avoid_ranged);
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_aspd"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_aspd");
                skill.Variable.Add("STR_VIT_AGI_UP_aspd", aspd);
            }
            else
            {
                //STR
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_STR"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_STR");
                skill.Variable.Add("STR_VIT_AGI_UP_STR", STR);
                actor.Status.str_skill += STR;
                //VIT
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_VIT"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_VIT");
                skill.Variable.Add("STR_VIT_AGI_UP_VIT", VIT);
                actor.Status.vit_skill += VIT;
                //AGI
                if (skill.Variable.ContainsKey("STR_VIT_AGI_UP_AGI"))
                    skill.Variable.Remove("STR_VIT_AGI_UP_AGI");
                skill.Variable.Add("STR_VIT_AGI_UP_AGI", AGI);
                actor.Status.agi_skill += AGI;

            }
            actor.Buff.STRUp = true;
            actor.Buff.AGIUp = true;
            actor.Buff.VITUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                actor.Status.min_atk1_skill -= (short)skill.Variable["STR_VIT_AGI_UP_minatk"];
                actor.Status.min_atk2_skill -= (short)skill.Variable["STR_VIT_AGI_UP_minatk"];
                actor.Status.min_atk3_skill -= (short)skill.Variable["STR_VIT_AGI_UP_minatk"];
                actor.Status.max_atk1_skill -= (short)skill.Variable["STR_VIT_AGI_UP_maxatk"];
                actor.Status.max_atk2_skill -= (short)skill.Variable["STR_VIT_AGI_UP_maxatk"];
                actor.Status.max_atk3_skill -= (short)skill.Variable["STR_VIT_AGI_UP_maxatk"];

                actor.Status.def_skill -= (short)skill.Variable["STR_VIT_AGI_UP_def"];
                actor.Status.mdef_skill -= (short)skill.Variable["STR_VIT_AGI_UP_mdef"];
                actor.Status.hp_skill -= (short)skill.Variable["STR_VIT_AGI_UP_hp"];
                actor.Status.sp_skill -= (short)skill.Variable["STR_VIT_AGI_UP_sp"];

                actor.Status.avoid_melee_skill -= (short)skill.Variable["STR_VIT_AGI_UP_avoid_melee"];
                actor.Status.avoid_ranged_skill -= (short)skill.Variable["STR_VIT_AGI_UP_avoid_ranged"];

                actor.Status.aspd_skill -= (short)skill.Variable["STR_VIT_AGI_UP_aspd"];
            }
            else
            {
                actor.Status.str_skill -= (short)skill.Variable["STR_VIT_AGI_UP_STR"];
                actor.Status.vit_skill -= (short)skill.Variable["STR_VIT_AGI_UP_VIT"];
                actor.Status.agi_skill -= (short)skill.Variable["STR_VIT_AGI_UP_AGI"];

            }
            actor.Buff.STRUp = false;
            actor.Buff.AGIUp = false;
            actor.Buff.VITUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}

