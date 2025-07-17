
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.ActorEventHandlers;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Gardener
{
    /// <summary>
    /// ギャザリング
    /// </summary>
    public class Gathering : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)sActor;
                List<uint> items = new List<uint>() { 10005601, 10005602, 10005603, 10005604, 10005605, 10005606, 10005607, 10005608, 10005609, 10005610, 10005611, 10005612, 10005613, 10005614, 10005615, 10005616, 10005617, 10005618, 10005619, 10005620, 10005621, 10005622, 10005623, 10005624, 10005625, 10005626, 10005627, 10005628, 10005629, 10005630, 10005631, 10005632, 10005633, 10005634, 10005635, 10005636, 10005637, 10005638, 10005639, 10005640, 10005641, 10005642, 10005643, 10005644, 10005645, 10005646, 10005648 };
                foreach (uint i in items)
                {
                    if (SkillHandler.Instance.CountItem(pc, i) > 0)
                    {
                        return 0;
                    }
                }
            }
            return -38;
            //return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*
             * ギャザリング †
                Active 
                習得JOBLV：5 
                効果：地面に多くの種を植え、植物を生やす。
                ファーマースキル「栽培」と専用のアイテムが必要。 
                消費は種×8個のみ（SP・MP消費無し） 
                栽培エリアは自分の周囲8マス 
                同じ種類の種を8個以上持っていない場合はスキルを使用できない
            */
            if (sActor is ActorPC)
            {
                ActorPC pc = (ActorPC)sActor;
                pc.TInt["GatheringX"] = args.x;
                pc.TInt["GatheringY"] = args.y;
                MapClient client1 = MapClient.FromActorPC((ActorPC)sActor);
                uint Event = 91000537;
                client1.EventActivate(Event);
            }

        }
        #endregion
    }
}