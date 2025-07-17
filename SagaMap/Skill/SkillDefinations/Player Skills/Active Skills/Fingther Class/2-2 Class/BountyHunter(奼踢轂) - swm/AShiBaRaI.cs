using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.BountyHunter
{
    /// <summary>
    /// 絆腿（足払い）
    /// </summary>
    public class AShiBaRaI : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (SkillHandler.Instance.CheckValidAttackTarget(sActor, dActor))
            {
                return 0;
            }
            else
            {
                return -14;
            }

        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {


            float factor = 0.9f + 0.1f * level;
            uint MartialArtDamUp_SkillID = 125;
            ActorPC actorPC = (ActorPC)sActor;
            if (actorPC.Skills2.ContainsKey(MartialArtDamUp_SkillID))
            {
                factor = factor + 0.1f * actorPC.Skills2[MartialArtDamUp_SkillID].Level;
            }
            if (actorPC.SkillsReserve.ContainsKey(MartialArtDamUp_SkillID))
            {
                factor = factor + 0.1f * actorPC.SkillsReserve[MartialArtDamUp_SkillID].Level;
            }
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            /*int[] rate = { 0, 5, 10, 15, 20, 25, 30 };
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Stiff, rate[level]))
            {
                Additions.Global.Stiff skill = new SagaMap.Skill.Additions.Global.Stiff(args.skill, dActor, 10000);
                SkillHandler.ApplyAddition(dActor, skill);
            }*/
            if (dActor.type == ActorType.MOB)
            {
                ActorMob m = (ActorMob)dActor;
                if (m.BaseData.fly)
                    return;

            }

            int[] rates = { 0, 50, 60, 70, 80, 90 };
            int rate = rates[level];
            if (SagaMap.Skill.SkillHandler.Instance.isBossMob(dActor))
                rate -= 30;

            if (SagaLib.Global.Random.Next(0, 99) < rate)
            {
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                if (dActor.Tasks.ContainsKey("SkillCast"))
                {
                    if (dActor.Tasks["SkillCast"].Activated)
                    {
                        dActor.Tasks["SkillCast"].Deactivate();
                        dActor.Tasks.Remove("SkillCast");

                        SkillArg arg = new SkillArg();
                        arg.sActor = dActor.ActorID;
                        arg.dActor = 0;
                        arg.skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3311, 1);
                        arg.x = 0;
                        arg.y = 0;
                        arg.hp = new List<int>();
                        arg.sp = new List<int>();
                        arg.mp = new List<int>();
                        arg.hp.Add(0);
                        arg.sp.Add(0);
                        arg.mp.Add(0);
                        arg.argType = SkillArg.ArgType.Active;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, dActor, true);
                    }
                    if (dActor.Tasks.ContainsKey("AutoCast"))
                    {
                        dActor.Tasks["AutoCast"].Deactivate();
                        dActor.Tasks.Remove("AutoCast");
                        dActor.Buff.CannotMove = false;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, dActor, true);
                    }
                    Additions.Global.Stun skill = new SagaMap.Skill.Additions.Global.Stun(args.skill, dActor, 5000);
                    SkillHandler.ApplyAddition(dActor, skill);
                }
            }

        }
        #endregion
    }
}
