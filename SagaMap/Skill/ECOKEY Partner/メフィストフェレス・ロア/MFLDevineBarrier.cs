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
    public class MFLDevineBarrier : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 125000;
            List<Actor> actors = Manager.MapManager.Instance.GetMap(dActor.MapID).GetActorsArea(dActor, 100, true);
            //取得有效Actor（即怪物）
            foreach (Actor i in actors)
            {
                if (!SkillHandler.Instance.CheckValidAttackTarget(dActor, i))
                {
                    DefaultBuff skill = new DefaultBuff(args.skill, dActor, "TRDevineBarrier", lifetime);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(dActor, skill);
                }
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short LDef = 0, LMDef = 0;
            int level = skill.skill.Level;
            LDef = 10;
            LMDef = 15;

            //左防
            if (skill.Variable.ContainsKey("TRDevineBarrier_LDef"))
                skill.Variable.Remove("TRDevineBarrier_LDef");
            skill.Variable.Add("TRDevineBarrier_LDef", LDef);
            //左魔防
            if (skill.Variable.ContainsKey("TRDevineBarrier_LMDef"))
                skill.Variable.Remove("TRDevineBarrier_LMDef");
            skill.Variable.Add("TRDevineBarrier_LMDef", LMDef);

            //左防
            actor.Status.def_skill += LDef;
            //左魔防
            actor.Status.mdef_skill += LMDef;

            actor.Buff.DefUp = true;
            actor.Buff.MagicDefUp = true;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //左防
            actor.Status.def_skill -= (short)skill.Variable["TRDevineBarrier_LDef"];
            //左魔防
            actor.Status.mdef_skill -= (short)skill.Variable["TRDevineBarrier_LMDef"];
            actor.Buff.DefUp = false;
            actor.Buff.MagicDefUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}