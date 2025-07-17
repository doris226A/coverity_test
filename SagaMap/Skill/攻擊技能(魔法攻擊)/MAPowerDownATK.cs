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
    public class MAPowerDownATK : ISkill
    {
        bool MobUse;
        public MAPowerDownATK()
        {
            this.MobUse = false;
        }
        public MAPowerDownATK(bool MobUse)
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
            if (MobUse)
            {
                level = 5;
            }
            int lifetime = 10000;
            if (sActor.type == ActorType.PC)
            {
                args.dActor = 0;
                Actor realdActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
                if (CheckPossible(realdActor))
                {
                    DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Concentricity", lifetime);
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
                SkillHandler.ApplyAddition(sActor, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            // 计算最小与最大攻击力减少的数值
            int min_atk1_down = (int)(actor.Status.min_atk1 * 0.1f);
            int min_atk2_down = (int)(actor.Status.min_atk2 * 0.1f);
            int min_atk3_down = (int)(actor.Status.min_atk3 * 0.1f);
            int max_atk1_down = (int)(actor.Status.max_atk1 * 0.1f);
            int max_atk2_down = (int)(actor.Status.max_atk2 * 0.1f);
            int max_atk3_down = (int)(actor.Status.max_atk3 * 0.1f);

            if (skill.Variable.ContainsKey("Twine_min_atk1"))
                skill.Variable.Remove("Twine_min_atk1");
            skill.Variable.Add("Twine_min_atk1", min_atk1_down);
            actor.Status.min_atk1_skill -= (short)min_atk1_down;

            if (skill.Variable.ContainsKey("Twine_min_atk2"))
                skill.Variable.Remove("Twine_min_atk2");
            skill.Variable.Add("Twine_min_atk2", min_atk2_down);
            actor.Status.min_atk2_skill -= (short)min_atk2_down;

            if (skill.Variable.ContainsKey("Twine_min_atk3"))
                skill.Variable.Remove("Twine_min_atk3");
            skill.Variable.Add("Twine_min_atk3", min_atk3_down);
            actor.Status.min_atk3_skill -= (short)min_atk3_down;

            if (skill.Variable.ContainsKey("Twine_max_atk1"))
                skill.Variable.Remove("Twine_max_atk1");
            skill.Variable.Add("Twine_max_atk1", max_atk1_down);
            actor.Status.max_atk1_skill -= (short)max_atk1_down;

            if (skill.Variable.ContainsKey("Twine_max_atk2"))
                skill.Variable.Remove("Twine_max_atk2");
            skill.Variable.Add("Twine_max_atk2", max_atk2_down);
            actor.Status.max_atk2_skill -= (short)max_atk2_down;

            if (skill.Variable.ContainsKey("Twine_max_atk3"))
                skill.Variable.Remove("Twine_max_atk3");
            skill.Variable.Add("Twine_max_atk3", max_atk3_down);
            actor.Status.max_atk3_skill -= (short)max_atk3_down;


            actor.Buff.MinAtkDown = true;
            actor.Buff.MaxAtkDown = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.min_atk1_skill += (short)skill.Variable["Twine_min_atk1"];
            actor.Status.min_atk2_skill += (short)skill.Variable["Twine_min_atk2"];
            actor.Status.min_atk3_skill += (short)skill.Variable["Twine_min_atk3"];

            actor.Status.max_atk1_skill += (short)skill.Variable["Twine_max_atk1"];
            actor.Status.max_atk2_skill += (short)skill.Variable["Twine_max_atk2"];
            actor.Status.max_atk3_skill += (short)skill.Variable["Twine_max_atk3"];

            actor.Buff.MinAtkDown = false;
            actor.Buff.MaxAtkDown = false;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
