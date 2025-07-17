
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaDB.Item;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Merchant
{
    /// <summary>
    /// 提升體積（パッキング）
    /// </summary>
    public class Packing : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = true;
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "Packing", active);
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
                int value = (int)(pc.Inventory.MaxVolume[ContainerType.BODY] * factor);
                actor.Status.CAPA_skill += value;
                if (skill.Variable.ContainsKey("Packing"))
                    skill.Variable.Remove("Packing");
                skill.Variable.Add("Packing", value);
            }
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            actor.Status.CAPA_skill -= skill.Variable["Packing"];
        }
        #endregion
    }
}

