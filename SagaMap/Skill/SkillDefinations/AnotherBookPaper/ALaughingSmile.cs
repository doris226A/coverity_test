using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    /// <summary>
    /// 絡みつく悪炎の笑み
    /// </summary>
    public class ALaughingSmile : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        int num = 0;
        SkillArg anotherArg;
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            anotherArg = args;
            float factor = 0;
            factor = new float[] { 0, 10.0f, 12.5f, 15.0f, 17.5f, 20.0f, 22.5f, 25.0f, 27.5f, 30.0f, 32.5f, 35.0f, 37.5f, 40.0f, 42.5f, 45.0f }[level];
            int nums = new int[] { 0, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 7, 7, 8, 9 }[level];
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "ALaughingSmile", 10000, 10000 / nums);
            skill.OnAdditionStart += this.StartEvent;
            skill.OnAdditionEnd += this.EndEvent;
            skill.OnUpdate += this.UpdateTimeHandler;
            SkillHandler.ApplyAddition(dActor, skill);

            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Fire, factor);
        }


        void StartEvent(Actor actor, DefaultBuff skill)
        {
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
        }
        void UpdateTimeHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.HP > 0 && !actor.Buff.Dead)
            {
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                int demage = 3000;
                actor.HP -= (uint)demage;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, anotherArg, actor, false);
            }
            else
            {
                actor.Status.Additions["ALaughingSmile"].AdditionEnd();
                actor.Status.Additions.TryRemove("ALaughingSmile", out _);
            }
        }
        #endregion
    }
}
