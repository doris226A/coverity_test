﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Marionest
{
    /// <summary>
    /// 風沙狂嘯（サンドトルネード）
    /// </summary>
    public class MarioWindEarth : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0.4f + 0.1f * level;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            if (sActor is ActorPC)
            {
                ActorPC pc = sActor as ActorPC;

                if (pc.Skills2.ContainsKey(981) || pc.SkillsReserve.ContainsKey(981) || pc.Skills2_2.ContainsKey(981) || pc.DualJobSkill.Exists(x => x.ID == 981))
                {
                    /*List<Actor> EleSelect = map.GetActorsArea(sActor, 400, false);
                    bool ok = false;
                    foreach (Actor act in EleSelect)
                    {
                        //周边召唤活动木偶判定失败
                        //if (act.type == ActorType.PARTNER)
                        //{
                        //    ActorPartner partner = (ActorPartner)act;
                        //    if (partner.BaseData.id == 16090000)
                        //    {
                        //        continue;
                        //    }
                        //}
                        //判定玩家活动木偶
                        if (act.type == ActorType.PC)
                        {
                            ActorPC apc = (ActorPC)act;
                            if (apc.Marionette != null)
                            {
                                if (apc.Marionette.ID == 10019200)//曼陀罗草
                                {
                                    ok = true;
                                    break;
                                }
                            }
                        }

                    }
                    if (ok == true)
                    {
                        var duallv = 0;
                        if (pc.DualJobSkill.Exists(x => x.ID == 981))
                            duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 981).Level;

                        var mainlv = 0;
                        if (pc.Skills2_2.ContainsKey(981))
                            mainlv = pc.Skills2_2[981].Level;

                        factor += (1.2f + 0.7f * Math.Max(duallv, mainlv));
                    }*/
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 981))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 981).Level;

                    var mainlv = 0;
                    if (pc.Skills2_2.ContainsKey(981))
                        mainlv = pc.Skills2_2[981].Level;
                    if (pc.Skills2.ContainsKey(981))
                        mainlv = pc.Skills2[981].Level;
                    if (pc.SkillsReserve.ContainsKey(981))
                        mainlv = pc.SkillsReserve[981].Level;

                    factor += (1.2f + 0.7f * Math.Max(duallv, mainlv));
                }
                else
                {
                    List<Actor> EleSelect = map.GetActorsArea(sActor, 400, false);
                    foreach (Actor act in EleSelect)
                    {
                        if (act.type == ActorType.PC)
                        {
                            ActorPC apc = (ActorPC)act;
                            if (apc.Marionette != null)
                            {
                                if (apc.Marionette.ID == 10019200)//曼陀罗草
                                {
                                    factor += (1.2f + 0.7f * 5);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (pc.Marionette != null)
                {
                    if (pc.Marionette.ID == 10030001)//电路机械
                    {
                        factor += 2.0f + 0.1f * level;
                    }
                }

            }

            List<Actor> affected = map.GetActorsArea(sActor, 400, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SagaLib.Elements.Earth, factor);
        }
        #endregion
    }
}
