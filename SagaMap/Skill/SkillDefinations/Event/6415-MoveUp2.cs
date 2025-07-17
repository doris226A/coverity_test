﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Event
{
    /// <summary>
    /// 點火
    /// </summary>
    public class MoveUp2 : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 90000;
            dActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "MoveUp2", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            if (actor.Status.Additions.ContainsKey("MoveUp3"))
            {
                actor.Status.Additions["MoveUp3"].AdditionEnd();
                actor.Status.Additions.TryRemove("MoveUp3", out _);
            }
            if (actor.Status.Additions.ContainsKey("MoveUp4"))
            {
                actor.Status.Additions["MoveUp4"].AdditionEnd();
                actor.Status.Additions.TryRemove("MoveUp4", out _);
            }
            if (actor.Status.Additions.ContainsKey("MoveUp5"))
            {
                actor.Status.Additions["MoveUp5"].AdditionEnd();
                actor.Status.Additions.TryRemove("MoveUp5", out _);
            }



            //移動速度
            int Speed_add = 90;
            if (skill.Variable.ContainsKey("MoveUp2_Speed"))
                skill.Variable.Remove("MoveUp2_Speed");
            skill.Variable.Add("MoveUp2_Speed", Speed_add);



            actor.Status.speed_skill += (ushort)Speed_add;
            actor.Buff.Ignition = true;
            actor.Buff.MoveSpeedUp = true;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);

            //移動速度
            actor.Status.speed_skill -= (ushort)skill.Variable["MoveUp2_Speed"];
            actor.Buff.Ignition = false;
            actor.Buff.MoveSpeedUp = false;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}
