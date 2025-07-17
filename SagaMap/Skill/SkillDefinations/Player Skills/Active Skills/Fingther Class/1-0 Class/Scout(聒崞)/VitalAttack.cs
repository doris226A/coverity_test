using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Scout
{
    /// <summary>
    /// 追擊要害
    /// </summary>
    public class VitalAttack:ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }


        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0.75f + 0.25f * level;
            int critbonus = new int[] { 0, 60, 65, 70, 75, 80 }[level];

            short[] pos = new short[2];
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);

            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.鈍足, 50))//顿足（目前对怪物无效）
            {
                Additions.Global.鈍足 skill = new SagaMap.Skill.Additions.Global.鈍足(args.skill, dActor, 3000);
                SkillHandler.ApplyAddition(dActor, skill);
            }
            //欠缺即死效果
            args.type = sActor.Status.attackType;
            SkillHandler.Instance.PhysicalAttack(sActor, new List<Actor>() { dActor }, args, SkillHandler.DefType.Def, sActor.WeaponElement, 0, factor, false, 0, false, 0, critbonus);
        }

        #endregion
    }
}
