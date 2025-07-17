using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;
using SagaDB;


namespace SagaMap.Skill.SkillDefinations.SoulTaker
{
    class fuenriru : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 200, null);
            List<Actor> affected = new List<Actor>();
            switch (level)
            {
                case 1:
                    if (SkillHandler.Instance.isBossMob(dActor))
                    {
                        return;
                    }
                    int lv = dActor.Level - sActor.Level;
                    int rate = 50;
                    if (lv > 20) rate = 40;
                    if (lv > 30) rate = 30;
                    if (lv > 40) rate = 20;
                    int a = new Random().Next(0, 100);
                    Logger.ShowInfo("a  " + a + "   rate   " + rate);
                    if (a <= rate)
                    {
                        sActor.Status.Pressure_lv = dActor.Level;
                        sActor.Buff.三转フエンリル = true;
                        Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, sActor, true);
                        //DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Pressure", 500000);
                        //SkillHandler.ApplyAddition(sActor, skill);
                        dActor.HP = 0;
                        if (dActor.type == ActorType.MOB)
                        {
                            ((SagaMap.ActorEventHandlers.MobEventHandler)dActor.e).OnDie(false);
                        }
                        else
                        {
                            dActor.e.OnDie();
                        }
                        args.affectedActors.Add(dActor);
                        args.Init();
                        args.flag[0] = SagaLib.AttackFlag.DIE;
                    }
                    break;
                case 2:
                    args = new SkillArg();
                    args.Init();
                    args.skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2066, 5);
                    int hpheal = 0;
                    int mpheal = 0;
                    int spheal = 0;
                    if (sActor.Buff.三转フエンリル)
                    {
                        int hpbouns = (sActor.Status.Pressure_lv - 10) / 2 + 5;
                        int mpspbouns = hpbouns / 2;
                        hpheal = (int)(sActor.MaxHP * (float)(hpbouns / 100.0f));
                        mpheal = (int)(sActor.MaxMP * (float)(mpspbouns / 100.0f));
                        spheal = (int)(sActor.MaxSP * (float)(mpspbouns / 100.0f));
                        sActor.Status.Pressure_lv = 0;
                        sActor.Buff.三转フエンリル = false;
                        Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, sActor, true);
                    }
                    else
                    {
                        hpheal = (int)(sActor.MaxHP * (float)(5.0f / 100.0f));
                        mpheal = (int)(sActor.MaxMP * (float)(2.0f / 100.0f));
                        spheal = (int)(sActor.MaxSP * (float)(2.0f / 100.0f));
                    }
                    args.hp.Add(hpheal);
                    args.mp.Add(mpheal);
                    args.sp.Add(spheal);
                    args.flag.Add(AttackFlag.HP_HEAL | AttackFlag.SP_HEAL | AttackFlag.MP_HEAL | AttackFlag.NO_DAMAGE);
                    sActor.HP += (uint)hpheal;
                    sActor.MP += (uint)mpheal;
                    sActor.SP += (uint)spheal;
                    if (sActor.HP > sActor.MaxHP)
                        sActor.HP = sActor.MaxHP;
                    if (sActor.MP > sActor.MaxMP)
                        sActor.MP = sActor.MaxMP;
                    if (sActor.SP > sActor.MaxSP)
                        sActor.SP = sActor.MaxSP;
                    SkillHandler.Instance.ShowEffectByActor(sActor, 4178);
                    SkillHandler.Instance.ShowVessel(sActor, -hpheal, -mpheal, -spheal);
                    Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, args, sActor, true);
                    break;
                case 3:
                    foreach (Actor i in actors)
                    {
                        if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                            affected.Add(i);
                    }
                    float factor = 11.0f;
                    if (sActor.Buff.三转フエンリル)
                    {
                        if (sActor.Status.Pressure_lv - sActor.Level + 5 > 0)
                        {
                            factor = (sActor.Status.Pressure_lv - sActor.Level + 5) * 0.6f + 11.0f;
                        }
                        sActor.Status.Pressure_lv = 0;
                        sActor.Buff.三转フエンリル = false;
                        Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, sActor, true);
                    }
                    SkillHandler.Instance.PhysicalAttack(sActor, affected, args, sActor.WeaponElement, factor);
                    break;

            }
        }
        #endregion
    }
}
