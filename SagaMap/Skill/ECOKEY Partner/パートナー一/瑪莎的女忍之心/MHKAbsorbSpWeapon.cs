using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Item;

namespace SagaMap.Skill.SkillDefinations.Explorer
{
    /// <summary>
    /// ブレイドマスタリー
    /// </summary>
    public class MHKAbsorbSpWeapon : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            if (dActor.Status.Additions.ContainsKey("BloodLeech"))
            {
                Additions.Global.BloodLeech add = (Additions.Global.BloodLeech)dActor.Status.Additions["BloodLeech"];
                add.rate = 0;
            }
            int time = 10000;//SP吸收持续时间
            SpLeech skill = new SpLeech(args.skill, dActor, time, 0.05f);

            SkillHandler.ApplyAddition(dActor, skill);
        }

    }
    #endregion
}
