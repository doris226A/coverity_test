
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Gardener
{
    /// <summary>
    /// ウェザーコントロール
    /// </summary>
    public class WeatherControl : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            SagaMap.Map map = SagaMap.Manager.MapManager.Instance.GetMap(sActor.MapID);
            if (map.OriID != 70000000 && map.OriID != 75000000)
                return -29;
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*
             * ウェザーコントロール †
                Active 
                習得JOBLV：28 
                効果：飛空庭周囲の空間を歪め、天候を自在に操作することが出来る。(憑依中使用不可) 
                他人の飛空庭でも自由に使うことが出来る。 
                変更できる天気は【雨】と【雪】

             */
            if (sActor is ActorPC)
            {
                ActorPC pc = (ActorPC)sActor;
                MapClient client1 = MapClient.FromActorPC((ActorPC)sActor);
                uint Event = 91000539;
                client1.EventActivate(Event);
            }
        }
        #endregion
    }
}