using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB;
using SagaDB.Actor;
using SagaLib;
using SagaMap.Manager;
using SagaDB.Item;
using SagaDB.Skill;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill
{
    public partial class SkillHandler
    {
        public void ItemUse(Actor sActor, Actor dActor, SkillArg arg)
        {
            List<Actor> list = new List<Actor>();
            list.Add(dActor);
            ItemUse(sActor, list, arg);
        }

        public void ItemUse(Actor sActor, List<Actor> dActor, SkillArg arg)
        {

            int counter = 0;
            arg.affectedActors = dActor;
            arg.Init();

            if (arg.item.BaseData.duration == 0)
            {
                foreach (Actor i in dActor)
                {
                    //並未將物件參考設定為物件的執行個體。
                    if (i == null || i.Buff == null)
                        continue;

                    if (i.Buff.NoRegen)
                        continue;
                    uint itemhp, itemsp, itemmp, itemep;
                    if (arg.item.BaseData.isRate)
                    {
                        float recover = 1.0f;
                        float recoverhp = 1.0f;
                        float recovermp = 1.0f;
                        float recoversp = 1.0f;
                        if (arg.item.BaseData.itemType == ItemType.FOOD)
                        {
                            float rate = 1, rate_iris = 1;
                            if (i.Status.Additions.ContainsKey("FoodFighter"))//食物技能加成
                            {
                                DefaultPassiveSkill dps = i.Status.Additions["FoodFighter"] as DefaultPassiveSkill;
                                rate = ((float)dps.Variable["FoodFighter"] / 100.0f + 1.0f);

                            }
                            if (i.Status.foot_iris > 100)//追加iris卡逻辑
                            {
                                rate_iris = i.Status.foot_iris / 100.0f;
                            }
                            recover = recover * (rate + rate_iris - 1);

                        }
                        if (arg.item.BaseData.itemType == ItemType.POTION)
                        {
                            float rate = 1, rate_iris = 1;
                            if (i.Status.Additions.ContainsKey("PotionFighter"))//药品技能加成
                            {
                                DefaultPassiveSkill dps = i.Status.Additions["PotionFighter"] as DefaultPassiveSkill;
                                rate = ((float)dps.Variable["PotionFighter"] / 100.0f + 1.0f);

                            }
                            if (i.Status.potion_iris > 100)//追加iris卡逻辑
                            {
                                rate_iris = i.Status.potion_iris / 100.0f;
                            }
                            recover = recover * (rate + rate_iris - 1);

                        }
                        if (i.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)i;
                            /*int pcvit = pc.Vit + pc.Status.vit_item + pc.Status.vit_iris + pc.Status.vit_skill + pc.Status.vit_rev;
                            int pcmag = pc.Mag + pc.Status.mag_item + pc.Status.mag_iris + pc.Status.mag_skill + pc.Status.mag_rev;
                            int pcint = pc.Int + pc.Status.int_item + pc.Status.int_iris + pc.Status.int_skill + pc.Status.int_rev;
                            recoverhp = Math.Min(3, recoverhp * ((100 + pcvit / 2 + pc.Status.hp_recover_iris + pc.Status.hp_recover_skill + pc.Status.hp_recover_item) / 100.0f));
                            recovermp = Math.Min(3, recovermp * ((100 + pcmag / 2 + pc.Status.mp_recover_iris + pc.Status.mp_recover_skill + pc.Status.mp_recover_item) / 100.0f));
                            recoversp = Math.Min(3, recoversp * ((100 + (pcint + pcvit) / 6 + pc.Status.sp_recover_iris + pc.Status.sp_recover_skill + pc.Status.sp_recover_item) / 100.0f));*/

                            recoverhp = pc.Status.hp_recover / 100.0f;
                            recovermp = pc.Status.mp_recover / 100.0f;
                            recoversp = pc.Status.sp_recover / 100.0f;

                        }

                        itemhp = (uint)((i.MaxHP * arg.item.BaseData.hp * recover * recoverhp) / 100.0f);
                        itemsp = (uint)((i.MaxSP * arg.item.BaseData.sp * recover * recoversp) / 100.0f);
                        itemmp = (uint)((i.MaxMP * arg.item.BaseData.mp * recover * recovermp) / 100.0f);
                        itemep = (uint)(arg.item.BaseData.delay / 100.0f);



                    }
                    else
                    {
                        float recover = 1.0f;
                        float recoverhp = 1.0f;
                        float recovermp = 1.0f;
                        float recoversp = 1.0f;
                        if (arg.item.BaseData.itemType == ItemType.FOOD)
                        {
                            float rate = 1, rate_iris = 1;
                            if (i.Status.Additions.ContainsKey("FoodFighter"))//食物技能加成
                            {
                                DefaultPassiveSkill dps = i.Status.Additions["FoodFighter"] as DefaultPassiveSkill;
                                rate = ((float)dps.Variable["FoodFighter"] / 100.0f + 1.0f);

                            }
                            if (i.Status.foot_iris > 100)//追加iris卡逻辑
                            {
                                rate_iris = i.Status.foot_iris / 100.0f;
                            }
                            recover = recover * (rate + rate_iris - 1);
                        }
                        if (arg.item.BaseData.itemType == ItemType.POTION)
                        {
                            float rate = 1, rate_iris = 1;
                            if (i.Status.Additions.ContainsKey("PotionFighter"))//药品技能加成
                            {
                                DefaultPassiveSkill dps = i.Status.Additions["PotionFighter"] as DefaultPassiveSkill;
                                rate = ((float)dps.Variable["PotionFighter"] / 100.0f + 1.0f);

                            }
                            if (i.Status.potion_iris > 100)//追加iris卡逻辑
                            {
                                rate_iris = i.Status.potion_iris / 100.0f;
                            }
                            recover = recover * (rate + rate_iris - 1);
                        }
                        if (i.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)i;
                            /*int pcvit = pc.Vit + pc.Status.vit_item + pc.Status.vit_iris + pc.Status.vit_skill + pc.Status.vit_rev;
                            int pcmag = pc.Mag + pc.Status.mag_item + pc.Status.mag_iris + pc.Status.mag_skill + pc.Status.mag_rev;
                            int pcint = pc.Int + pc.Status.int_item + pc.Status.int_iris + pc.Status.int_skill + pc.Status.int_rev;
                            recoverhp = Math.Min(3, recoverhp * ((100 + pcvit / 2 + pc.Status.hp_recover_iris + pc.Status.hp_recover_skill + pc.Status.hp_recover_item + pc.Status.hp_recover_mario) / 100.0f));
                            recovermp = Math.Min(3, recovermp * ((100 + pcmag / 2 + pc.Status.mp_recover_iris + pc.Status.mp_recover_skill + pc.Status.mp_recover_item + pc.Status.mp_recover_mario) / 100.0f));
                            recoversp = Math.Min(3, recoversp * ((100 + (pcint + pcvit) / 6 + pc.Status.sp_recover_iris + pc.Status.sp_recover_skill + pc.Status.sp_recover_item) / 100.0f));*/

                            recoverhp = pc.Status.hp_recover / 100.0f;
                            recovermp = pc.Status.mp_recover / 100.0f;
                            recoversp = pc.Status.sp_recover / 100.0f;
                        }
                        itemhp = (uint)(arg.item.BaseData.hp * recover * recoverhp);
                        itemsp = (uint)(arg.item.BaseData.sp * recover * recoversp);
                        itemmp = (uint)(arg.item.BaseData.mp * recover * recovermp);
                        itemep = (uint)(arg.item.BaseData.delay);
                    }
                    i.HP = (i.HP + itemhp);
                    i.SP = (i.SP + itemsp);
                    i.MP = (i.MP + itemmp);
                    i.EP = (i.EP + itemep);

                    if (i.HP > i.MaxHP)
                        i.HP = i.MaxHP;
                    if (i.SP > i.MaxSP)
                        i.SP = i.MaxSP;
                    if (i.MP > i.MaxMP)
                        i.MP = i.MaxMP;
                    if (i.EP > i.MaxEP)
                        i.EP = i.MaxEP;

                    if (arg.item.BaseData.hp > 0)
                    {
                        arg.flag[counter] |= AttackFlag.HP_HEAL;
                        arg.hp[counter] = (int)(-itemhp);
                    }
                    else if (arg.item.BaseData.hp < 0)
                    {
                        arg.flag[counter] |= AttackFlag.HP_DAMAGE;
                        arg.hp[counter] = (int)(-itemhp);
                    }
                    if (arg.item.BaseData.sp > 0)
                    {
                        arg.flag[counter] |= AttackFlag.SP_HEAL;
                        arg.sp[counter] = (int)(-itemsp);
                    }
                    else if (arg.item.BaseData.sp < 0)
                    {
                        arg.flag[counter] |= AttackFlag.SP_DAMAGE;
                        arg.sp[counter] = (int)(-itemsp);
                    }
                    if (arg.item.BaseData.mp > 0)
                    {
                        arg.flag[counter] |= AttackFlag.MP_HEAL;
                        arg.mp[counter] = (int)(-itemmp);
                    }
                    else if (arg.item.BaseData.mp < 0)
                    {
                        arg.flag[counter] |= AttackFlag.MP_DAMAGE;
                        arg.mp[counter] = (int)(-itemmp);
                    }
                    counter++;
                    abnormalStatus(i, arg);//ECOKEY 異常狀態系列道具
                    Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, i, true);
                }
            }
            else if (arg.item.BaseData.duration > 0)
            {
                foreach (Actor i in dActor)
                {
                    if (i.Buff.NoRegen)
                        continue;
                    if (arg.item.BaseData.hp > 0)
                    {
                        i.Status.hp_medicine = arg.item.BaseData.hp;
                        Additions.Global.Medicine1 skill1 = new SagaMap.Skill.Additions.Global.Medicine1(null, i, (int)arg.item.BaseData.duration);
                        SkillHandler.ApplyAddition(i, skill1);
                    }
                    if (arg.item.BaseData.mp > 0)
                    {
                        i.Status.mp_medicine = arg.item.BaseData.mp;
                        Additions.Global.Medicine2 skill2 = new SagaMap.Skill.Additions.Global.Medicine2(null, i, (int)arg.item.BaseData.duration);
                        SkillHandler.ApplyAddition(i, skill2);
                    }
                    if (arg.item.BaseData.sp > 0)
                    {
                        i.Status.sp_medicine = arg.item.BaseData.sp;
                        Additions.Global.Medicine3 skill3 = new SagaMap.Skill.Additions.Global.Medicine3(null, i, (int)arg.item.BaseData.duration);
                        SkillHandler.ApplyAddition(i, skill3);
                    }
                    abnormalStatus(i, arg);//ECOKEY 異常狀態系列道具
                }
            }
            //arg.delay = 5000;
        }
        //ECOKEY 異常狀態系列道具
        void abnormalStatus(Actor dActor, SkillArg skill)
        {
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Poisen))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Poisen] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("Poison"))
                    {
                        dActor.Status.Additions["Poison"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Poison", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Poisen] == -100)
                {
                    Skill.Additions.Global.Poison Poison = new Poison(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Poison);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Stone))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Stone] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("Stone"))
                    {
                        dActor.Status.Additions["Stone"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Stone", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Stone] == -100)
                {
                    Skill.Additions.Global.Stone Stone = new Stone(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Stone);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Paralyse))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Paralyse] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("Paralyse"))
                    {
                        dActor.Status.Additions["Paralyse"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Paralyse", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Stun] == -100)
                {
                    Skill.Additions.Global.Stun Stun = new Stun(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Stun);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Sleep))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Sleep] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("Sleep"))
                    {
                        dActor.Status.Additions["Sleep"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Sleep", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Sleep] == -100)
                {
                    Skill.Additions.Global.Sleep Sleep = new Sleep(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Sleep);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Silence))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Silence] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("Silence"))
                    {
                        dActor.Status.Additions["Silence"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Silence", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Silence] == -100)
                {
                    Skill.Additions.Global.Silence Silence = new Silence(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Silence);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.MoveSpeedDown))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.MoveSpeedDown] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("MoveSpeedDown"))
                    {
                        dActor.Status.Additions["MoveSpeedDown"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("MoveSpeedDown", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.MoveSpeedDown] == -100)
                {
                    Skill.Additions.Global.MoveSpeedDown MoveSpeedDown = new MoveSpeedDown(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, MoveSpeedDown);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Confused))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Confused] == 100)
                {
                    //混亂修復
                    if (dActor.Status.Additions.ContainsKey("Confuse"))
                    {
                        dActor.Status.Additions["Confuse"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Confuse", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Confused] == -100)
                {
                    Skill.Additions.Global.Confuse Confuse = new Confuse(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Confuse);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Frosen))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Frosen] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("Frosen"))
                    {
                        dActor.Status.Additions["Frosen"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Frosen", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Frosen] == -100)
                {
                    Skill.Additions.Global.Freeze Freeze = new Freeze(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Freeze);
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Stun))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Stun] == 100)
                {
                    if (dActor.Status.Additions.ContainsKey("Stun"))
                    {
                        dActor.Status.Additions["Stun"].AdditionEnd();
                        dActor.Status.Additions.TryRemove("Stun", out _);
                    }
                }
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Stun] == -100)
                {
                    Skill.Additions.Global.Stun Stun = new Stun(skill.skill, dActor, 10000);
                    SkillHandler.ApplyAddition(dActor, Stun);
                }
            }
        }
    }
}
