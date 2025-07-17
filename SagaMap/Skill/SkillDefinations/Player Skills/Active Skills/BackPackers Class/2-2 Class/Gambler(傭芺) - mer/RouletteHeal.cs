﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 幸福輪盤（ルーレットヒーリング）
    /// </summary>
    public class RouletteHeal : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = -(1.0f + 0.6f * level);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 300, true);
            List<Actor> realAffected = new List<Actor>();
            /*realAffected.Add(sActor);
            foreach (Actor act in affected)
            {
                if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            int healCount = SagaLib.Global.Random.Next(0, realAffected.Count-1);
            affected.Clear();
            for (int i = 0; i < healCount; i++)
            {
                affected.Add(realAffected[i]);
            }
            SkillHandler.Instance.MagicAttack(sActor, affected, args, SagaLib.Elements.Holy, factor);*/

            ActorPC sPC = (ActorPC)sActor;
            if (sPC.Mode == PlayerMode.KNIGHT_EAST ||
                    sPC.Mode == PlayerMode.KNIGHT_WEST ||
                    sPC.Mode == PlayerMode.KNIGHT_SOUTH ||
                    sPC.Mode == PlayerMode.KNIGHT_NORTH)
            {
                realAffected.Add(sActor);
                foreach (Actor act in affected)
                {
                    if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        realAffected.Add(act);
                    }
                }
            }
            else
            {
                if (sPC.Party != null)
                {
                    foreach (Actor act in affected)
                    {
                        if (act.type == ActorType.PC)
                        {
                            ActorPC aPC = (ActorPC)act;
                            if (aPC.Party != null && sPC.Party != null)
                            {
                                if ((aPC.Party.ID == sPC.Party.ID) && aPC.Party.ID != 0 && !aPC.Buff.Dead && aPC.PossessionTarget == 0)
                                {
                                    if (act.Buff.NoRegen) continue;

                                    if (aPC.Party.ID == sPC.Party.ID)
                                    {
                                        realAffected.Add(act);
                                    }
                                }
                            }
                        }
                        if (act.type == ActorType.PARTNER)
                        {
                            ActorPartner par = (ActorPartner)act;
                            ActorPC aPC = (ActorPC)par.Owner;
                            if (aPC.Party != null && sPC.Party != null)
                            {
                                if ((aPC.Party.ID == sPC.Party.ID) && aPC.Party.ID != 0)
                                {
                                    if (act.Buff.NoRegen) continue;

                                    if (aPC.Party.ID == sPC.Party.ID)
                                    {
                                        realAffected.Add(act);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    realAffected.Add(sActor);
                    foreach (Actor act in affected)
                    {
                        if (act.type == ActorType.PARTNER)
                        {
                            ActorPartner par = (ActorPartner)act;
                            if (par.Owner == sPC)
                                realAffected.Add(act);
                        }
                    }
                }
            }

            affected.Clear();
            foreach (Actor act in realAffected)
            {
                if (SagaLib.Global.Random.Next(0, 99) < 40)
                {
                    affected.Add(act);
                }
            }
            SkillHandler.Instance.MagicAttack(sActor, affected, args, SagaLib.Elements.Holy, factor);
        }
        #endregion
    }
}