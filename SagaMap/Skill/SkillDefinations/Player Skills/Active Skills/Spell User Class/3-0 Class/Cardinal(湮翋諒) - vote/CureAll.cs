using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Cardinal
{
    public class CureAll : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.Party != null) return 0;
            else return -12;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            List<Actor> realAffected = new List<Actor>();
            ActorPC sPC = (ActorPC)sActor;
            int[] cureRate = new int[] { 0, 40, 60, 60, 60, 60 };
            foreach (ActorPC act in sPC.Party.Members.Values)
            {
                if (act.Online)
                {
                    if (act.Party.ID != 0 && !act.Buff.Dead && act.MapID == sActor.MapID)
                    {
                        if (SagaLib.Global.Random.Next(0, 100) <= cureRate[level])
                        {
                            RemoveAddition(act, "Poison");
                            RemoveAddition(act, "鈍足");
                            RemoveAddition(act, "Stone");
                            RemoveAddition(act, "Silence");
                            RemoveAddition(act, "Stun");
                            RemoveAddition(act, "Sleep");
                            RemoveAddition(act, "Frosen");
                            RemoveAddition(act, "Confuse");
                        }
                        else
                        {
                            if (sPC.Skills.ContainsKey(3058)) RemoveAddition(act, "Stun");//昏迷

                            if (sPC.Skills.ContainsKey(3060)) RemoveAddition(act, "Poison");//毒

                            if (sPC.Skills.ContainsKey(3062)) RemoveAddition(act, "Silence");//沉默

                            if (sPC.Skills.ContainsKey(3064)) RemoveAddition(act, "Confuse");//混亂

                            if (sPC.Skills.ContainsKey(3066)) RemoveAddition(act, "Sleep");//睡眠

                            //if (sPC.Skills.ContainsKey(3148))//麻痺

                            if (sPC.Skills.ContainsKey(3150)) RemoveAddition(act, "Stone");//石化

                            if (sPC.Skills.ContainsKey(3152)) RemoveAddition(act, "MoveSpeedDown");//遲鈍

                            if (sPC.Skills.ContainsKey(3154)) RemoveAddition(act, "Frosen");//凍結
                        }
                    }
                }
            }
        }
        public void RemoveAddition(Actor actor, String additionName)
        {
            if (actor.Status.Additions.ContainsKey(additionName))
            {
                Addition addition = actor.Status.Additions[additionName];
                actor.Status.Additions.TryRemove(additionName, out _);
                if (addition.Activated)
                {
                    addition.AdditionEnd();
                }
                addition.Activated = false;
            }
        }
    }
}
