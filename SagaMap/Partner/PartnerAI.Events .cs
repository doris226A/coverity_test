using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Skill;
using SagaMap.Manager;
using SagaMap.Skill;
using SagaDB.Partner;
using SagaMap.Network.Client;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SagaMap.Partner
{
    public partial class PartnerAI
    {
        DateTime lastSkillCast = DateTime.Now;
        DateTime cannotAttack = DateTime.Now;
        public Actor lastAttacker;

        public DateTime LastSkillCast
        {
            get { return this.lastSkillCast; }
            set { this.lastSkillCast = value; }
        }

        public DateTime CannotAttack
        {
            get { return this.cannotAttack; }
            set { this.cannotAttack = value; }
        }

        //新增部分开始 by:TT
        Dictionary<uint, DateTime> skillCast = new Dictionary<uint, DateTime>();
        DateTime shortSkillTime = DateTime.Now;
        DateTime longSkillTime = DateTime.Now;
        public uint NextSurelySkillID = 0;
        int Sequence = 0;
        bool skillOK = false;
        /// <summary>
        /// AnAI的当前顺序
        /// </summary>
        uint NowSequence = 0;
        bool NeedNewSkillList = true;
        bool CastIsFinished = true;
        public float needlen = 1f;
        public bool skillisok = false;
        DateTime SkillWait = DateTime.Now;
        AIMode.SkillList Now_SkillList = new AIMode.SkillList();
        List<AIMode.SkillList> Temp_skillList = new List<AIMode.SkillList>();
        int SkillDelay = 0;
        public void OnShouldCastSkill_An(AIMode mode, Actor currentTarget)
        {
            try
            {
                if (this.Partner.Tasks.ContainsKey("SkillCast"))
                    return;

                #region 根据条件抽选技能列表
                if (NeedNewSkillList)
                {
                    int totalRate = 0;
                    int determinator = 0;
                    Now_SkillList = new AIMode.SkillList();
                    Temp_skillList = new List<AIMode.SkillList>();
                    foreach (KeyValuePair<uint, AIMode.SkillList> item in mode.AnAI_SkillAssemblage)
                    {
                        if (this.Partner.HP >= item.Value.MinHP * this.Partner.HP / 100 && this.Partner.HP <= item.Value.MaxHP * this.Partner.HP / 100)
                        {
                            totalRate += item.Value.Rate;
                            Temp_skillList.Add(item.Value);
                        }
                    }
                    int ran = Global.Random.Next(0, totalRate);
                    foreach (AIMode.SkillList item in Temp_skillList)
                    {
                        determinator += item.Rate;
                        if (ran <= determinator)
                        {
                            Now_SkillList = item;
                            break;
                        }
                    }
                    NeedNewSkillList = false;
                    Sequence = 1;
                }
                #endregion
                #region 按照顺序释放技能

                foreach (KeyValuePair<uint, AIMode.SkillsInfo> item in Now_SkillList.AnAI_SkillList)
                {
                    skillisok = false;
                    SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(item.Value.SkillID, 1);
                    if (GetLengthD(this.Partner.X, this.Partner.Y, currentTarget.X, currentTarget.Y) <= skill.Range * 145)
                        skillisok = true;
                    else
                    {
                        needlen = skill.Range;
                        needlen -= 1f;
                        if (needlen < 1f)
                            needlen = 1f;
                    }
                    if (item.Key >= Sequence)
                    {
                        //CannotAttack = DateTime.Now.AddMilliseconds(item.Value.Delay);
                        if (skillisok)
                        {
                            SkillDelay = item.Value.Delay;
                            CastIsFinished = false;
                            CastSkill(item.Value.SkillID, 1, currentTarget);
                            Sequence++;
                        }
                        else if (SkillWait <= DateTime.Now)
                        {
                            Sequence++;
                            SkillWait = DateTime.Now.AddSeconds(5);
                        }

                        break;
                    }
                    if (Sequence > Now_SkillList.AnAI_SkillList.Count)
                    {
                        NeedNewSkillList = true;
                        break;
                    }
                    if (Now_SkillList.AnAI_SkillList.Count == 0)
                    {
                        NeedNewSkillList = true;
                        break;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                SagaLib.Logger.ShowError(ex);
            }
        }
        public void OnShouldCastSkill_New(AIMode mode, Actor currentTarget)
        {
            if (this.Partner.Tasks.ContainsKey("SkillCast"))
                return;
            double len = GetLengthD(this.Partner.X, this.Partner.Y, currentTarget.X, currentTarget.Y) / 145;

            uint skillID = 0;
            int totalRate = 0;

            Dictionary<uint, AIMode.SkilInfo> temp_skillList = new Dictionary<uint, AIMode.SkilInfo>();
            Dictionary<uint, AIMode.SkilInfo> skillList = new Dictionary<uint, AIMode.SkilInfo>();

            if (mode.Distance < len && longSkillTime < DateTime.Now)
            {
                temp_skillList = mode.SkillOfLong;
                //远程
            }
            else if (shortSkillTime < DateTime.Now)
            {
                temp_skillList = mode.SkillOfShort;
                //近身
            }

            foreach (KeyValuePair<uint, AIMode.SkilInfo> i in temp_skillList)
            {
                int MaxHPLimit = (int)(this.Partner.MaxHP * (i.Value.MaxHP * 0.01f)) + 1;
                int MinHPLimit = (int)(this.Partner.MaxHP * (i.Value.MinHP * 0.01f)) + 1;
                if (MaxHPLimit >= this.Partner.HP && MinHPLimit <= this.Partner.HP)
                {
                    if (skillCast.ContainsKey(i.Key))
                    {
                        if (skillCast[i.Key] < DateTime.Now)
                        {
                            skillList.Add(i.Key, i.Value);
                            skillCast.Remove(i.Key);
                        }
                    }
                    else
                    {
                        skillList.Add(i.Key, i.Value);
                    }
                }
            }

            foreach (AIMode.SkilInfo i in skillList.Values)
            {
                totalRate += i.Rate;
            }
            int ran = 0;
            if (totalRate > 1)
                ran = Global.Random.Next(0, totalRate);
            int determinator = 0;

            foreach (uint i in skillList.Keys)
            {
                determinator += skillList[i].Rate;
                if (ran <= determinator)
                {
                    skillID = i;
                    break;
                }
            }
            if (NextSurelySkillID != 0)
            {
                skillID = NextSurelySkillID;
                NextSurelySkillID = 0;
            }
            //释放技能
            if (skillID != 0)
            {
                CastSkill(skillID, 1, currentTarget);
                if (skillOK)
                {
                    try
                    {
                        skillCast.Add(skillID, DateTime.Now.AddSeconds(skillList[skillID].CD));
                    }
                    catch
                    {

                    }
                    if (mode.Distance < len)
                    {
                        longSkillTime = DateTime.Now.AddSeconds(mode.LongCD);
                        //远程
                    }
                    else
                    {
                        shortSkillTime = DateTime.Now.AddSeconds(mode.ShortCD);
                        //近身
                    }
                    skillOK = false;
                }
            }
        }
        //新增结束

        public void OnShouldCastSkill(Dictionary<uint, int> skillList, Actor currentTarget)
        {
            if (!this.Partner.Tasks.ContainsKey("SkillCast") && skillList.Count > 0)
            {
                //确定释放的技能
                uint skillID = 0;
                int totalRate = 0;
                foreach (int i in skillList.Values)
                {
                    totalRate += i;
                }
                int ran = Global.Random.Next(0, totalRate);
                int determinator = 0;

                foreach (uint i in skillList.Keys)
                {
                    determinator += skillList[i];
                    if (ran <= determinator)
                    {
                        skillID = i;
                        break;
                    }
                }

                //释放技能
                if (skillID != 0)
                {
                    //Partner.TTime["攻击僵直"] = DateTime.Now + new TimeSpan(0, 0, 0, 3);
                    CastSkill(skillID, 1, currentTarget);
                }
            }
        }

        #region 寵物AI技能施放重新優化
        public Dictionary<byte, DateTime> ai_lastSkillCast = new Dictionary<byte, DateTime>(); //ECOKEY放技能的時間
        byte skillTarget = 0;
        public void OnShouldCastSkill_AI(Actor currentTarget)
        {
            byte AiSkillID = 0;
            uint skillID = 0;
            Actor target_now = null;
            if (!this.Partner.Tasks.ContainsKey("SkillCast"))
            {
                ActorPartner partner = (ActorPartner)this.Partner;
                ActorEventHandlers.PartnerEventHandler eh = (ActorEventHandlers.PartnerEventHandler)partner.e;
                foreach (byte i in partner.ai_states.Keys)
                {
                    if (partner.ai_states[i] == true)//判斷技能開關
                        continue;
                    if ((DateTime.Now - ai_lastSkillCast[i]).TotalSeconds >= partner.ai_intervals[i])//判斷冷卻時間
                    {
                        if (partner.ai_reactions.ContainsKey(i) && PartnerFactory.Instance.actcubes_db_uniqueID.ContainsKey(partner.ai_reactions[i]))
                        {
                            ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[partner.ai_reactions[i]];
                            if(acd.skillID == 0)
                            {
                                Action(partner.ai_conditions[i], currentTarget, partner.ai_HPs[i], partner.ai_Targets[i], acd.actionID);
                                continue;
                            }
                            Logger.ShowInfo("acd.skillID  " + acd.skillID);
                            SagaDB.Skill.Skill skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(acd.skillID, 1);//技能
                            if (IsConditionSatisfied(partner.ai_conditions[i], currentTarget, partner.ai_HPs[i], partner.ai_Targets[i]) == false)  //判斷條件
                                continue;
                            ActorPC pc = partner.Owner;

                            Logger.ShowInfo("過條件?  ");


                            //有復活效果的技能判斷
                            if (skillTarget == 1 && pc.Buff.Dead)
                            {
                                if (!skill.DeadOnly && skill.Target != 4)//死人專用和範圍技能
                                {
                                    continue;
                                }
                            }
                            //ECOKEY 技能的TryCast判斷，不保證所有技能通用
                            SkillArg arg = new SkillArg();
                            if (SagaMap.Skill.SkillHandler.Instance.skillHandlers.ContainsKey(acd.skillID))//ECOKEY 避免禁止技能時出現error
                            {
                                if (SagaMap.Skill.SkillHandler.Instance.skillHandlers[acd.skillID].TryCast(pc, currentTarget, arg) < 0)
                                    continue;
                            }

                            if (skillTarget == 1) target_now = (Actor)pc;
                            if (skillTarget == 2) target_now = (Actor)partner;
                            if (skillTarget == 4) //隨機選一個隊友放技能
                            {
                                Dictionary<byte, ActorPC> partyMembers = pc.Party.Members;
                                List<Actor> targetMembers = new List<Actor>(partyMembers.Values);
                                targetMembers.Remove(pc); // 移除主人自己
                                                          //避免技能放給PE中的人
                                foreach (Actor p in targetMembers.ToArray())
                                {
                                    // 新增判斷避免對已下線的隊友放技能
                                    if (p.type == ActorType.PC && ((ActorPC)p).Online)
                                    {
                                        ActorPC p1 = (ActorPC)p;
                                        if (p1.PossessionTarget != 0)
                                        {
                                            targetMembers.Remove(p);
                                        }
                                        //避免放技給不同地圖的人
                                        if (p.MapID != partner.MapID)
                                        {
                                            targetMembers.Remove(p);
                                        }
                                        //超過一定距離的隊友不放技
                                        if (GetDistance(partner.X, partner.Y, p1.X, p1.Y) >= 1500)
                                        {
                                            targetMembers.Remove(p);
                                        }
                                    }
                                    else
                                    {
                                        targetMembers.Remove(p);
                                    }
                                }
                                if (targetMembers.Count > 0)
                                {
                                    Random myObject = new Random();
                                    int ranNum = myObject.Next(0, targetMembers.Count);
                                    Actor target = targetMembers[ranNum];
                                    target_now = (Actor)target;
                                }
                            }
                            if (skillTarget == 3)
                            {
                                if (currentTarget.ActorID == partner.Owner.ActorID)//判斷目前寵物目標是敵人還是主人
                                    continue;
                                target_now = (Actor)currentTarget;
                            }
                            Logger.ShowInfo("過2  " + acd.skillID);
                            //ECOKEY距離
                            //double distanceToTarget = PartnerAI.GetLengthD(partner.X, partner.Y, target_now.X, target_now.Y);
                            //if (distanceToTarget > (skill.Range * 100)) continue;

                            if (partner.Owner.TInt.ContainsKey("PartnerX"))//ECOKEY如果有坐下紀錄，直接更改位置紀錄
                            {
                                partner.Owner.TInt.Remove("PartnerX");
                                partner.Owner.TInt.Remove("PartnerY");
                            }
                            //這裡記錄所有技能所需資料
                            AiSkillID = i;
                            skillID = acd.skillID;
                            break;
                        }
                        else
                        {
                            // 處理技能塊不存在的情況，例如輸出錯誤消息或者執行默認操作
                            MapClient.FromActorPC((ActorPC)partner.Owner).SendSystemMessage("你寵物沒有裝技能。");
                        }
                    }
                }

                if (skillID != 0 && skillTarget != 0 && target_now != null)
                {
                    Logger.ShowInfo("skillID  " + skillID);
                    CustomCastSkill_New(skillID, 1, skillTarget, target_now, AiSkillID);
                }
            }
        }
        //ECOKEY 修正寵物攻擊主人(整段覆蓋)
        public void OnShouldCastSkill_AI2(Actor currentTarget)
        {
            byte AiSkillID = 0;
            uint skillID = 0;
            Actor target_now = null;
            if (!this.Partner.Tasks.ContainsKey("SkillCast"))
            {
                ActorPartner partner = (ActorPartner)this.Partner;
                ActorEventHandlers.PartnerEventHandler eh = (ActorEventHandlers.PartnerEventHandler)partner.e;
                foreach (byte i in partner.ai_states2.Keys)
                {
                    if (partner.ai_states2[i] == true)//判斷技能開關
                        continue;
                    if ((DateTime.Now - ai_lastSkillCast[i]).TotalSeconds >= partner.ai_intervals2[i])//判斷冷卻時間
                    {
                        if (partner.ai_reactions2.ContainsKey(i) && PartnerFactory.Instance.actcubes_db_uniqueID.ContainsKey(partner.ai_reactions2[i]))//0705再更新，注意此處更新是AI2
                        {
                            ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[partner.ai_reactions2[i]];
                            SagaDB.Skill.Skill skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(acd.skillID, 1);//技能
                            if (IsConditionSatisfied(partner.ai_conditions2[i], currentTarget, partner.ai_HPs2[i], partner.ai_Targets2[i]) == false)  //判斷條件
                                continue;
                            ActorPC pc = partner.Owner;

                            //有復活效果的技能判斷
                            if (skillTarget == 1 && pc.Buff.Dead)
                            {
                                if (!skill.DeadOnly && skill.Target != 4)//死人專用和範圍技能
                                {
                                    continue;
                                }
                            }
                            //ECOKEY距離
                            double distanceToTarget = PartnerAI.GetLengthD(partner.X, partner.Y, currentTarget.X, currentTarget.Y);
                            //ECOKEY 技能的TryCast判斷，不保證所有技能通用
                            SkillArg arg = new SkillArg();
                            if (SagaMap.Skill.SkillHandler.Instance.skillHandlers.ContainsKey(acd.skillID))//ECOKEY 避免禁止技能時出現error
                            {
                                if (SagaMap.Skill.SkillHandler.Instance.skillHandlers[acd.skillID].TryCast(pc, currentTarget, arg) < 0)
                                    continue;
                            }

                            if (skillTarget == 1) target_now = (Actor)pc;
                            if (skillTarget == 2) target_now = (Actor)partner;
                            if (skillTarget == 4) //隨機選一個隊友放技能
                            {
                                Dictionary<byte, ActorPC> partyMembers = pc.Party.Members;
                                List<Actor> targetMembers = new List<Actor>(partyMembers.Values);
                                targetMembers.Remove(pc); // 移除主人自己
                                                          //避免技能放給PE中的人
                                foreach (Actor p in targetMembers.ToArray())
                                {
                                    // 新增判斷避免對已下線的隊友放技能
                                    if (p.type == ActorType.PC && ((ActorPC)p).Online)
                                    {
                                        ActorPC p1 = (ActorPC)p;
                                        if (p1.PossessionTarget != 0)
                                        {
                                            targetMembers.Remove(p);
                                        }
                                        //避免放技給不同地圖的人
                                        if (p.MapID != partner.MapID)
                                        {
                                            targetMembers.Remove(p);
                                        }
                                        //超過一定距離的隊友不放技
                                        if (GetDistance(partner.X, partner.Y, p1.X, p1.Y) >= 1500)
                                        {
                                            targetMembers.Remove(p);
                                        }
                                    }
                                    else
                                    {
                                        targetMembers.Remove(p);
                                    }
                                }
                                if (targetMembers.Count > 0)
                                {
                                    Random myObject = new Random();
                                    int ranNum = myObject.Next(0, targetMembers.Count);
                                    Actor target = targetMembers[ranNum];
                                    target_now = (Actor)target;
                                }
                            }
                            if (distanceToTarget <= (skill.Range * 154)) // 距離值
                            {
                                if (skillTarget == 3)
                                {
                                    if (currentTarget.ActorID == partner.Owner.ActorID)//判斷目前寵物目標是敵人還是主人
                                        continue;
                                    target_now = (Actor)currentTarget;
                                }
                            }
                            if (partner.Owner.TInt.ContainsKey("PartnerX"))//ECOKEY如果有坐下紀錄，直接更改位置紀錄
                            {
                                partner.Owner.TInt.Remove("PartnerX");
                                partner.Owner.TInt.Remove("PartnerY");
                            }
                            //這裡記錄所有技能所需資料
                            AiSkillID = i;
                            skillID = acd.skillID;
                            break;
                        }
                        else
                        {
                            // 處理技能塊不存在的情況，例如輸出錯誤消息或者執行默認操作
                            MapClient.FromActorPC((ActorPC)partner.Owner).SendSystemMessage("你寵物沒有裝技能。");
                        }
                    }
                }

                if (skillID != 0 && skillTarget != 0 && target_now != null)
                {
                    CustomCastSkill_New(skillID, 1, skillTarget, target_now, AiSkillID);
                }
            }
        }

        void Action(ushort conditionID, Actor currentTarget, int hps, int target, int actionID)
        {
            bool check = false;
            ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[conditionID];
            ActorPartner partner = (ActorPartner)this.Partner;
            ActorPC pc = partner.Owner;
            skillTarget = (byte)target;
            #region まで系列
            if (acd.actionID == 7 && partner.HP >= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜主人＞
                check = true;
            if (acd.actionID == 8 && partner.SP >= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜主人＞
                check = true;
            if (acd.actionID == 9 && partner.MP >= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜主人＞
                check = true;
            if (acd.actionID == 10 && pc.HP >= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜主人＞
                check = true;
            if (acd.actionID == 11 && pc.SP >= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜主人＞
                check = true;
            if (acd.actionID == 12 && pc.MP >= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜主人＞
                check = true;
            if (acd.actionID == 15 && currentTarget.HP >= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                check = true;
            //判斷主人的敵人
            if (pc.LastAttackActorID != 0)
            {
                Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                if (pcTarget != null)
                {
                    if (acd.actionID == 16 && pcTarget.HP >= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                        check = true;
                }
            }
            #endregion
            #region 數值以上
            if (acd.actionID == 1 && partner.HP <= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜主人＞
                check = true;
            if (acd.actionID == 2 && partner.SP <= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜主人＞
                check = true;
            if (acd.actionID == 3 && partner.MP <= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜主人＞
                check = true;
            if (acd.actionID == 4 && pc.HP <= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜主人＞
                check = true;
            if (acd.actionID == 5 && pc.SP <= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜主人＞
                check = true;
            if (acd.actionID == 6 && pc.MP <= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜主人＞
                check = true;
            if (acd.actionID == 13 && currentTarget.HP <= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                check = true;
            //判斷主人的敵人
            if (pc.LastAttackActorID != 0)
            {
                Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                if (pcTarget != null)
                {
                    if (acd.actionID == 14 && pcTarget.HP <= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                        check = true;
                }
            }
            #endregion
            if (check)
            {
                if (actionID == 1000)
                {
                    this.Mode.mask.SetValue(AIFlag.NoMove, true);
                }
            }
            else
            {
                this.Mode.mask.SetValue(AIFlag.NoMove, false);
            }
            /**/
        }
        private bool IsConditionSatisfied(ushort conditionID, Actor currentTarget, int hps, int target)
        {
            ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[conditionID];
            ActorPartner partner = (ActorPartner)this.Partner;
            ActorPC pc = partner.Owner;
            //int hps = acd.parameter1;
            //int target = target;
            skillTarget = (byte)target;
            //目標是主人
            if (target == 1)
            {

                if (target == 1 && partner.Owner.PossessionTarget != 0)
                    return false;
                #region まで系列
                if (acd.actionID == 7 && target == 1 && partner.HP >= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && target == 1 && partner.SP >= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && target == 1 && partner.MP >= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && target == 1 && pc.HP >= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && target == 1 && pc.SP >= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && target == 1 && pc.MP >= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && target == 1 && currentTarget.HP >= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && target == 1 && pcTarget.HP >= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && target == 1 && partner.HP <= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 2 && target == 1 && partner.SP <= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 3 && target == 1 && partner.MP <= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 4 && target == 1 && pc.HP <= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 5 && target == 1 && pc.SP <= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 6 && target == 1 && pc.MP <= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 13 && target == 1 && currentTarget.HP <= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && target == 1 && pcTarget.HP <= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
            }
            //目標是自己
            if (target == 2)
            {
                #region まで系列
                if (acd.actionID == 7 && target == 2 && partner.HP >= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && target == 2 && partner.SP >= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && target == 2 && partner.MP >= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && target == 2 && pc.HP >= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && target == 2 && pc.SP >= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && target == 2 && pc.MP >= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && target == 2 && currentTarget.HP >= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && target == 2 && pcTarget.HP >= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && target == 2 && partner.HP <= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜自分＞
                    return true;
                if (acd.actionID == 2 && target == 2 && partner.SP <= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜自分＞
                    return true;
                if (acd.actionID == 3 && target == 2 && partner.MP <= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜自分＞
                    return true;
                if (acd.actionID == 4 && target == 2 && pc.HP <= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜自分＞
                    return true;
                if (acd.actionID == 5 && target == 2 && pc.SP <= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜自分＞
                    return true;
                if (acd.actionID == 6 && target == 2 && pc.MP <= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜自分＞
                    return true;
                if (acd.actionID == 13 && target == 2 && currentTarget.HP <= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜自分＞
                    return true;
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && target == 2 && pcTarget.HP <= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜自分＞
                            return true;
                    }
                }
                #endregion
            }
            //目標是敵人
            if (target == 3)
            {
                #region まで系列
                if (acd.actionID == 7 && target == 3 && partner.HP >= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && target == 3 && partner.SP >= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && target == 3 && partner.MP >= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && target == 3 && pc.HP >= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && target == 3 && pc.SP >= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && target == 3 && pc.MP >= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && target == 3 && currentTarget.HP >= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && target == 3 && pcTarget.HP >= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && target == 3 && partner.HP <= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 2 && target == 3 && partner.SP <= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 3 && target == 3 && partner.MP <= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 4 && target == 3 && pc.HP <= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 5 && target == 3 && pc.SP <= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 6 && target == 3 && pc.MP <= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 13 && target == 3 && currentTarget.HP <= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜敵人＞
                    return true;
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && target == 3 && pcTarget.HP <= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜敵人＞
                            return true;
                    }
                }
                #endregion
            }


            if (pc.Party != null)
            {
                #region まで系列
                if (acd.actionID == 7 && target == 4 && partner.HP >= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && target == 4 && partner.SP >= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && target == 4 && partner.MP >= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && target == 4 && pc.HP >= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && target == 4 && pc.SP >= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && target == 4 && pc.MP >= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && target == 4 && currentTarget.HP >= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && target == 4 && pcTarget.HP >= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && target == 4 && partner.HP <= (partner.MaxHP * (hps * 0.01))) //自分のＨＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 2 && target == 4 && partner.SP <= (partner.MaxSP * (hps * 0.01))) //自分のＳＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 3 && target == 4 && partner.MP <= (partner.MaxMP * (hps * 0.01))) //自分のＭＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 4 && target == 4 && pc.HP <= (pc.MaxHP * (hps * 0.01))) //主人のＨＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 5 && target == 4 && pc.SP <= (pc.MaxSP * (hps * 0.01))) //主人のＳＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 6 && target == 4 && pc.MP <= (pc.MaxMP * (hps * 0.01))) //主人のＭＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 13 && target == 4 && currentTarget.HP <= (currentTarget.MaxHP * (hps * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜仲間＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = this.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && target == 4 && pcTarget.HP <= (pcTarget.MaxHP * (hps * 0.01))) //主人的敵人のＨＰ≦＜仲間＞
                            return true;
                    }
                }
                #endregion
            }
            return false;
        }

        public void CustomCastSkill_New(uint skillID, byte lv, byte target, Actor ActorTarget, byte AiSkillID)
        {
            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, lv);
            //1玩家2寵物3敵人4夥伴
            /*1.指定目標的輔助技能
            2.自己施放的輔助技能
            3.指定目標的攻擊技能
            4.自己施放的攻擊技能
            5.對死亡對象的復活技能?*/
            if (skill == null)
                return;
            //ECOKEY MPSP不夠禁止放技 CheckSkillUse
            if (skill.MP > this.Partner.MP || skill.SP > this.Partner.SP)
                return;
            //技能強制兩秒
            if (skill.CastTime == 0)
            {
                skill.CastTime = 2000;
            }
            SkillArg arg = new SkillArg();
            if (skill.Support == true)
            {
                if (target == 3) return;
                if (skill.Target == 1)//修正技能判斷
                {
                    if (target != 2) return;
                }
            }
            if (skill.Attack == true)
            {
                if (skill.CanHasTarget == false)
                {
                    if (target != 2) return;
                }
                else
                {
                    if (target != 3 && skillID != 6790) return;//ECOKEY 出現例外技能!!!先這樣做
                }
            }
            if (skill.DeadOnly == true)
            {
                if (target == 2 || target == 3) return;
            }
            arg.sActor = this.Partner.ActorID;
            arg.skill = skill;
            arg.argType = SkillArg.ArgType.Cast;
            arg.delay = (uint)skill.CastTime;
            arg.dActor = ActorTarget.ActorID;
            //新增技能範圍
            arg.x = SagaLib.Global.PosX16to8(ActorTarget.X, map.Width);
            arg.y = SagaLib.Global.PosY16to8(ActorTarget.Y, map.Height);
            //詠唱處理
            if (skill.BaseData.flag.Test(SkillFlags.PHYSIC))
                arg.delay = (uint)((float)skill.CastTime * (1.0f - (float)this.Partner.Status.aspd / 1000.0f));
            else
                arg.delay = (uint)Math.Max(((float)skill.CastTime * (1.0f - (float)this.Partner.Status.cspd / 1000.0f)), 500);
            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Partner, false);
            //打開詠唱
            /*if (skill.CastTime > 0)
            {
                Tasks.Partner.SkillCast task = new SagaMap.Tasks.Partner.SkillCast(this, arg);
                this.Partner.Tasks.Add("SkillCast", task);

                task.Activate();
            }
            else
            {
                OnSkillCastComplete(arg);
            }*/
            ai_lastSkillCast[AiSkillID] = DateTime.Now.AddSeconds(2 + (skill.Delay / 1000)); //紀錄技能施放時間，用於判斷冷卻，並加上單獨的冷卻技能

            //Logger.ShowInfo("skill.Delay" + skill.Delay.ToString());
            OnSkillCastComplete(arg);
        }
        #endregion
        public bool CustomCastSkill(uint skillID, byte lv, byte target, Actor ActorTarget)
        {
            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, lv);
            //1玩家2寵物3敵人4夥伴
            //1.指定目標的輔助技能
            //2.自己施放的輔助技能
            //3.指定目標的攻擊技能
            //4.自己施放的攻擊技能
            //5.對死亡對象的復活技能?
            //詠唱中停止執行
            if (this.Partner.Tasks.ContainsKey("SkillCast"))
                return false;
            if (skill == null)
                return false;
            //ECOKEY MPSP不夠禁止放技 CheckSkillUse
            if (skill.MP > this.Partner.MP || skill.SP > this.Partner.SP)
                return false;
            //技能強制兩秒
            if (skill.CastTime == 0)
            {
                skill.CastTime = 2000;
            }
            SkillArg arg = new SkillArg();
            if (skill.Support == true)
            {
                if (target == 3) return false;
                if (skill.Target == 1)//修正技能判斷
                {
                    if (target != 2) return false;
                }
            }
            if (skill.Attack == true)
            {
                if (skill.CanHasTarget == false)
                {
                    if (target != 2) return false;
                }
                else
                {
                    if (target != 3) return false;
                }
            }
            if (skill.DeadOnly == true)
            {
                if (target == 2 || target == 3) return false;
            }
            arg.sActor = this.Partner.ActorID;
            arg.skill = skill;
            arg.argType = SkillArg.ArgType.Cast;
            arg.delay = (uint)skill.CastTime;
            arg.dActor = ActorTarget.ActorID;
            //新增技能範圍
            arg.x = SagaLib.Global.PosX16to8(ActorTarget.X, map.Width);
            arg.y = SagaLib.Global.PosY16to8(ActorTarget.Y, map.Height);
            //詠唱處理
            if (skill.BaseData.flag.Test(SkillFlags.PHYSIC))
                arg.delay = (uint)((float)skill.CastTime * (1.0f - (float)this.Partner.Status.aspd / 1000.0f));
            else
                arg.delay = (uint)Math.Max(((float)skill.CastTime * (1.0f - (float)this.Partner.Status.cspd / 1000.0f)), 500);
            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Partner, false);
            //打開詠唱
            if (skill.CastTime > 0)
            {
                Tasks.Partner.SkillCast task = new SagaMap.Tasks.Partner.SkillCast(this, arg);
                this.Partner.Tasks.Add("SkillCast", task);

                task.Activate();
            }
            else
            {
                OnSkillCastComplete(arg);
            }
            //OnSkillCastComplete(arg);
            return true;
        }
        //ECOKEY 自定義技能發動
        /*     public bool CustomCastSkill(uint skillID, byte lv, byte target, Actor ActorTarget)
             {
                 SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, lv);
                 //1玩家2寵物3敵人4夥伴
                 /*1.指定目標的輔助技能
                 2.自己施放的輔助技能
                 3.指定目標的攻擊技能
                 4.自己施放的攻擊技能
                 5.對死亡對象的復活技能?*/
        /*     if (skill == null)
                 return false;
             //ECOKEY MPSP不夠禁止放技 CheckSkillUse
             if (skill.MP > this.Partner.MP || skill.SP > this.Partner.SP)
                 return false;

             SkillArg arg = new SkillArg();
             if (skill.Support == true)
             {
                 if (target == 3) return false;
                 if (skill.CanHasTarget == false)
                 {
                     if (target != 2) return false;
                 }
             }
             if (skill.Attack == true)
             {
                 if (skill.CanHasTarget == false)
                 {
                     if (target != 2) return false;
                 }
                 else
                 {
                     if (target != 3) return false;
                 }
             }
             if (skill.DeadOnly == true)
             {
                 if (target == 2 || target == 3) return false;
             }
             arg.sActor = this.Partner.ActorID;
             arg.skill = skill;
             arg.argType = SkillArg.ArgType.Cast;
             arg.delay = (uint)skill.CastTime;
             arg.dActor = ActorTarget.ActorID;
             this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Partner, false);

             //ECOKEY 放出一個技能需要多久
             /*    if (skill.CastTime > 0)
                 {
                     if (SkillHandler.Instance.MobskillHandlers.ContainsKey(arg.skill.ID))
                     {
                         Actor dactor = this.map.GetActor(target);
                         SkillHandler.Instance.MobskillHandlers[arg.skill.ID].BeforeCast(this.Partner, dactor, arg, lv);
                     }
                     Tasks.Partner.SkillCast task = new SagaMap.Tasks.Partner.SkillCast(this, arg);
                     this.Partner.Tasks.Add("SkillCast", task);

                     task.Activate();
                 }
                 else
                 {
                     OnSkillCastComplete(arg);
                 }*/
        /*    OnSkillCastComplete(arg);
            return true;
        }*/

        public void CastSkill(uint skillID, byte lv, uint target, short x, short y)
        {
            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, lv);

            if (skill == null)
                return;
            if (!CanUseSkill)
                return;
            SkillArg arg = new SkillArg();
            arg.sActor = this.Partner.ActorID;

            if (target != 0xFFFFFFFF)
            {
                Actor dactor = this.map.GetActor(target);
                if (dactor == null)
                {
                    if (this.Partner.Tasks.ContainsKey("AutoCast"))
                    {
                        this.Partner.Tasks.Remove("AutoCast");
                        this.Partner.Buff.CannotMove = false;
                    }
                    return;
                }

                if (GetLengthD(this.Partner.X, this.Partner.Y, dactor.X, dactor.Y) <= skill.Range * 145)
                {
                    if (skill.Target == 2)
                    {
                        //如果是辅助技能
                        if (skill.Support)
                        {
                            if (this.Partner.type == ActorType.PET)
                            {
                                ActorPet pet = (ActorPet)this.Partner;
                                if (pet.Owner != null)
                                    arg.dActor = pet.Owner.ActorID;
                                else
                                    arg.dActor = this.Partner.ActorID;
                            }
                            else
                            {
                                if (this.master == null)
                                    arg.dActor = this.Partner.ActorID;
                                else
                                    arg.dActor = this.master.ActorID;
                            }
                        }
                        else
                            arg.dActor = target;
                    }
                    else if (skill.Target == 1)
                    {
                        if (this.Partner.type == ActorType.PET)
                        {
                            ActorPet pet = (ActorPet)this.Partner;
                            if (pet.Owner != null)
                                arg.dActor = pet.Owner.ActorID;
                            else
                                arg.dActor = this.Partner.ActorID;
                        }
                        else
                            arg.dActor = this.Partner.ActorID;
                    }
                    else
                        arg.dActor = 0xFFFFFFFF;

                    if (arg.dActor != 0xFFFFFFFF)
                    {
                        Actor dst = map.GetActor(arg.dActor);
                        if (dst != null)
                        {
                            if (dst.Buff.Dead != skill.DeadOnly)
                            {
                                if (this.Partner.Tasks.ContainsKey("AutoCast"))
                                {
                                    this.Partner.Tasks.Remove("AutoCast");
                                    this.Partner.Buff.CannotMove = false;
                                }
                                return;
                            }
                        }
                        else
                        {
                            if (this.Partner.Tasks.ContainsKey("AutoCast"))
                            {
                                this.Partner.Tasks.Remove("AutoCast");
                                this.Partner.Buff.CannotMove = false;
                            }
                            return;
                        }
                    }

                    if (this.master != null)
                    {
                        if ((this.master.ActorID == target) && !skill.Support)
                        {
                            if (this.Partner.Tasks.ContainsKey("AutoCast"))
                            {
                                this.Partner.Tasks.Remove("AutoCast");
                                this.Partner.Buff.CannotMove = false;
                            }
                            return;
                        }
                    }

                    arg.skill = skill;
                    arg.x = Global.PosX16to8(x, this.map.Width);
                    arg.y = Global.PosY16to8(y, this.map.Height);
                    arg.argType = SkillArg.ArgType.Cast;

                    //arg.delay = (uint)(skill.CastTime * (1f - this.Mob.Status.cspd / 1000f));//怪物技能吟唱时间
                    arg.delay = (uint)skill.CastTime;
                }
                else if (skill.Range == -1)
                {
                    if (skill.Target == 2)
                    {
                        //如果是辅助技能
                        if (skill.Support)
                        {
                            if (this.Partner.type == ActorType.PET)
                            {
                                ActorPet pet = (ActorPet)this.Partner;
                                if (pet.Owner != null)
                                    arg.dActor = pet.Owner.ActorID;
                                else
                                    arg.dActor = this.Partner.ActorID;
                            }
                            else
                            {
                                if (this.master == null)
                                    arg.dActor = this.Partner.ActorID;
                                else
                                    arg.dActor = this.master.ActorID;
                            }
                        }
                        else
                            arg.dActor = target;
                    }
                    else if (skill.Target == 1)
                    {
                        if (this.Partner.type == ActorType.PET)
                        {
                            ActorPet pet = (ActorPet)this.Partner;
                            if (pet.Owner != null)
                                arg.dActor = pet.Owner.ActorID;
                            else
                                arg.dActor = this.Partner.ActorID;
                        }
                        else
                            arg.dActor = this.Partner.ActorID;
                    }
                    else
                        arg.dActor = 0xFFFFFFFF;

                    if (arg.dActor != 0xFFFFFFFF)
                    {
                        Actor dst = map.GetActor(arg.dActor);
                        if (dst != null)
                        {
                            if (dst.Buff.Dead != skill.DeadOnly)
                            {
                                if (this.Partner.Tasks.ContainsKey("AutoCast"))
                                {
                                    this.Partner.Tasks.Remove("AutoCast");
                                    this.Partner.Buff.CannotMove = false;
                                }
                                return;
                            }
                        }
                        else
                        {
                            if (this.Partner.Tasks.ContainsKey("AutoCast"))
                            {
                                this.Partner.Tasks.Remove("AutoCast");
                                this.Partner.Buff.CannotMove = false;
                            }
                            return;
                        }
                    }

                    if (this.master != null)
                    {
                        if ((this.master.ActorID == target) && !skill.Support)
                        {
                            if (this.Partner.Tasks.ContainsKey("AutoCast"))
                            {
                                this.Partner.Tasks.Remove("AutoCast");
                                this.Partner.Buff.CannotMove = false;
                            }
                            return;
                        }
                    }

                    arg.skill = skill;
                    arg.x = Global.PosX16to8(x, this.map.Width);
                    arg.y = Global.PosY16to8(y, this.map.Height);
                    arg.argType = SkillArg.ArgType.Cast;

                    //arg.delay = (uint)(skill.CastTime * (1f - this.Mob.Status.cspd / 1000f));//怪物技能吟唱时间
                    arg.delay = (uint)skill.CastTime;
                }
                else
                {
                    if (this.Partner.Tasks.ContainsKey("AutoCast"))
                    {
                        this.Partner.Tasks.Remove("AutoCast");
                        this.Partner.Buff.CannotMove = false;
                    }
                    return;
                }
            }
            else
            {
                arg.dActor = 0xFFFFFFFF;
                if (GetLengthD(this.Partner.X, this.Partner.Y, x, y) <= skill.CastRange * 145)
                {
                    arg.skill = skill;
                    arg.x = Global.PosX16to8(x, this.map.Width);
                    arg.y = Global.PosY16to8(x, this.map.Height);
                    arg.argType = SkillArg.ArgType.Cast;

                    //arg.delay = (uint)(skill.CastTime * (1f - this.Mob.Status.cspd / 1000f));
                    arg.delay = (uint)skill.CastTime;
                }
                else
                {
                    if (this.Partner.Tasks.ContainsKey("AutoCast"))
                    {
                        this.Partner.Tasks.Remove("AutoCast");
                        this.Partner.Buff.CannotMove = false;
                    }
                    return;
                }
            }
            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Partner, false);
            /*if (skill.CastTime > 0)
            {
                if (SkillHandler.Instance.MobskillHandlers.ContainsKey(arg.skill.ID))
                {
                    Actor dactor = this.map.GetActor(target);
                    SkillHandler.Instance.MobskillHandlers[arg.skill.ID].BeforeCast(this.Partner, dactor, arg, lv);
                }
                Tasks.Partner.SkillCast task = new SagaMap.Tasks.Partner.SkillCast(this, arg);
                this.Partner.Tasks.Add("SkillCast", task);

                task.Activate();
            }
            else
            {
                OnSkillCastComplete(arg);
            }*/
            OnSkillCastComplete(arg);
            skillOK = true;
        }

        public void CastSkill(uint skillID, byte lv, Actor currentTarget)
        {
            CastSkill(skillID, lv, currentTarget.ActorID, currentTarget.X, currentTarget.Y);
        }

        public void CastSkill(uint skillID, byte lv, short x, short y)
        {
            CastSkill(skillID, lv, 0xFFFFFFFF, x, y);
        }

        public void AttackActor(uint actorID)
        {
            if (this.Hate.ContainsKey(actorID))
                this.Hate[actorID] = this.Partner.MaxHP;
            else
                this.Hate.TryAdd(actorID, this.Partner.MaxHP);
        }

        public void StopAttacking()
        {
            this.Hate.Clear();
        }

        public void OnSkillCastComplete(SkillArg skill)
        {
            if (this.Mode.isAnAI)
            {
                CannotAttack = DateTime.Now.AddMilliseconds(SkillDelay);
                SkillDelay = 0;
                CastIsFinished = true;
            }
            if (skill.dActor != 0xFFFFFFFF)
            {
                Actor dActor = this.map.GetActor(skill.dActor);
                if (dActor != null)
                {
                    skill.argType = SkillArg.ArgType.Active;
                    SkillHandler.Instance.SkillCast(this.Partner, dActor, skill);
                }
            }
            else
            {
                skill.argType = SkillArg.ArgType.Active;
                SkillHandler.Instance.SkillCast(this.Partner, this.Partner, skill);
            }

            if (this.Partner.type == ActorType.PET)
                SkillHandler.Instance.ProcessPetGrowth(this.Partner, PetGrowthReason.UseSkill);
            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, skill, this.Partner, false);

            /*if (skill.skill.Effect != 0)
            {
                EffectArg eff = new EffectArg();
                eff.actorID = skill.dActor;
                eff.effectID = skill.skill.Effect;
                eff.x = skill.x;
                eff.y = skill.y;
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, eff, this.Partner, false);
            }*/
            //ECOKEY 新增自身特效
            if (skill.skill.BaseData.effect5 != 0 && skill.sActor == skill.dActor)
            {
                EffectArg eff = new EffectArg();
                eff.actorID = skill.dActor;
                eff.effectID = (uint)skill.skill.BaseData.effect5;
                eff.x = skill.x;
                eff.y = skill.y;
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, eff, this.Partner, false);
            }
            //Partner.TTime["攻击僵直"] = DateTime.Now + new TimeSpan(0, 0, 0, 0, 500);//ECOKEY 這裡清掉
            if (this.Partner.Tasks.ContainsKey("AutoCast"))
                this.Partner.Tasks["AutoCast"].Activate();
            else
            {
                if (skill.autoCast.Count != 0)
                {
                    this.Partner.Buff.CannotMove = true;
                    this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Partner, true);
                    Tasks.Skill.AutoCast task = new SagaMap.Tasks.Skill.AutoCast(this.Partner, skill);
                    this.Partner.Tasks.Add("AutoCast", task);
                    task.Activate();
                }
            }
            //ECOKEY 成功放技減少MPSP
            uint mpCost = skill.skill.MP;
            uint spCost = skill.skill.SP;
            if (mpCost > this.Partner.MP && spCost > this.Partner.SP)
            {
                skill.result = -1;
                this.Partner.e.OnActorSkillUse(this.Partner, skill);
                return;
            }
            else if (mpCost > this.Partner.MP)
            {
                skill.result = -15;
                this.Partner.e.OnActorSkillUse(this.Partner, skill);
                return;
            }
            else if (spCost > this.Partner.SP)
            {
                skill.result = -16;
                this.Partner.e.OnActorSkillUse(this.Partner, skill);
                return;
            }
            this.Partner.MP -= mpCost;
            if (this.Partner.MP < 0)
                this.Partner.MP = 0;

            this.Partner.SP -= spCost;
            if (this.Partner.SP < 0)
                this.Partner.SP = 0;

            PartnerHPMPSP();

            /*
            ECOKEY 讓寵物MPSP即時增減
            ActorPartner par = (ActorPartner)this.map.GetActor(this.Partner.ActorID);
            ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)par.Owner.e;
            eh.Client.SendPartnerHPMPSP(par);
            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.Partner, false);
             */
        }
        //ECOKEY 讓寵物MPSP即時增減，Attack要使用，所以分函數出來
        public void PartnerHPMPSP()
        {
            ActorPartner par = (ActorPartner)this.map.GetActor(this.Partner.ActorID);
            if (par != null)
            {
                ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)par.Owner.e;
                eh.Client.SendPartnerHPMPSP(par);
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.Partner, true);
            }
        }

        public void OnPathInterupt()
        {
            if (commands.ContainsKey("Move"))
            {
                AICommands.Move command = (AICommands.Move)commands["Move"];
                command.FindPath();
            }

            if (commands.ContainsKey("Chase"))
            {
                AICommands.Chase command = (AICommands.Chase)commands["Chase"];
                command.FindPath();
            }
        }

        public void OnAttacked(Actor sActor, int damage)
        {
            if (this.actor.Buff.Dead)
                return;
            if (this.Activated == false)
            {
                this.Start();
            }
            if (sActor.ActorID == this.actor.ActorID)
                return;
            lastAttacker = sActor;
            uint tmp = (uint)(Math.Abs(damage));
            if (sActor.type == ActorType.PC)
            {
                if (((ActorPC)sActor).Skills2.ContainsKey(612))
                {
                    float rate_add = 0.1f * ((ActorPC)sActor).Skills2[612].Level;
                    tmp += (uint)(tmp * rate_add);
                }
                if (sActor.Status.Nooheito_rate > 0)//弓3转13级技能
                    tmp -= (uint)(tmp * (sActor.Status.Nooheito_rate / 100));
                if (sActor.Status.HatredUp_rate > 0)//骑士3转13级技能
                    tmp += (uint)(tmp * (sActor.Status.HatredUp_rate / 100));
            }
            if (this.Hate.ContainsKey(sActor.ActorID))
            {
                if (tmp == 0)
                    tmp = 1;
                this.Hate[sActor.ActorID] += tmp;
            }
            else
            {
                if (tmp == 0)
                    tmp = 1;
                if (this.Hate.Count == 0)//保存怪物战斗前位置
                {
                    this.X_pb = this.actor.X;
                    this.Y_pb = this.actor.Y;
                }
                this.Hate.TryAdd(sActor.ActorID, tmp);
            }
            if (damage > 0)
            {
                if (this.DamageTable.ContainsKey(sActor.ActorID))
                {
                    this.DamageTable[sActor.ActorID] += damage;
                }
                else
                {
                    this.DamageTable.Add(sActor.ActorID, damage);
                }

                if (this.DamageTable[sActor.ActorID] >= Partner.MaxHP)// if (this.DamageTable[sActor.ActorID] > Partner.MaxHP)
                {
                    this.DamageTable[sActor.ActorID] = (int)Partner.MaxHP;
                }
            }

            if (firstAttacker != null)
            {
                if (firstAttacker == sActor)
                {
                    attackStamp = DateTime.Now;
                }
                else
                {
                    if ((DateTime.Now - attackStamp).TotalMinutes > 15)
                    {
                        firstAttacker = sActor;
                        attackStamp = DateTime.Now;
                    }
                }
            }
            else
            {
                firstAttacker = sActor;
                attackStamp = DateTime.Now;
            }
        }

        public void OnSeenSkillUse(SkillArg arg)
        {
            if (map == null)
            { 
                Logger.ShowWarning(string.Format("Mob:{0}({1})'s map is null!", this.Partner.ActorID, Partner.Name));
                return;
            }
            if (master != null)
            {
                for (int i = 0; i < arg.affectedActors.Count; i++)
                {
                    if (arg.affectedActors[i].ActorID == master.ActorID)
                    {
                        Actor actor = map.GetActor(arg.sActor);
                        if (actor != null)
                        {
                            //ECOKEY 暫時解決寵物互毆問題
                            if (actor.type == ActorType.PC || actor.type == ActorType.PARTNER)
                                continue;
                            //ECOKEY 解決跟隨狀態的寵物會跟著怪物跑的問題
                            ActorPartner p = (ActorPartner)this.Partner;
                            if (p.ai_mode == 2 || p.basic_ai_mode == 2)
                                continue;
                            this.OnAttacked(actor, arg.hp[i]);
                            if (this.Hate.Count == 1)
                                SendAggroEffect();
                        }
                    }
                }
            }
            if (this.Mode.HateHeal)
            {
                Actor actor = map.GetActor(arg.sActor);
                if (actor != null && arg.skill != null && this.Hate.Count > 0)
                {
                    if (arg.skill.Support && actor.type == ActorType.PC)
                    {
                        int damage = 0;
                        foreach (int i in arg.hp)
                        {
                            damage += -i;
                        }
                        if (damage > 0)
                        {
                            if (this.Hate.Count == 0)
                                SendAggroEffect();
                            this.OnAttacked(actor, (int)(damage));
                        }
                    }
                }
            }
            if (arg.skill != null)
            {
                if (arg.skill.Support && !this.Mode.HateHeal)
                {
                    Actor actor = map.GetActor(arg.sActor);
                    if (actor.type == ActorType.PC)
                    {
                        int damage = 0;
                        foreach (int i in arg.hp)
                        {
                            damage += -i * 2;
                        }
                        if (this.DamageTable.ContainsKey(actor.ActorID))
                        {
                            this.DamageTable[actor.ActorID] += damage;
                        }
                        else this.DamageTable.Add(actor.ActorID, damage);
                        if (this.DamageTable[actor.ActorID] > Partner.MaxHP)
                            this.DamageTable[actor.ActorID] = (int)Partner.MaxHP;
                    }
                }
                else if (arg.skill.ID == 3055)//复活
                {
                    Actor actor = map.GetActor(arg.sActor);
                    Actor dActor = map.GetActor(arg.dActor);
                    if (actor.type == ActorType.PC && dActor.type == ActorType.PC && actor != null && dActor != null)
                    {
                        int damage = 0;
                        damage = (int)dActor.MaxHP * 2;
                        if (this.DamageTable.ContainsKey(actor.ActorID))
                        {
                            this.DamageTable[actor.ActorID] += damage;
                        }
                        else this.DamageTable.Add(actor.ActorID, damage);
                        if (this.DamageTable[actor.ActorID] > Partner.MaxHP)
                            this.DamageTable[actor.ActorID] = (int)Partner.MaxHP;
                    }
                }
            }
            if (this.Mode.HateMagic)
            {
                Actor actor = map.GetActor(arg.sActor);
                if (actor != null && arg.skill != null)
                {
                    if (arg.skill.Magical)
                    {
                        if (actor.type == ActorType.PC)
                        {
                            if (this.Hate.Count == 0)
                                SendAggroEffect();
                            this.OnAttacked(actor, (int)(this.Partner.MaxHP / 10));
                        }
                    }
                }
            }
        }

        void SendAggroEffect()
        {
            EffectArg arg = new EffectArg();
            arg.actorID = this.Partner.ActorID;
            arg.effectID = 4539;
            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg, this.Partner, false);
        }

    }
}
