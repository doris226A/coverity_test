using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Sorcerer
{
    /// <summary>
    /// マジックバリア
    /// </summary>
    public class TAMagicBarrier : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int life = 420000;

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 250, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    DefaultBuff skill = new DefaultBuff(args.skill, act, "TAMagicBarrier", life);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(act, skill);
                    EffectArg arg2 = new EffectArg();
                    arg2.effectID = 5169;
                    arg2.actorID = act.ActorID;
                    Manager.MapManager.Instance.GetMap(act.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, act, true);
                }
            }

        }
        int levelnum = 1;
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int atk1 = 12, atk2 = 10;
            if (skill.Variable.ContainsKey("TAMagicBarrier_MDef"))
                skill.Variable.Remove("TAMagicBarrier_MDef");
            skill.Variable.Add("TAMagicBarrier_MDef", atk1);
            actor.Status.mdef_skill += (short)atk1;
            if (skill.Variable.ContainsKey("TAMagicBarrier_MDefAdd"))
                skill.Variable.Remove("TAMagicBarrier_MDefAdd");
            skill.Variable.Add("TAMagicBarrier_MDefAdd", atk2);
            actor.Status.mdef_add_skill += (short)atk2;

            actor.Buff.MagicDefUp = true;
            actor.Buff.MagicDefRateUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            int value = skill.Variable["TAMagicBarrier_MDef"];
            actor.Status.mdef_skill -= (short)value;
            value = skill.Variable["TAMagicBarrier_MDefAdd"];
            actor.Status.mdef_add_skill -= (short)value;

            actor.Buff.MagicDefUp = false;
            actor.Buff.MagicDefRateUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        #endregion
    }
}