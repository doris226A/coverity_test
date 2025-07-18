﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SagaDB.Actor;
using SagaDB.Item;
using SagaDB.Experience;
using SagaDB.Partner;
using SagaDB.Party;
using SagaLib;
using SagaLib.VirtualFileSystem;
using SagaMap.Network.Client;
using SagaMap.Scripting;
using System.Linq;
using System.Collections.Concurrent;

namespace SagaMap.Scripting
{
    public enum LevelType : byte
    {
        CLEVEL = 1,
        JLEVEL = 2,
        JLEVEL2 = 3,
        JLEVEL2T = 4,
        CLEVEL2 = 5,
        JLEVEL3 = 6,
        DUALJ = 7,
        PLEVEL = 8,
        PLEVEL2 = 9
    }

}
namespace SagaMap.Manager
{
    public sealed class ExperienceManager : Singleton<ExperienceManager>
    {
        short[,] clTable = new short[,]{
            {0,1},
            {82,3},
            {96,4},
            {124,5},
            {152,6},
            {163,11},
            {177,12},
            {192,13},
            {207,14},
            {223,15},
            {238,16},
            {243,23},
            {248,24},
            {258,25},
            {269,26},
            {279,27},
            {289,28},
            {300,29},
            {310,30},
            {320,31},
            {320,31},
            {325,41},
            {332,42},
            {340,43},
            {347,44},
            {355,45},
            {363,46},
            {371,47},
            {380,48},
            {388,49},
            {395,50},
            {402,51},
            {406,63},
            {409,64},
            {415,65},
            {422,66},
            {427,67},
            {433,68},
            {440,69},
            {446,70},
            {448,90},
            {452,91},
            {457,92},
            {461,93},
            {465,94},
            {468,95},
            {475,96},
            {474,97},
            {477,98},
            {480,99},
            {484,100},
            {486,500},
            {30000,70},
            {30000,70}
        };
        short[,] clTableDom = new short[,]{
            {0,1},
            {82,3},
            {96,4},
            {124,5},
            {152,6},
            {163,11},
            {177,12},
            {192,13},
            {207,14},
            {223,15},
            {238,16},
            {244,23},
            {248,24},
            {258,25},
            {269,26},
            {279,27},
            {289,28},
            {300,29},
            {310,30},
            {320,31},
            {325,41},
            {332,42},
            {340,43},
            {347,44},
            {355,45},
            {363,46},
            {371,47},
            {380,48},
            {388,49},
            {395,50},
            {402,51},
            {406,63},
            {409,64},
            {415,65},
            {422,66},
            {427,67},
            {433,68},
            {440,69},
            {446,70},
            {448,90},
            {452,91},
            {457,92},
            {461,93},
            {465,94},
            {468,95},
            {475,96},
            {474,97},
            {477,98},
            {480,99},
            {484,100},
            {486,500},
            {30000,6},
            {30000,6}
    };

        #region Private fields

        private Dictionary<uint, Level> PCEXPChart = new Dictionary<uint, Level>();
        /// <summary>
        /// Cummulative experience for partner level, level starts at 1
        /// </summary>
        private Dictionary<byte, ulong> PartnerLvEXPChart = new Dictionary<byte, ulong>();
        /// <summary>
        /// Cummulative experience for partner rank, rank starts at 1
        /// </summary>
        //  private Dictionary<byte, ulong> PartnerRankEXPChart = new Dictionary<byte, ulong>();
        public Dictionary<byte, ulong> PartnerRankEXPChart = new Dictionary<byte, ulong>(); //New
        /// <summary>
        /// Cummulative experience for partner reliability color, color starts at 0
        /// </summary>
        public Dictionary<byte, ulong> PartnerReliabilityEXPChart = new Dictionary<byte, ulong>()
            { //信賴度愛心 ECOKEY
               {0,0}, //黑
               {1,2400}, //紫 總2,400
               {2,12000}, //青 總12,000
               {3,48000}, //水總48,000
               {4,96000}, //綠 96,000
               {5,144000}, //黃綠144,000
               {6,192000}, //黃192,000
               {7,240000}, //橙240,000
               {8,360000}, //赤360,000
               {9,480000}, //桃 總480,000
            };

        private byte PartnerLvMax = 110;
        private byte PartnerRankMax = 110;
        private byte PartnerReliabilityMax = 9;
        // Fields of local use only, declared here for memory optimization
        private uint currentMax;

        #endregion

        #region Public fields
        public uint MaxCLevel = 110;
        public readonly uint MaxJLevel = 50;
        public uint MaxCLevel2 = 100;
        public readonly uint MaxJLevel3 = 50;
        public readonly byte MaxDualJobLevel = 110;
        public readonly uint LastTimeLevelLimit = 100;
        #endregion

        #region Enums/Structs

        public struct Level
        {
            public readonly ulong cxp, jxp, jxp2, cxp2, jxp3;//, dualjexp;
            public Level(ulong cxp, ulong jxp, ulong jxp2, ulong cxp2, ulong jxp3)//,ulong dualjexp)
            {
                this.cxp = cxp;
                this.jxp = jxp;
                this.jxp2 = jxp2;
                this.cxp2 = cxp2;
                this.jxp3 = jxp3;
                // this.dualjexp = dualjexp;
            }
        }

        #endregion

        #region EXP table loading methods

        public void LoadTable(string path)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                XmlElement root;
                XmlNodeList list;
                xml.Load(VirtualFileSystemManager.Instance.FileSystem.OpenFile(path));
                root = xml["exp"];
                list = root.ChildNodes;
                foreach (object j in list)
                {
                    XmlElement i;
                    if (j.GetType() != typeof(XmlElement)) continue;
                    i = (XmlElement)j;
                    switch (i.Name.ToLower())
                    {
                        case "level":
                            XmlNodeList maps = i.ChildNodes;
                            ulong cexp = 0, cexp2 = 0, jexp = 0, jexp2 = 0, jexp3 = 0;//, dualjexp = 0;
                            byte lv = 0;
                            foreach (object l in maps)
                            {
                                XmlElement k;
                                if (l.GetType() != typeof(XmlElement)) continue;
                                k = (XmlElement)l;
                                switch (k.Name.ToLower())
                                {
                                    case "lv":
                                        lv = byte.Parse(k.InnerText);
                                        break;
                                    case "c":
                                        cexp = ulong.Parse(k.InnerText);
                                        break;
                                    case "j1":
                                        jexp = ulong.Parse(k.InnerText);
                                        break;
                                    case "j2":
                                        jexp2 = ulong.Parse(k.InnerText);
                                        break;
                                    case "c2":
                                        cexp2 = ulong.Parse(k.InnerText);
                                        break;
                                    case "j3":
                                        jexp3 = ulong.Parse(k.InnerText);
                                        break;
                                        /*case "dualjexp":
                                            dualjexp = ulong.Parse(k.InnerText);
                                            break;*/

                                }
                            }
                            Level newLv = new Level(cexp, jexp, jexp2, cexp2, jexp3);//, dualjexp);
                            PCEXPChart.Add(lv, newLv);
                            PartnerLvEXPChart.Add(lv, cexp2);
                            PartnerRankEXPChart.Add(lv, cexp);
                            break;
                    }
                }
                Logger.ShowInfo("EXP table loaded");
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        #endregion

        #region Public methods

        ulong checkExpGap(uint ori, uint add, byte maxLv, LevelType type)
        {
            ulong output = ori + add;
            if (output >= GetExpForLevel((uint)(maxLv + 1), type))
                output = GetExpForLevel((uint)(maxLv + 1), type) - 1;
            return output;
        }

        private void checkCL(ActorPC pc)
        {
            if (pc.Race == PC_RACE.DEM)
            {
                byte shouldLv = (byte)((pc.CL - 9) / 4 + 1);
                if (shouldLv > pc.Level)
                    SendLevelUp(MapClient.FromActorPC(pc), LevelType.CLEVEL, (uint)(shouldLv - pc.Level));
            }
        }

        public short GetEPRequired(ActorPC pc)
        {
            if (pc.Race == PC_RACE.DEM)
            {
                return GetEPRequired(pc.CL, false);
            }
            return 0;
        }

        public short GetEPRequired(short cl, bool dominion)
        {
            if (dominion)
            {
                for (int i = 0; i < clTableDom.Length; i++)
                {
                    if (cl < clTableDom[i, 0])
                        return clTableDom[i - 1, 1];
                }
            }
            else
            {
                for (int i = 0; i < clTableDom.Length; i++)
                {
                    if (cl < clTable[i, 0])
                        return clTable[i - 1, 1];
                }
            }
            return 0;
        }

        public void ApplyEP(ActorPC pc, short count)
        {
            bool dominion = MapManager.Instance.GetMap(pc.MapID).Info.Flag.Test(SagaDB.Map.MapFlags.Dominion);
            short cl = 0;
            while (count > 0)
            {
                if (dominion)
                {
                    short max_ep = GetEPRequired((short)(pc.DominionCL + cl), dominion);
                    if (pc.DominionEPUsed + count >= max_ep)
                    {
                        cl++;
                        count -= (short)(max_ep - pc.DominionEPUsed);
                        pc.DominionEPUsed = 0;
                    }
                    else
                    {
                        pc.DominionEPUsed += count;
                        count = 0;
                    }
                }
                else
                {
                    short max_ep = GetEPRequired((short)(pc.CL + cl), dominion);
                    if (pc.EPUsed + count >= max_ep)
                    {
                        cl++;
                        count -= (short)(max_ep - pc.EPUsed);
                        pc.EPUsed = 0;
                    }
                    else
                    {
                        pc.EPUsed += count;
                        count = 0;
                    }
                }
            }
            if (cl > 0)
                ApplyCL(pc, cl);
        }

        public void ApplyCL(ActorPC pc, short count)
        {
            if (pc.Race == PC_RACE.DEM)
            {
                pc.CL += count;
                checkCL(pc);
            }
        }


        /// <summary>
        /// Apply input percentage of experience from input targetNPC to input targetPC.
        /// The percentage gets capped at 1f and are multiplied by global EXP rate(s).
        /// </summary>
        /// <param name="targetPC">The target PC (the player)</param>
        /// <param name="percentage">The percentage of the NPC's exp to gain (for instance: the percentage of HP deducted by input player)</param>

        public void ApplyExp(ActorPC targetPC, uint exp, uint jexp, float percentage)
        {
            // TODO implement different rates for different exp types
            //percentage *= (float)Config.Instance.EXPRate / 100f;
            //Weapon weapon = WeaponFactory.GetActiveWeapon(targetPC);

            float realexp = exp * percentage;
            float realjobexp = jexp * percentage;

            //ECOKEY 經驗判斷更改
            /*if (targetPC.Level >= MaxCLevel || targetPC.Level >= MaxCLevel2)
                realexp = 0;*/
            if (targetPC.Race == PC_RACE.DEM)//ECOKEY DEM經驗判斷，原有的code放到else
            {
                if (targetPC.CEXP >= 999999999) realexp = 0;
                if (targetPC.JEXP >= 999999999) realjobexp = 0;
            }
            else
            {
                if ((targetPC.Rebirth && targetPC.Level >= MaxCLevel2) || (!targetPC.Rebirth && targetPC.Level >= MaxCLevel))
                    realexp = 0;

                // 修正EXP显示
                if (targetPC.DualJobID != 0 && targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobLevel >= MaxDualJobLevel)
                    if (targetPC.CurrentJobLevel >= MaxJLevel)
                        realjobexp = 0;
            }

            ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)targetPC.e;

            if (realexp != 0 || realjobexp != 0)
                eh.Client.SendSystemMessage(string.Format(LocalManager.Instance.Strings.GET_EXP, (uint)(realexp), (uint)(realjobexp)));

            Map map = MapManager.Instance.GetMap(targetPC.MapID);

            if (targetPC.Race == PC_RACE.DEM)//ECOKEY DEM新增經驗判斷
            {
                targetPC.CEXP += (ulong)(realexp);
                targetPC.JEXP += (ulong)(realjobexp);
                if (targetPC.CEXP > 999999999) targetPC.CEXP = 999999999;
                if (targetPC.JEXP > 999999999) targetPC.JEXP = 999999999;
                MapClient.FromActorPC(targetPC).SendEXP();
                if (targetPC.Partner != null)
                {
                    float partnerexpDEM = exp * percentage;  //Ecokey

                    ActorPartner partnerDEM = targetPC.Partner;
                    if (partnerDEM == null) return;

                    if (partnerDEM.Level >= 110)
                        return;
                    if (partnerDEM.Level >= 1)
                    {
                        partnerexpDEM /= 10;
                        targetPC.Partner.exp += (ulong)(partnerexpDEM);
                        MapClient.FromActorPC(targetPC).SendSystemMessage("夥伴 " + partnerDEM.Name + " 獲得了" + (int)partnerexpDEM + "點經驗值");
                    }
                    if (!targetPC.Partner.rebirth)
                        CheckExp(eh.Client, LevelType.PLEVEL);
                    else
                        CheckExp(eh.Client, LevelType.PLEVEL2);
                    MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                }
                return;
            }

            if (targetPC.JobJoint == PC_JOB.NONE)
            {
                if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                {
                    targetPC.DominionCEXP += (ulong)(realexp);
                    if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(51, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(51, LevelType.CLEVEL) - 1;

                        targetPC.DominionJEXP += (ulong)(realjobexp);
                        if (targetPC.DominionJEXP >= GetExpForLevel(51, LevelType.JLEVEL2))
                            targetPC.DominionJEXP = GetExpForLevel(51, LevelType.JLEVEL2) - 1;
                    }
                    else
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(31, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(31, LevelType.CLEVEL) - 1;

                        targetPC.DominionJEXP += (ulong)(jexp * percentage);
                        if (targetPC.DominionJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                            targetPC.DominionJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                    }
                }
                else
                {
                    if (!targetPC.Rebirth)
                    {
                        if (targetPC.CEXP < GetExpForLevel(MaxCLevel, LevelType.CLEVEL))
                            targetPC.CEXP += (ulong)(realexp);
                    }
                    else
                    {
                        if (targetPC.CEXP < GetExpForLevel(MaxCLevel2, LevelType.CLEVEL2))
                        {
                            targetPC.CEXP += (ulong)(realexp);
                        }
                    }

                    if (targetPC.DualJobID != 0)
                    {
                        if (targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp < GetExpForLevel(MaxDualJobLevel, LevelType.DUALJ))
                            targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp += (ulong)(realjobexp);
                    }
                    else if (targetPC.Job == targetPC.JobBasic)
                    {
                        if (targetPC.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL))
                            targetPC.JEXP += (ulong)(realjobexp);
                    }
                    else if (!targetPC.Rebirth)
                    {
                        if (targetPC.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL2))
                            targetPC.JEXP += (ulong)(realjobexp);
                    }
                    else
                    {
                        if (targetPC.JEXP < GetExpForLevel(MaxJLevel3, LevelType.JLEVEL3))
                            targetPC.JEXP += (ulong)(realjobexp);
                    }
                }
            }
            else
            {
                if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                {
                    targetPC.DominionCEXP += (ulong)(realexp);
                    if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(51, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(51, LevelType.CLEVEL) - 1;
                    }
                    else
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(31, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(31, LevelType.CLEVEL) - 1;
                    }
                }
                else
                {
                    if (targetPC.CEXP < GetExpForLevel(MaxCLevel, LevelType.CLEVEL))
                        targetPC.CEXP += (ulong)(realexp);
                }
                targetPC.JointJEXP += (ulong)(realjobexp);
                if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                {
                    //ECOKEY 馴獸師等級上限是30
                    if (targetPC.JointJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                        targetPC.JointJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                }
                else
                {
                    if (targetPC.JointJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                        targetPC.JointJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                }
            }

            if (targetPC.Race != PC_RACE.DEM)
            {
                eh = (ActorEventHandlers.PCEventHandler)targetPC.e;
                if (!targetPC.Rebirth)
                {
                    CheckExp(eh.Client, LevelType.CLEVEL);
                }
                else
                {
                    CheckExp(eh.Client, LevelType.CLEVEL2);
                }

                if (targetPC.JobJoint == PC_JOB.NONE)
                {
                    if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                        CheckExp(eh.Client, LevelType.JLEVEL2);
                    else
                    {
                        if (targetPC.DualJobID != 0)
                            CheckExp(eh.Client, LevelType.DUALJ);
                        else if (targetPC.Job == targetPC.JobBasic)
                            CheckExp(eh.Client, LevelType.JLEVEL);
                        else if (targetPC.Job == targetPC.Job2T || targetPC.Job == targetPC.Job2X)
                            CheckExp(eh.Client, LevelType.JLEVEL2);
                        else
                            CheckExp(eh.Client, LevelType.JLEVEL3);
                    }
                }
                else
                {
                    CheckExp(eh.Client, LevelType.JLEVEL2);
                }
            }
            float partnerexp = exp * percentage;  //Ecokey

            ActorPartner partner = targetPC.Partner;
            if (partner == null) return;

            if (partner.Level >= 110)
                return;
            if (partner.Level >= 1)
            {
                partnerexp /= 10;
                targetPC.Partner.exp += (ulong)(partnerexp);
                //  partner.exp += (ulong)partnerexp; //主人完成任務後，獲得的經驗寵物也會享有10%，但不會獲得信賴度。
                MapClient.FromActorPC(targetPC).SendSystemMessage("夥伴 " + partner.Name + " 獲得了" + (int)partnerexp + "點經驗值");
                MapClient.FromActorPC(targetPC).SendPetBasicInfo();
            }

            //ECOKEY 寵物憑依經驗
            foreach (ActorPartner p in targetPC.PossesionedPartner.Values)
            {
                ulong pexp = (ulong)(partnerexp * CalcPetLevelDiffExp(p.Level - targetPC.Level));
                if (p.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL))//寵物經驗不超過主人
                    p.exp += pexp;
                uint lvlDelta = this.GetLevelDelta(p.Level, p.exp, LevelType.PLEVEL, true);
                if (lvlDelta > 0)
                {
                    var dif = 110 - p.Level;
                    p.Level += (byte)lvlDelta;
                }
            }

            //ECOKEY 騎寵專用升等判斷（機器人）
            if (targetPC.Pet != null && targetPC.Pet.Ride == true && targetPC.Partner != null)
            {
                if (!targetPC.Partner.rebirth)
                {
                    if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                        targetPC.Partner.exp += (ulong)(partnerexp);
                    MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                    CheckExp(eh.Client, LevelType.PLEVEL);
                }
                else
                {
                    if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL2))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                        targetPC.Partner.exp += (ulong)(partnerexp);
                    MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                    CheckExp(eh.Client, LevelType.PLEVEL2);
                }
            }

            if (targetPC.Partner != null)
            {
                //防止一种没有属性的Partner（目前DB内无资料并且连召唤、收回都做不到的）卡死整个分配经验进程
                ActorEventHandlers.PartnerEventHandler eh2 = (ActorEventHandlers.PartnerEventHandler)targetPC.Partner.e;
                if (eh2 != null)
                {
                    if (!targetPC.Partner.rebirth)
                        CheckExp(eh.Client, LevelType.PLEVEL);
                    else
                        CheckExp(eh.Client, LevelType.PLEVEL2);
                }
            }
            percentage = percentage * Configuration.Instance.CalcQuestRateForPC(targetPC);
        }


        /// <summary>
        /// Apply input percentage of experience from input targetNPC to input targetPC.
        /// The percentage gets capped at 1f and are multiplied by global EXP rate(s).
        /// </summary>
        /// <param name="targetPC">The target PC (the player)</param>
        /// <param name="targetNPC">The target NPC (the "mob")</param>
        /// <param name="percentage">The percentage of the NPC's exp to gain (for instance: the percentage of HP deducted by input player)</param>
        public void ApplyExp(ActorPC targetPC, ActorMob targetNPC, float percentage)
        {
            // TODO implement different rates for different exp types
            //percentage *= (float)Config.Instance.EXPRate / 100f;
            //Weapon weapon = WeaponFactory.GetActiveWeapon(targetPC);

            percentage = percentage * Configuration.Instance.CalcEXPRateForPC(targetPC);

            ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)targetPC.e;

            ulong baseExp = (ulong)(targetNPC.BaseData.baseExp * percentage);
            ulong jobExp = (ulong)(targetNPC.BaseData.jobExp * percentage);

            ulong targetexp = baseExp;
            ulong targetjexp = jobExp;

            ulong partnerexp = (ulong)(targetNPC.BaseData.baseExp * percentage);

            float expup = 1;
            float jobexpup = 1;
            //ECOKEY 經驗加倍技能
            if (targetPC.Buff.EXPUp)
            {
                expup = expup * 1.05f;
                jobexpup = jobexpup * 1.05f;
                /*targetexp = (ulong)((float)targetexp * 1.05f);
                targetjexp = (ulong)((float)targetjexp * 1.05f);
                partnerexp = (ulong)((float)partnerexp * 1.05f);*/
            }
            //ECOKEY 經驗戒指
            if (targetPC.TimerItems.ContainsKey("EXP_ALL_BOUNS"))
            {
                expup = expup * (1.0f + float.Parse(targetPC.TimerItems["EXP_ALL_BOUNS"].BuffValues) / 100.0f);
                jobexpup = jobexpup * (1.0f + float.Parse(targetPC.TimerItems["EXP_ALL_BOUNS"].BuffValues) / 100.0f);
                /*float expup = float.Parse(targetPC.TimerItems["EXP_ALL_BOUNS"].BuffValues);
                targetexp = (ulong)((float)targetexp * (1.0f + expup / 100.0f));
                targetjexp = (ulong)((float)targetjexp * (1.0f + expup / 100.0f));
                partnerexp = (ulong)((float)partnerexp * (1.0f + expup / 100.0f));*/
            }
            //ECOKEY 職業經驗戒指
            if (targetPC.TimerItems.ContainsKey("EXP_JOB_BOUNS"))
            {
                jobexpup = jobexpup * (1.0f + float.Parse(targetPC.TimerItems["EXP_JOB_BOUNS"].BuffValues) / 100.0f);
                //float expup = float.Parse(targetPC.TimerItems["EXP_JOB_BOUNS"].BuffValues);
                //targetjexp = (ulong)((float)targetjexp * (1.0f + expup / 100.0f));
            }
            targetexp = (ulong)((float)targetexp * expup);
            targetjexp = (ulong)((float)targetjexp * jobexpup);
            //partnerexp = (ulong)((float)partnerexp * expup);//ECOKEY 強迫症發作，寵物計算統一移到下面
            //ECOKEY 經驗戒指無效問題修復
            baseExp = targetexp;
            jobExp = targetjexp;
            //ECOKEY 經驗判斷更改
            /*if (targetPC.Level >= MaxCLevel || targetPC.Level >= MaxCLevel2)
                targetexp = 0;*/
            if (targetPC.Race == PC_RACE.DEM)//ECOKEY DEM經驗判斷，原有的code放到else
            {
                if (targetPC.CEXP >= 999999999) targetexp = 0;
                if (targetPC.JEXP >= 999999999) targetjexp = 0;
            }
            else
            {
                if ((targetPC.Rebirth && targetPC.Level >= MaxCLevel2) || (!targetPC.Rebirth && targetPC.Level >= MaxCLevel))
                    targetexp = 0;
                // 修正EXP显示
                if (targetPC.DualJobID != 0 && targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobLevel >= MaxDualJobLevel)
                    if (targetPC.CurrentJobLevel >= MaxJLevel)
                        targetjexp = 0;
            }

            if ((targetexp != 0 || targetjexp != 0))
                eh.Client.SendSystemMessage(string.Format(LocalManager.Instance.Strings.GET_EXP, targetexp, targetjexp));
            //ECOKEY 強迫症發作，寵物計算統一移到下面
            /*partnerexp /= 10;
            if (partnerexp == 0)
                partnerexp = 1;
            if (targetPC.Partner != null)
                eh.Client.SendSystemMessage("夥伴 " + targetPC.Partner.Name + " 獲得了" + partnerexp + "點經驗值");
            MapClient.FromActorPC(targetPC).SendPetBasicInfo();*/
            //ECOKEY 寵物計算統一移到這邊，外加訓獸師技能
            if (targetPC.Partner != null)
            {
                //ECOKEY新增IRIS寵物經驗
                if (targetPC.Status.part_expup_iris != 0)
                {
                    partnerexp = (ulong)((float)partnerexp * (float)(1.0f + targetPC.Status.part_expup_iris / 100.0f));
                }
                if (targetPC.JobJoint == PC_JOB.BREEDER)
                {
                    if (targetPC.JointJobLevel >= 5 && targetPC.JointJobLevel < 15)
                    {
                        partnerexp = (ulong)((float)partnerexp * 1.02f);
                    }
                    else if (targetPC.JointJobLevel >= 15)
                    {
                        partnerexp = (ulong)((float)partnerexp * 1.05f);
                    }
                }
                partnerexp = (ulong)((float)partnerexp * expup);
                partnerexp /= 10;
                if (targetPC.TimerItems.ContainsKey("EXP_PARTNER_BOUNS"))//ECOKEY 新增超級蔬菜棒
                {
                    partnerexp = (ulong)(partnerexp * (1.0f + float.Parse(targetPC.TimerItems["EXP_PARTNER_BOUNS"].BuffValues) / 100.0f));
                }
                if (partnerexp == 0) //ECOKEY 寵物經驗最低是1
                    partnerexp = 1;
                eh.Client.SendSystemMessage("夥伴 " + targetPC.Partner.Name + " 獲得了" + partnerexp + "點經驗值");
            }

            //ECOKEY 寵物憑依經驗
            foreach (ActorPartner p in targetPC.PossesionedPartner.Values)
            {
                ulong pexp = (ulong)(partnerexp * CalcPetLevelDiffExp(p.Level - targetPC.Level));
                if (p.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL))//寵物經驗不超過主人
                    p.exp += pexp;
                uint lvlDelta = this.GetLevelDelta(p.Level, p.exp, LevelType.PLEVEL, true);
                if (lvlDelta > 0)
                {
                    var dif = 110 - p.Level;
                    p.Level += (byte)lvlDelta;
                }
            }

            Map map = MapManager.Instance.GetMap(targetPC.MapID);

            foreach (ActorPC i in targetPC.PossesionedActors)
            {
                eh = (ActorEventHandlers.PCEventHandler)i.e;
                if (i == targetPC)
                    continue;
                if (i.JobJoint == PC_JOB.NONE)
                {
                    if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                    {
                        i.DominionCEXP += (ulong)(baseExp * 0.1f);
                        if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                        {
                            if (i.DominionCEXP >= GetExpForLevel(51, LevelType.CLEVEL))
                                i.DominionCEXP = GetExpForLevel(51, LevelType.CLEVEL) - 1;

                            targetPC.DominionJEXP += (ulong)(jobExp * 0.1f);
                            if (i.DominionJEXP >= GetExpForLevel(50, LevelType.JLEVEL2))
                                i.DominionJEXP = GetExpForLevel(50, LevelType.JLEVEL2) - 1;
                        }
                        else
                        {
                            if (i.DominionCEXP >= GetExpForLevel(31, LevelType.CLEVEL))
                                i.DominionCEXP = GetExpForLevel(31, LevelType.CLEVEL) - 1;

                            targetPC.DominionJEXP += (ulong)(targetNPC.BaseData.jobExp * percentage * 0.1f);
                            if (i.DominionJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                                i.DominionJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                        }

                    }
                    else
                    {
                        if (!i.Rebirth)
                        {
                            if (i.CEXP < GetExpForLevel(MaxCLevel, LevelType.CLEVEL))
                                i.CEXP += (ulong)(baseExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }
                        else
                        {
                            if (i.CEXP < GetExpForLevel(MaxCLevel2, LevelType.CLEVEL2))
                                i.CEXP += (ulong)(baseExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }

                        if (i.DualJobID != 0)
                        {
                            if (i.PlayerDualJobList[i.DualJobID].DualJobExp < GetExpForLevel(MaxDualJobLevel, LevelType.DUALJ))
                                i.PlayerDualJobList[i.DualJobID].DualJobExp += (ulong)(jobExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }
                        else if (i.Job == i.JobBasic)
                        {
                            if (i.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL))
                                i.JEXP += (ulong)(jobExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }
                        else if (!i.Rebirth)
                        {
                            if (i.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL2))
                                i.JEXP += (ulong)(jobExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }
                        else
                        {
                            if (i.JEXP < GetExpForLevel(MaxJLevel3, LevelType.JLEVEL3))
                                i.JEXP += (ulong)(jobExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }

                    }
                }
                else
                {
                    if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                    {
                        i.DominionCEXP += (ulong)(baseExp * 0.1f);
                        if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                        {
                            if (i.DominionCEXP >= GetExpForLevel(51, LevelType.CLEVEL))
                                i.DominionCEXP = GetExpForLevel(51, LevelType.CLEVEL) - 1;
                        }
                        else
                        {
                            if (i.DominionCEXP >= GetExpForLevel(31, LevelType.CLEVEL))
                                i.DominionCEXP = GetExpForLevel(31, LevelType.CLEVEL) - 1;
                        }
                    }
                    else
                    {
                        if (!i.Rebirth)
                        {
                            if (i.CEXP < GetExpForLevel(MaxCLevel, LevelType.CLEVEL))
                                i.CEXP += (ulong)(baseExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }
                        else
                        {
                            if (i.CEXP < GetExpForLevel(MaxCLevel2, LevelType.CLEVEL2))
                                i.CEXP += (ulong)(baseExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                        }
                    }
                    i.JointJEXP += (ulong)(jobExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                    if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                    {
                        if (i.JointJEXP >= GetExpForLevel(MaxJLevel, LevelType.JLEVEL2))
                            i.JointJEXP = GetExpForLevel(MaxJLevel, LevelType.JLEVEL2) - 1;
                    }
                    else
                    {
                        if (i.JointJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                            i.JointJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                    }
                    if (i.DualJobID != 0)
                    {
                        if (i.PlayerDualJobList[i.DualJobID].DualJobExp < GetExpForLevel(MaxDualJobLevel, LevelType.DUALJ))
                            i.PlayerDualJobList[i.DualJobID].DualJobExp += (ulong)(jobExp * GetPossesionLevelDiffExpFactor(i, targetPC));
                    }
                }
                if (i.Race != PC_RACE.DEM)
                {
                    if (!i.Rebirth)
                        CheckExp(eh.Client, LevelType.CLEVEL);
                    else
                        CheckExp(eh.Client, LevelType.CLEVEL2);
                    if (i.JobJoint == PC_JOB.NONE)
                    {
                        if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                            CheckExp(eh.Client, LevelType.JLEVEL2);
                        else
                        {
                            if (i.DualJobID != 0)
                                CheckExp(eh.Client, LevelType.DUALJ);
                            else if (i.Job == i.JobBasic)
                                CheckExp(eh.Client, LevelType.JLEVEL);
                            else if (!i.Rebirth)
                                CheckExp(eh.Client, LevelType.JLEVEL2);
                            else
                                CheckExp(eh.Client, LevelType.JLEVEL3);
                        }
                    }
                    else
                        CheckExp(eh.Client, LevelType.JLEVEL2);
                }
                if (eh.Client.state != MapClient.SESSION_STATE.DISCONNECTED)
                {
                    eh.Client.SendEXP();
                    //ECOKEY 新增經驗戒指PE經驗
                    if (targetNPC.BaseData.baseExp != 0 || targetNPC.BaseData.jobExp != 0)
                    {
                        ulong PEbaseExp = (ulong)(targetNPC.BaseData.baseExp);
                        ulong PEjobExp = (ulong)(targetNPC.BaseData.jobExp);
                        if (i.Buff.EXPUp)
                        {
                            PEbaseExp = (ulong)(PEbaseExp * 1.05f);
                            PEjobExp = (ulong)(PEjobExp * 1.05f);
                        }
                        if (i.TimerItems.ContainsKey("EXP_ALL_BOUNS"))
                        {
                            PEbaseExp = (ulong)(PEbaseExp * (1.0f + float.Parse(i.TimerItems["EXP_ALL_BOUNS"].BuffValues) / 100.0f));
                            PEjobExp = (ulong)(PEjobExp * (1.0f + float.Parse(i.TimerItems["EXP_ALL_BOUNS"].BuffValues) / 100.0f));
                        }
                        if (i.TimerItems.ContainsKey("EXP_JOB_BOUNS"))
                        {
                            PEjobExp = (ulong)(PEjobExp * (1.0f + float.Parse(i.TimerItems["EXP_JOB_BOUNS"].BuffValues) / 100.0f));
                        }
                        eh.Client.SendSystemMessage(string.Format(LocalManager.Instance.Strings.POSSESSION_EXP, (uint)(PEbaseExp * percentage * GetPossesionLevelDiffExpFactor(i, targetPC)), (uint)(PEjobExp * percentage * GetPossesionLevelDiffExpFactor(i, targetPC)))); ;
                    }
                }
            }
            if (targetPC.Race == PC_RACE.DEM)//ECOKEY DEM新增經驗判斷
            {
                targetPC.CEXP += (ulong)(baseExp);
                targetPC.JEXP += (ulong)(jobExp);
                if (targetPC.CEXP > 999999999) targetPC.CEXP = 999999999;
                if (targetPC.JEXP > 999999999) targetPC.JEXP = 999999999;
                MapClient.FromActorPC(targetPC).SendEXP();
                if (targetPC.Partner != null)
                {
                    ActorEventHandlers.PartnerEventHandler eh2 = (ActorEventHandlers.PartnerEventHandler)targetPC.Partner.e;
                    if (eh2 != null)
                    {
                        if (!targetPC.Partner.rebirth)
                        {
                            if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL))
                                targetPC.Partner.exp += (ulong)(partnerexp);
                            MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                        }
                        else
                        {
                            if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL2))
                                targetPC.Partner.exp += (ulong)(partnerexp);
                            MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                        }
                        if (!targetPC.Partner.rebirth)
                            CheckExp(eh.Client, LevelType.PLEVEL);
                        else
                            CheckExp(eh.Client, LevelType.PLEVEL2);
                    }
                }
                return;
            }

            if (targetPC.JobJoint == PC_JOB.NONE)
            {
                if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                {
                    targetPC.DominionCEXP += (ulong)(baseExp);
                    if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(51, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(51, LevelType.CLEVEL) - 1;

                        targetPC.DominionJEXP += (ulong)(jobExp);
                        if (targetPC.DominionJEXP >= GetExpForLevel(50, LevelType.JLEVEL2))
                            targetPC.DominionJEXP = GetExpForLevel(50, LevelType.JLEVEL2) - 1;
                    }
                    else
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(31, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(31, LevelType.CLEVEL) - 1;

                        targetPC.DominionJEXP += (ulong)(jobExp);
                        if (targetPC.DominionJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                            targetPC.DominionJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                    }
                }
                else
                {
                    if (!targetPC.Rebirth)
                    {
                        if (targetPC.CEXP < GetExpForLevel(MaxCLevel, LevelType.CLEVEL))
                            targetPC.CEXP += (ulong)(baseExp);
                    }
                    else
                    {
                        if (targetPC.CEXP < GetExpForLevel(MaxCLevel2, LevelType.CLEVEL2))
                            targetPC.CEXP += (ulong)(baseExp);
                    }

                    if (targetPC.DualJobID != 0)
                    {
                        if (targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp < GetExpForLevel(MaxDualJobLevel, LevelType.DUALJ))
                            targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp += (ulong)(jobExp);
                    }
                    else if (targetPC.Job == targetPC.JobBasic)
                    {
                        if (targetPC.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL))
                            targetPC.JEXP += (ulong)(jobExp);
                    }
                    else if (!targetPC.Rebirth)
                    {
                        if (targetPC.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL2))
                            targetPC.JEXP += (ulong)(jobExp);
                    }
                    else
                    {
                        if (targetPC.JEXP < GetExpForLevel(MaxJLevel3, LevelType.JLEVEL3))
                            targetPC.JEXP += (ulong)(jobExp);
                    }
                    if (targetPC.Partner != null)
                    {
                        //防止一种没有属性的Partner（目前DB内无资料并且连召唤、收回都做不到的）卡死整个分配经验进程
                        ActorEventHandlers.PartnerEventHandler eh2 = (ActorEventHandlers.PartnerEventHandler)targetPC.Partner.e;
                        if (eh2 != null)
                        {
                            if (!targetPC.Partner.rebirth)
                            {
                                if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                                    targetPC.Partner.exp += (ulong)(partnerexp);
                                MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                            }
                            else
                            {
                                if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL2))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                                    targetPC.Partner.exp += (ulong)(partnerexp);
                                MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                            }
                        }

                    }
                }
            }
            else
            {
                if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                {
                    targetPC.DominionCEXP += (ulong)(baseExp);
                    if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(51, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(51, LevelType.CLEVEL) - 1;
                    }
                    else
                    {
                        if (targetPC.DominionCEXP >= GetExpForLevel(31, LevelType.CLEVEL))
                            targetPC.DominionCEXP = GetExpForLevel(31, LevelType.CLEVEL) - 1;
                    }
                }
                else
                {
                    if (targetPC.CEXP < GetExpForLevel(MaxCLevel, LevelType.CLEVEL))
                        targetPC.CEXP += (ulong)(baseExp);
                }
                targetPC.JointJEXP += (ulong)(jobExp);
                if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                {
                    //ECOKEY 馴獸師等級上限是30
                    if (targetPC.JointJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                        targetPC.JointJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                }
                else
                {
                    if (targetPC.JointJEXP >= GetExpForLevel(31, LevelType.JLEVEL2))
                        targetPC.JointJEXP = GetExpForLevel(31, LevelType.JLEVEL2) - 1;
                }
                if (targetPC.DualJobID != 0)
                {
                    targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp += (ulong)(jobExp);
                }
                if (targetPC.Partner != null)
                {
                    //防止一种没有属性的Partner（目前DB内无资料并且连召唤、收回都做不到的）卡死整个分配经验进程
                    ActorEventHandlers.PartnerEventHandler eh2 = (ActorEventHandlers.PartnerEventHandler)targetPC.Partner.e;
                    if (eh != null)
                    {
                        if (!targetPC.Partner.rebirth)
                        {
                            if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                                targetPC.Partner.exp += (ulong)(partnerexp);
                            MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                        }
                        else
                            if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL2))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                            targetPC.Partner.exp += (ulong)(partnerexp);
                        MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                    }

                }
            }
            eh = (ActorEventHandlers.PCEventHandler)targetPC.e;
            if (eh.Client.state != MapClient.SESSION_STATE.DISCONNECTED)
            {
                eh.Client.SendEXP();
            }
            if (targetPC.Race != PC_RACE.DEM)
            {
                if (!targetPC.Rebirth)
                    CheckExp(eh.Client, LevelType.CLEVEL);
                else
                    CheckExp(eh.Client, LevelType.CLEVEL2);
                if (targetPC.JobJoint == PC_JOB.NONE)
                {
                    if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                        CheckExp(eh.Client, LevelType.JLEVEL2);
                    else
                    {
                        if (targetPC.DualJobID != 0)
                            CheckExp(eh.Client, LevelType.DUALJ);
                        else if (targetPC.Job == targetPC.JobBasic)
                            CheckExp(eh.Client, LevelType.JLEVEL);
                        else if (!targetPC.Rebirth)
                            CheckExp(eh.Client, LevelType.JLEVEL2);
                        else
                            CheckExp(eh.Client, LevelType.JLEVEL3);
                    }
                }
                else
                    CheckExp(eh.Client, LevelType.JLEVEL2);
                //ECOKEY 騎寵專用升等判斷（機器人）（0310補上這裡！！）
                if (targetPC.Pet != null && targetPC.Pet.Ride == true && targetPC.Partner != null)
                {
                    if (!targetPC.Partner.rebirth)
                    {
                        if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                            targetPC.Partner.exp += (ulong)(partnerexp);
                        MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                        CheckExp(eh.Client, LevelType.PLEVEL);
                    }
                    else
                    {
                        if (targetPC.Partner.exp < GetExpForLevel(targetPC.Level, LevelType.PLEVEL2))//ECOKEY 寵物經驗更改//寵物經驗不超過主人
                            targetPC.Partner.exp += (ulong)(partnerexp);
                        MapClient.FromActorPC(targetPC).SendPetBasicInfo();
                        CheckExp(eh.Client, LevelType.PLEVEL2);
                    }
                }
                if (targetPC.Partner != null)
                {
                    //防止一种没有属性的Partner（目前DB内无资料并且连召唤、收回都做不到的）卡死整个分配经验进程
                    ActorEventHandlers.PartnerEventHandler eh2 = (ActorEventHandlers.PartnerEventHandler)targetPC.Partner.e;
                    if (eh2 != null)
                    {
                        if (!targetPC.Partner.rebirth)
                            CheckExp(eh.Client, LevelType.PLEVEL);
                        else
                            CheckExp(eh.Client, LevelType.PLEVEL2);
                    }
                }
            }
        }


        public void ApplyTamaireExp(uint cexp, uint jexp, ActorPC targetPC)
        {
            MapClient client = MapClientManager.Instance.FindClient(targetPC.CharID);
            var eh = (ActorEventHandlers.PCEventHandler)targetPC.e;
            if (!targetPC.Rebirth)
            {
                if (targetPC.CEXP < GetExpForLevel(MaxCLevel, LevelType.CLEVEL))
                    targetPC.CEXP += (ulong)(cexp);
                if (targetPC.Job == targetPC.JobBasic)
                {
                    if (targetPC.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL))
                        targetPC.JEXP += (ulong)(jexp);
                }
                else
                {
                    if (targetPC.JEXP < GetExpForLevel(MaxJLevel, LevelType.JLEVEL2))
                        targetPC.JEXP += (ulong)(jexp);
                }
            }
            else
            {
                if (targetPC.CEXP < GetExpForLevel(MaxCLevel2, LevelType.CLEVEL2))
                    targetPC.CEXP += (ulong)(cexp);
                if (targetPC.JEXP < GetExpForLevel(MaxJLevel3, LevelType.JLEVEL3))
                    targetPC.JEXP += (ulong)(jexp);
                if (targetPC.DualJobID != 0)//ECOKEY 心PE經驗，副職判斷
                {
                    if ((ulong)targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp < GetExpForLevel(MaxDualJobLevel, LevelType.DUALJ))
                        targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp += (ulong)jexp;
                }
                /*  if ((ulong)targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp < GetExpForLevel(MaxDualJobLevel, LevelType.DUALJ))
                      targetPC.PlayerDualJobList[targetPC.DualJobID].DualJobExp += (ulong)jexp;*/
            }
            eh.Client.SendEXPMessage((long)cexp, (long)jexp, 0, Packets.Server.SSMG_PLAYER_EXP_MESSAGE.EXP_MESSAGE_TYPE.TamaireGain);
            if (eh.Client.state != MapClient.SESSION_STATE.DISCONNECTED)
            {
                eh.Client.SendEXP();
            }
            if (targetPC.Race != PC_RACE.DEM)
            {
                if (!targetPC.Rebirth)
                    CheckExp(eh.Client, LevelType.CLEVEL);
                else
                    CheckExp(eh.Client, LevelType.CLEVEL2);
                if (targetPC.JobJoint == PC_JOB.NONE)
                {
                    if (targetPC.Job == targetPC.JobBasic)
                        CheckExp(eh.Client, LevelType.JLEVEL);
                    else if (!targetPC.Rebirth)
                        CheckExp(eh.Client, LevelType.JLEVEL2);
                    else if (targetPC.Rebirth && targetPC.DualJobID == 0)
                        CheckExp(eh.Client, LevelType.JLEVEL3);
                    else if (targetPC.Rebirth && targetPC.DualJobID != 0)
                        CheckExp(eh.Client, LevelType.DUALJ);
                }
                else
                    CheckExp(eh.Client, LevelType.JLEVEL2);
            }
        }

        public void ApplyAnotherExp()
        {

        }

        /// <summary>
        /// Check whether input clients experience at the input level type has reached beyond it's current level or not.
        /// If it has, process the new level (update database and inform client), if not, proceed as nothing happened.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="type"></param>
        public void CheckExp(MapClient client, LevelType type, byte when = 1)
        {
            if (client == null)
                return;
            if (client.state != MapClient.SESSION_STATE.LOADING && when == 0)
                return;
            if (client.state != MapClient.SESSION_STATE.LOADED && when == 1)
                return;
            uint lvlDelta = 0;
            switch (type)
            {
                case LevelType.CLEVEL:
                    lvlDelta = this.GetLevelDelta(client.Character.Level, client.Character.CEXP, LevelType.CLEVEL, true);
                    if (lvlDelta > 0)
                        this.SendLevelUp(client, type, lvlDelta);
                    break;
                case LevelType.JLEVEL:
                    lvlDelta = this.GetLevelDelta(client.Character.JobLevel1, client.Character.JEXP, LevelType.JLEVEL, true);
                    if (lvlDelta > 0)
                        this.SendLevelUp(client, type, lvlDelta);
                    break;
                case LevelType.JLEVEL2:
                    if (client.Character.JobJoint == PC_JOB.NONE)
                    {
                        if (client.Character.Job == client.Character.Job2X)
                        {
                            lvlDelta = this.GetLevelDelta(client.Character.JobLevel2X, client.Character.JEXP, LevelType.JLEVEL2, true);
                            if (lvlDelta > 0)
                                this.SendLevelUp(client, type, lvlDelta);
                        }
                        else
                        {
                            lvlDelta = this.GetLevelDelta(client.Character.JobLevel2T, client.Character.JEXP, LevelType.JLEVEL2, true);
                            if (lvlDelta > 0)
                                this.SendLevelUp(client, LevelType.JLEVEL2T, lvlDelta);
                        }
                    }
                    else
                    {
                        lvlDelta = this.GetLevelDelta(client.Character.JointJobLevel, client.Character.JointJEXP, LevelType.JLEVEL2, true);
                        if (lvlDelta > 0)
                            this.SendLevelUp(client, type, lvlDelta);
                    }
                    break;
                case LevelType.CLEVEL2:
                    lvlDelta = this.GetLevelDelta(client.Character.Level, client.Character.CEXP, LevelType.CLEVEL2, true);
                    if (lvlDelta > 0)
                        this.SendLevelUp(client, type, lvlDelta);
                    break;
                case LevelType.JLEVEL3:
                    if (client.Character.JobLevel3 < 50)
                    {
                        lvlDelta = this.GetLevelDelta(client.Character.JobLevel3, client.Character.JEXP, LevelType.JLEVEL3, true);
                        if (lvlDelta > 0)
                            this.SendLevelUp(client, type, lvlDelta);
                    }
                    break;
                case LevelType.DUALJ:
                    if (client.Character.PlayerDualJobList[client.Character.DualJobID].DualJobLevel < 110)
                    {
                        lvlDelta = this.GetLevelDelta(client.Character.PlayerDualJobList[client.Character.DualJobID].DualJobLevel, (ulong)client.Character.PlayerDualJobList[client.Character.DualJobID].DualJobExp, LevelType.DUALJ, true);
                        if (lvlDelta > 0)
                            this.SendLevelUp(client, type, lvlDelta);
                    }
                    break;
                case LevelType.PLEVEL:
                    lvlDelta = this.GetLevelDelta(client.Character.Partner.Level, client.Character.Partner.exp, LevelType.PLEVEL, true);//ECOKEY 寵物經驗更新
                    if (lvlDelta > 0)
                        this.SendLevelUp(client, type, lvlDelta);
                    break;
                case LevelType.PLEVEL2:
                    lvlDelta = this.GetLevelDelta(client.Character.Partner.Level, client.Character.Partner.exp, LevelType.PLEVEL2, true);//ECOKEY 寵物經驗更新
                    if (lvlDelta > 0)
                        this.SendLevelUp(client, type, lvlDelta);
                    break;
            }
        }

        /// <summary>
        /// Get ccumulative experience to get the input level. 
        /// </summary>
        /// <param name="level">The level to get the experience for</param>
        /// <param name="type">The level type to get the experience for</param>
        /// <returns> 
        /// If a non existing type or level is input, the method returns 0.
        /// </returns>
        public ulong GetExpForLevel(uint level, LevelType type)
        {
            //if (this.PCEXPChart.ContainsKey(level))
            if (SagaDB.Experience.PCExperienceFactory.Instance.Items.ContainsKey(level))
            {
                //Level levelData = this.PCEXPChart[level];
                SagaDB.Experience.PCLevel levelData = SagaDB.Experience.PCExperienceFactory.Instance.Items[level];
                switch (type)
                {
                    case LevelType.CLEVEL:
                        if (!PartnerRankEXPChart.ContainsKey((byte)level))
                        {
                            PartnerRankEXPChart.Add((byte)level, levelData.cexp);
                        }
                        return levelData.cexp;
                    case LevelType.CLEVEL2:
                        if (!PartnerLvEXPChart.ContainsKey((byte)level))
                        {
                            PartnerLvEXPChart.Add((byte)level, levelData.cexp2);
                        }
                        return levelData.cexp2;
                    case LevelType.JLEVEL:
                        return levelData.jexp;
                    case LevelType.JLEVEL2:
                    case LevelType.JLEVEL2T:
                        return levelData.jexp2;
                    case LevelType.JLEVEL3:
                        return levelData.jexp3;
                    case LevelType.DUALJ:
                        return levelData.dualjexp;
                    //ECOKEY 寵物經驗新增
                    case LevelType.PLEVEL:
                        return levelData.pexp;
                    case LevelType.PLEVEL2:
                        return levelData.pexp2;
                    default:
                        return uint.MaxValue;

                        // Level newLv = new Level((ulong)LevelType.CLEVEL, (ulong)LevelType.CLEVEL2, (ulong)LevelType.JLEVEL, (ulong)LevelType.JLEVEL2, (ulong)LevelType.JLEVEL3, (ulong)LevelType.DUALJ);
                        //PCEXPChart.Add((byte)level, newLv);
                }
            }
            else
            {
                return uint.MaxValue;
            }
        }

        float CalcLevelDiffReduction(int lvDelta)
        {
            if (lvDelta >= 25 && lvDelta < 30)
                return 0.75f;
            if (lvDelta >= 30 && lvDelta < 35)
                return 0.50f;
            if (lvDelta >= 35 && lvDelta < 40)
                return 0.25f;
            if (lvDelta >= 40)
                return 0.10f;
            return 1f;
        }

        float CalcPartyLevelDiffReduction(int lvDelta)
        {
            if (lvDelta >= 0 && lvDelta < 9)
                return 1.00f;
            if (lvDelta >= 10 && lvDelta < 19)
                return 0.90f;
            if (lvDelta >= 20 && lvDelta < 29)
                return 0.50f;
            if (lvDelta >= 30 && lvDelta < 39)
                return 0.20f;
            if (lvDelta >= 40 && lvDelta < 49)
                return 0.10f;
            if (lvDelta >= 50)
                return 0.01f;
            return 1f;
        }

        float CalcPlayerLevelFactor(byte pclevel)
        {
            return 1f;
        }

        //EcoKey
        public bool PARTNERattack = false; //判斷怪物是不是整隻都寵物打的
        /// <summary>
        /// 处理怪物死亡的经验分配
        /// </summary>
        /// <param name="mob">怪物</param>
        public void ProcessMobExp(ActorMob mob) //6/2/2024
        {
            ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
            ConcurrentDictionary<uint, int> damagetable = eh.AI.DamageTable;//二哈更改1206
            Dictionary<uint, int> damageParty = new Dictionary<uint, int>();
            uint maxHP = mob.MaxHP;

            //New
            foreach (uint k in damagetable.Keys.ToArray())
            {
                if (damagetable.ContainsKey(k) && damagetable[k] > mob.MaxHP)//0518 更改
                {
                    damagetable[k] = (int)mob.MaxHP;
                    break;
                }
            }

            //处理伤害表
            foreach (uint i in damagetable.Keys)
            {
                Actor actor = eh.AI.map.GetActor(i);
                if (actor == null)
                    continue;
                if (!damagetable.ContainsKey(i))//0518 更改
                    continue;
                //ECOKEY 1003更新
                if (actor.type != ActorType.PC)//狗頭 # 如果不是玩家就跳過
                    continue;
                ActorPC pc;

                PARTNERattack = false;
                pc = (ActorPC)actor;
                //ECOKEY 重開註解，改成騎寵專用 1205更新信賴度加倍道具
                if (pc.Pet != null && pc.Pet.Ride == true && pc.Partner != null)
                {
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
                    if (pc.Partner.Owner.TimerItems.ContainsKey("RELIABILITY_BOUNS"))
                    {
                        rexp = rexp * (uint.Parse(pc.TimerItems["RELIABILITY_BOUNS"].BuffValues) / 100);
                    }
                    pc.Partner.reliabilityexp += rexp;
                    MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了" + rexp + "點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
                    //信賴度EXP升信賴度by ECOKEY
                    if (pc.Partner.reliabilityexp >= 2400 && pc.Partner.reliabilityexp <= 11999) //紫 總2,400
                    {
                        pc.Partner.reliability = 1;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp >= 12000 && pc.Partner.reliabilityexp <= 47999) //青 總12,000
                    {
                        pc.Partner.reliability = 2;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp >= 48000 && pc.Partner.reliabilityexp <= 95999) //水 總48,000
                    {
                        pc.Partner.reliability = 3;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp >= 96000 && pc.Partner.reliabilityexp <= 143999)//綠 總96,000
                    {
                        pc.Partner.reliability = 4;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp >= 144000 && pc.Partner.reliabilityexp <= 191999)//黃綠 總144,000
                    {
                        pc.Partner.reliability = 5;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp >= 192000 && pc.Partner.reliabilityexp <= 239999)//黃 總192,000
                    {
                        pc.Partner.reliability = 6;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp >= 240000 && pc.Partner.reliabilityexp <= 359999)//橙 總240,000
                    {
                        pc.Partner.reliability = 7;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp >= 360000 && pc.Partner.reliabilityexp <= 479999) //赤 總360,000
                    {
                        pc.Partner.reliability = 8;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                    if (pc.Partner.reliabilityexp > 480000) // && pc.Partner.reliabilityexp <= 480000) //桃 總480,000
                    {
                        pc.Partner.reliability = 9;
                        MapClient.FromActorPC(pc).SendPetBasicInfo();
                    }
                }


                if (!pc.Online)
                    continue;
                //檢查旅人目錄
                ProcessMonsterGuide(mob.MobID, pc);
                //ECOKEY 爆炸後更新
                if (damagetable[i] > mob.MaxHP)//0518 更改
                    damagetable[i] = (int)mob.MaxHP;

                if (pc.Party == null)//如果不屬於任何團隊
                {
                    int lvDelta = Math.Abs(pc.Level - mob.BaseData.level);
                    if (!pc.Buff.Dead)
                    {
                        ApplyExp(pc, mob, ((float)CalcPlayerLevelFactor(pc.Level) * (damagetable[i]) / maxHP) * CalcLevelDiffReduction(lvDelta));

                    }
                }
                else
                {
                    //計算團隊總傷害
                    if (damageParty.ContainsKey(pc.Party.ID))
                        damageParty[pc.Party.ID] += damagetable[i];
                    else
                        damageParty.Add(pc.Party.ID, damagetable[i]);
                    if (damageParty[pc.Party.ID] > mob.MaxHP)
                        damageParty[pc.Party.ID] = (int)mob.MaxHP;
                }
            }

            //處理團隊經驗分配
            foreach (uint i in damageParty.Keys)
            {
                Party party = PartyManager.Instance.GetParty(i);
                List<ActorPC> validPC = new List<ActorPC>();
                //計算有效成員
                foreach (ActorPC pc in party.Members.Values)
                {
                    if (!pc.Online || pc.Buff.Dead)
                        continue;
                    if (pc.MapID != mob.MapID)
                        continue;
                    if (Math.Abs(mob.X - pc.X) > 1200 || Math.Abs(mob.Y - pc.Y) > 1200)
                        continue;
                    validPC.Add(pc);
                }
                // 如果有效成員數為0或者玩家不屬於任何團隊，對怪物造成零傷害
                if (validPC.Count == 0)
                    continue;
                float bonus;
                //計算成員人數經驗總量
                if (validPC.Count > 1)
                    bonus = 1.2f + (0.3f * validPC.Count);
                else
                    bonus = 1f;
                //計算團隊傷害比重
                if (damageParty[i] < 0)
                {
                    Logger.ShowInfo("fix damage party value low than 0");
                    damageParty[i] = 0;
                }
                bonus = bonus * ((float)(damageParty[i]) / maxHP);
                //隊伍等級差計算
                uint maxlv = 0;
                uint minlv = 150;
                uint totallevel = 0;
                foreach (ActorPC pc in validPC)
                {
                    if (pc.Level > maxlv)
                        maxlv = pc.Level;
                    if (pc.Level < minlv)
                        minlv = pc.Level;
                    totallevel += pc.Level;
                }

                //計算團隊經驗分配
                uint difference = (uint)Math.Abs((maxlv - minlv));
                if (bonus < 0)
                {
                    Logger.ShowInfo("fix bonus low than 0");
                    bonus = 0;
                }
                if (difference < 10)
                    bonus = bonus * 1f;
                else if (difference >= 10 && difference < 20)
                    bonus = bonus * 0.9f;
                else if (difference >= 20 && difference < 30)
                    bonus = bonus * 0.5f;
                else if (difference >= 30 && difference < 40)
                    bonus = bonus * 0.2f;
                else if (difference >= 40 && difference < 50)
                    bonus = bonus * 0.1f;
                else if (difference >= 50)
                    bonus = bonus * 0.01f;
                //現在經驗獎勵不再除以隊伍人數了
                //bonus = bonus / validPC.Count;
                //分配經驗
                foreach (ActorPC pc in validPC)
                {
                    int lvDelta = Math.Abs(pc.Level - mob.BaseData.level);
                    ApplyExp(pc, mob, CalcPlayerLevelFactor(pc.Level) * bonus * CalcLevelDiffReduction(lvDelta) * ((float)pc.Level / (float)totallevel));
                }
            }
        }

        /*  public void ProcessMobExp(ActorMob mob) //old
             {
                 ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                 Dictionary<uint, int> damagetable = eh.AI.DamageTable;
                 Dictionary<uint, int> damageParty = new Dictionary<uint, int>();
                 uint maxHP = mob.MaxHP;
                 //處理寵物傷害加到主人身上
                 //ECOKEY 0118嘗試優化
                 foreach (uint j in damagetable.Keys.ToArray())
                 {
                     Actor actor = eh.AI.map.GetActor(j);


                     if (actor == null)
                         continue;
                     //ECOKEY 處理召喚物經驗
                     if (actor.type == ActorType.MOB)
                     {
                         ActorMob parA = (ActorMob)actor;
                         if (parA.Owner != null && parA.Owner.type == ActorType.PC)
                         {
                             if (damagetable.ContainsKey(parA.Owner.ActorID))
                             {
                                 // 键已存在，执行更新操作
                                 damagetable[parA.Owner.ActorID] += damagetable[j];
                                 damagetable.Remove(j);
                             }
                             else
                             {
                                 if (parA.Owner.TInt["DogHead"] == 1)//狗頭開著
                                 {
                                     // 键不存在，添加新项目
                                     damagetable.Add(parA.Owner.ActorID, damagetable[j]);
                                     damagetable.Remove(j);
                                     PARTNERattack = false;
                                 }
                                 else
                                 {
                                     damagetable.Remove(j);//移除寵物傷害
                                     PARTNERattack = true;//禁止掉落物
                                 }
                             }
                         }
                     }
                     if (actor.type != ActorType.PARTNER)
                         continue;
                     ActorPartner par = (ActorPartner)actor;
                     //ECOKEY 騎寵處理
                     if (par.Owner.Pet != null && par.Owner.Pet.Ride == true)
                     {
                         //轉移寵物傷害給主人，並移除寵物 
                         if (damagetable.ContainsKey(par.Owner.ActorID))
                         {
                             // 键已存在，执行更新操作
                             damagetable[par.Owner.ActorID] += damagetable[j];
                             damagetable.Remove(j);
                         }
                         else
                         {
                             // 键不存在，添加新项目
                             damagetable.Add(par.Owner.ActorID, damagetable[j]);
                             damagetable.Remove(j);
                         }
                     }
                     //ECOKEY 1003更新
                     //判斷寵物主人是否攻擊
                     if (damagetable.ContainsKey(par.Owner.ActorID))
                     {
                         //轉移寵物傷害給主人，並移除寵物 
                         if (damagetable.ContainsKey(par.Owner.ActorID))
                         {
                             // 键已存在，执行更新操作
                             damagetable[par.Owner.ActorID] += damagetable[j];
                             damagetable.Remove(j);
                             PARTNERattack = false;
                         }
                         else
                         {
                             // 键不存在，添加新项目
                             damagetable.Add(par.Owner.ActorID, damagetable[j]);
                             damagetable.Remove(j);
                             PARTNERattack = false;
                         }
                         //  damagetable.Add(par.Owner.ActorID, damagetable[j]);

                         // damagetable[par.Owner.ActorID] = damagetable[j] + damagetable[par.Owner.ActorID];
                         //  damagetable.Remove(j);
                         //  PARTNERattack = false;
                         break;
                     }
                     else
                     {
                         if (par.Owner.TInt["DogHead"] == 1)//狗頭開著
                         {
                             if (damagetable.ContainsKey(par.Owner.ActorID))
                             {
                                 // 键已存在，执行更新操作
                                 damagetable[par.Owner.ActorID] += damagetable[j];
                                 damagetable.Remove(j);
                                 PARTNERattack = false;
                             }
                             else
                             {
                                 // 键不存在，添加新项目
                                 damagetable.Add(par.Owner.ActorID, damagetable[j]);
                                 damagetable.Remove(j);
                                 PARTNERattack = false;
                             }

                             //轉移寵物傷害給主人，並移除寵物
                             /*  damagetable.Add(par.Owner.ActorID, damagetable[j]);
                               damagetable.Remove(j);
                               PARTNERattack = false;*/
        /*     }
             else
             {
                 damagetable.Remove(j);//移除寵物傷害
                 PARTNERattack = true;//禁止掉落物
                 if (par.Owner.Partner != null)
                 {
                     if (mob.Level >= 1 && mob.Level <= 20) //[lv1-21怪+1信賴度]
                     {
                         par.Owner.Partner.reliabilityexp += 1;
                         MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了1點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                     }
                     if (mob.Level >= 21 && mob.Level <= 40) //[lv21-40怪+2信賴度]
                     {
                         par.Owner.Partner.reliabilityexp += 2;
                         MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了2點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                     }
                     if (mob.Level >= 41 && mob.Level <= 60) //[41-60怪+3信賴度]
                     {
                         par.Owner.Partner.reliabilityexp += 3;
                         MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了3點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                     }
                     if (mob.Level >= 61 && mob.Level <= 80) //[61-80怪+4信賴度]
                     {
                         par.Owner.Partner.reliabilityexp += 4;
                         MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了4點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                     }
                     if (mob.Level >= 81 && mob.Level <= 110) //[81-110怪+5信賴度]
                     {
                         par.Owner.Partner.reliabilityexp += 5;
                         MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了5點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                     }

                     //信賴度EXP升信賴度by ECOKEY
                     if (par.Owner.Partner.reliabilityexp >= 2400 && par.Owner.Partner.reliabilityexp <= 11999) //紫 總2,400
                     {
                         par.Owner.Partner.reliability = 1;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp >= 12000 && par.Owner.Partner.reliabilityexp <= 47999) //青 總12,000
                     {
                         par.Owner.Partner.reliability = 2;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp >= 48000 && par.Owner.Partner.reliabilityexp <= 95999) //水 總48,000
                     {
                         par.Owner.Partner.reliability = 3;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp >= 96000 && par.Owner.Partner.reliabilityexp <= 143999)//綠 總96,000
                     {
                         par.Owner.Partner.reliability = 4;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp >= 144000 && par.Owner.Partner.reliabilityexp <= 191999)//黃綠 總144,000
                     {
                         par.Owner.Partner.reliability = 5;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp >= 192000 && par.Owner.Partner.reliabilityexp <= 239999)//黃 總192,000
                     {
                         par.Owner.Partner.reliability = 6;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp >= 240000 && par.Owner.Partner.reliabilityexp <= 359999)//橙 總240,000
                     {
                         par.Owner.Partner.reliability = 7;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp >= 360000 && par.Owner.Partner.reliabilityexp <= 479999) //赤 總360,000
                     {
                         par.Owner.Partner.reliability = 8;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                     if (par.Owner.Partner.reliabilityexp > 480000)// && par.Owner.Partner.reliabilityexp <= 480000) //桃 總480,000
                     {
                         par.Owner.Partner.reliability = 9;
                         MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                     }
                 }
             }
             break;
         }
     }
     //ECOKEY 0118嘗試優化
     foreach (uint k in damagetable.Keys.ToArray())
     {
         if (damagetable[k] > mob.MaxHP)
         {
             damagetable[k] = (int)mob.MaxHP;
             break;
         }
     }

     //处理伤害表
     foreach (uint i in damagetable.Keys)
     {
         Actor actor = eh.AI.map.GetActor(i);
         if (actor == null)
             continue;
         /*if (actor.type != ActorType.PC && actor.type != ActorType.PARTNER)//狗頭 # 如果不是人也不是寵物就跳過
             continue;*/
        //ECOKEY 1003更新
        /* if (actor.type != ActorType.PC)//狗頭 # 如果不是玩家就跳過
              continue;
          ActorPC pc;

          PARTNERattack = false;
          pc = (ActorPC)actor;
          if (pc.Partner != null)
          {
              if (mob.Level >= 1 && mob.Level <= 20) //[lv1-21怪+1信賴度]
              {
                  pc.Partner.reliabilityexp += 1;
                  MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了1點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
              }
              if (mob.Level >= 21 && mob.Level <= 40) //[lv21-40怪+2信賴度]
              {
                  pc.Partner.reliabilityexp += 2;
                  MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了2點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
              }
              if (mob.Level >= 41 && mob.Level <= 60) //[41-60怪+3信賴度]
              {
                  pc.Partner.reliabilityexp += 3;
                  MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了3點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
              }
              if (mob.Level >= 61 && mob.Level <= 80) //[61-80怪+4信賴度]
              {
                  pc.Partner.reliabilityexp += 4;
                  MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了4點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
              }
              if (mob.Level >= 81 && mob.Level <= 110) //[81-110怪+5信賴度]
              {
                  pc.Partner.reliabilityexp += 5;
                  MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了5點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
              }

              //信賴度EXP升信賴度by ECOKEY
              if (pc.Partner.reliabilityexp >= 2400 && pc.Partner.reliabilityexp <= 11999) //紫 總2,400
              {
                  pc.Partner.reliability = 1;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp >= 12000 && pc.Partner.reliabilityexp <= 47999) //青 總12,000
              {
                  pc.Partner.reliability = 2;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp >= 48000 && pc.Partner.reliabilityexp <= 95999) //水 總48,000
              {
                  pc.Partner.reliability = 3;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp >= 96000 && pc.Partner.reliabilityexp <= 143999)//綠 總96,000
              {
                  pc.Partner.reliability = 4;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp >= 144000 && pc.Partner.reliabilityexp <= 191999)//黃綠 總144,000
              {
                  pc.Partner.reliability = 5;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp >= 192000 && pc.Partner.reliabilityexp <= 239999)//黃 總192,000
              {
                  pc.Partner.reliability = 6;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp >= 240000 && pc.Partner.reliabilityexp <= 359999)//橙 總240,000
              {
                  pc.Partner.reliability = 7;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp >= 360000 && pc.Partner.reliabilityexp <= 479999) //赤 總360,000
              {
                  pc.Partner.reliability = 8;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
              if (pc.Partner.reliabilityexp > 480000) // && pc.Partner.reliabilityexp <= 480000) //桃 總480,000
              {
                  pc.Partner.reliability = 9;
                  MapClient.FromActorPC(pc).SendPetBasicInfo();
              }
          }


          if (!pc.Online)
              continue;
          //檢查旅人目錄
          ProcessMonsterGuide(mob.MobID, pc);
          //ECOKEY 爆炸後更新
          //ECOKEY 修正集合已修改問題
          /*if (damagetable[i] > mob.MaxHP)
              damagetable[i] = (int)mob.MaxHP;*/
        /*     if (pc.Party == null)//如果不屬於任何團隊
              {
                  int lvDelta = Math.Abs(pc.Level - mob.BaseData.level);
                  if (!pc.Buff.Dead)
                  {
                      ApplyExp(pc, mob, ((float)CalcPlayerLevelFactor(pc.Level) * (damagetable[i]) / maxHP) * CalcLevelDiffReduction(lvDelta));

                  }
              }
              else
              {
                  //計算團隊總傷害
                  if (damageParty.ContainsKey(pc.Party.ID))
                      damageParty[pc.Party.ID] += damagetable[i];
                  else
                      damageParty.Add(pc.Party.ID, damagetable[i]);
                  if (damageParty[pc.Party.ID] > mob.MaxHP)
                      damageParty[pc.Party.ID] = (int)mob.MaxHP;
              }
          }

          //處理團隊經驗分配
          foreach (uint i in damageParty.Keys)
          {
              Party party = PartyManager.Instance.GetParty(i);
              List<ActorPC> validPC = new List<ActorPC>();
              //計算有效成員
              foreach (ActorPC pc in party.Members.Values)
              {
                  if (!pc.Online || pc.Buff.Dead)
                      continue;
                  if (pc.MapID != mob.MapID)
                      continue;
                  if (Math.Abs(mob.X - pc.X) > 1200 || Math.Abs(mob.Y - pc.Y) > 1200)
                      continue;
                  validPC.Add(pc);
              }
              // 如果有效成員數為0或者玩家不屬於任何團隊，對怪物造成零傷害
              if (validPC.Count == 0)
                  continue;
              float bonus;
              //計算成員人數經驗總量
              if (validPC.Count > 1)
                  bonus = 1.2f + (0.3f * validPC.Count);
              else
                  bonus = 1f;
              //計算團隊傷害比重
              if (damageParty[i] < 0)
              {
                  Logger.ShowInfo("fix damage party value low than 0");
                  damageParty[i] = 0;
              }
              bonus = bonus * ((float)(damageParty[i]) / maxHP);
              //隊伍等級差計算
              uint maxlv = 0;
              uint minlv = 150;
              uint totallevel = 0;
              foreach (ActorPC pc in validPC)
              {
                  if (pc.Level > maxlv)
                      maxlv = pc.Level;
                  if (pc.Level < minlv)
                      minlv = pc.Level;
                  totallevel += pc.Level;
              }

              //計算團隊經驗分配
              uint difference = (uint)Math.Abs((maxlv - minlv));
              if (bonus < 0)
              {
                  Logger.ShowInfo("fix bonus low than 0");
                  bonus = 0;
              }
              if (difference < 10)
                  bonus = bonus * 1f;
              else if (difference >= 10 && difference < 20)
                  bonus = bonus * 0.9f;
              else if (difference >= 20 && difference < 30)
                  bonus = bonus * 0.5f;
              else if (difference >= 30 && difference < 40)
                  bonus = bonus * 0.2f;
              else if (difference >= 40 && difference < 50)
                  bonus = bonus * 0.1f;
              else if (difference >= 50)
                  bonus = bonus * 0.01f;
              //現在經驗獎勵不再除以隊伍人數了
              //bonus = bonus / validPC.Count;
              //分配經驗
              foreach (ActorPC pc in validPC)
              {
                  int lvDelta = Math.Abs(pc.Level - mob.BaseData.level);
                  ApplyExp(pc, mob, CalcPlayerLevelFactor(pc.Level) * bonus * CalcLevelDiffReduction(lvDelta) * ((float)pc.Level / (float)totallevel));
              }
          }
      }
       */


        /*   public void ProcessMobExp(ActorMob mob)
           {
               ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
               Dictionary<uint, int> damagetable = eh.AI.DamageTable;
               Dictionary<uint, int> damageParty = new Dictionary<uint, int>();
               uint maxHP = mob.MaxHP;
               //處理寵物傷害加到主人身上
               for (int i = 0; i < damagetable.Count; i++) // for (int i = 0; i <= damagetable.Count; i++)
               {
                   foreach (uint j in damagetable.Keys)
                   {
                       Actor actor = eh.AI.map.GetActor(j);
                       if (actor == null)
                           continue;
                       if (actor.type != ActorType.PARTNER)
                           continue;
                       ActorPartner par = (ActorPartner)actor;
                       //ECOKEY 騎寵處理
                       if (par.Owner.Pet != null && par.Owner.Pet.Ride == true)
                       {
                           //轉移寵物傷害給主人，並移除寵物 
                           if (damagetable.ContainsKey(par.Owner.ActorID))
                           {
                               // 键已存在，执行更新操作
                               damagetable[par.Owner.ActorID] += damagetable[j];
                               damagetable.Remove(j);
                           }
                           else
                           {
                               // 键不存在，添加新项目
                               damagetable.Add(par.Owner.ActorID, damagetable[j]);
                               damagetable.Remove(j);
                           }
                       }
                       //ECOKEY 1003更新
                       //判斷寵物主人是否攻擊
                       if (damagetable.ContainsKey(par.Owner.ActorID))
                       {
                           //轉移寵物傷害給主人，並移除寵物 
                           if (damagetable.ContainsKey(par.Owner.ActorID))
                           {
                               // 键已存在，执行更新操作
                               damagetable[par.Owner.ActorID] += damagetable[j];
                               damagetable.Remove(j);
                               PARTNERattack = false;
                           }
                           else
                           {
                               // 键不存在，添加新项目
                               damagetable.Add(par.Owner.ActorID, damagetable[j]);
                               damagetable.Remove(j);
                               PARTNERattack = false;
                           }
                           //  damagetable.Add(par.Owner.ActorID, damagetable[j]);

                           // damagetable[par.Owner.ActorID] = damagetable[j] + damagetable[par.Owner.ActorID];
                         //  damagetable.Remove(j);
                         //  PARTNERattack = false;
                           break;
                       }
                       else
                       {
                           if (par.Owner.TInt["DogHead"] == 1)//狗頭開著
                           {
                               if (damagetable.ContainsKey(par.Owner.ActorID))
                               {
                                   // 键已存在，执行更新操作
                                   damagetable[par.Owner.ActorID] += damagetable[j];
                                   damagetable.Remove(j);
                                   PARTNERattack = false;
                               }
                               else
                               {
                                   // 键不存在，添加新项目
                                   damagetable.Add(par.Owner.ActorID, damagetable[j]);
                                   damagetable.Remove(j);
                                   PARTNERattack = false;
                               }


                           }
                           else
                           {
                               damagetable.Remove(j);//移除寵物傷害
                               PARTNERattack = true;//禁止掉落物
                               if (par.Owner.Partner != null)
                               {
                                   if (mob.Level >= 1 && mob.Level <= 20) //[lv1-21怪+1信賴度]
                               {
                                   par.Owner.Partner.reliabilityexp += 1;
                                   MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了1點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                               }
                               if (mob.Level >= 21 && mob.Level <= 40) //[lv21-40怪+2信賴度]
                               {
                                   par.Owner.Partner.reliabilityexp += 2;
                                   MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了2點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                               }
                               if (mob.Level >= 41 && mob.Level <= 60) //[41-60怪+3信賴度]
                               {
                                   par.Owner.Partner.reliabilityexp += 3;
                                   MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了3點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                               }
                               if (mob.Level >= 61 && mob.Level <= 80) //[61-80怪+4信賴度]
                               {
                                   par.Owner.Partner.reliabilityexp += 4;
                                   MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了4點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                               }
                               if (mob.Level >= 81 && mob.Level <= 110) //[81-110怪+5信賴度]
                               {
                                   par.Owner.Partner.reliabilityexp += 5;
                                   MapClient.FromActorPC(par.Owner).SendSystemMessage("夥伴 " + par.Owner.Partner.Name + " 獲得了5點信賴度.目前信賴度" + par.Owner.Partner.reliabilityexp + "");
                               }

                               //信賴度EXP升信賴度by ECOKEY
                               if (par.Owner.Partner.reliabilityexp >= 2400 && par.Owner.Partner.reliabilityexp <= 11999) //紫 總2,400
                               {
                                   par.Owner.Partner.reliability = 1;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp >= 12000 && par.Owner.Partner.reliabilityexp <= 47999) //青 總12,000
                               {
                                   par.Owner.Partner.reliability = 2;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp >= 48000 && par.Owner.Partner.reliabilityexp <= 95999) //水 總48,000
                               {
                                   par.Owner.Partner.reliability = 3;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp >= 96000 && par.Owner.Partner.reliabilityexp <= 143999)//綠 總96,000
                               {
                                   par.Owner.Partner.reliability = 4;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp >= 144000 && par.Owner.Partner.reliabilityexp <= 191999)//黃綠 總144,000
                               {
                                   par.Owner.Partner.reliability = 5;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp >= 192000 && par.Owner.Partner.reliabilityexp <= 239999)//黃 總192,000
                               {
                                   par.Owner.Partner.reliability = 6;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp >= 240000 && par.Owner.Partner.reliabilityexp <= 359999)//橙 總240,000
                               {
                                   par.Owner.Partner.reliability = 7;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp >= 360000 && par.Owner.Partner.reliabilityexp <= 479999) //赤 總360,000
                               {
                                   par.Owner.Partner.reliability = 8;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               if (par.Owner.Partner.reliabilityexp > 480000)// && par.Owner.Partner.reliabilityexp <= 480000) //桃 總480,000
                               {
                                   par.Owner.Partner.reliability = 9;
                                   MapClient.FromActorPC(par.Owner).SendPetBasicInfo();
                               }
                               }
                           }
                           break;
                       }
                   }
                   //ECOKEY 1003更新
                   //ECOKEY 爆炸後更新
               }
               //ECOKEY 修正集合已修改問題
               foreach (uint k in damagetable.Keys)
                   {
                       if (damagetable[k] > mob.MaxHP)
                       {
                           damagetable[k] = (int)mob.MaxHP;
                           break;
                       }
                   }

                   //处理伤害表
                   foreach (uint i in damagetable.Keys)
               {
                   Actor actor = eh.AI.map.GetActor(i);
                   if (actor == null)
                       continue;

                   //ECOKEY 1003更新
                   if (actor.type != ActorType.PC)//狗頭 # 如果不是玩家就跳過
                       continue;
                   ActorPC pc;

                   PARTNERattack = false;
                   pc = (ActorPC)actor;
                  // if (pc.Partner != null)
                  if (pc.Pet != null && pc.Pet.Ride == true && pc.Partner != null)
                     {
                       if (mob.Level >= 1 && mob.Level <= 20) //[lv1-21怪+1信賴度]
                       {
                           pc.Partner.reliabilityexp += 1;
                           MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了1點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
                       }
                       if (mob.Level >= 21 && mob.Level <= 40) //[lv21-40怪+2信賴度]
                       {
                           pc.Partner.reliabilityexp += 2;
                           MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了2點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
                       }
                       if (mob.Level >= 41 && mob.Level <= 60) //[41-60怪+3信賴度]
                       {
                           pc.Partner.reliabilityexp += 3;
                           MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了3點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
                       }
                       if (mob.Level >= 61 && mob.Level <= 80) //[61-80怪+4信賴度]
                       {
                           pc.Partner.reliabilityexp += 4;
                           MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了4點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
                       }
                       if (mob.Level >= 81 && mob.Level <= 110) //[81-110怪+5信賴度]
                       {
                           pc.Partner.reliabilityexp += 5;
                           MapClient.FromActorPC(pc).SendSystemMessage("夥伴 " + pc.Partner.Name + " 獲得了5點信賴度.目前信賴度" + pc.Partner.reliabilityexp + "");
                       }

                       //信賴度EXP升信賴度by ECOKEY
                       if (pc.Partner.reliabilityexp >= 2400 && pc.Partner.reliabilityexp <= 11999) //紫 總2,400
                       {
                           pc.Partner.reliability = 1;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp >= 12000 && pc.Partner.reliabilityexp <= 47999) //青 總12,000
                       {
                           pc.Partner.reliability = 2;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp >= 48000 && pc.Partner.reliabilityexp <= 95999) //水 總48,000
                       {
                           pc.Partner.reliability = 3;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp >= 96000 && pc.Partner.reliabilityexp <= 143999)//綠 總96,000
                       {
                           pc.Partner.reliability = 4;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp >= 144000 && pc.Partner.reliabilityexp <= 191999)//黃綠 總144,000
                       {
                           pc.Partner.reliability = 5;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp >= 192000 && pc.Partner.reliabilityexp <= 239999)//黃 總192,000
                       {
                           pc.Partner.reliability = 6;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp >= 240000 && pc.Partner.reliabilityexp <= 359999)//橙 總240,000
                       {
                           pc.Partner.reliability = 7;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp >= 360000 && pc.Partner.reliabilityexp <= 479999) //赤 總360,000
                       {
                           pc.Partner.reliability = 8;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                       if (pc.Partner.reliabilityexp > 480000) // && pc.Partner.reliabilityexp <= 480000) //桃 總480,000
                       {
                           pc.Partner.reliability = 9;
                           MapClient.FromActorPC(pc).SendPetBasicInfo();
                       }
                   }


                   if (!pc.Online)
                       continue;
                   //檢查旅人目錄
                   ProcessMonsterGuide(mob.MobID, pc);
                   //ECOKEY 爆炸後更新

                   if (pc.Party == null)//如果不屬於任何團隊
                   {
                       int lvDelta = Math.Abs(pc.Level - mob.BaseData.level);
                       if (!pc.Buff.Dead)
                       {
                           ApplyExp(pc, mob, ((float)CalcPlayerLevelFactor(pc.Level) * (damagetable[i]) / maxHP) * CalcLevelDiffReduction(lvDelta));

                       }
                   }
                   else
                   {
                       //計算團隊總傷害
                       if (damageParty.ContainsKey(pc.Party.ID))
                           damageParty[pc.Party.ID] += damagetable[i];
                       else
                           damageParty.Add(pc.Party.ID, damagetable[i]);
                       if (damageParty[pc.Party.ID] > mob.MaxHP)
                           damageParty[pc.Party.ID] = (int)mob.MaxHP;
                   }
               }

               //處理團隊經驗分配
               foreach (uint i in damageParty.Keys)
               {
                   Party party = PartyManager.Instance.GetParty(i);
                   List<ActorPC> validPC = new List<ActorPC>();
                   //計算有效成員
                   foreach (ActorPC pc in party.Members.Values)
                   {
                       if (!pc.Online || pc.Buff.Dead)
                           continue;
                       if (pc.MapID != mob.MapID)
                           continue;
                       if (Math.Abs(mob.X - pc.X) > 1200 || Math.Abs(mob.Y - pc.Y) > 1200)
                           continue;
                       validPC.Add(pc);
                   }
                   // 如果有效成員數為0或者玩家不屬於任何團隊，對怪物造成零傷害
                   if (validPC.Count == 0)
                       continue;
                   float bonus;
                   //計算成員人數經驗總量
                   if (validPC.Count > 1)
                       bonus = 1.2f + (0.3f * validPC.Count);
                   else
                       bonus = 1f;
                   //計算團隊傷害比重
                   if (damageParty[i] < 0)
                   {
                       Logger.ShowInfo("fix damage party value low than 0");
                       damageParty[i] = 0;
                   }
                   bonus = bonus * ((float)(damageParty[i]) / maxHP);
                   //隊伍等級差計算
                   uint maxlv = 0;
                   uint minlv = 150;
                   uint totallevel = 0;
                   foreach (ActorPC pc in validPC)
                   {
                       if (pc.Level > maxlv)
                           maxlv = pc.Level;
                       if (pc.Level < minlv)
                           minlv = pc.Level;
                       totallevel += pc.Level;
                   }

                   //計算團隊經驗分配
                   uint difference = (uint)Math.Abs((maxlv - minlv));
                   if (bonus < 0)
                   {
                       Logger.ShowInfo("fix bonus low than 0");
                       bonus = 0;
                   }
                   if (difference < 10)
                       bonus = bonus * 1f;
                   else if (difference >= 10 && difference < 20)
                       bonus = bonus * 0.9f;
                   else if (difference >= 20 && difference < 30)
                       bonus = bonus * 0.5f;
                   else if (difference >= 30 && difference < 40)
                       bonus = bonus * 0.2f;
                   else if (difference >= 40 && difference < 50)
                       bonus = bonus * 0.1f;
                   else if (difference >= 50)
                       bonus = bonus * 0.01f;
                   //現在經驗獎勵不再除以隊伍人數了
                   //bonus = bonus / validPC.Count;
                   //分配經驗
                   foreach (ActorPC pc in validPC)
                   {
                       int lvDelta = Math.Abs(pc.Level - mob.BaseData.level);
                       ApplyExp(pc, mob, CalcPlayerLevelFactor(pc.Level) * bonus * CalcLevelDiffReduction(lvDelta) * ((float)pc.Level / (float)totallevel));
                   }
               }
           }*/


        public float CalcPartyMemberExpBonus(int PartyMemberCount, Map map)
        {
            if (map.Info.Flag.Test(SagaDB.Map.MapFlags.AbyssOfEchoChrome))
            {
                return 1.0f;
            }
            else if (map.Info.Flag.Test(SagaDB.Map.MapFlags.EchoChrome))
            {
                return 1.0f;
            }
            else
                return (1.2f + (0.3f * PartyMemberCount));
        }

        public void DeathPenalty(ActorPC pc)
        {
            Map map = MapManager.Instance.GetMap(pc.MapID);
            MapClient client = MapClient.FromActorPC(pc);
            ulong shouldBase = 0;
            ulong shouldJob = 0;

            if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
            {
                if (pc.Race == PC_RACE.DEM)
                {
                    pc.DominionCEXP -= (ulong)(pc.DominionCEXP * Configuration.Instance.DeathPenaltyBaseDominion);
                    pc.DominionJEXP -= (ulong)(pc.DominionJEXP * Configuration.Instance.DeathPenaltyJobDominion);
                    return;
                }
                shouldBase = (ulong)((GetExpForLevel((uint)(pc.DominionLevel + 1), LevelType.CLEVEL) - GetExpForLevel(pc.DominionLevel, LevelType.CLEVEL)) * Configuration.Instance.DeathPenaltyBaseDominion);
                shouldJob = (ulong)((GetExpForLevel((uint)(pc.DominionJobLevel + 1), LevelType.JLEVEL2) - GetExpForLevel(pc.DominionJobLevel, LevelType.JLEVEL2)) * Configuration.Instance.DeathPenaltyJobDominion);
                if (pc.DominionCEXP > shouldBase)
                    pc.DominionCEXP -= shouldBase;
                else
                    pc.DominionCEXP = 0;
                if (pc.DominionJEXP > shouldJob)
                    pc.DominionJEXP -= shouldJob;
                else
                    pc.DominionJEXP = 0;
                if (pc.DominionCEXP < GetExpForLevel(pc.DominionLevel, LevelType.CLEVEL))
                {
                    int shouldStats = pc.DominionLevel / 3 + 3;
                    while (pc.DominionStatsPoint < shouldStats)
                    {
                        List<int> statsHad = new List<int>();
                        if (pc.Str > Configuration.Instance.StartupSetting[pc.Race].Str)
                            statsHad.Add(0);
                        if (pc.Dex > Configuration.Instance.StartupSetting[pc.Race].Dex)
                            statsHad.Add(1);
                        if (pc.Int > Configuration.Instance.StartupSetting[pc.Race].Int)
                            statsHad.Add(2);
                        if (pc.Vit > Configuration.Instance.StartupSetting[pc.Race].Vit)
                            statsHad.Add(3);
                        if (pc.Agi > Configuration.Instance.StartupSetting[pc.Race].Agi)
                            statsHad.Add(4);
                        if (pc.Mag > Configuration.Instance.StartupSetting[pc.Race].Mag)
                            statsHad.Add(5);
                        if (statsHad.Count == 0)
                        {
                            shouldStats = 0;
                            break;
                        }
                        switch (statsHad[Global.Random.Next(0, statsHad.Count - 1)])
                        {
                            case 0:
                                pc.Str--;
                                pc.DominionStatsPoint += PC.StatusFactory.Instance.RequiredBonusPoint(pc.Str);
                                break;
                            case 1:
                                pc.Dex--;
                                pc.DominionStatsPoint += PC.StatusFactory.Instance.RequiredBonusPoint(pc.Dex);
                                break;
                            case 2:
                                pc.Int--;
                                pc.DominionStatsPoint += PC.StatusFactory.Instance.RequiredBonusPoint(pc.Int);
                                break;
                            case 3:
                                pc.Vit--;
                                pc.DominionStatsPoint += PC.StatusFactory.Instance.RequiredBonusPoint(pc.Vit);
                                break;
                            case 4:
                                pc.Agi--;
                                pc.DominionStatsPoint += PC.StatusFactory.Instance.RequiredBonusPoint(pc.Agi);
                                break;
                            case 5:
                                pc.Mag--;
                                pc.DominionStatsPoint += PC.StatusFactory.Instance.RequiredBonusPoint(pc.Mag);
                                break;
                        }
                    }
                    pc.DominionStatsPoint -= (ushort)shouldStats;
                    pc.DominionLevel--;
                }
                if (pc.DominionJEXP < GetExpForLevel(pc.DominionJobLevel, LevelType.JLEVEL2))
                    pc.DominionJobLevel--;
                client.SendEXP();
                client.SendPlayerLevel();
            }
            else
            {
                if (pc.Race == PC_RACE.DEM)
                {
                    pc.CEXP -= (ulong)(pc.CEXP * Configuration.Instance.DeathPenaltyBaseEmil);
                    pc.JEXP -= (ulong)(pc.JEXP * Configuration.Instance.DeathPenaltyJobEmil);
                    return;
                }
                if (pc.Job == pc.JobBasic)
                    shouldJob = (ulong)((GetExpForLevel((uint)(pc.JobLevel1 + 1), LevelType.JLEVEL) - GetExpForLevel(pc.JobLevel1, LevelType.JLEVEL)) * Configuration.Instance.DeathPenaltyJobEmil);
                else if (pc.Job == pc.Job2X)
                    shouldJob = (ulong)((GetExpForLevel((uint)(pc.JobLevel2X + 1), LevelType.JLEVEL2) - GetExpForLevel(pc.JobLevel2X, LevelType.JLEVEL2)) * Configuration.Instance.DeathPenaltyJobEmil);
                else if (pc.Job == pc.Job2T)
                    shouldJob = (ulong)((GetExpForLevel((uint)(pc.JobLevel2T + 1), LevelType.JLEVEL2) - GetExpForLevel(pc.JobLevel2T, LevelType.JLEVEL2)) * Configuration.Instance.DeathPenaltyJobEmil);
                else
                    shouldJob = (ulong)((GetExpForLevel((uint)(pc.JobLevel3 + 1), LevelType.JLEVEL3) - GetExpForLevel(pc.JobLevel3, LevelType.JLEVEL3)) * Configuration.Instance.DeathPenaltyJobEmil);

                if (!pc.Rebirth)
                    shouldBase = (ulong)((GetExpForLevel((uint)(pc.Level + 1), LevelType.CLEVEL) - GetExpForLevel(pc.Level, LevelType.CLEVEL)) * Configuration.Instance.DeathPenaltyBaseEmil);
                else
                    shouldBase = (ulong)((GetExpForLevel((uint)(pc.Level + 1), LevelType.CLEVEL2) - GetExpForLevel(pc.Level, LevelType.CLEVEL2)) * Configuration.Instance.DeathPenaltyBaseEmil);

                if (pc.CEXP > shouldBase)
                    pc.CEXP -= shouldBase;
                else
                    pc.CEXP = 0;
                if (pc.JEXP > shouldJob)
                    pc.JEXP -= shouldJob;
                else
                    pc.JEXP = 0;
                if (!pc.Rebirth)
                {
                    if (pc.CEXP < GetExpForLevel(pc.Level, LevelType.CLEVEL))
                        pc.CEXP = GetExpForLevel(pc.Level, LevelType.CLEVEL);
                }
                else if (pc.CEXP < GetExpForLevel(pc.Level, LevelType.CLEVEL2))
                    pc.CEXP = GetExpForLevel(pc.Level, LevelType.CLEVEL2);


                if (pc.Job == pc.JobBasic)
                {
                    if (pc.JEXP < GetExpForLevel(pc.JobLevel1, LevelType.JLEVEL))
                        pc.JEXP = GetExpForLevel(pc.JobLevel1, LevelType.JLEVEL);
                }
                else if (pc.Job == pc.Job2X)
                {
                    if (pc.JEXP < GetExpForLevel(pc.JobLevel2X, LevelType.JLEVEL2))
                        pc.JEXP = GetExpForLevel(pc.JobLevel2X, LevelType.JLEVEL2);
                }
                else if (pc.Job == pc.Job2T)
                {
                    if (pc.JEXP < GetExpForLevel(pc.JobLevel2T, LevelType.JLEVEL2))
                        pc.JEXP = GetExpForLevel(pc.JobLevel2T, LevelType.JLEVEL2);
                }
                else
                {
                    if (pc.JEXP < GetExpForLevel(pc.JobLevel3, LevelType.JLEVEL3))
                        pc.JEXP = GetExpForLevel(pc.JobLevel3, LevelType.JLEVEL3);
                }

                client.SendEXP();
                client.SendPlayerLevel();
            }
            if (map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
            {
                client.SendSystemMessage(LocalManager.Instance.Strings.DEATH_PENALTY);
            }
        }

        public void ProcessWrp(ActorPC src, ActorPC dst)
        {
            int shouldWrp = 0;
            int srcLv = 1, dstLv = 1;
            Map map = MapManager.Instance.GetMap(dst.MapID);
            srcLv = src.Level;
            dstLv = src.Level;
            if (dstLv > srcLv)
            {
                shouldWrp = (dstLv - srcLv) + 10;
            }
            else
                shouldWrp = 5;
            if (src.Party == null)
            {
                src.WRP += shouldWrp;
            }
            else
            {
                List<ActorPC> validPC = new List<ActorPC>();
                foreach (ActorPC pc in src.Party.Members.Values)
                {
                    if (!pc.Online || pc.Buff.Dead)
                        continue;
                    if (pc.MapID != dst.MapID)
                        continue;
                    if (Math.Abs(dst.X - pc.X) > 1200 || Math.Abs(dst.Y - pc.Y) > 1200)
                        continue;
                    validPC.Add(pc);
                }
                int value = shouldWrp / validPC.Count;
                if (value == 0)
                    value = 1;
                foreach (ActorPC pc in validPC)
                {
                    pc.WRP += value;
                }
            }
            int lostWrp = shouldWrp / 2;
            if (dst.WRP > lostWrp)
                dst.WRP -= lostWrp;
            else
                dst.WRP = 0;
            WRPRankingManager.Instance.UpdateRanking();
        }
        //ECOKEY 攻防戰 - WRP怪物獲取
        public void ProcessWrpMob(ActorPC src, ActorMob dst)
        {
            if (dst.Owner != null && dst.Owner.type == ActorType.PC) return;
            int shouldWrp = 0;
            int dstLv = 1;
            dstLv = dst.Level;
            shouldWrp = dstLv / 5;
            if (SagaMap.Skill.SkillHandler.Instance.CheckMobType(dst, "champ"))
            {
                shouldWrp = 50;
            }
            src.WRP += shouldWrp;
            WRPRankingManager.Instance.UpdateRanking();
        }
        int getPartnerMaxLv(ActorPartner partner)
        {
            int maxlv = PartnerLvMax;
            if (partner.rebirth)
                maxlv += 10;
            return maxlv;
        }
        int getPartnerMaxLv(ActorPC PC)
        {
            ActorPartner partner = PC.Partner;
            return getPartnerMaxLv(partner);
        }

        public void ApplyPartnerLvExp(ActorPC PC, ulong exp)
        {
            MapClient client = MapClient.FromActorPC(PC);
            //client.SendSystemMessage("階段一");
            if (!PC.Inventory.Equipments.ContainsKey(EnumEquipSlot.PET))
            {
                //client.SendSystemMessage("階段二");
                return;
            }
            //client.SendSystemMessage("階段二-+一");
            if (!PC.Inventory.Equipments[EnumEquipSlot.PET].IsPartner)
            {
                //client.SendSystemMessage("階段三");
                return;
            }
            //client.SendSystemMessage("階段四");
            ActorPartner partner = PC.Partner;
            //client.SendSystemMessage("階段五" + partner);
            if (partner == null) return;
            //client.SendSystemMessage("階段六" + partner.Level);
            if (PC.TInt["PartnerExpUPRate"] > 0 && PC.Status.Additions.ContainsKey("PartnerExpUP"))
            {
                float r = PC.TInt["PartnerExpUPRate"] / 100f;
                exp += (ulong)(exp * r);
            }
            int maxlv = getPartnerMaxLv(PC);
            exp /= 10;
            //if (partner.Level >= maxlv) return;
            //client.SendSystemMessage("階段七" + partner.exp);
            partner.exp += exp;
            byte lvup = GetPartnerSpecDelta(partner.Level, partner.exp, 0, (byte)maxlv);
            if (lvup > 0)
            {
                partner.perkpoint += lvup;

                partner.Level += lvup;
                Skill.SkillHandler.Instance.ShowEffectOnActor(partner, 9964);
                Partner.StatusFactory.Instance.CalcPartnerStatus(partner);
                partner.HP = partner.MaxHP;
                partner.MP = partner.MaxMP;
                partner.SP = partner.MaxSP;
                //broadcast status and hpmpsp
                MapServer.charDB.SavePartner(partner);
                MapClient.FromActorPC(PC).SendPetBasicInfo();
                MapClient.FromActorPC(PC).SendPetDetailInfo();

                MapClient.FromActorPC(PC).PartnerTalking(partner, MapClient.TALK_EVENT.LVUP, 100, 0);
            }
            PC.Inventory.Equipments[EnumEquipSlot.PET].PartnerLevel = partner.Level;
            MapClient.FromActorPC(PC).SendItemInfo(PC.Inventory.Equipments[EnumEquipSlot.PET]);
        }

        public void ApplyPartnerFoodEXP(ActorPartner partner, uint foodrankexp) //foodrankexp代表食物提供的伙伴rank經驗值,need test
        {

            int maxlv = getPartnerMaxLv(partner);

            partner.rankexp += foodrankexp; //伙伴的rankexp加或等於foodrankexp這函數
            byte rankup = GetPartnerSpecDelta(partner.rank, partner.rankexp, 1, (byte)maxlv); //ECOKEY,這一大段是伙伴rankuplv 試重寫
            if (rankup > 0)
            {
                partner.rank += rankup; //rankup加或等於目前伙伴ranklv
                partner.perkpoint += rankup; //perkpoint是額外獎勵點,自配寵素質點
                partner.rankexp += rankup; //New
                Packets.Server.SSMG_PARTNER_RANK_UPDATE pr = new Packets.Server.SSMG_PARTNER_RANK_UPDATE();
                pr.PartnerInventorySlot = partner.Owner.Inventory.Equipments[EnumEquipSlot.PET].Slot;
                pr.PartnerRank = partner.rank;
                pr.PerkPoint = partner.perkpoint;
                ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)partner.Owner.e;
                eh.Client.netIO.SendPacket(pr);
                MapServer.charDB.SavePartner(partner);
            }
        }

        public void ApplyPartnerReliabilityEXP(ActorPartner partner, uint reliabilityexp)
        {
            if (partner.reliability >= PartnerReliabilityMax)
            {
                reliabilityexp = 0;
            }
            partner.reliabilityexp += reliabilityexp;
            byte reliabilityup = GetPartnerSpecDelta(partner.reliability, partner.reliabilityexp, 2, 9);
            if (reliabilityup > 0 && partner.reliability < 9)
            {
                partner.reliability += reliabilityup;
                Skill.SkillHandler.Instance.ShowEffectOnActor(partner, 9913);
            }
            MapServer.charDB.SavePartner(partner);
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Send level up packet to client and update database
        /// </summary>
        /// <param name="client">The MapClient</param>
        /// <param name="type">The LevelType that gained level(s)</param>
        /// <param name="numLevels">The number of levels gained</param>
        private void SendLevelUp(MapClient client, LevelType type, uint numLevels)
        {
            byte lvtype = 0;
            if (type == LevelType.CLEVEL || type == LevelType.CLEVEL2)
            {
                if (client.Character.Level < 110)
                {
                    ushort stats = 0;
                    if (client.Character.Race != PC_RACE.DEM)
                    {
                        for (int i = 1; i <= numLevels; i++)
                            stats += (ushort)(Math.Ceiling((float)(client.Character.Level + i) / 3.0f) + 2);

                        client.Character.StatsPoint += stats;
                    }
                    client.Character.Level += (byte)numLevels;
                    lvtype = 1;
                }

            }
            else if (type == LevelType.PLEVEL || type == LevelType.PLEVEL2)//ECOKEY 寵物經驗優化
            {
                client.Character.Partner.Level += (byte)numLevels;
                if (client.Character.Partner.Level + numLevels > client.Character.Level)
                {
                    client.Character.Partner.Level = client.Character.Level;
                    if (client.Character.Partner.exp >= GetExpForLevel(client.Character.Level, LevelType.PLEVEL))//寵物經驗不超過主人
                        client.Character.Partner.exp = GetExpForLevel(client.Character.Level, LevelType.PLEVEL) - 1;//寵物經驗不超過主人
                }
                Skill.SkillHandler.Instance.ShowEffectOnActor(client.Character.Partner, 9964);
                //ECOKEY 騎寵專用升等判斷
                if (client.Character.Pet != null && client.Character.Pet.Ride == true)
                {
                    Skill.SkillHandler.Instance.ShowEffectOnActor(client.Character, 9964);
                }
            }
            else
            {
                if (client.Character.Race != PC_RACE.DEM)
                {
                    if (client.Character.JobJoint == PC_JOB.NONE)
                    {
                        if (type == LevelType.JLEVEL)
                        {
                            client.Character.JobLevel1 += (byte)numLevels;
                            client.Character.SkillPoint += (byte)numLevels;
                        }
                        else if (type == LevelType.JLEVEL2)
                        {
                            client.Character.JobLevel2X += (byte)numLevels;
                            client.Character.SkillPoint2X += (byte)numLevels;
                        }
                        else if (type == LevelType.JLEVEL2T)
                        {
                            client.Character.JobLevel2T += (byte)numLevels;
                            client.Character.SkillPoint2T += (byte)numLevels;
                        }
                        else if (type == LevelType.JLEVEL3)
                        {
                            client.Character.JobLevel3 += (byte)numLevels;
                            client.Character.SkillPoint3 += (byte)numLevels;
                        }
                        else if (type == LevelType.DUALJ)
                        {
                            if (client.Character.PlayerDualJobList[client.Character.DualJobID].DualJobLevel < 110)
                                client.Character.PlayerDualJobList[client.Character.DualJobID].DualJobLevel += (byte)numLevels;
                        }
                        else
                        {
                            client.Character.JointJobLevel += (byte)numLevels;
                        }
                    }
                    else//ECOKEY 馴獸師升等判斷
                    {
                        client.Character.JointJobLevel += (byte)numLevels;
                    }
                }
                lvtype = 2;
            }
            SkillArg arg = new SkillArg();
            arg.x = lvtype;

            client.PartnerTalking(client.Character.Partner, MapClient.TALK_EVENT.LVUP, 100, 0);

            client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.LEVEL_UP, arg, (type == LevelType.PLEVEL || type == LevelType.PLEVEL2) ? client.Character.Partner as Actor : client.Character as Actor, (type == LevelType.PLEVEL || type == LevelType.PLEVEL2) ? false : true);
            if (type != LevelType.PLEVEL && type != LevelType.PLEVEL2)
            {
                PC.StatusFactory.Instance.CalcStatus(client.Character);
                client.Character.HP = client.Character.MaxHP;
                client.Character.MP = client.Character.MaxMP;
                client.Character.SP = client.Character.MaxSP;
                client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, client.Character, false);
                client.SendPlayerInfo();
                client.SendRingMember();
                client.SendPartyInfo();
            }
            else
            {
                client.Character.Partner.HP = client.Character.Partner.MaxHP;
                client.Character.Partner.MP = client.Character.Partner.MaxMP;
                client.Character.Partner.SP = client.Character.Partner.MaxSP;
                Partner.StatusFactory.Instance.CalcPartnerStatus(client.Character.Partner);
                client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, client.Character.Partner, false);
                client.SendPetInfo();
            }
        }

        /// <summary>
        /// Get the number of levels the overflow of exp represents compared to the current level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exp"></param>
        /// <param name="type"></param>
        /// <param name="allowMultilevel"></param>
        private uint GetLevelDelta(uint level, ulong exp, LevelType type, bool allowMultilevel)
        {
            if (level <= this.GetTypeSpecificMaxLevel(type))
                this.currentMax = this.GetTypeSpecificMaxLevel(type) - level;	// Calculate maximum allowed levels to gain from current level
            else
                this.currentMax = 0;

            uint delta = 0;
            for (delta = 0; (allowMultilevel ? true : delta < 2) &&
                delta < this.currentMax &&
                exp > this.GetExpForLevel(level + delta + 1, type);

                delta++)
            {
            }
            // Multilevel constraint
            // Max level constraint
            // Walk the passed levels (note that GetExpForLevel() returns 0 if level is greater than max level, so it's vital that the max levels are synced with the exp chart)
            // Increase level delta
            return delta;
        }

        /// <summary>
        /// Get the max level for the input LevelType
        /// </summary>
        /// <param name="type">The LevelType to get the max level for</param>
        /// <returns>The max level for the input LevelType</returns>
        private uint GetTypeSpecificMaxLevel(LevelType type)
        {
            switch (type)
            {
                case LevelType.CLEVEL: return MaxCLevel;
                case LevelType.JLEVEL: return MaxJLevel;
                case LevelType.JLEVEL2: return MaxJLevel;
                case LevelType.CLEVEL2: return MaxCLevel2;
                case LevelType.JLEVEL3: return MaxJLevel3;
                case LevelType.DUALJ: return MaxDualJobLevel;
                //ECOKEY 寵物經驗新增
                case LevelType.PLEVEL: return MaxCLevel;
                case LevelType.PLEVEL2: return MaxCLevel2;
                default:
                    return 0;
            }
        }

        private float GetPossesionLevelDiffExpFactor(Actor aPossesionor, Actor aPossesioned)
        {
            float result = 1.0f;
            int leveldiff = aPossesionor.Level - aPossesioned.Level;
            if (leveldiff > 100)
                return 0.6f;
            if (leveldiff > 90 && leveldiff <= 100)
                return 0.5f;
            if (leveldiff > 60 && leveldiff <= 90)
                return 0.4f;
            if (leveldiff > 30 && leveldiff <= 60)
                return 0.3f;
            if (leveldiff > -30 && leveldiff <= 30)
                return 0.1f;
            if (leveldiff >= -60 && leveldiff <= -30)
                return 0.005f;
            if (leveldiff >= -90 && leveldiff < -60)
                return 0.001f;
            if (leveldiff >= -99 && leveldiff < -90)
                return 0.0001f;
            if (leveldiff < -100)
                return 0.00001f;
            return result;
        }
        /// <summary>
        /// Get the change in partner lv, rank or reliability
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exp"></param>
        /// <param name="type">0-lv, 1-rank, 2-reliability</param>
        /// <returns></returns>
        /// 
        private byte GetPartnerSpecDelta(byte level, ulong exp, byte type, byte maxlv = 30)
        {
            byte max = 0;
            Dictionary<byte, ulong> expchart = new Dictionary<byte, ulong>();

            switch (type)
            {
                case 0:
                    max = maxlv;
                    expchart = PartnerLvEXPChart;
                    break;
                case 1:
                    max = PartnerRankMax;
                    expchart = PartnerRankEXPChart;
                    break;
                case 2:
                    max = PartnerReliabilityMax;
                    expchart = PartnerReliabilityEXPChart;
                    break;
            }

            byte delta;
            for (delta = 0; delta < this.currentMax; delta++)
            {
                byte nextLevel = (byte)(level + delta + 1);
                if (expchart.ContainsKey(nextLevel) && exp >= expchart[nextLevel])
                {

                }
                else
                {
                    break;
                }
            }

            return delta;
        }

        /*  private byte GetPartnerSpecDelta(byte level, ulong exp, byte type, byte maxlv = 30)
          {
              byte max = 0;
              Dictionary<byte, ulong> expchart = new Dictionary<byte, ulong>();

              switch (type)
              {
                  case 0:
                      max = maxlv;
                      expchart = PartnerLvEXPChart;
                      break;
                  case 1:
                      max = PartnerRankMax;
                      expchart = PartnerRankEXPChart;
                      break;
                  case 2:
                      max = PartnerReliabilityMax;
                      expchart = PartnerReliabilityEXPChart;
                      break;
              }
              if (level <= max)
                  this.currentMax = (uint)(max - level);	// Calculate maximum allowed levels to gain from current level
              else
                  this.currentMax = 0;
              if (level >= 115) //110)
                  currentMax = 0;

              byte delta;
              for (delta = 0; delta < this.currentMax &&								// Max level constraint
                  exp >= expchart[(byte)(level + delta + 1)];		       	// Walk the passed levels (note that GetExpForLevel() returns 0 if level is greater than max level, so it's vital that the max levels are synced with the exp chart)
                  delta++) ;												// Increase level delta

              return delta;
          }*/

        #endregion

        public void ProcessMonsterGuide(uint MobID, ActorPC pc)
        {
            if (!pc.MosterGuide.ContainsKey(MobID))
            {
                pc.MosterGuide.Add(MobID, true);
                MapServer.charDB.SaveMosterGuide(pc, MobID, true);
                MapClient client = MapClient.FromActorPC(pc);
                client.OnNewMosterDiscover(MobID);
            }
            else if (pc.MosterGuide[MobID] == false)
            {
                pc.MosterGuide[MobID] = true;
                MapServer.charDB.SaveMosterGuide(pc, MobID, true);
                MapClient client = MapClient.FromActorPC(pc);
                client.OnNewMosterDiscover(MobID);
            }
        }

        /// <summary>
        /// ECOKEY 新增寵物憑依專用等差經驗計算
        /// </summary>
        float CalcPetLevelDiffExp(int lvDelta)
        {
            if (lvDelta >= -30 && lvDelta < 0)
                return 0.1f;
            if (lvDelta >= -60 && lvDelta < -31)
                return 0.08f;
            if (lvDelta >= -90 && lvDelta < -61)
                return 0.06f;
            if (lvDelta >= -99 && lvDelta < -91)
                return 0.04f;
            if (lvDelta < -100)
                return 0.02f;
            return 0.02f;
        }
    }
}
