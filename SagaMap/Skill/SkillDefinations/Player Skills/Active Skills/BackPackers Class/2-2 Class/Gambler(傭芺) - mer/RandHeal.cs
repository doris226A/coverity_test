
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 擲骰子（ランダムヒーリング）（随机治疗）
    /// </summary>
    public class RandHeal : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (dActor.Buff.NoRegen)
            {
                return -1;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            int min = 0, max = 0;
            if ((SagaLib.Global.Random.Next(0, 99)) < 40)
            {
                switch (level)
                {
                    case 1:
                        min = (int)((sActor.Status.max_matk * 0.36f) + 18);
                        max = (int)((sActor.Status.max_matk * 0.65f) + 32);
                        break;
                    case 2:
                        min = (int)((sActor.Status.max_matk * 0.45f) + 25);
                        max = (int)((sActor.Status.max_matk * 1.16f) + 52);
                        break;
                    case 3:
                        min = (int)((sActor.Status.max_matk * 0.6f) + 30);
                        max = (int)((sActor.Status.max_matk * 1.60f) + 80);
                        break;

                }
                uint calc = (uint)(SagaLib.Global.Random.Next(min, max));
                uint healhp = 0, healsp = 0, healmp = 0;
                healhp = calc;
                dActor.HP += healhp;
                if (dActor.HP > dActor.MaxHP)
                    dActor.HP = dActor.MaxHP;
                if ((SagaLib.Global.Random.Next(0, 99)) < 40)
                {
                    healsp = calc;
                    healmp = calc;
                    dActor.MP += healmp;
                    dActor.SP += healsp;
                    if (dActor.SP > dActor.MaxSP)
                        dActor.SP = dActor.MaxSP;
                    if (dActor.MP > dActor.MaxMP)
                        dActor.MP = dActor.MaxMP;
                    args.affectedActors.Clear();
                    args.affectedActors.Add(dActor);
                    args.Init();
                    args.hp[0] = (int)healhp;
                    args.mp[0] = (int)healmp;
                    args.sp[0] = (int)healsp;
                    args.flag[0] |= SagaLib.AttackFlag.MP_HEAL | SagaLib.AttackFlag.HP_HEAL | SagaLib.AttackFlag.SP_HEAL | SagaLib.AttackFlag.NO_DAMAGE;
                }
                else
                {
                    args.affectedActors.Clear();
                    args.affectedActors.Add(dActor);
                    args.Init();
                    args.hp[0] = (int)healhp;
                    args.flag[0] |= SagaLib.AttackFlag.HP_HEAL | SagaLib.AttackFlag.NO_DAMAGE;
                }


                if (SagaLib.Global.Random.Next(0, 99) < 30)
                {
                    RemoveAddition(dActor, "Poison");
                    RemoveAddition(dActor, "鈍足");
                    RemoveAddition(dActor, "Stone");
                    RemoveAddition(dActor, "Silence");
                    RemoveAddition(dActor, "Stun");
                    RemoveAddition(dActor, "Frosen");
                    RemoveAddition(dActor, "Confuse");
                }
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, dActor, true);
            }
            else
            {
                //避免1HP時死掉
                if (sActor.HP == 1)
                {
                    bool tome = true;
                    SkillArg arg = new SkillArg();
                    arg.affectedActors.Add(sActor);
                    arg.Init();
                    arg.sActor = sActor.ActorID;
                    arg.argType = SkillArg.ArgType.Item_Active;
                    SagaDB.Item.Item item0 = SagaDB.Item.ItemFactory.Instance.GetItem(10000000);
                    arg.item = item0;
                    arg.hp[0] = 0;
                    arg.argType = SkillArg.ArgType.Attack;
                    arg.flag[0] |= SagaLib.AttackFlag.HP_DAMAGE;

                    Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, sActor, tome);
                }
                else if (sActor.HP > sActor.Status.min_matk)
                    SkillHandler.Instance.FixAttack(sActor, sActor, args, SagaLib.Elements.Neutral, sActor.Status.min_matk);
                else if (sActor.HP == sActor.Status.min_matk)
                    SkillHandler.Instance.FixAttack(sActor, sActor, args, SagaLib.Elements.Neutral, sActor.Status.min_matk - 1);
                else
                    SkillHandler.Instance.FixAttack(sActor, sActor, args, SagaLib.Elements.Neutral, sActor.HP - 1);
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, sActor, true);
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
        #endregion
    }
}