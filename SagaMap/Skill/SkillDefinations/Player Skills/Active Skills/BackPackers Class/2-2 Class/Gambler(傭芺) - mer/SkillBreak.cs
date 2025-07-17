
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 攻擊預測（スキルブレイク）
    /// </summary>
    public class SkillBreak : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, 1f);
            int[] rates = { 0, 50, 70, 90 };
            int rate = rates[level];
            if (SagaMap.Skill.SkillHandler.Instance.isBossMob(dActor))
                rate -= 30;

            if (SagaLib.Global.Random.Next(0, 99) < rate)
            {
                if (dActor.Tasks.ContainsKey("SkillCast"))
                {
                    Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                    if (dActor.Tasks["SkillCast"].Activated)
                    {
                        dActor.Tasks["SkillCast"].Deactivate();
                        dActor.Tasks.Remove("SkillCast");

                        SkillArg arg = new SkillArg();
                        arg.sActor = dActor.ActorID;
                        arg.dActor = 0;
                        arg.skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3311, 1);
                        arg.x = 0;
                        arg.y = 0;
                        arg.hp = new List<int>();
                        arg.sp = new List<int>();
                        arg.mp = new List<int>();
                        arg.hp.Add(0);
                        arg.sp.Add(0);
                        arg.mp.Add(0);
                        arg.argType = SkillArg.ArgType.Active;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, dActor, true);
                    }
                    if (dActor.Tasks.ContainsKey("AutoCast"))
                    {
                        dActor.Tasks["AutoCast"].Deactivate();
                        dActor.Tasks.Remove("AutoCast");
                        dActor.Buff.CannotMove = false;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, dActor, true);
                    }
                }
            }

        }
        #endregion
    }
}