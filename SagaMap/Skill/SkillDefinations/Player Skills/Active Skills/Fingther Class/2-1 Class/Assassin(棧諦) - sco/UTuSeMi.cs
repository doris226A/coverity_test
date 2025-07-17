
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Assassin
{
    /// <summary>
    /// 幻視空蟬（幻視空蝉）
    /// </summary>
    public class UTuSeMi : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            map.TeleportActor(sActor, SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height));

            int lifetime = 5000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "UTuSeMi", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);

            EffectArg arg2 = new EffectArg();
            arg2.effectID = 4011;
            arg2.actorID = dActor.ActorID;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, dActor, true);

            if (!dActor.Status.Additions.ContainsKey("CannotMove"))
            {
                int rate = 30 + 10 * level;
                if (SkillHandler.Instance.CanAdditionApply(dActor, dActor, SkillHandler.DefaultAdditions.CannotMove, rate))
                {
                    int lt = 11000 - 1000 * level;
                    CannotMove sk = new CannotMove(args.skill, dActor, lt);
                    SkillHandler.ApplyAddition(dActor, sk);
                }
            }
            ActorEventHandlers.PCEventHandler _e;
            SagaMap.Network.Client.MapClient myClinet;
            ActorPC myActor = (ActorPC)dActor;
            _e = (ActorEventHandlers.PCEventHandler)dActor.e;
            myClinet = _e.Client;
            myClinet.SendSkillDummy();
            if (myClinet.Character.Tasks.ContainsKey("SkillCast"))
            {
                if (myClinet.Character.Tasks["SkillCast"].Activated)
                {
                    myClinet.Character.Tasks["SkillCast"].Deactivate();
                    myClinet.Character.Tasks.Remove("SkillCast");
                }
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            if (skill.Variable.ContainsKey("UTuSeMi"))
                skill.Variable.Remove("UTuSeMi");
            skill.Variable.Add("UTuSeMi", 50 + 50 * skill.skill.Level);
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_ENTER, skill.skill.Name));
            }
        }


        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (skill.Variable.ContainsKey("UTuSeMi"))
                skill.Variable.Remove("UTuSeMi");
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_LEAVE, skill.skill.Name));
            }
        }
        #endregion
    }
}
