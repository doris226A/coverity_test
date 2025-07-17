using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 
    /// </summary>
    public class ASVITUp : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.dActor = 0;//不显示效果
            int life = 65000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "VITUp", life);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short VIT = 2;
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                short def = (short)((float)(VIT / 3.0f) + (float)(VIT / 4.5f));
                short mdef = (short)((float)(VIT / 3.0f) + (float)(VIT / 4.0f));
                actor.Status.def_skill += def;
                actor.Status.mdef_skill += mdef;
                short hp = (short)(actor.MaxHP * (float)(VIT / 130.0f));
                short sp = (short)(actor.MaxSP * (float)(VIT / 1000.0f * 5.0f));
                actor.Status.hp_skill += hp;
                actor.Status.sp_skill += sp;

                if (skill.Variable.ContainsKey("VIT_UP_def"))
                    skill.Variable.Remove("VIT_UP_def");
                skill.Variable.Add("VIT_UP_def", def);
                if (skill.Variable.ContainsKey("VIT_UP_mdef"))
                    skill.Variable.Remove("VIT_UP_mdef");
                skill.Variable.Add("VIT_UP_mdef", mdef);
                if (skill.Variable.ContainsKey("VIT_UP_hp"))
                    skill.Variable.Remove("VIT_UP_hp");
                skill.Variable.Add("VIT_UP_hp", hp);
                if (skill.Variable.ContainsKey("VIT_UP_sp"))
                    skill.Variable.Remove("VIT_UP_sp");
                skill.Variable.Add("VIT_UP_sp", sp);
            }
            else
            {
                //VIT
                if (skill.Variable.ContainsKey("VIT_UP_VIT"))
                    skill.Variable.Remove("VIT_UP_VIT");
                skill.Variable.Add("VIT_UP_VIT", VIT);
            }

            actor.Status.vit_skill += VIT;
            actor.Buff.VITUp = true;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                actor.Status.def_skill -= (short)skill.Variable["VIT_UP_def"];
                actor.Status.mdef_skill -= (short)skill.Variable["VIT_UP_mdef"];
                actor.Status.hp_skill -= (short)skill.Variable["VIT_UP_hp"];
                actor.Status.sp_skill -= (short)skill.Variable["VIT_UP_sp"];
            }
            else
            {
                actor.Status.vit_skill -= (short)skill.Variable["VIT_UP_VIT"];
                actor.Buff.VITUp = false;
            }

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
