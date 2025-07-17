
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Sorcerer
{
    /// <summary>
    /// 固体光环（ソリッドオーラ）
    /// </summary>
    public class SolidAura : ISkill
    {
        KyrieUser user = KyrieUser.Player;
        public enum KyrieUser
        {
            Player,
            Mob,
            Boss
        }
        public SolidAura()
        {
        }
        public SolidAura(KyrieUser user)
        {
            this.user = user;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {

            int lifetime = 7000 + 1000 * level;
            if (user == KyrieUser.Mob)
            {
                lifetime = 16000;
                dActor = sActor;
            }
            else if (user == KyrieUser.Boss)
            {
                lifetime = 60000;
                dActor = sActor;
            }
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 500, false);
            Actor RealActor = dActor;
            if (sActor.type != ActorType.PC)
            {
                foreach (Actor act in affected)
                {
                    if (act.type == ActorType.MOB && !SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        if (SagaLib.Global.Random.Next(1, 100) < 20 && !act.Status.Additions.ContainsKey("MobKyrie"))
                        {
                            RealActor = act;
                        }
                    }

                }
            }

            if (sActor.ActorID == RealActor.ActorID)
            {
                EffectArg arg2 = new EffectArg();
                arg2.effectID = 5167;
                arg2.actorID = RealActor.ActorID;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, RealActor, true);
            }

            DefaultBuff skill = new DefaultBuff(args.skill, RealActor, "MobKyrie", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(RealActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            if (user == KyrieUser.Mob)
            {
                skill["MobKyrie"] = 7;
            }
            else if (user == KyrieUser.Boss)
            {
                skill["MobKyrie"] = 25;
            }
            else
            {
                int[] times = { 0, 5, 5, 6, 6, 7 };
                skill["MobKyrie"] = times[skill.skill.Level];
                if (actor.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)actor;
                    Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_ENTER, skill.skill.Name));
                }
            }
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                Network.Client.MapClient.FromActorPC(pc).SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.SKILL_STATUS_LEAVE, skill.skill.Name));
            }
        }
        #endregion
    }
}
