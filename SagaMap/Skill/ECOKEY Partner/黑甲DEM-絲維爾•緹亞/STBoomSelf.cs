using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    /// <summary>
    /// 雷爆
    /// </summary>
    public class STBoomSelf : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 8.0f;
            List<Actor> actors = Manager.MapManager.Instance.GetMap(dActor.MapID).GetActorsArea(sActor, 300, true);
            List<Actor> affected = new List<Actor>();
            foreach (Actor i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                    affected.Add(i);
            }
            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, SagaLib.Elements.Neutral, factor);

            foreach (Actor i in affected)
            {
                if (SkillHandler.Instance.CanAdditionApply(sActor, i, SkillHandler.DefaultAdditions.Paralyse, 80))
                {
                    Additions.Global.Paralysis skillP = new SagaMap.Skill.Additions.Global.Paralysis(args.skill, i, 5000);
                    SkillHandler.ApplyAddition(i, skillP);
                }
            }


            uint hpdmg = sActor.HP - 1;
            int lifetime = 30000;
            sActor.HP = 1;
            SkillHandler.Instance.ShowVessel(sActor, (int)hpdmg);

            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "STDefDonn", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short defdown = (short)(actor.Status.def - 1);
            short mdefdown = (short)(actor.Status.mdef - 1);

            if (skill.Variable.ContainsKey("STDefDonn_defdown"))
                skill.Variable.Remove("STDefDonn_defdown");
            skill.Variable.Add("STDefDonn_defdown", defdown);

            if (skill.Variable.ContainsKey("STDefDonn_mdefdown"))
                skill.Variable.Remove("STDefDonn_mdefdown");
            skill.Variable.Add("STDefDonn_mdefdown", mdefdown);

            actor.Status.def_skill -= defdown;
            actor.Status.mdef_skill -= mdefdown;

            actor.Buff.DefDown = true;
            actor.Buff.MagicDefDown = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.def_skill += (short)skill.Variable["STDefDonn_defdown"];
            actor.Status.mdef_skill += (short)skill.Variable["STDefDonn_mdefdown"];
            actor.Buff.DefDown = false;
            actor.Buff.MagicDefDown = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
