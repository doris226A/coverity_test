using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Item;

namespace SagaMap.Skill.SkillDefinations.Explorer
{
    /// <summary>
    /// ブレイドマスタリー
    /// </summary>
    public class AbsorbSpWeapon : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ActorPC pc = (ActorPC)sActor;
            if (pc.Party == null)
            {
                if (pc.Status.Additions.ContainsKey("BloodLeech"))
                {
                    Additions.Global.BloodLeech add = (Additions.Global.BloodLeech)pc.Status.Additions["BloodLeech"];
                    add.rate = 0;
                }
                int time = 30000 + 30000 * level;//SP吸收持续时间
                SpLeech skill = new SpLeech(args.skill, pc, time, 0.05f * level);

                SkillHandler.ApplyAddition(pc, skill);
                EffectArg arg2 = new EffectArg();
                arg2.effectID = 5238;
                arg2.actorID = pc.ActorID;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, pc, true);
                if (pc.Partner != null)
                {
                    ActorPartner par = (ActorPartner)pc.Partner;
                    if (par.Status.Additions.ContainsKey("BloodLeech"))
                    {
                        Additions.Global.BloodLeech add = (Additions.Global.BloodLeech)par.Status.Additions["BloodLeech"];
                        add.rate = 0;
                    }
                    int timeA = 30000 + 30000 * level;//SP吸收持续时间
                    SpLeech skillA = new SpLeech(args.skill, par, timeA, 0.05f * level);

                    SkillHandler.ApplyAddition(par, skillA);
                    EffectArg argA = new EffectArg();
                    arg2.effectID = 5238;
                    arg2.actorID = par.ActorID;
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, argA, par, true);
                    Manager.MapManager.Instance.GetMap(par.MapID).SendEffect(par, 5238);
                }
            }
            else
            {
                List<Actor> affected = map.GetActorsArea(sActor, 200, true);
                foreach (Actor act in affected)
                {
                    if (act.type == ActorType.PC)
                    {
                        if (pc.Party.IsMember((ActorPC)act))
                        {
                            if (!SkillHandler.Instance.CheckValidAttackTarget(sActor, act) && !act.Buff.Dead)
                            {

                                if (act.Status.Additions.ContainsKey("BloodLeech"))
                                {
                                    Additions.Global.BloodLeech add = (Additions.Global.BloodLeech)act.Status.Additions["BloodLeech"];
                                    add.rate = 0;
                                }
                                int time = 30000 + 30000 * level;//SP吸收持续时间
                                SpLeech skill = new SpLeech(args.skill, act, time, 0.05f * level);

                                SkillHandler.ApplyAddition(act, skill);
                                EffectArg arg2 = new EffectArg();
                                arg2.effectID = 5238;
                                arg2.actorID = act.ActorID;
                                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, act, true);
                                ActorPC pc2 = (ActorPC)act;
                                if (pc2.Partner != null)
                                {
                                    ActorPartner par = (ActorPartner)pc2.Partner;
                                    if (par.Status.Additions.ContainsKey("BloodLeech"))
                                    {
                                        Additions.Global.BloodLeech add = (Additions.Global.BloodLeech)par.Status.Additions["BloodLeech"];
                                        add.rate = 0;
                                    }
                                    int timeA = 30000 + 30000 * level;//SP吸收持续时间
                                    SpLeech skillA = new SpLeech(args.skill, par, timeA, 0.05f * level);

                                    SkillHandler.ApplyAddition(par, skillA);
                                    EffectArg argA = new EffectArg();
                                    arg2.effectID = 5238;
                                    arg2.actorID = par.ActorID;
                                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, argA, par, true);
                                    Manager.MapManager.Instance.GetMap(par.MapID).SendEffect(par, 5238);
                                }
                            }
                        }
                    }
                }
            }

        }
        #endregion
    }
}
