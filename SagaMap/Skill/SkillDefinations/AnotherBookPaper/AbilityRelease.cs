using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    public class AbilityRelease : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int[] lifetimelist = new int[] { 0, 180, 185, 190, 195, 200, 205, 210, 220, 230, 240, 250, 270, 280, 290, 300, 1800 };
            var lifetime = lifetimelist[level] * 1000;

            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "AnotherAbilityRelease", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.AnotherAbilityRelease = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.AnotherAbilityRelease = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}
