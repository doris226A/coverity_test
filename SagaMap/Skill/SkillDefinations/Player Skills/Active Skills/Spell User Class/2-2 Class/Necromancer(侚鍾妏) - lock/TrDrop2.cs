
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Necromancer
{
    /// <summary>
    /// 解除憑依（魂抜き）
    /// </summary>
    public class TrDrop2 : ISkill
    {
        bool MobUse;
        public TrDrop2()
        {
            this.MobUse = false;
        }
        public TrDrop2(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (dActor.type == ActorType.PC)
            {
                if (MobUse)
                {
                    level = 5;
                }
                int rate = 20 + 10 * level;
                if (SagaLib.Global.Random.Next(0, 99) < rate)
                {
                    SkillHandler.Instance.PossessionCancel((ActorPC)dActor, SagaLib.PossessionPosition.NONE);
                    if (sActor.type == ActorType.PC && args.skill.ID == 3122)
                    {
                        SagaMap.Skill.Additions.Global.Undead skill = new SagaMap.Skill.Additions.Global.Undead(dActor);
                        SagaMap.Skill.SkillHandler.ApplyAddition(dActor, skill);
                    }
                }
            }
        }
        #endregion
    }
}