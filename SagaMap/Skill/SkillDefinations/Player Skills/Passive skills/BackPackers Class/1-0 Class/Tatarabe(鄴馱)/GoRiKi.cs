
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaDB.Item;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Tatarabe
{
    /// <summary>
    /// 提升重量（ごうりき）
    /// </summary>
    public class GoRiKi : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "GoRiKi", true);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                float factor = new float[] { 0, 0.10f, 0.20f, 0.30f, 0.40f, 0.50f }[skill.skill.Level];
                int value = (int)(pc.Inventory.MaxPayload[ContainerType.BODY] * factor);
                actor.Status.PAYL_skill += value;
                if (skill.Variable.ContainsKey("GoRiKi1"))
                    skill.Variable.Remove("GoRiKi1");
                skill.Variable.Add("GoRiKi1", value);
            }
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            actor.Status.PAYL_skill -= skill.Variable["GoRiKi1"];
        }
        #endregion
    }
}

