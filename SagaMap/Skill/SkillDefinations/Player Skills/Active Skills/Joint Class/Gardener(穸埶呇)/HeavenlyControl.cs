
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Gardener
{
    /// <summary>
    /// ヘブンリーコントロール
    /// </summary>
    public class HeavenlyControl : ISkill
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
             * ヘブンリーコントロール †
                Active 
                習得JOBLV：29 
                効果：飛空庭周囲の空間を歪め、天体を自在に操作することが出来る。(憑依中使用不可) 
                他人の飛空庭でも自由に使うことが出来る。 
                庭の全体的な色調は変わらない。(夜でも庭が暗くなったりしない) 
                変更できる天体 
                【夕方】（胡桃＆若菜イベントと同様） 
                【夜】（クリスマスイベント時と同様） 
                【宇宙】（夜と似ているが星空に星間雲が浮かぶ）
            */
            if (sActor is ActorPC)
            {
                ActorPC pc = (ActorPC)sActor;
                MapClient client1 = MapClient.FromActorPC((ActorPC)sActor);
                uint Event = 91000538;
                client1.EventActivate(Event);
            }
        }
        #endregion
    }
}