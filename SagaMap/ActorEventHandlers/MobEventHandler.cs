using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaMap.Network.Client;

using SagaLib;
using SagaDB;
using SagaDB.Actor;
using SagaDB.Item;

using SagaMap.Manager;
using System.Collections.Concurrent;
using SagaDB.Skill;

namespace SagaMap.ActorEventHandlers
{
    public class MobEventHandler : ActorEventHandler
    {
        public ActorMob mob;
        public Mob.MobAI AI;
        Scripting.MobCallback currentCall;
        ActorPC currentPC;
        public event Scripting.MobCallback Dying;
        public event Scripting.MobCallback Attacking;
        public event Scripting.MobCallback Moving;
        public event Scripting.MobCallback Defending;
        public event Scripting.MobCallback Returning;
        public event Scripting.MobCallback SkillUsing;
        public event Scripting.MobCallback Updating;
        public event Scripting.MobCallback FirstTimeDefending;

        public MobEventHandler(ActorMob mob)
        {
            this.mob = mob;
            this.AI = new SagaMap.Mob.MobAI(mob);
        }

        #region ActorEventHandler Members
        public void OnActorSkillCancel(Actor sActor)
        {

        }
        public void OnActorAppears(Actor aActor)
        {
            if (!mob.VisibleActors.Contains(aActor.ActorID))
                mob.VisibleActors.Add(aActor.ActorID);
            if (aActor.type == ActorType.PC)
            {
                if (!this.AI.Activated)
                    this.AI.Start();
            }
            //這段會強制怪物攻擊SHADOW
            /*if (aActor.type == ActorType.SHADOW && this.AI.Hate.Count != 0)
            {
                if (!this.AI.Hate.ContainsKey(aActor.ActorID))
                    this.AI.Hate.Add(aActor.ActorID, this.mob.MaxHP);
            }*/
        }
        public void OnPlayerShopChange(Actor aActor)
        {

        }
        public void OnPlayerShopChangeClose(Actor aActor)
        {

        }
        public void OnActorChangeEquip(Actor sActor, MapEventArgs args)
        {

        }

        public void OnActorChat(Actor cActor, MapEventArgs args)
        {

        }

        public void OnActorDisappears(Actor dActor)
        {
            //ECOKEY 四重奏error
            lock (mob.VisibleActors)
            {
                if (mob.VisibleActors != null && mob.VisibleActors.Contains(dActor.ActorID))
                {
                    mob.VisibleActors.Remove(dActor.ActorID);
                }
            }
            /*if (mob.VisibleActors != null && mob.VisibleActors.Contains(dActor.ActorID))
                  mob.VisibleActors.Remove(dActor.ActorID);*/
            //這段要新增PARTNER，不然怪會追寵追到天荒地老
            if (dActor.type == ActorType.PC || dActor.type == ActorType.PARTNER)
            {
                if (this.AI.Hate.ContainsKey(dActor.ActorID))
                    this.AI.Hate.TryRemove(dActor.ActorID, out _);//二哈更改Hate
            }
        }
        public void OnActorReturning(Actor sActor)
        {
            try
            {
                if (Returning != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                        {
                            RunCallback(Returning, (ActorPC)AI.lastAttacker);
                            return;
                        }
                        else if (AI.lastAttacker.type == ActorType.SHADOW)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                {
                                    ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                    RunCallback(Returning, pc);
                                    return;
                                }
                            }
                        }
                    }
                    RunCallback(Returning, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void OnActorSkillUse(Actor sActor, MapEventArgs args)
        {
            SkillArg arg = (SkillArg)args;
            try
            {
                AI.OnSeenSkillUse(arg);
            }
            catch { }
            try
            {
                if (SkillUsing != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                        {
                            RunCallback(SkillUsing, (ActorPC)AI.lastAttacker);
                            return;
                        }
                        else if (AI.lastAttacker.type == ActorType.SHADOW)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                {
                                    ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                    RunCallback(SkillUsing, (ActorPC)pc);
                                    return;
                                }
                            }
                        }
                    }
                    RunCallback(SkillUsing, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }

        }
        public void OnActorStartsMoving(Actor mActor, short[] pos, ushort dir, ushort speed)
        {

        }
        public void OnActorStartsMoving(Actor mActor, short[] pos, ushort dir, ushort speed, MoveType moveType)
        {
        }

        public void OnActorStopsMoving(Actor mActor, short[] pos, ushort dir, ushort speed)
        {
            try
            {
                if (Moving != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                            RunCallback(Moving, (ActorPC)AI.lastAttacker);
                        else if (AI.lastAttacker.type == ActorType.SHADOW)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                {
                                    ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                    RunCallback(Moving, (ActorPC)pc);
                                }
                                else
                                    RunCallback(Moving, null);
                            }
                            else
                                RunCallback(Moving, null);
                        }
                        else
                            RunCallback(Moving, null);
                    }
                    else
                        RunCallback(Moving, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void OnCreate(bool success)
        {

        }


        public void OnActorChangeEmotion(Actor aActor, MapEventArgs args)
        {

        }

        public void OnActorChangeMotion(Actor aActor, MapEventArgs args)
        {

        }
        public void OnActorChangeWaitType(Actor aActor) { }
        public void OnDelete()
        {
            AI.Pause();
        }


        public void OnCharInfoUpdate(Actor aActor)
        {

        }


        public void OnPlayerSizeChange(Actor aActor)
        {

        }

        bool checkDropSpecial()
        {
            if (this.AI.firstAttacker != null)
            {
                if (this.AI.firstAttacker.Status != null)
                {
                    foreach (Addition i in this.AI.firstAttacker.Status.Additions.Values)
                    {
                        if (i.GetType() == typeof(Skill.Additions.Global.Knowledge))
                        {
                            Skill.Additions.Global.Knowledge know = (Skill.Additions.Global.Knowledge)i;
                            if (know.MobTypes.Contains(this.mob.BaseData.mobType))
                                return true;
                        }
                    }
                }
                else
                    return false;
            }
            else
                return false;
            return false;
        }

        public void OnDie()
        {
            OnDie(true);
        }

        public void OnDie(bool loot)
        {
            if (this.mob.Buff.Dead) return;
            this.mob.Buff.Dead = true;
            try
            {
                if (this.mob.Owner != null)
                    this.mob.Owner.Slave.Remove(this.mob);
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
            if (AI.firstAttacker != null)
            {
                if (AI.firstAttacker.type == ActorType.GOLEM)
                {
                    ActorGolem golem = (ActorGolem)AI.firstAttacker;
                    MobEventHandler ehs = (MobEventHandler)golem.e;
                    Skill.Additions.Global.OtherAddition skills = new Skill.Additions.Global.OtherAddition(null, golem, "石像击杀怪物CD", Global.Random.Next(10000, 45000));
                    skills.OnAdditionStart += (s, e) =>
                    {
                        ehs.AI.Mode.mask.SetValue(1, false);
                    };
                    skills.OnAdditionEnd += (s, e) =>
                    {
                        ehs.AI.Mode.mask.SetValue(1, true);
                    };
                    Skill.SkillHandler.ApplyAddition(golem, skills);
                }
            }
            //ECOKEY 召喚物禁止增加信賴度
            if (this.AI.Master != null && this.AI.Master.type == ActorType.PC)
            {
                loot = false;
            }
            else
            {
                //ECOKEY 是否有經驗掉落判斷
                if (this.AI.DamageTable.Count != 0 && loot)//ECOKEY 0720禁止掉落物時不執行此處
                {
                    loot = false;
                    //ECOKEY 參考ProcessMobExp優化->下面兩句
                    ConcurrentDictionary<uint, int> damagetable = this.AI.DamageTable;//二哈更改1206
                    foreach (uint i in damagetable.Keys.ToArray())//ECOKEY 0701優化這句
                    {
                        Actor actor = this.AI.map.GetActor(i);
                        if (actor == null)
                            continue;
                        if (actor.type == ActorType.PC)
                        {
                            loot = true;
                            //break;
                        }
                        //ECOKEY 寵物新增信賴度 1205更新信賴度加倍道具
                        if (actor.type == ActorType.PARTNER)
                        {
                            ActorPartner par = (ActorPartner)actor;
                            uint rexp = 1;
                            if (mob.Level >= 1 && mob.Level <= 20) //[lv1-21怪+1信賴度]
                            {
                                rexp = 1;
                            }
                            if (mob.Level >= 21 && mob.Level <= 40) //[lv21-40怪+2信賴度]
                            {
                                rexp = 2;
                            }
                            if (mob.Level >= 41 && mob.Level <= 60) //[41-60怪+3信賴度]
                            {
                                rexp = 3;
                            }
                            if (mob.Level >= 61 && mob.Level <= 80) //[61-80怪+4信賴度]
                            {
                                rexp = 4;
                            }
                            if (mob.Level >= 81)//0705更新，此處不須限制110以下//&& mob.Level <= 110) //[81-110怪+5信賴度]
                            {
                                rexp = 5;
                            }
                            if (par.Owner.TimerItems.ContainsKey("RELIABILITY_BOUNS"))
                            {
                                rexp = rexp * (uint.Parse(par.Owner.TimerItems["RELIABILITY_BOUNS"].BuffValues) / 100);
                            }
                            par.reliabilityexp += rexp;
                            MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Name + " 獲得了" + rexp + "點信賴度.目前信賴度" + par.reliabilityexp + "");
                            //信賴度EXP升信賴度by ECOKEY
                            if (par.reliabilityexp >= 2400 && par.reliabilityexp <= 11999) //紫 總2,400
                            {
                                par.reliability = 1;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp >= 12000 && par.reliabilityexp <= 47999) //青 總12,000
                            {
                                par.reliability = 2;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp >= 48000 && par.reliabilityexp <= 95999) //水 總48,000
                            {
                                par.reliability = 3;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp >= 96000 && par.reliabilityexp <= 143999)//綠 總96,000
                            {
                                par.reliability = 4;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp >= 144000 && par.reliabilityexp <= 191999)//黃綠 總144,000
                            {
                                par.reliability = 5;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp >= 192000 && par.reliabilityexp <= 239999)//黃 總192,000
                            {
                                par.reliability = 6;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp >= 240000 && par.reliabilityexp <= 359999)//橙 總240,000
                            {
                                par.reliability = 7;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp >= 360000 && par.reliabilityexp <= 479999) //赤 總360,000
                            {
                                par.reliability = 8;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            if (par.reliabilityexp > 480000) // && par.reliabilityexp <= 480000) //桃 總480,000
                            {
                                par.reliability = 9;
                                MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                            }
                            //continue;
                        }
                    }
                }
            }
            if (this.mob.Status.Additions.ContainsKey("Rebone"))
            {
                this.mob.Buff.Dead = false;

                this.mob.HP = this.mob.MaxHP;
                Skill.SkillHandler.RemoveAddition(this.mob, "Rebone");
                this.AI.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.mob, false);
                Skill.Additions.Global.Zombie zombie = new SagaMap.Skill.Additions.Global.Zombie(this.mob);
                Skill.SkillHandler.ApplyAddition(this.mob, zombie);
                this.mob.Status.undead = true;
                this.AI.DamageTable.Clear();
                this.AI.Hate.Clear();
                this.AI.firstAttacker = null;
            }
            else
            {
                try
                {
                    if(AI.Mode.EventDieSkill != 0)//ECOKEY 怪物死亡後放技能
                    {
                        SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(AI.Mode.EventDieSkill, 1);
                        SkillArg arg = new SkillArg();
                        arg.sActor = this.mob.ActorID;
                        arg.dActor = this.mob.ActorID;
                        arg.skill = skill;
                        AI.OnSkillCastComplete(arg);
                    }
                    if (Dying != null)
                    {
                        if (AI.lastAttacker != null)
                        {
                            if (AI.lastAttacker.type == ActorType.PC)
                                RunCallback(Dying, (ActorPC)AI.lastAttacker);
                            else if(AI.lastAttacker.type == ActorType.PARTNER)//ECOKEY 為圖書館以防萬一的優化
                                RunCallback(Dying, ((ActorPartner)AI.lastAttacker).Owner);
                            else if (AI.lastAttacker.type == ActorType.SHADOW)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                                {
                                    if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                    {
                                        ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                        RunCallback(Dying, (ActorPC)pc);
                                    }
                                    else
                                        RunCallback(Dying, null);
                                }
                                else
                                    RunCallback(Dying, null);
                            }
                            else
                                RunCallback(Dying, null);
                        }
                        else
                            RunCallback(Dying, null);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
                this.AI.Pause();
                if (loot)
                {

                    //分配经验
                    ExperienceManager.Instance.ProcessMobExp(mob);
                    //drops
                    //special drops
                    //boss掉心
                    if (Configuration.Instance.ActiveSpecialLoot)
                        if (mob.BaseData.mobType.ToString().Contains("BOSS") && AI.SpawnDelay >= 1800000)
                        {
                            if (Global.Random.Next(0, 10000) <= Configuration.Instance.BossSpecialLootRate)
                                for (int i = 0; i < Configuration.Instance.BossSpecialLootNum; i++)
                                    this.AI.map.AddItemDrop(Configuration.Instance.BossSpecialLootID, null, this.mob, false, false, false);
                        }
                        else
                            if (Global.Random.Next(0, 10000) <= Configuration.Instance.NomalMobSpecialLootRate && ((ActorEventHandlers.MobEventHandler)mob.e).AI.SpawnDelay != 0)
                            for (int i = 0; i < Configuration.Instance.NomalMobSpecialLootNum; i++)
                                this.AI.map.AddItemDrop(Configuration.Instance.NomalMobSpecialLootID, null, this.mob, false, false, false);

                    //pllboss ECOKEY
                    if (Configuration.Instance.ActiveSpecialLoot)
                        if (mob.BaseData.mobType.ToString().Contains("EVENT_BOSS") && AI.SpawnDelay >= 1800000)
                        {
                            if (Global.Random.Next(0, 1) <= Configuration.Instance.BossSpecialLootRate)
                                for (int i = 0; i < Configuration.Instance.BossSpecialLootNum; i++)
                                    this.AI.map.AddItemDrop(Configuration.Instance.BossSpecialLootID, null, this.mob, false, true, false);
                        }
                        else
                            if (Global.Random.Next(0, 1) <= Configuration.Instance.NomalMobSpecialLootRate && ((ActorEventHandlers.MobEventHandler)mob.e).AI.SpawnDelay != 0)
                            for (int i = 0; i < Configuration.Instance.NomalMobSpecialLootNum; i++)
                                this.AI.map.AddItemDrop(Configuration.Instance.NomalMobSpecialLootID, null, this.mob, false, true, false);


                    //drops
                    int dropDeterminator = Global.Random.Next(0, 10000);
                    int baseValue = 0, maxVlaue = 0;
                    bool stamp = false;
                    bool special = false;
                    ActorPC owner = null;
                    if (mob.type == ActorType.MOB)
                    {
                        //List<Actor> actors = MapManager.Instance.GetMap(mob.MapID).GetActorsArea(mob, 12700, false).Where(x => x.type == ActorType.PC && (x as ActorPC).Online).ToList();
                        ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)mob.e;
                        //这里只认在线的第一攻击者为owner
                        if (eh.AI.firstAttacker != null && eh.AI.firstAttacker.type == ActorType.PC && (eh.AI.firstAttacker as ActorPC).Online)
                            owner = (ActorPC)eh.AI.firstAttacker;
                    }
                    //如果owner不存在, 那就没掉落了. 虽然这不官方, 不过暂时还没办法做成官方的无主掉落
                    if (owner != null)
                    {
                       /* //一周年PLL活動/聖誕老人怪(這打野怪有機率出現)
                        int rate_pll = 0;
                        if (owner.Level - mob.Level > 4) //藍名
                        {
                            rate_pll = 20;
                        }
                        if (owner.Level - mob.Level < 4) //紅名
                        {
                            rate_pll = 100;
                        }
                        if (owner.Level - mob.Level <= 4 && owner.Level - mob.Level >= -4) //綠名
                        {
                            rate_pll = 50;
                        }
                        if (Global.Random.Next(0, 1000) <= rate_pll)
                        {
                            this.AI.map.SpawnMob(92400011, this.AI.Mob.X, this.AI.Mob.Y, 10, null);//ID改成專用怪物
                        }*/

                        //印章因为目前掉率全都是0,所以取了最小的万分之一
                        if (mob.BaseData.stampDrop != null)
                        {
                            //EcoKey判斷怪物是不是整隻都寵物打的，如果整隻都寵物打就不會掉東西
                            /*if (ExperienceManager.Instance.PARTNERattack == false)
                            {
                                if (Global.Random.Next(0, 9999) <= (mob.BaseData.stampDrop.Rate * Configuration.Instance.CalcStampDropRateForPC(owner)))
                            {
                                this.AI.map.AddItemDrop(mob.BaseData.stampDrop.ItemID, null, this.mob, false, false, false);
                                stamp = true;
                            }
                            }*/
                            //ECOKEY 全新傷害計算不用判斷
                            if (Global.Random.Next(0, 9999) <= (mob.BaseData.stampDrop.Rate * Configuration.Instance.CalcStampDropRateForPC(owner)))
                            {
                                this.AI.map.AddItemDrop(mob.BaseData.stampDrop.ItemID, null, this.mob, false, false, false);
                                stamp = true;
                            }
                        }

                        //dropDeterminator = this.mob.BaseData.dropItems.Sum(x => x.Rate) + this.mob.BaseData.dropItemsSpecial.Sum(x => x.Rate);
                        //特殊掉落(知识掉落)
                        if ((!stamp || Configuration.Instance.MultipleDrop) && checkDropSpecial())
                        {
                            foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItemsSpecial)
                            {

                                dropDeterminator = Global.Random.Next(0, 9999);
                                if (!Configuration.Instance.MultipleDrop)
                                {
                                    maxVlaue = baseValue + (int)(i.Rate * Configuration.Instance.CalcSpecialDropRateForPC(owner) / 100.0f);
                                    if (dropDeterminator >= baseValue && dropDeterminator < maxVlaue)
                                    {
                                        this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                        special = true;
                                    }
                                    baseValue = maxVlaue;
                                }
                                else
                                {
                                    if (dropDeterminator < (i.Rate * Configuration.Instance.CalcSpecialDropRateForPC(owner) / 100.0f))
                                    {
                                        this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                        special = true;
                                    }
                                }
                            }
                        }

                        baseValue = 0;
                        maxVlaue = 0;
                        //如果已经掉落印章,并且掉落特殊物品,同时开启了多重掉落
                        if ((!stamp && !special) || Configuration.Instance.MultipleDrop)
                        {
                            if (Configuration.Instance.MultipleDrop)
                            {
                                foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItems)
                                {
                                    int denominator = mob.BaseData.dropItems.Sum(x => x.Rate);

                                    //这里简单的做一个头发的过滤
                                    if (i.ItemID != 10000000)
                                        continue;

                                    if (Global.Random.Next(1, denominator) < (i.Rate * Configuration.Instance.CalcGlobalDropRateForPC(owner)))
                                        this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);

                                }

                            }
                            else
                            {
                                //如果这个怪物有掉落的话...
                                if (this.mob.BaseData.dropItems.Count > 0)
                                {
                                    //EcoKey判斷怪物是不是整隻都寵物打的，如果整隻都寵物打就不會掉東西
                                    /*if (ExperienceManager.Instance.PARTNERattack == false)
                                    {
                                        
                                    }*/
                                    //ECOKEY 全新傷害計算不用判斷
                                    maxVlaue = baseValue = 0;
                                    bool oneshotdrop = false;
                                    int denominator = Global.Random.Next(1, mob.BaseData.dropItems.Sum(x => x.Rate));
                                    for (int ix = 0; ix < (int)Configuration.Instance.CalcGlobalDropRateForPC(owner); ix++)
                                    {
                                        foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItems)
                                        {
                                            if (oneshotdrop)
                                                continue;
                                            //ECOKEY 掉寶提升
                                            if (mob.Status.SkillRate.ContainsKey(3349))
                                            {
                                                int num = mob.BaseData.dropItems.Sum(x => x.Rate);
                                                switch (mob.Status.SkillRate[3349])
                                                {
                                                    case 1:
                                                        denominator += (int)(num * 0.001f);
                                                        break;
                                                    case 2:
                                                        denominator += (int)(num * 0.005f);
                                                        break;
                                                    case 3:
                                                        denominator += (int)(num * 0.01f);
                                                        break;
                                                    default:
                                                        denominator += (int)(num * 0.001f);
                                                        break;
                                                }
                                                if (denominator > num)
                                                    denominator = num;
                                            }
                                            maxVlaue = baseValue + i.Rate;
                                            if (denominator >= baseValue && denominator < maxVlaue)
                                            {

                                                //这里简单的做一个头发的过滤, 掉了个头发也算是掉东西了.
                                                if (i.ItemID != 10000000)
                                                {
                                                    this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                                    oneshotdrop = true;
                                                }
                                                else
                                                {
                                                    if (ix == (int)Configuration.Instance.CalcGlobalDropRateForPC(owner) - 1)
                                                        oneshotdrop = true;
                                                }
                                            }
                                            baseValue = maxVlaue;
                                        }
                                    }
                                }
                            }
                        }
                        #region "舊紀錄"
                        /*
                         if ((!stamp || Configuration.Instance.MultipleDrop) && checkDropSpecial())
                    {
                        foreach (SagaDB.Mob.MobData.DropData i in this.mob.dropItemsSpecial)//团队道具
                        {
                            if (!Configuration.Instance.MultipleDrop)
                            {
                                maxVlaue = baseValue + i.Rate;
                                if (dropDeterminator >= baseValue && dropDeterminator < maxVlaue)
                                {
                                    this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20, i.count);
                                    special = true;
                                }
                                baseValue = maxVlaue;
                            }
                            else //个人道具掉率
                            {
                                if (Global.Random.Next(0, 10000) < (i.Rate * Configuration.Instance.CalcSpecialDropRateForPC(owner)))
                                {
                                    this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20, i.count);
                                    special = true;
                                }
                            }
                        }
                    }

                    baseValue = 0;
                    maxVlaue = 0;

                    int usedtime = (int)(DateTime.Now - mob.BattleStartTime).TotalSeconds;
                    if ((!stamp && !special) || Configuration.Instance.MultipleDrop)
                    {
                        int clv = 10;
                        if (mob.Level > 60)
                            clv = mob.Level - 50;
                        int maxcount = clv / 4;
                        if (maxcount < 3) maxcount = 3;
                        if (maxcount > 6) maxcount = 6;

                        Map map = MapManager.Instance.GetMap(mob.MapID);
                        if (!map.IsDungeon)
                        {
                            if (mob.MobID == 10000000)//下次活动再降低原版怪掉落
                            {
                                if (Global.Random.Next(0, 10000) < 5800)
                                    AI.map.AddItemDrop(952000000, null, this.mob, false, false, false, (ushort)Global.Random.Next(1, maxcount));
                            }
                            else
                            {
                                if (Global.Random.Next(0, 10000) < 2500)
                                    AI.map.AddItemDrop(952000000, null, this.mob, false, false, false, (ushort)Global.Random.Next(1, maxcount));
                            }
                        }
                        int count = (int)(mob.MaxHP / 30000);
                        if (count > 5) count = 5;
                        if(count < 1) count = 1;
                        if (mob.MobID == 10000000)
                        {
                            /*if (Global.Random.Next(0, 10000) < 3000)
                            AI.map.AddItemDrop(110005605, null, this.mob, false, false, false, (ushort)count);
                    }
                    else
                    {
                        if (Global.Random.Next(0, 10000) < 2000)
                            AI.map.AddItemDrop(110005605, null, this.mob, false, false, false, (ushort)count);
                        if (OriMobSetting.Instance.Items.ContainsKey(mob.MobID) && !mob.YggMob)
                        {
                            byte attendeeCount = 0;
                            AI.DamageTable.Keys.ToList().ForEach(e =>
                            {
                                Actor ac = AI.map.GetActor(e);
                                if (ac != null)
                                    if (ac.type == ActorType.PC)
                                        attendeeCount++;
                            });
                            int rate = attendeeCount * 8;
                            if (Global.Random.Next(0, 100) < rate)
                            {
                                if (Global.Random.Next(0, 100) < 80)
                                {
                                    AI.map.AddItemDrop(950000015, null, mob, false, true, false, 1, 0, 0, 10000, false, mob.MobID);
                                    AI.map.AddItemDrop(950000015, null, mob, false, false, true, 1, 0, 0, 10000, false, mob.MobID);
                                }
                                else
                                {
                                    AI.map.AddItemDrop(10020758, null, mob, false, true, false, 1, 0, 0, 10000, false, mob.MobID);
                                    AI.map.AddItemDrop(10020758, null, mob, false, false, true, 1, 0, 0, 10000, false, mob.MobID);
                                }
                                //TODO:增加设定好的额外掉落
                            }
                        }
                        if (Global.Random.Next(0, 10000) < 5)
                            AI.map.AddItemDrop(10020758, null, mob, false, false, false, (ushort)1, 0, 0, 10000, false, mob.MobID);
                    }

                    foreach (SagaDB.Mob.MobData.DropData i in this.mob.dropItems)
                    {
                        if ((i.GreaterThanTime == 0 && i.LessThanTime == 0) || (usedtime > i.GreaterThanTime && i.GreaterThanTime != 0) || (usedtime <= i.LessThanTime && i.LessThanTime != 0))
                        {
                            if (!Configuration.Instance.MultipleDrop)
                            {
                                maxVlaue = baseValue + i.Rate;
                                if (dropDeterminator >= baseValue && dropDeterminator < maxVlaue)
                                    AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20, i.count);
                                baseValue = maxVlaue;
                            }
                            else//个人道具掉率
                            {
                                if (i.Roll)
                                {
                                    if (Global.Random.Next(0, 10000) < i.Rate)
                                        AI.map.AddItemDrop(i.ItemID, null, this.mob, false, false, false, i.count, i.MinCount, i.MaxCount, i.Rate, i.Roll);//ROLL
                                }
                                else if (i.Party)
                                {
                                    if (Global.Random.Next(0, 10000) < i.Rate)
                                        AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20, i.count, i.MinCount, i.MaxCount, i.Rate);
                                }
                                else
                                {
                                    if (Global.Random.Next(0, 10000) < (i.Rate * Configuration.Instance.CalcGlobalDropRateForPC(owner)))
                                        AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20, i.count, i.MinCount, i.MaxCount);
                                }
                            }
                        }
                    }
                    */
                        #endregion

                        #region "富豪戒指"
                        //ECOKEY 富豪戒指
                        if (owner.TimerItems.ContainsKey("ITEM_DROP_BOUNS"))
                        {
                            int dropup = int.Parse(owner.TimerItems["ITEM_DROP_BOUNS"].BuffValues) / 100 - 1;
                            do
                            {
                                dropup--;
                                //印章因为目前掉率全都是0,所以取了最小的万分之一
                                if (mob.BaseData.stampDrop != null)
                                {
                                    //EcoKey判斷怪物是不是整隻都寵物打的，如果整隻都寵物打就不會掉東西
                                    /* if (ExperienceManager.Instance.PARTNERattack == false)
                                     {
                                         if (Global.Random.Next(0, 9999) <= (mob.BaseData.stampDrop.Rate * Configuration.Instance.CalcStampDropRateForPC(owner)))
                                     {
                                         this.AI.map.AddItemDrop(mob.BaseData.stampDrop.ItemID, null, this.mob, false, false, false);
                                         stamp = true;
                                     }
                                     }*/
                                    //ECOKEY 全新傷害計算不用判斷
                                    if (Global.Random.Next(0, 9999) <= (mob.BaseData.stampDrop.Rate * Configuration.Instance.CalcStampDropRateForPC(owner)))
                                    {
                                        this.AI.map.AddItemDrop(mob.BaseData.stampDrop.ItemID, null, this.mob, false, false, false);
                                        stamp = true;
                                    }
                                }
                                //特殊掉落(知识掉落)
                                if ((!stamp || Configuration.Instance.MultipleDrop) && checkDropSpecial())
                                {
                                    foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItemsSpecial)
                                    {
                                        dropDeterminator = Global.Random.Next(0, 9999);
                                        if (!Configuration.Instance.MultipleDrop)
                                        {
                                            maxVlaue = baseValue + (int)(i.Rate * Configuration.Instance.CalcSpecialDropRateForPC(owner) / 100.0f);
                                            if (dropDeterminator >= baseValue && dropDeterminator < maxVlaue)
                                            {
                                                this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                                special = true;
                                            }
                                            baseValue = maxVlaue;
                                        }
                                        else
                                        {
                                            if (dropDeterminator < (i.Rate * Configuration.Instance.CalcSpecialDropRateForPC(owner) / 100.0f))
                                            {
                                                this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                                special = true;
                                            }
                                        }
                                    }
                                }

                                baseValue = 0;
                                maxVlaue = 0;
                                //如果已经掉落印章,并且掉落特殊物品,同时开启了多重掉落
                                if ((!stamp && !special) || Configuration.Instance.MultipleDrop)
                                {
                                    if (Configuration.Instance.MultipleDrop)
                                    {
                                        foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItems)
                                        {
                                            int denominator = mob.BaseData.dropItems.Sum(x => x.Rate);
                                            //这里简单的做一个头发的过滤
                                            if (i.ItemID != 10000000)
                                                continue;
                                            if (Global.Random.Next(1, denominator) < (i.Rate * Configuration.Instance.CalcGlobalDropRateForPC(owner)))
                                                this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                        }
                                    }
                                    else
                                    {
                                        //如果这个怪物有掉落的话...
                                        if (this.mob.BaseData.dropItems.Count > 0)
                                        {
                                            //EcoKey判斷怪物是不是整隻都寵物打的，如果整隻都寵物打就不會掉東西
                                            /*if (ExperienceManager.Instance.PARTNERattack == false)
                                            {
                                                
                                            }*/
                                            //ECOKEY 全新傷害計算不用判斷
                                            maxVlaue = baseValue = 0;
                                            bool oneshotdrop = false;
                                            int denominator = Global.Random.Next(1, mob.BaseData.dropItems.Sum(x => x.Rate));
                                            for (int ix = 0; ix < (int)Configuration.Instance.CalcGlobalDropRateForPC(owner); ix++)
                                            {
                                                foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItems)
                                                {
                                                    if (oneshotdrop)
                                                        continue;
                                                    //ECOKEY 掉寶提升
                                                    if (mob.Status.SkillRate.ContainsKey(3349))
                                                    {
                                                        int num = mob.BaseData.dropItems.Sum(x => x.Rate);
                                                        switch (mob.Status.SkillRate[3349])
                                                        {
                                                            case 1:
                                                                denominator += (int)(num * 0.001f);
                                                                break;
                                                            case 2:
                                                                denominator += (int)(num * 0.005f);
                                                                break;
                                                            case 3:
                                                                denominator += (int)(num * 0.01f);
                                                                break;
                                                            default:
                                                                denominator += (int)(num * 0.001f);
                                                                break;
                                                        }
                                                        if (denominator > num)
                                                            denominator = num;
                                                    }
                                                    maxVlaue = baseValue + i.Rate;
                                                    if (denominator >= baseValue && denominator < maxVlaue)
                                                    {

                                                        //这里简单的做一个头发的过滤, 掉了个头发也算是掉东西了.
                                                        if (i.ItemID != 10000000)
                                                        {
                                                            this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                                            oneshotdrop = true;
                                                        }
                                                        else
                                                        {
                                                            if (ix == (int)Configuration.Instance.CalcGlobalDropRateForPC(owner) - 1)
                                                                oneshotdrop = true;
                                                        }
                                                    }
                                                    baseValue = maxVlaue;
                                                }
                                            }
                                        }
                                    }
                                }
                            } while (dropup > 0);
                        }
                        #endregion
                    }


                }

                this.mob.ClearTaskAddition();
                this.AI.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.mob, false);
                /*Tasks.Mob.DeleteCorpse task = new SagaMap.Tasks.Mob.DeleteCorpse(this.mob);
                this.mob.Tasks.Add("DeleteCorpse", task);
                task.Activate();*/
                //ECOKEY 檢查是否已經存在相同索引鍵的任務
                if (!this.mob.Tasks.ContainsKey("DeleteCorpse"))
                {
                    Tasks.Mob.DeleteCorpse task = new SagaMap.Tasks.Mob.DeleteCorpse(this.mob);
                    this.mob.Tasks.Add("DeleteCorpse", task);
                    task.Activate();
                }
                //ECOKEY 檢查是否已經存在相同索引鍵的任務
                if (!this.mob.Tasks.ContainsKey("Respawn"))
                {
                    if (this.AI.SpawnDelay != 0)
                    {
                        Tasks.Mob.Respawn respawn = new SagaMap.Tasks.Mob.Respawn(this.mob, this.AI.SpawnDelay);
                        this.mob.Tasks.Add("Respawn", respawn);
                        respawn.Activate();

                    }
                }
                /*if (this.AI.SpawnDelay != 0)
                {
                    Tasks.Mob.Respawn respawn = new SagaMap.Tasks.Mob.Respawn(this.mob, this.AI.SpawnDelay);
                    this.mob.Tasks.Add("Respawn", respawn);
                    respawn.Activate();

                    //if (this.AI.Announce != "" && this.AI.SpawnDelay >= 300)
                    //{
                    //    Tasks.Mob.RespawnAnnounce respawnannounce = new Tasks.Mob.RespawnAnnounce(this.mob, this.AI.SpawnDelay - 300000);
                    //    this.mob.Tasks.Add("RespawnAnnounce", respawnannounce);
                    //    respawnannounce.Activate();
                    //}
                }*/

                AncientArkManager.Instance.討罰任務判斷(this.AI.map, this.mob);//ECOKEY 圖書館
                this.AI.firstAttacker = null;
                this.mob.Status.attackingActors.Clear();
                this.AI.DamageTable.Clear();
                this.mob.VisibleActors.Clear();
                if (this.AI.Mode.Symbol || this.AI.Mode.SymbolTrash)
                {
                    ODWarManager.Instance.SymbolDown(this.mob.MapID, this.mob);
                }
            }

        }


        public void OnKick()
        {
            throw new NotImplementedException();
        }

        public void OnMapLoaded()
        {
            throw new NotImplementedException();
        }

        public void OnReSpawn()
        {
            throw new NotImplementedException();
        }

        public void OnSendMessage(string from, string message)
        {
            throw new NotImplementedException();
        }

        public void OnSendWhisper(string name, string message, byte flag)
        {
            throw new NotImplementedException();
        }

        public void OnTeleport(short x, short y)
        {
            throw new NotImplementedException();
        }

        public void OnAttack(Actor aActor, MapEventArgs args)
        {
            SkillArg arg = (SkillArg)args;
            AI.OnSeenSkillUse(arg);
            try
            {
                if (Attacking != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                        {
                            RunCallback(Attacking, (ActorPC)AI.lastAttacker);
                            return;
                        }
                        else if (AI.lastAttacker.type == ActorType.SHADOW)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                {
                                    ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                    RunCallback(Attacking, (ActorPC)pc);
                                    return;
                                }
                            }
                        }
                    }
                    RunCallback(Attacking, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void OnHPMPSPUpdate(Actor sActor)
        {
            /*if (Skill.SkillHandler.Instance.isBossMob(this.mob))
            {
                if (!this.mob.Tasks.ContainsKey("MobRecover"))
                {
                    Tasks.Mob.MobRecover MobRecover = new SagaMap.Tasks.Mob.MobRecover((ActorMob)this.mob);
                    this.mob.Tasks.Add("MobRecover", MobRecover);
                    MobRecover.Activate();
                }
            }*///关闭怪物回复线程以节省资源
            if (sActor.HP < sActor.MaxHP * 0.05f) return;
            if (Defending != null)
            {
                if (AI.lastAttacker != null)
                {
                    if (AI.lastAttacker.type == ActorType.PC)
                    {
                        RunCallback(Defending, (ActorPC)AI.lastAttacker);
                        return;
                    }
                    else if (AI.lastAttacker.type == ActorType.SHADOW)
                    {
                        if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                RunCallback(Defending, pc);
                                return;
                            }
                        }
                    }
                }
                RunCallback(Defending, null);
            }
            if (FirstTimeDefending != null && !mob.FirstDefending)
            {
                mob.FirstDefending = true;
                if (AI.lastAttacker != null)
                {
                    if (AI.lastAttacker.type == ActorType.PC)
                    {
                        RunCallback(FirstTimeDefending, (ActorPC)AI.lastAttacker);
                        return;
                    }
                    else if (AI.lastAttacker.type == ActorType.SHADOW)
                    {
                        if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                RunCallback(FirstTimeDefending, pc);
                                return;
                            }
                        }
                    }
                }
                RunCallback(FirstTimeDefending, null);
            }

        }

        public void OnPlayerChangeStatus(ActorPC aActor)
        {

        }

        public void OnActorChangeBuff(Actor sActor)
        {

        }
        public void OnLevelUp(Actor sActor, MapEventArgs args)
        {
        }
        public void OnPlayerMode(Actor aActor)
        {
        }

        public void OnShowEffect(Actor aActor, MapEventArgs args)
        {
        }

        public void OnActorPossession(Actor aActor, MapEventArgs args)
        {

        }
        public void OnActorPartyUpdate(ActorPC aActor)
        {

        }
        public void OnActorSpeedChange(Actor mActor)
        {

        }

        public void OnSignUpdate(Actor aActor)
        {

        }

        public void PropertyUpdate(UpdateEvent arg, int para)
        {
            switch (arg)
            {
                case UpdateEvent.SPEED:
                    AI.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SPEED_UPDATE, null, this.mob, true);
                    break;
            }
        }
        public void PropertyRead(UpdateEvent arg)
        {
        }

        public void OnActorRingUpdate(ActorPC aActor)
        { }

        public void OnActorWRPRankingUpdate(ActorPC aActor)
        { }

        public void OnActorChangeAttackType(ActorPC aActor)
        { }
        public void OnActorFurnitureSit(ActorPC aActor)
        { }
        public void OnActorFurnitureList(Object obj)
        { }
        void RunCallback(Scripting.MobCallback callback, ActorPC pc)
        {
            try
            {
                currentCall = callback;
                currentPC = pc;
                System.Threading.Thread th = new System.Threading.Thread(Run);
                th.Start();
            }
            catch (Exception ex)
            {
                SagaLib.Logger.ShowError(ex);
            }
        }
        DateTime mark = DateTime.Now;
        void Run()
        {
            try
            {
                if (currentCall != null)
                {
                    if (currentPC != null)
                        currentCall.Invoke(this, currentPC);
                    else
                    {
                        if (this.AI.map.Creator != null)
                            currentCall.Invoke(this, this.AI.map.Creator);
                    }
                }
            }
            catch (Exception ex)
            {
                SagaLib.Logger.ShowError(ex);
            }
        }
        public void OnUpdate(Actor aActor)
        {
            try
            {
                if (Updating != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                            RunCallback(Updating, (ActorPC)AI.lastAttacker);
                        else
                            RunCallback(Updating, null);
                    }
                    else
                        RunCallback(Updating, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }
        public void OnActorPaperChange(ActorPC aActor)
        {
        }
        #endregion
    }
}



/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaMap.Network.Client;

using SagaLib;
using SagaDB;
using SagaDB.Actor;
using SagaDB.Item;

using SagaMap.Manager;

namespace SagaMap.ActorEventHandlers
{
    public class MobEventHandler : ActorEventHandler
    {
        public ActorMob mob;
        public Mob.MobAI AI;
        Scripting.MobCallback currentCall;
        ActorPC currentPC;
        public event Scripting.MobCallback Dying;
        public event Scripting.MobCallback Attacking;
        public event Scripting.MobCallback Moving;
        public event Scripting.MobCallback Defending;
        public event Scripting.MobCallback Returning;
        public event Scripting.MobCallback SkillUsing;
        public event Scripting.MobCallback Updating;
        public event Scripting.MobCallback FirstTimeDefending;

        public MobEventHandler(ActorMob mob)
        {
            this.mob = mob;
            this.AI = new SagaMap.Mob.MobAI(mob);
        }

        #region ActorEventHandler Members
        public void OnActorSkillCancel(Actor sActor)
        {

        }
        public void OnActorAppears(Actor aActor)
        {
            if (!mob.VisibleActors.Contains(aActor.ActorID))
                mob.VisibleActors.Add(aActor.ActorID);
            if (aActor.type == ActorType.PC)
            {
                if (!this.AI.Activated)
                    this.AI.Start();
            }
            if (aActor.type == ActorType.SHADOW && this.AI.Hate.Count != 0)
            {
                if (!this.AI.Hate.ContainsKey(aActor.ActorID))
                    this.AI.Hate.Add(aActor.ActorID, this.mob.MaxHP);
            }
        }
        public void OnPlayerShopChange(Actor aActor)
        {

        }
        public void OnPlayerShopChangeClose(Actor aActor)
        {

        }
        public void OnActorChangeEquip(Actor sActor, MapEventArgs args)
        {

        }

        public void OnActorChat(Actor cActor, MapEventArgs args)
        {

        }

        public void OnActorDisappears(Actor dActor)
        {
            //ECOKEY 四重奏error
            lock (mob.VisibleActors)
            {
                if (mob.VisibleActors != null && mob.VisibleActors.Contains(dActor.ActorID))
                {
                    mob.VisibleActors.Remove(dActor.ActorID);
                }
            }

            // if (mob.VisibleActors.Contains(dActor.ActorID))
            //       mob.VisibleActors.Remove(dActor.ActorID);

            if (this.AI.Hate != null && this.AI.Hate.ContainsKey(dActor.ActorID))
            {
                this.AI.Hate.Remove(dActor.ActorID);
            }


            if (dActor.type == ActorType.PC)
            {
                if (this.AI.Hate.ContainsKey(dActor.ActorID))
                    this.AI.Hate.Remove(dActor.ActorID);
            }
        }
        public void OnActorReturning(Actor sActor)
        {
            try
            {
                if (Returning != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                        {
                            RunCallback(Returning, (ActorPC)AI.lastAttacker);
                            return;
                        }
                        else if (AI.lastAttacker.type == ActorType.SHADOW)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                {
                                    ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                    RunCallback(Returning, pc);
                                    return;
                                }
                            }
                        }
                    }
                    RunCallback(Returning, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void OnActorSkillUse(Actor sActor, MapEventArgs args)
        {
            SkillArg arg = (SkillArg)args;
            try
            {
                AI.OnSeenSkillUse(arg);
            }
            catch { }
            try
            {
                if (SkillUsing != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                        {
                            RunCallback(SkillUsing, (ActorPC)AI.lastAttacker);
                            return;
                        }
                        else if (AI.lastAttacker.type == ActorType.SHADOW)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                {
                                    ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                    RunCallback(SkillUsing, (ActorPC)pc);
                                    return;
                                }
                            }
                        }
                    }
                    RunCallback(SkillUsing, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }

        }
        public void OnActorStartsMoving(Actor mActor, short[] pos, ushort dir, ushort speed)
        {

        }
        public void OnActorStartsMoving(Actor mActor, short[] pos, ushort dir, ushort speed, MoveType moveType)
        {
        }

        public void OnActorStopsMoving(Actor mActor, short[] pos, ushort dir, ushort speed)
        {
            try
            {
                if (Moving != null)
                {
                    if (AI.lastAttacker != null)
                    {
                        if (AI.lastAttacker.type == ActorType.PC)
                            RunCallback(Moving, (ActorPC)AI.lastAttacker);
                        else if (AI.lastAttacker.type == ActorType.SHADOW)
                        {
                            if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                {
                                    ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                    RunCallback(Moving, (ActorPC)pc);
                                }
                                else
                                    RunCallback(Moving, null);
                            }
                            else
                                RunCallback(Moving, null);
                        }
                        else
                            RunCallback(Moving, null);
                    }
                    else
                        RunCallback(Moving, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void OnCreate(bool success)
        {

        }


        public void OnActorChangeEmotion(Actor aActor, MapEventArgs args)
        {

        }

        public void OnActorChangeMotion(Actor aActor, MapEventArgs args)
        {

        }
        public void OnActorChangeWaitType(Actor aActor) { }
        public void OnDelete()
        {
            AI.Pause();
        }


        public void OnCharInfoUpdate(Actor aActor)
        {

        }


        public void OnPlayerSizeChange(Actor aActor)
        {

        }

        bool checkDropSpecial()
        {
            if (this.AI.firstAttacker != null)
            {
                if (this.AI.firstAttacker.Status != null)
                {
                    foreach (Addition i in this.AI.firstAttacker.Status.Additions.Values)
                    {
                        if (i.GetType() == typeof(Skill.Additions.Global.Knowledge))
                        {
                            Skill.Additions.Global.Knowledge know = (Skill.Additions.Global.Knowledge)i;
                            if (know.MobTypes.Contains(this.mob.BaseData.mobType))
                                return true;
                        }
                    }
                }
                else
                    return false;
            }
            else
                return false;
            return false;
        }

        public void OnDie()
        {
            OnDie(true);
        }

        public void OnDie(bool loot)
        {
            if (this.mob.Buff.Dead) return;
            this.mob.Buff.Dead = true;
            try
            {
                if (this.mob.Owner != null)
                    this.mob.Owner.Slave.Remove(this.mob);
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
            if (AI.firstAttacker != null)
            {
                if (AI.firstAttacker.type == ActorType.GOLEM)
                {
                    ActorGolem golem = (ActorGolem)AI.firstAttacker;
                    MobEventHandler ehs = (MobEventHandler)golem.e;
                    Skill.Additions.Global.OtherAddition skills = new Skill.Additions.Global.OtherAddition(null, golem, "石像击杀怪物CD", Global.Random.Next(10000, 45000));
                    skills.OnAdditionStart += (s, e) =>
                    {
                        ehs.AI.Mode.mask.SetValue(1, false);
                    };
                    skills.OnAdditionEnd += (s, e) =>
                    {
                        ehs.AI.Mode.mask.SetValue(1, true);
                    };
                    Skill.SkillHandler.ApplyAddition(golem, skills);
                }
            }
            if (this.mob.Status.Additions.ContainsKey("Rebone"))
            {
                this.mob.Buff.Dead = false;

                this.mob.HP = this.mob.MaxHP;
                Skill.SkillHandler.RemoveAddition(this.mob, "Rebone");
                this.AI.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.mob, false);
                Skill.Additions.Global.Zombie zombie = new SagaMap.Skill.Additions.Global.Zombie(this.mob);
                Skill.SkillHandler.ApplyAddition(this.mob, zombie);
                this.mob.Status.undead = true;
                this.AI.DamageTable.Clear();
                this.AI.Hate.Clear();
                this.AI.firstAttacker = null;
            }
            else
            {
                try
                {
                    if (Dying != null)
                    {
                        if (AI.lastAttacker != null)
                        {
                            if (AI.lastAttacker.type == ActorType.PC)
                                RunCallback(Dying, (ActorPC)AI.lastAttacker);
                            else if (AI.lastAttacker.type == ActorType.SHADOW)
                            {
                                if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                                {
                                    if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                                    {
                                        ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                                        RunCallback(Dying, (ActorPC)pc);
                                    }
                                    else
                                        RunCallback(Dying, null);
                                }
                                else
                                    RunCallback(Dying, null);
                            }
                            else
                                RunCallback(Dying, null);
                        }
                        else
                            RunCallback(Dying, null);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
                this.AI.Pause();
                if (loot)
                {
                    //分配经验
                    ExperienceManager.Instance.ProcessMobExp(mob);
                    //drops
                    //special drops

                    //boss掉心
                    if (Configuration.Instance.ActiveSpecialLoot)
                        if (mob.BaseData.mobType.ToString().Contains("BOSS") && AI.SpawnDelay >= 1800000)
                        {
                            if (Global.Random.Next(0, 10000) <= Configuration.Instance.BossSpecialLootRate)
                                for (int i = 0; i < Configuration.Instance.BossSpecialLootNum; i++)
                                    this.AI.map.AddItemDrop(Configuration.Instance.BossSpecialLootID, null, this.mob, false, false, false);
                        }
                        else
                            if (Global.Random.Next(0, 10000) <= Configuration.Instance.NomalMobSpecialLootRate && ((ActorEventHandlers.MobEventHandler)mob.e).AI.SpawnDelay != 0)
                            for (int i = 0; i < Configuration.Instance.NomalMobSpecialLootNum; i++)
                                this.AI.map.AddItemDrop(Configuration.Instance.NomalMobSpecialLootID, null, this.mob, false, false, false);

                    //pllboss ECOKEY
                    if (Configuration.Instance.ActiveSpecialLoot)
                        if (mob.BaseData.mobType.ToString().Contains("EVENT_BOSS") && AI.SpawnDelay >= 1800000)
                        {
                            if (Global.Random.Next(0, 1) <= Configuration.Instance.BossSpecialLootRate)
                                for (int i = 0; i < Configuration.Instance.BossSpecialLootNum; i++)
                                    this.AI.map.AddItemDrop(Configuration.Instance.BossSpecialLootID, null, this.mob, false, true, false);
                        }
                        else
                            if (Global.Random.Next(0, 1) <= Configuration.Instance.NomalMobSpecialLootRate && ((ActorEventHandlers.MobEventHandler)mob.e).AI.SpawnDelay != 0)
                            for (int i = 0; i < Configuration.Instance.NomalMobSpecialLootNum; i++)
                                this.AI.map.AddItemDrop(Configuration.Instance.NomalMobSpecialLootID, null, this.mob, false, true, false);


                    //drops
                    int dropDeterminator = Global.Random.Next(0, 10000);
                    int baseValue = 0, maxVlaue = 0;
                    bool stamp = false;
                    bool special = false;
                    ActorPC owner = null;
                    if (mob.type == ActorType.MOB)
                    {
                        //List<Actor> actors = MapManager.Instance.GetMap(mob.MapID).GetActorsArea(mob, 12700, false).Where(x => x.type == ActorType.PC && (x as ActorPC).Online).ToList();
                        ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)mob.e;
                        //这里只认在线的第一攻击者为owner
                        if (eh.AI.firstAttacker != null && eh.AI.firstAttacker.type == ActorType.PC && (eh.AI.firstAttacker as ActorPC).Online)
                            owner = (ActorPC)eh.AI.firstAttacker;
                    }
                    //如果owner不存在, 那就没掉落了. 虽然这不官方, 不过暂时还没办法做成官方的无主掉落
                    if (owner != null)
                    {
                        //印章因为目前掉率全都是0,所以取了最小的万分之一
                        if (mob.BaseData.stampDrop != null)
                        {
                            //EcoKey判斷怪物是不是整隻都寵物打的，如果整隻都寵物打就不會掉東西
                            if (ExperienceManager.Instance.PARTNERattack == false)
                            {
                                if (Global.Random.Next(0, 9999) <= (mob.BaseData.stampDrop.Rate * Configuration.Instance.CalcStampDropRateForPC(owner)))
                            {
                                this.AI.map.AddItemDrop(mob.BaseData.stampDrop.ItemID, null, this.mob, false, false, false);
                                stamp = true;
                            }
                            }
                        }

                        //dropDeterminator = this.mob.BaseData.dropItems.Sum(x => x.Rate) + this.mob.BaseData.dropItemsSpecial.Sum(x => x.Rate);
                        //特殊掉落(知识掉落)
                        if ((!stamp || Configuration.Instance.MultipleDrop) && checkDropSpecial())
                        {
                            foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItemsSpecial)
                            {
                                dropDeterminator = Global.Random.Next(0, 9999);
                                if (!Configuration.Instance.MultipleDrop)
                                {
                                    maxVlaue = baseValue + (int)(i.Rate * Configuration.Instance.CalcSpecialDropRateForPC(owner) / 100.0f);
                                    if (dropDeterminator >= baseValue && dropDeterminator < maxVlaue)
                                    {
                                        this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                        special = true;
                                    }
                                    baseValue = maxVlaue;
                                }
                                else
                                {
                                    if (dropDeterminator < (i.Rate * Configuration.Instance.CalcSpecialDropRateForPC(owner) / 100.0f))
                                    {
                                        this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                        special = true;
                                    }
                                }
                            }
                        }

                        baseValue = 0;
                        maxVlaue = 0;
                        //如果已经掉落印章,并且掉落特殊物品,同时开启了多重掉落
                        if ((!stamp && !special) || Configuration.Instance.MultipleDrop)
                        {
                            if (Configuration.Instance.MultipleDrop)
                            {
                                foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItems)
                                {
                                    int denominator = mob.BaseData.dropItems.Sum(x => x.Rate);

                                    //这里简单的做一个头发的过滤
                                    if (i.ItemID != 10000000)
                                        continue;

                                    if (Global.Random.Next(1, denominator) < (i.Rate * Configuration.Instance.CalcGlobalDropRateForPC(owner)))
                                        this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                }

                            }
                            else
                            {
                                //如果这个怪物有掉落的话...
                                if (this.mob.BaseData.dropItems.Count > 0)
                                {
                                    //EcoKey判斷怪物是不是整隻都寵物打的，如果整隻都寵物打就不會掉東西
                                    if (ExperienceManager.Instance.PARTNERattack == false)
                                    {
                                        maxVlaue = baseValue = 0;
                                        bool oneshotdrop = false;
                                        int denominator = Global.Random.Next(1, mob.BaseData.dropItems.Sum(x => x.Rate));

                                        for (int ix = 0; ix < (int)Configuration.Instance.CalcGlobalDropRateForPC(owner); ix++)
                                        {
                                            foreach (SagaDB.Mob.MobData.DropData i in this.mob.BaseData.dropItems)
                                            {
                                                if (oneshotdrop)
                                                    continue;

                                                //ECOKEY 掉寶提升
                                                if (mob.Buff.DoubleUp)
                                                {
                                                    switch (owner.Skills2_1[3349].Level)
                                                    {
                                                        case 1:
                                                            denominator += 500;
                                                            break;
                                                        case 2:
                                                            denominator += 800;
                                                            break;
                                                        case 3:
                                                            denominator += 1000;
                                                            break;
                                                    }
                                                    int num = mob.BaseData.dropItems.Sum(x => x.Rate);
                                                    if (denominator > num)
                                                        denominator = num;
                                                }

                                                maxVlaue = baseValue + i.Rate;
                                                if (denominator >= baseValue && denominator < maxVlaue)
                                                {
                                                    //这里简单的做一个头发的过滤, 掉了个头发也算是掉东西了.
                                                    if (i.ItemID != 10000000)
                                                    {
                                                        this.AI.map.AddItemDrop(i.ItemID, i.TreasureGroup, this.mob, i.Party, i.Public, i.Public20);
                                                        oneshotdrop = true;
                                                    }
                                                    else
                                                    {
                                                        if (ix == (int)Configuration.Instance.CalcGlobalDropRateForPC(owner) - 1)
                                                            oneshotdrop = true;
                                                    }
                                                }
                                                baseValue = maxVlaue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                this.mob.ClearTaskAddition();
                this.AI.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.mob, false);
                Tasks.Mob.DeleteCorpse task = new SagaMap.Tasks.Mob.DeleteCorpse(this.mob);
                this.mob.Tasks.Add("DeleteCorpse", task);
                task.Activate();

                /* if (this.AI.SpawnDelay != 0)
                 {
                     Tasks.Mob.Respawn respawn = new SagaMap.Tasks.Mob.Respawn(this.mob, this.AI.SpawnDelay);
                     this.mob.Tasks.Add("Respawn", respawn);
                     respawn.Activate();

                 }*/

/*        string taskKey = "Respawn";

        // 檢查是否已經存在相同索引鍵的任務
        if (!this.mob.Tasks.ContainsKey(taskKey))
        {
            if (this.AI.SpawnDelay != 0)
            {
                Tasks.Mob.Respawn respawn = new SagaMap.Tasks.Mob.Respawn(this.mob, this.AI.SpawnDelay);
                this.mob.Tasks.Add(taskKey, respawn);
                respawn.Activate();

            }
        }


        this.AI.firstAttacker = null;
        this.mob.Status.attackingActors.Clear();
        this.AI.DamageTable.Clear();
        this.mob.VisibleActors.Clear();

        if (this.AI.Mode.Symbol || this.AI.Mode.SymbolTrash)
        {
            ODWarManager.Instance.SymbolDown(this.mob.MapID, this.mob);
        }
    }

}

public void OnKick()
{
    throw new NotImplementedException();
}

public void OnMapLoaded()
{
    throw new NotImplementedException();
}

public void OnReSpawn()
{
    throw new NotImplementedException();
}

public void OnSendMessage(string from, string message)
{
    throw new NotImplementedException();
}

public void OnSendWhisper(string name, string message, byte flag)
{
    throw new NotImplementedException();
}

public void OnTeleport(short x, short y)
{
    throw new NotImplementedException();
}

public void OnAttack(Actor aActor, MapEventArgs args)
{
    SkillArg arg = (SkillArg)args;
    AI.OnSeenSkillUse(arg);
    try
    {
        if (Attacking != null)
        {
            if (AI.lastAttacker != null)
            {
                if (AI.lastAttacker.type == ActorType.PC)
                {
                    RunCallback(Attacking, (ActorPC)AI.lastAttacker);
                    return;
                }
                else if (AI.lastAttacker.type == ActorType.SHADOW)
                {
                    if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                    {
                        if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                            RunCallback(Attacking, (ActorPC)pc);
                            return;
                        }
                    }
                }
            }
            RunCallback(Attacking, null);
        }
    }
    catch (Exception ex)
    {
        Logger.ShowError(ex);
    }
}

public void OnHPMPSPUpdate(Actor sActor)
{
    /*if (Skill.SkillHandler.Instance.isBossMob(this.mob))
    {
        if (!this.mob.Tasks.ContainsKey("MobRecover"))
        {
            Tasks.Mob.MobRecover MobRecover = new SagaMap.Tasks.Mob.MobRecover((ActorMob)this.mob);
            this.mob.Tasks.Add("MobRecover", MobRecover);
            MobRecover.Activate();
        }
    }*///关闭怪物回复线程以节省资源
/*      if (sActor.HP < sActor.MaxHP * 0.05f) return;
      if (Defending != null)
      {
          if (AI.lastAttacker != null)
          {
              if (AI.lastAttacker.type == ActorType.PC)
              {
                  RunCallback(Defending, (ActorPC)AI.lastAttacker);
                  return;
              }
              else if (AI.lastAttacker.type == ActorType.SHADOW)
              {
                  if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                  {
                      if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                      {
                          ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                          RunCallback(Defending, pc);
                          return;
                      }
                  }
              }
          }
          RunCallback(Defending, null);
      }
      if (FirstTimeDefending != null && !mob.FirstDefending)
      {
          mob.FirstDefending = true;
          if (AI.lastAttacker != null)
          {
              if (AI.lastAttacker.type == ActorType.PC)
              {
                  RunCallback(FirstTimeDefending, (ActorPC)AI.lastAttacker);
                  return;
              }
              else if (AI.lastAttacker.type == ActorType.SHADOW)
              {
                  if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master != null)
                  {
                      if (((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master.type == ActorType.PC)
                      {
                          ActorPC pc = (ActorPC)((PetEventHandler)((ActorShadow)AI.lastAttacker).e).AI.Master;
                          RunCallback(FirstTimeDefending, pc);
                          return;
                      }
                  }
              }
          }
          RunCallback(FirstTimeDefending, null);
      }

  }

  public void OnPlayerChangeStatus(ActorPC aActor)
  {

  }

  public void OnActorChangeBuff(Actor sActor)
  {

  }
  public void OnLevelUp(Actor sActor, MapEventArgs args)
  {
  }
  public void OnPlayerMode(Actor aActor)
  {
  }

  public void OnShowEffect(Actor aActor, MapEventArgs args)
  {
  }

  public void OnActorPossession(Actor aActor, MapEventArgs args)
  {

  }
  public void OnActorPartyUpdate(ActorPC aActor)
  {

  }
  public void OnActorSpeedChange(Actor mActor)
  {

  }

  public void OnSignUpdate(Actor aActor)
  {

  }

  public void PropertyUpdate(UpdateEvent arg, int para)
  {
      switch (arg)
      {
          case UpdateEvent.SPEED:
              AI.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SPEED_UPDATE, null, this.mob, true);
              break;
      }
  }
  public void PropertyRead(UpdateEvent arg)
  {
  }

  public void OnActorRingUpdate(ActorPC aActor)
  { }

  public void OnActorWRPRankingUpdate(ActorPC aActor)
  { }

  public void OnActorChangeAttackType(ActorPC aActor)
  { }
  public void OnActorFurnitureSit(ActorPC aActor)
  { }
  public void OnActorFurnitureList(Object obj)
  { }
  void RunCallback(Scripting.MobCallback callback, ActorPC pc)
  {
      try
      {
          currentCall = callback;
          currentPC = pc;
          System.Threading.Thread th = new System.Threading.Thread(Run);
          th.Start();
      }
      catch (Exception ex)
      {
          SagaLib.Logger.ShowError(ex);
      }
  }
  DateTime mark = DateTime.Now;
  void Run()
  {
      try
      {
          if (currentCall != null)
          {
              if (currentPC != null)
                  currentCall.Invoke(this, currentPC);
              else
              {
                  if (this.AI.map.Creator != null)
                      currentCall.Invoke(this, this.AI.map.Creator);
              }
          }
      }
      catch (Exception ex)
      {
          SagaLib.Logger.ShowError(ex);
      }
  }
  public void OnUpdate(Actor aActor)
  {
      try
      {
          if (Updating != null)
          {
              if (AI.lastAttacker != null)
              {
                  if (AI.lastAttacker.type == ActorType.PC)
                      RunCallback(Updating, (ActorPC)AI.lastAttacker);
                  else
                      RunCallback(Updating, null);
              }
              else
                  RunCallback(Updating, null);
          }
      }
      catch (Exception ex)
      {
          Logger.ShowError(ex);
      }
  }
  public void OnActorPaperChange(ActorPC aActor)
  {
  }
  #endregion
}
}
*/