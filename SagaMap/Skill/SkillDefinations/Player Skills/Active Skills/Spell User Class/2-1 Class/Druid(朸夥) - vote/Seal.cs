
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Druid
{
    /// <summary>
    /// 封印（封印）
    /// </summary>
    public class Seal : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            // 檢查攻擊是否是物理攻擊
            /*if (args.skill != null && args.skill.Physical)
            {
                int lifetime = 3000 + 1000 * level;
                MoveSpeedDown skill = new MoveSpeedDown(args.skill, dActor, lifetime);
                SkillHandler.ApplyAddition(dActor, skill);
            }*/
            int rate = 10 * level;
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                rate = rate + pc.Int;
            }
            if (rate > 80) rate = 80;

            if (SagaLib.Global.Random.Next(0, 99) < rate)
            {
                if (dActor.TInt["skilltype"] != 1)
                    return;
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                if (dActor.Tasks.ContainsKey("SkillCast"))
                {
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

                    int lifetime = 3000 + 1000 * level;
                    DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Seal", lifetime);
                    skill.OnAdditionStart += this.StartEventHandler;
                    skill.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(dActor, skill);
                }
            }
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Sealed = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Sealed = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}