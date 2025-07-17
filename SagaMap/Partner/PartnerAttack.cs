﻿using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaMap;
using SagaMap.Scripting;

namespace SagaMap.Partner
{
    public class PartnerAttack : MultiRunTask
    {
        private PartnerAI partnerai;
        public Actor dActor;

        public PartnerAttack(PartnerAI partnerai, Actor dActor)
        {
            Name = "搭档普攻线程";
            dueTime = 0;
            this.partnerai = partnerai;
            period = calcDelay(partnerai.Partner);
            this.dActor = dActor;
        }

        int calcDelay(Actor actor)
        {
            int aspd = 0;
            uint delay = 0;
            ActorPartner partner = (ActorPartner)partnerai.Partner;
            aspd = partner.Status.aspd;
            delay = 2000 - (uint)(2000 * aspd * 0.001f);
            //ECOKEY 達人系列
            if (actor.Status.aspd_skill_perc >= 1f)
                delay = (uint)(delay / actor.Status.aspd_skill_perc);
            return (int)delay;
        }

        public override void CallBack()
        {
            try
            {
                if (!partnerai.CanAttack)
                    return;
                if (dActor == null || partnerai.Partner == null)
                {
                    if (this.Activated) this.Deactivate();
                    return;
                }

                if (partnerai.Partner.HP == 0 || dActor.HP == 0 || partnerai.Partner.Tasks.ContainsKey("AutoCast"))
                {
                    if (partnerai.Hate.ContainsKey(dActor.ActorID)) partnerai.Hate.TryRemove(dActor.ActorID, out _);//二哈更改Hate
                    if (this.Activated) this.Deactivate();
                    return;
                }
                ActorPartner partner = (ActorPartner)partnerai.Partner;

                /*if (DateTime.Now < partner.TTime["攻击僵直"])//ECOKEY 這裡清掉
                     return;*/

                if (partner.Owner.ActorID == dActor.ActorID)
                {
                    if (Activated) Deactivate();
                    return;
                }
                if (partnerai.Master != null)
                {
                    if (dActor.ActorID == partnerai.Master.ActorID)
                        return;
                    if (dActor.type == ActorType.MOB)
                    {
                        if (((ActorEventHandlers.MobEventHandler)dActor.e).AI.Master != null)
                        {
                            if (((ActorEventHandlers.MobEventHandler)dActor.e).AI.Master.ActorID == partnerai.Master.ActorID)
                                return;
                        }
                    }
                }
                if (dActor.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)dActor;

                    if (pc.HP == 0 || pc.Mode == PlayerMode.NORMAL || pc.PossessionTarget != 0)
                    {
                        if (partnerai.Hate.ContainsKey(dActor.ActorID))
                            partnerai.Hate.TryRemove(dActor.ActorID, out _);//二哈更改Hate
                        if (this.Activated)
                            this.Deactivate();
                        return;
                    }
                }
                if (dActor.type == ActorType.PARTNER)
                {
                    ActorPartner ptner = dActor as ActorPartner;
                    if (ptner.HP == 0 || ptner.Owner.Mode == PlayerMode.NORMAL || ptner.Owner.PossessionTarget != 0)
                    {
                        if (partnerai.Hate.ContainsKey(dActor.ActorID))
                            partnerai.Hate.TryRemove(dActor.ActorID, out _);//二哈更改Hate
                        if (this.Activated)
                            this.Deactivate();
                        return;
                    }
                }
                if (dActor.type == ActorType.PET)
                {
                    ActorPet actorPet = dActor as ActorPet;
                    if (actorPet.HP == 0 || actorPet.Owner.Mode == PlayerMode.NORMAL || actorPet.Owner.PossessionTarget != 0)
                    {
                        if (partnerai.Hate.ContainsKey(dActor.ActorID))
                            partnerai.Hate.TryRemove(dActor.ActorID, out _);//二哈更改Hate
                        if (this.Activated)
                            this.Deactivate();
                        return;
                    }
                }
                SkillArg arg = new SkillArg();
                Skill.SkillHandler.Instance.Attack(partnerai.Partner, dActor, arg);
                partnerai.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, partnerai.Partner, true);
                period = calcDelay(this.partnerai.Partner);
                partner.TTime["攻击僵直"] = DateTime.Now + new TimeSpan(0, 0, 0, 0, period - 500);
            }
            catch (Exception ex)
            {
                Deactivate();
                Logger.ShowError(ex);
            }
        }
    }
}
