using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Actor;
using SagaDB.AnotherBook;
using SagaDB.Skill;
using SagaLib;
using SagaMap;
using SagaMap.Manager;
using SagaMap.Skill;
using SagaMap.Skill.Additions.Global;
using System.Threading;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        DateTime skillDelay = DateTime.Now;
        DateTime attackStamp = DateTime.Now;
        DateTime hackStamp = DateTime.Now;
        DateTime assassinateStamp = DateTime.Now;
        bool AttactFinished = false;
        int hackCount = 0;
#pragma warning disable CS0169 // 从不使用字段“MapClient.lastAttackRandom”
        short lastAttackRandom;
#pragma warning restore CS0169 // 从不使用字段“MapClient.lastAttackRandom”
        short lastCastRandom;
        public List<uint> nextCombo = new List<uint>();
        //技能独立cd列表
        Dictionary<uint, DateTime> SingleCDLst = new Dictionary<uint, DateTime>();
        public DateTime SkillDelay { set { skillDelay = value; } }
        public void OnSkillLvUP(Packets.Client.CSMG_SKILL_LEVEL_UP p)
        {
            Packets.Server.SSMG_SKILL_LEVEL_UP p1 = new SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP();
            ushort skillID = p.SkillID;
            byte type = 0;
            if (SkillFactory.Instance.SkillList(this.Character.JobBasic).ContainsKey(skillID))
                type = 1;
            else if (SkillFactory.Instance.SkillList(this.Character.Job2X).ContainsKey(skillID))
                type = 2;
            else if (SkillFactory.Instance.SkillList(this.Character.Job2T).ContainsKey(skillID))
                type = 3;
            else if (SkillFactory.Instance.SkillList(this.Character.Job3).ContainsKey(skillID))
                type = 4;
            if (type == 0)
            {
                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_NOT_EXIST;
            }
            else
            {
                if (type == 1)
                {
                    if (!this.Character.Skills.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, this.Character.Skills[skillID].Level);
                        if (this.Character.JobLevel1 < skill.JobLv)
                        {
                            this.SendSystemMessage(string.Format("{1} 未达到技能升级等级！需求等级：{0}", skill.JobLv, skill.Name));
                            return;
                        }
                        if (this.Character.SkillPoint < 1)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            if (skill.MaxLevel == 6 && this.Character.Skills[skillID].Level == 5)
                            {
                                this.SendSystemMessage((string.Format("无法直接领悟这项技能的精髓。")));
                                return;
                            }
                            if (this.Character.Skills[skillID].Level == this.Character.Skills[skillID].MaxLevel)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_MAX_LEVEL_EXEED;
                            }
                            else
                            {
                                this.Character.SkillPoint -= 1;
                                this.Character.Skills[skillID] = SkillFactory.Instance.GetSkill(skillID, (byte)(this.Character.Skills[skillID].Level + 1));
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.OK;
                                p1.SkillID = skillID;
                            }
                        }
                    }
                }
                if (type == 2)
                {
                    if (!this.Character.Skills2.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, this.Character.Skills2[skillID].Level);
                        if (this.Character.JobLevel2X < skill.JobLv)
                        {
                            this.SendSystemMessage(string.Format("{1} 未达到技能升级等级！需求等级：{0}", skill.JobLv, skill.Name));
                            return;
                        }
                        if (this.Character.SkillPoint2X < 1)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            if (skill.MaxLevel == 6 && this.Character.Skills[skillID].Level == 5)
                            {
                                this.SendSystemMessage((string.Format("无法直接领悟这项技能的精髓。")));
                                return;
                            }
                            if (this.Character.Skills2[skillID].Level == this.Character.Skills2[skillID].MaxLevel)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_MAX_LEVEL_EXEED;
                            }
                            else
                            {
                                this.Character.SkillPoint2X -= 1;
                                SagaDB.Skill.Skill data = SkillFactory.Instance.GetSkill(skillID, (byte)(this.Character.Skills2[skillID].Level + 1));
                                this.Character.Skills2[skillID] = data;
                                this.Character.Skills2_1[skillID] = data;
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.OK;
                                p1.SkillID = skillID;
                            }
                        }
                    }
                }
                if (type == 3)
                {
                    if (!this.Character.Skills2.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, this.Character.Skills2[skillID].Level);
                        if (this.Character.JobLevel2T < skill.JobLv)
                        {
                            this.SendSystemMessage(string.Format("{1} 未达到技能升级等级！需求等级：{0}", skill.JobLv, skill.Name));
                            return;
                        }
                        if (this.Character.SkillPoint2T < 1)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            if (skill.MaxLevel == 6 && this.Character.Skills[skillID].Level == 5)
                            {
                                this.SendSystemMessage((string.Format("无法直接领悟这项技能的精髓。")));
                                return;
                            }
                            if (this.Character.Skills2[skillID].Level == this.Character.Skills2[skillID].MaxLevel)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_MAX_LEVEL_EXEED;
                            }
                            else
                            {
                                this.Character.SkillPoint2T -= 1;
                                SagaDB.Skill.Skill data = SkillFactory.Instance.GetSkill(skillID, (byte)(this.Character.Skills2[skillID].Level + 1));
                                this.Character.Skills2[skillID] = data;
                                this.Character.Skills2_2[skillID] = data;
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.OK;
                                p1.SkillID = skillID;
                            }
                        }
                    }
                }
                if (type == 4)
                {
                    if (!this.Character.Skills3.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, this.Character.Skills3[skillID].Level);
                        if (this.Character.JobLevel3 < skill.JobLv)
                        {
                            this.SendSystemMessage(string.Format("{1} 未达到技能升级等级！需求等级：{0}", skill.JobLv, skill.Name));
                            return;
                        }
                        if (this.Character.SkillPoint3 < 1)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            if (skill.MaxLevel == 6 && this.Character.Skills[skillID].Level == 5)
                            {
                                this.SendSystemMessage((string.Format("无法直接领悟这项技能的精髓。")));
                                return;
                            }
                            if (this.Character.Skills3[skillID].Level == this.Character.Skills3[skillID].MaxLevel)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.SKILL_MAX_LEVEL_EXEED;
                            }
                            else
                            {
                                this.Character.SkillPoint3 -= 1;
                                this.Character.Skills3[skillID] = SkillFactory.Instance.GetSkill(skillID, (byte)(this.Character.Skills3[skillID].Level + 1));
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEVEL_UP.LearnResult.OK;
                                p1.SkillID = skillID;
                            }
                        }
                    }
                }
            }

            p1.SkillPoints = this.Character.SkillPoint;
            if (this.Character.Job == this.Character.Job2X)
            {
                p1.SkillPoints2 = this.Character.SkillPoint2X;
                p1.Job = 1;
            }
            else if (this.Character.Job == this.Character.Job2T)
            {
                p1.SkillPoints2 = this.Character.SkillPoint2T;
                p1.Job = 2;
            }
            else if (this.Character.Job == this.Character.Job3)
            {
                p1.SkillPoints2 = this.Character.SkillPoint3;
                p1.Job = 3;
            }
            else
            {
                p1.Job = 0;
            }
            this.netIO.SendPacket(p1);
            SendSkillList();

            SkillHandler.Instance.CastPassiveSkills(this.Character);
            SendPlayerInfo();

            MapServer.charDB.SaveChar(this.Character, true);
        }

        public void OnSkillLearn(Packets.Client.CSMG_SKILL_LEARN p)
        {
            Packets.Server.SSMG_SKILL_LEARN p1 = new SagaMap.Packets.Server.SSMG_SKILL_LEARN();
            ushort skillID = p.SkillID;
            byte type = 0;
            if (SkillFactory.Instance.SkillList(this.Character.JobBasic).ContainsKey(skillID))
                type = 1;
            else if (SkillFactory.Instance.SkillList(this.Character.Job2X).ContainsKey(skillID))
                type = 2;
            else if (SkillFactory.Instance.SkillList(this.Character.Job2T).ContainsKey(skillID))
                type = 3;
            else if (SkillFactory.Instance.SkillList(this.Character.Job3).ContainsKey(skillID))
                type = 4;
            if (type == 0)
            {
                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.SKILL_NOT_EXIST;
            }
            else
            {
                if (type == 1)
                {
                    byte jobLV = SkillFactory.Instance.SkillList(this.Character.JobBasic)[skillID];
                    if (this.Character.Skills.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        if (this.Character.SkillPoint < 3)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, 1);
                            if (skill.JobLv != 0)
                                jobLV = skill.JobLv;

                            if (this.Character.JobLevel1 < jobLV)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_JOB_LEVEL;
                            }

                            else
                            {
                                this.Character.SkillPoint -= 3;
                                this.Character.Skills.Add(skillID, skill);
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.OK;
                                p1.SkillID = skillID;
                                p1.SkillPoints = this.Character.SkillPoint;
                                if (this.Character.Job == this.Character.Job2X)
                                    p1.SkillPoints2 = this.Character.SkillPoint2X;
                                else if (this.Character.Job == this.Character.Job2T)
                                    p1.SkillPoints2 = this.Character.SkillPoint2T;
                            }
                        }
                    }
                }
                else if (type == 2)
                {
                    byte jobLV = SkillFactory.Instance.SkillList(this.Character.Job2X)[skillID];
                    if (this.Character.Skills2.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        if (this.Character.SkillPoint2X < 3)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, 1);
                            if (skill.JobLv != 0)
                                jobLV = skill.JobLv;
                            if (this.Character.JobLevel2X < jobLV)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_JOB_LEVEL;
                            }
                            else
                            {
                                this.Character.SkillPoint2X -= 3;
                                this.Character.Skills2.Add(skillID, skill);
                                this.Character.Skills2_1.Add(skillID, skill);
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.OK;
                                p1.SkillID = skillID;
                                p1.SkillPoints = this.Character.SkillPoint;
                                p1.SkillPoints2 = this.Character.SkillPoint2X;
                            }
                        }
                    }
                }
                else if (type == 3)
                {
                    byte jobLV = SkillFactory.Instance.SkillList(this.Character.Job2T)[skillID];

                    if (this.Character.Skills2.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        if (this.Character.SkillPoint2T < 3)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, 1);
                            if (skill.JobLv != 0)
                                jobLV = skill.JobLv;
                            if (this.Character.JobLevel2T < jobLV)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_JOB_LEVEL;
                            }
                            else
                            {
                                this.Character.SkillPoint2T -= 3;
                                this.Character.Skills2.Add(skillID, skill);
                                this.Character.Skills2_2.Add(skillID, skill);
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.OK;
                                p1.SkillID = skillID;
                                p1.SkillPoints = this.Character.SkillPoint;
                                p1.SkillPoints2 = this.Character.SkillPoint2T;
                            }
                        }
                    }
                }
                else if (type == 4)
                {
                    byte jobLV = SkillFactory.Instance.SkillList(this.Character.Job3)[skillID];

                    if (this.Character.Skills3.ContainsKey((uint)skillID))
                    {
                        p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.SKILL_NOT_LEARNED;
                    }
                    else
                    {
                        if (this.Character.SkillPoint3 < 3)
                        {
                            p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_SKILL_POINT;
                        }
                        else
                        {
                            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillID, 1);
                            if (skill.JobLv != 0)
                                jobLV = skill.JobLv;
                            if (this.Character.JobLevel3 < jobLV)
                            {
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.NOT_ENOUGH_JOB_LEVEL;
                            }
                            else
                            {
                                this.Character.SkillPoint3 -= 3;
                                this.Character.Skills3.Add(skillID, skill);
                                p1.Result = SagaMap.Packets.Server.SSMG_SKILL_LEARN.LearnResult.OK;
                                p1.SkillID = skillID;
                                p1.SkillPoints = this.Character.SkillPoint;
                                p1.SkillPoints2 = this.Character.SkillPoint3;
                            }
                        }
                    }
                }
            }

            this.netIO.SendPacket(p1);
            SendSkillList();

            SkillHandler.Instance.CastPassiveSkills(this.Character);
            SendPlayerInfo();

            MapServer.charDB.SaveChar(this.Character, true);
        }

        Packets.Client.CSMG_SKILL_ATTACK Lastp;
        int delay;

        Thread main;
        public void OnSkillAttack(Packets.Client.CSMG_SKILL_ATTACK p, bool auto)
        {
            bool needthread = true;

            if (this.Character == null)
                return;
            if (!this.Character.Online || this.Character.HP == 0)
                return;

            Actor dActor = this.Map.GetActor(p.ActorID);
            SkillArg arg;

            Actor sActor = map.GetActor(Character.ActorID);
            if (sActor == null) return;
            if (dActor == null) return;
            if (sActor.MapID != dActor.MapID) return;
            if (sActor.TInt["targetID"] != dActor.ActorID)
            {
                sActor.TInt["targetID"] = (int)dActor.ActorID;
            }

            if (needthread)
            {
                if (!auto && this.Character.AutoAttack)//客户端发来的攻击，但已开启自动
                {
                    Character.TInt["攻击检测"] += 1;
                    if (Character.TInt["攻击检测"] >= 3)
                        ScriptManager.Instance.VariableHolder.AInt[Character.Name + "攻击检测"] += Character.TInt["攻击检测"];
                    Lastp = p;
                    //return;
                }
                if (auto && !this.Character.AutoAttack)//自动攻击，但人物处于不能自动攻击状态
                    return;
            }
            byte s = 0;

            //射程判定
            if (this.Character == null || dActor == null)
                return;
            if (this.Character.Range + 1 < Math.Max(Math.Abs(this.Character.X - dActor.X) / 100, Math.Abs(this.Character.Y - dActor.Y) / 100))
            {
                arg = new SkillArg();
                arg.sActor = this.Character.ActorID;
                arg.type = (ATTACK_TYPE)0xff;
                arg.affectedActors.Add(Character);
                arg.Init();
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);
                this.Character.AutoAttack = false;
                return;
            }
            this.Character.LastAttackActorID = 0;

            //this.lastAttackRandom = p.Random;
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
                this.Character.Status.Additions.Remove("Hiding");
            }
            if (this.Character.Status.Additions.ContainsKey("fish"))
            {
                this.Character.Status.Additions["fish"].AdditionEnd();
                this.Character.Status.Additions.Remove("fish");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
                this.Character.Status.Additions.Remove("Cloaking");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }

            if (this.Character.PossessionTarget != 0)
            {
                var act = Map.GetActor(this.Character.PossessionTarget);
                if (act is ActorPC)
                {
                    act = act as ActorPC;
                    if (act.Status.Additions.ContainsKey("Meditatioon"))
                    {
                        act.Status.Additions["Meditatioon"].AdditionEnd();
                        act.Status.Additions.Remove("Meditatioon");
                    }
                    if (act.Status.Additions.ContainsKey("Hiding"))
                    {
                        act.Status.Additions["Hiding"].AdditionEnd();
                        act.Status.Additions.Remove("Hiding");
                    }
                    if (act.Status.Additions.ContainsKey("fish"))
                    {
                        act.Status.Additions["fish"].AdditionEnd();
                        act.Status.Additions.Remove("fish");
                    }
                    if (act.Status.Additions.ContainsKey("Cloaking"))
                    {
                        act.Status.Additions["Cloaking"].AdditionEnd();
                        act.Status.Additions.Remove("Cloaking");
                    }
                    if (act.Status.Additions.ContainsKey("IAmTree"))
                    {
                        act.Status.Additions["IAmTree"].AdditionEnd();
                        act.Status.Additions.Remove("IAmTree");
                    }
                    if (act.Status.Additions.ContainsKey("Invisible"))
                    {
                        act.Status.Additions["Invisible"].AdditionEnd();
                        act.Status.Additions.Remove("Invisible");
                    }
                }
            }

            if (this.Character.Status.Additions.ContainsKey("Stun") || this.Character.Status.Additions.ContainsKey("Sleep") || this.Character.Status.Additions.ContainsKey("Frosen") || this.Character.Status.Additions.ContainsKey("Stone"))
                return;
            if (dActor == null || DateTime.Now < attackStamp)
            {
                if (s == 1)
                {
                    arg = new SkillArg();
                    arg.sActor = this.Character.ActorID;
                    arg.type = (ATTACK_TYPE)0xff;
                    arg.affectedActors.Add(this.Character);
                    arg.Init();
                    this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);
                    this.Character.AutoAttack = false;
                    return;
                }
                else
                {
                    arg = new SkillArg();
                    arg.sActor = this.Character.ActorID;
                    arg.type = (ATTACK_TYPE)0xff;
                    arg.affectedActors.Add(this.Character);
                    arg.Init();
                    this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);
                    this.Character.AutoAttack = false;
                    return;
                }
            }
            if (dActor.HP == 0 || dActor.Buff.Dead)
            {
                arg = new SkillArg();
                arg.sActor = this.Character.ActorID;
                arg.type = (ATTACK_TYPE)0xff;
                arg.affectedActors.Add(this.Character);
                arg.Init();
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);
                this.Character.AutoAttack = false;
                return;
            }
            arg = new SkillArg();

            delay = (int)((float)2000 * (1.0f - (float)this.Character.Status.aspd / 1000.0f));
            delay = (int)(delay * arg.delayRate);
            if (this.Character.Status.aspd_skill_perc >= 1f)
                delay = (int)(delay / this.Character.Status.aspd_skill_perc);

            if (!needthread && Character.HP > 0)
                SkillHandler.Instance.Attack(this.Character, dActor, arg);//攻击

            if (this.Character.HP > 0 && !AttactFinished && needthread)//处于战斗状态
                SkillHandler.Instance.Attack(this.Character, dActor, arg);//攻击

            if (arg.affectedActors.Count > 0)
                attackStamp = DateTime.Now + new TimeSpan(0, 0, 0, 0, delay);

            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);

            AttactFinished = false;
            PartnerTalking(Character.Partner, TALK_EVENT.BATTLE, 1, 20000);
            //新加
            if (needthread && s == 0)
            {
                Lastp = p;
                this.Character.LastAttackActorID = dActor.ActorID;
                delay = (int)((float)2000 * (1.0f - (float)this.Character.Status.aspd / 1000.0f));
                delay = (int)(delay * arg.delayRate);
                if (this.Character.Status.aspd_skill_perc >= 1f)
                    delay = (int)(delay / this.Character.Status.aspd_skill_perc);

                try
                {
                    if (main != null)
                        ClientManager.RemoveThread(main.Name);
                    if (Character == null)
                        return;
                    if (this == null)
                        return;
                    main = new Thread(MainLoop);
                    main.Name = string.Format("ThreadPoolMainLoopAUTO({0})" + Character.Name, main.ManagedThreadId);
                    ClientManager.AddThread(main);
                    this.Character.AutoAttack = true;
                    main.Start();
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }

            if (sActor.Status.Additions.ContainsKey("EyeballKeeper"))
            {
                if (Global.Random.Next(1, 100) > 95)
                {
                    int skill = Global.Random.Next(1, 5);
                    SkillArg autoheal = new SkillArg();

                    SagaDB.Skill.Skill atskill = null;
                    autoheal.sActor = sActor.ActorID;
                    autoheal.dActor = sActor.ActorID;
                    autoheal.argType = SkillArg.ArgType.Cast;
                    autoheal.useMPSP = false;

                    switch (skill)
                    {
                        case 1:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3146, 1);
                            break;
                        case 2:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3300, 1);
                            break;
                        case 3:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3313, 1);
                            break;
                        case 4:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3254, 1);
                            break;
                        case 5:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3308, 1);
                            break;
                        default:
                            break;
                    }
                    autoheal.skill = atskill;
                    MapClient.FromActorPC(sActor as ActorPC).OnSkillCastComplete(autoheal);
                }
            }
            if (sActor.Status.Additions.ContainsKey("EyeballRaiders"))
            {
                if (Global.Random.Next(1, 100) > 95)
                {
                    int skill = Global.Random.Next(1, 5);
                    SkillArg autoheal = new SkillArg();

                    SagaDB.Skill.Skill atskill = null;
                    autoheal.sActor = sActor.ActorID;
                    autoheal.dActor = sActor.ActorID;
                    autoheal.argType = SkillArg.ArgType.Cast;
                    autoheal.useMPSP = false;
                    Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                    List<Actor> affected = map.GetActorsArea(sActor, 200, false);
                    switch (skill)
                    {
                        case 1:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3054, 1);
                            break;
                        case 2:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2001, 1);
                            break;
                        case 3:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6653, 1);
                            break;
                        case 4:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6654, 1);
                            break;
                        case 5:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6655, 1);
                            break;
                        default:
                            break;
                    }
                    autoheal.skill = atskill;
                    MapClient.FromActorPC(sActor as ActorPC).OnSkillCastComplete(autoheal);
                }
            }
            if (sActor.Status.Additions.ContainsKey("EyeballUsual"))
            {
                if (Global.Random.Next(1, 100) > 95)
                {
                    int skill = Global.Random.Next(1, 5);
                    SkillArg autoheal = new SkillArg();

                    SagaDB.Skill.Skill atskill = null;
                    autoheal.sActor = sActor.ActorID;
                    autoheal.dActor = sActor.ActorID;
                    autoheal.argType = SkillArg.ArgType.Cast;
                    autoheal.useMPSP = false;

                    switch (skill)
                    {
                        case 1:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3054, 1);
                            break;
                        case 2:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2002, 2);
                            break;
                        case 3:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2004, 2);
                            break;
                        case 4:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6654, 1);
                            break;
                        case 5:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3300, 1);
                            break;
                        default:
                            break;
                    }
                    autoheal.skill = atskill;
                    MapClient.FromActorPC(sActor as ActorPC).OnSkillCastComplete(autoheal);
                }
            }
        }

        //12.20ECOKEY
        /*    public void OnSkillAttack(Packets.Client.CSMG_SKILL_ATTACK p, bool auto)
             {
              try
              {
                  if (Character.Tasks.ContainsKey("SkillCast"))
                  throw new Exception("正在吟唱其他技能");
              bool needthread = true;
              if (Character == null || !Character.Online || Character.HP == 0)
                  throw new Exception("角色狀態異常");
              Actor dActor = Map.GetActor(p.ActorID);
              SkillArg arg;
              if (dActor.type == ActorType.PC && ((ActorPC)dActor).Mode != PlayerMode.COLISEUM_MODE)
                  throw new Exception("非PVP狀態玩家目標");
              Actor sActor = map.GetActor(Character.ActorID);
              if (sActor == null || dActor == null)
                  throw new Exception("角色狀態異常");
              if (sActor.MapID != dActor.MapID)
                  throw new Exception("不同地圖");
              if (sActor.TInt["targetID"] != dActor.ActorID)
              {
                  sActor.TInt["targetID"] = (int)dActor.ActorID;
                 // SendSystemMessage("鎖定了【" + dActor.Name + "】作為目標");
              }

              if (needthread)
              {
                  if (!auto && Character.AutoAttack)//客戶端發出的攻擊，但已開啟自動
                  {
                      Character.TInt["攻擊偵測"] += 1;
                      if (Character.TInt["攻擊偵測"] >= 3)
                          ScriptManager.Instance.VariableHolder.AInt[Character.Name + "攻擊偵測"] += Character.TInt["攻擊偵測"];
                      Lastp = p;

                          return;
                  }
                  if (auto && !Character.AutoAttack)//自動攻擊，但人物處於不能自動攻擊狀態
                      throw new Exception("無法進行自動攻擊");
                  }
              byte s = 0;

              //射程判定
              if (Character == null || dActor == null)
                  throw new Exception("角色狀態異常");
              if (Character.Range + 1
                  < Math.Max(Math.Abs(Character.X - dActor.X) / 100
                  , Math.Abs(Character.Y - dActor.Y) / 100))
              {
                  arg = new SkillArg();
                  arg.sActor = Character.ActorID;
                  arg.type = (ATTACK_TYPE)0xff;
                  arg.affectedActors.Add(Character);
                  arg.Init();
                  Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, Character, true);
                  Character.AutoAttack = false;
                  return;
              }
              Character.LastAttackActorID = 0;

              //this.lastAttackRandom = p.Random;
              if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
                 {
                     this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                     this.Character.Status.Additions.Remove("Meditatioon");
                 }
                 if (this.Character.Status.Additions.ContainsKey("Hiding"))
                 {
                     this.Character.Status.Additions["Hiding"].AdditionEnd();
                     this.Character.Status.Additions.Remove("Hiding");
                 }
                 if (this.Character.Status.Additions.ContainsKey("fish"))
                 {
                     this.Character.Status.Additions["fish"].AdditionEnd();
                     this.Character.Status.Additions.Remove("fish");
                 }
                 if (this.Character.Status.Additions.ContainsKey("IAmTree"))
                 {
                     this.Character.Status.Additions["IAmTree"].AdditionEnd();
                     this.Character.Status.Additions.Remove("IAmTree");
                 }
                 if (this.Character.Status.Additions.ContainsKey("Cloaking"))
                 {
                     this.Character.Status.Additions["Cloaking"].AdditionEnd();
                     this.Character.Status.Additions.Remove("Cloaking");
                 }
                 if (this.Character.Status.Additions.ContainsKey("Invisible"))
                 {
                     this.Character.Status.Additions["Invisible"].AdditionEnd();
                     this.Character.Status.Additions.Remove("Invisible");
                 }

                 if (this.Character.PossessionTarget != 0)
                 {
                     var act = Map.GetActor(this.Character.PossessionTarget);
                     if (act is ActorPC)
                     {
                         act = act as ActorPC;
                         if (act.Status.Additions.ContainsKey("Meditatioon"))
                         {
                             act.Status.Additions["Meditatioon"].AdditionEnd();
                             act.Status.Additions.Remove("Meditatioon");
                         }
                         if (act.Status.Additions.ContainsKey("Hiding"))
                         {
                             act.Status.Additions["Hiding"].AdditionEnd();
                             act.Status.Additions.Remove("Hiding");
                         }
                         if (act.Status.Additions.ContainsKey("fish"))
                         {
                             act.Status.Additions["fish"].AdditionEnd();
                             act.Status.Additions.Remove("fish");
                         }
                         if (act.Status.Additions.ContainsKey("Cloaking"))
                         {
                             act.Status.Additions["Cloaking"].AdditionEnd();
                             act.Status.Additions.Remove("Cloaking");
                         }
                         if (act.Status.Additions.ContainsKey("IAmTree"))
                         {
                             act.Status.Additions["IAmTree"].AdditionEnd();
                             act.Status.Additions.Remove("IAmTree");
                         }
                         if (act.Status.Additions.ContainsKey("Invisible"))
                         {
                             act.Status.Additions["Invisible"].AdditionEnd();
                             act.Status.Additions.Remove("Invisible");
                         }
                     }
                 }

                 if (this.Character.Status.Additions.ContainsKey("Stun") || this.Character.Status.Additions.ContainsKey("Sleep") || this.Character.Status.Additions.ContainsKey("Frosen") || this.Character.Status.Additions.ContainsKey("Stone"))
                     return;
              //new
              /*  if (dActor == null || DateTime.Now + new TimeSpan(0, 0, 0, 0, 100) < attackStamp )
              return;
              if (dActor.HP == 0 || dActor.Buff.Dead)
              {
                  Character.AutoAttack = false;
                  return;
              }*/
        //old
        /*   if (dActor == null || DateTime.Now < attackStamp)
              {
                  if (s == 1)
                  {
                      arg = new SkillArg();
                      arg.sActor = this.Character.ActorID;
                      arg.type = (ATTACK_TYPE)0xff;
                      arg.affectedActors.Add(this.Character);
                      arg.Init();
                      this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);
                      this.Character.AutoAttack = false;
                      return;
                  }
                  else
                  {
                      arg = new SkillArg();
                      arg.sActor = this.Character.ActorID;
                      arg.type = (ATTACK_TYPE)0xff;
                      arg.affectedActors.Add(this.Character);
                      arg.Init();
                      this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);
                      this.Character.AutoAttack = false;
                      return;
                  }
              }
              if (dActor.HP == 0 || dActor.Buff.Dead)
              {
                  arg = new SkillArg();
                  arg.sActor = this.Character.ActorID;
                  arg.type = (ATTACK_TYPE)0xff;
                  arg.affectedActors.Add(this.Character);
                  arg.Init();
                  this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, this.Character, true);
                  this.Character.AutoAttack = false;
                  return;
              }
              arg = new SkillArg();

              delay = (int)((float)2000 * (1.0f - (float)this.Character.Status.aspd / 1000.0f));
              delay = (int)(delay * arg.delayRate);
              if (this.Character.Status.aspd_skill_perc >= 1f)
                  delay = (int)(delay / this.Character.Status.aspd_skill_perc);

            //  if (!needthread && Character.HP > 0)
            //      SkillHandler.Instance.Attack(this.Character, dActor, arg);//攻击

           if (Character.HP > 0 && !AttactFinished && needthread && auto)//處於戰鬥狀態
               SkillHandler.Instance.Attack(Character, dActor, arg);//攻擊
           if (arg.affectedActors.Count > 0)
               attackStamp = DateTime.Now + new TimeSpan(0, 0, 0, 0, delay);
           Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, Character, true);
           AttactFinished = false;
           PartnerTalking(Character.Partner, TALK_EVENT.BATTLE, 1, 20000);

           if (needthread)
           {
               Lastp = p;
               Character.LastAttackActorID = dActor.ActorID;
               try
               {
                   if (Character == null) return;
                   if (this == null) return;
                   if (auto) return;
                   int duetime = delay;
                   if (DateTime.Now > attackStamp)
                       duetime = 0;
                   if (!Character.Tasks.ContainsKey("自動攻擊執行緒"))
                   {
                       Character.AutoAttack = true;
                       Tasks.PC.AutoAttack task = new Tasks.PC.AutoAttack(this, duetime, delay, Lastp);
                       Character.Tasks.Add("自動攻擊執行緒", task);
                       task.Activate();
                   }
               }
               catch (Exception ex)
               {
                   Logger.ShowError(ex);
               }
           }*/

        //新加
        /*   if (needthread && s == 0)
              {
                  Lastp = p;
                  this.Character.LastAttackActorID = dActor.ActorID;
                  delay = (int)((float)2000 * (1.0f - (float)this.Character.Status.aspd / 1000.0f));
                  delay = (int)(delay * arg.delayRate);
                  if (this.Character.Status.aspd_skill_perc >= 1f)
                      delay = (int)(delay / this.Character.Status.aspd_skill_perc);

                  try
                  {
                   // 先移除現有的執行緒
                   if (main != null)
                   {
                       ClientManager.RemoveThread(main.Name);
                   }
                   // 先移除現有的執行緒
                   if (main != null && (main.ThreadState == ThreadState.Running || main.ThreadState == ThreadState.Background))
                   {
                       // 在執行緒執行中或已經終止時，不重新啟動
                        ClientManager.RemoveThread(main.Name);
                   }

                   // if (main != null)
                   //   ClientManager.RemoveThread(main.Name);
                   if (Character == null)
                          return;
                      if (this == null)
                          return;
                   // 創建新的執行緒
                   main = new Thread(MainLoop);
                      main.Name = string.Format("ThreadPoolMainLoopAUTO({0})" + Character.Name, main.ManagedThreadId);
                      ClientManager.AddThread(main);
                      this.Character.AutoAttack = true;
                      main.Start();
                  }
                  catch (Exception ex)
                  {
                      Logger.ShowError(ex);
                  }
              }
           // 如果需要，可以加上以下這段檢查ECOKEY
           if (main == null || !main.IsAlive)
           {
               main = new Thread(MainLoop);
               main.Name = string.Format("ThreadPoolMainLoopAUTO({0})" + Character.Name, main.ManagedThreadId);
               ClientManager.AddThread(main);
               this.Character.AutoAttack = true;
               main.Start();
           }*/

        /*   if (sActor.Status.Additions.ContainsKey("EyeballKeeper"))
              {
                  if (Global.Random.Next(1, 100) > 95)
                  {
                      int skill = Global.Random.Next(1, 5);
                      SkillArg autoheal = new SkillArg();

                      SagaDB.Skill.Skill atskill = null;
                      autoheal.sActor = sActor.ActorID;
                      autoheal.dActor = sActor.ActorID;
                      autoheal.argType = SkillArg.ArgType.Cast;
                      autoheal.useMPSP = false;

                      switch (skill)
                      {
                          case 1:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3146, 1);
                              break;
                          case 2:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3300, 1);
                              break;
                          case 3:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3313, 1);
                              break;
                          case 4:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3254, 1);
                              break;
                          case 5:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3308, 1);
                              break;
                          default:
                              break;
                      }
                      autoheal.skill = atskill;
                      MapClient.FromActorPC(sActor as ActorPC).OnSkillCastComplete(autoheal);
                  }
              }
              if (sActor.Status.Additions.ContainsKey("EyeballRaiders"))
              {
                  if (Global.Random.Next(1, 100) > 95)
                  {
                      int skill = Global.Random.Next(1, 5);
                      SkillArg autoheal = new SkillArg();

                      SagaDB.Skill.Skill atskill = null;
                      autoheal.sActor = sActor.ActorID;
                      autoheal.dActor = sActor.ActorID;
                      autoheal.argType = SkillArg.ArgType.Cast;
                      autoheal.useMPSP = false;
                      Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                      List<Actor> affected = map.GetActorsArea(sActor, 200, false);
                      switch (skill)
                      {
                          case 1:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3054, 1);
                              break;
                          case 2:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2001, 1);
                              break;
                          case 3:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6653, 1);
                              break;
                          case 4:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6654, 1);
                              break;
                          case 5:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6655, 1);
                              break;
                          default:
                              break;
                      }
                      autoheal.skill = atskill;
                      MapClient.FromActorPC(sActor as ActorPC).OnSkillCastComplete(autoheal);
                  }
              }
              if (sActor.Status.Additions.ContainsKey("EyeballUsual"))
              {
                  if (Global.Random.Next(1, 100) > 95)
                  {
                      int skill = Global.Random.Next(1, 5);
                      SkillArg autoheal = new SkillArg();

                      SagaDB.Skill.Skill atskill = null;
                      autoheal.sActor = sActor.ActorID;
                      autoheal.dActor = sActor.ActorID;
                      autoheal.argType = SkillArg.ArgType.Cast;
                      autoheal.useMPSP = false;

                      switch (skill)
                      {
                          case 1:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3054, 1);
                              break;
                          case 2:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2002, 2);
                              break;
                          case 3:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2004, 2);
                              break;
                          case 4:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6654, 1);
                              break;
                          case 5:
                              atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3300, 1);
                              break;
                          default:
                              break;
                      }
                      autoheal.skill = atskill;
                      MapClient.FromActorPC(sActor as ActorPC).OnSkillCastComplete(autoheal);
                  }
              }
           }
           catch (Exception ex)
           {
               SkillArg arg = new SkillArg();
               arg.sActor = Character.ActorID;
               arg.type = (ATTACK_TYPE)0xff;
               arg.affectedActors.Add(Character);
               arg.Init();
               Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK, arg, Character, true);
               Character.AutoAttack = false;
               return;
           }
       }*/


        private void MainLoop()
        {
            try
            {
                if (Character == null)
                {
                    if (main != null)
                        ClientManager.RemoveThread(main.Name);
                    return;
                }
                if (this == null)
                    return;

                if (delay <= 0)
                    delay = 60;
                Thread.Sleep(delay);

                if (Character != null)
                {
                    OnSkillAttack(Lastp, true);
                    Character.TInt["攻击检测"] = 0;
                }
                else
                    ClientManager.RemoveThread(main.Name);
            }

            catch (Exception ex)
            {
                Logger.ShowError(main.Name + " Thread " + ex);
                return;
            }
        }


        public void OnSkillChangeBattleStatus(Packets.Client.CSMG_SKILL_CHANGE_BATTLE_STATUS p)
        {
            //ECOKEY 詠唱中禁止移動
            if (this.Character.Tasks.ContainsKey("SkillCast"))
            {
                return;
            }
            if (p.Status == 0)
                this.Character.AutoAttack = false;

            if (this.Character.BattleStatus != p.Status)
            {
                this.Character.BattleStatus = p.Status;
                SendChangeStatus();
            }
            if (this.Character.Tasks.ContainsKey("RangeAttack") && Character.BattleStatus == 0)
            {
                Character.Tasks["RangeAttack"].Deactivate();
                Character.Tasks.Remove("RangeAttack");
                Character.TInt["RangeAttackMark"] = 0;
            }
            if (this.Character.Tasks.ContainsKey("SkillCast") && Character.BattleStatus == 0)
            {
                Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.Character, true);

                Packets.Server.SSMG_SKILL_CAST_RESULT p2 = new SagaMap.Packets.Server.SSMG_SKILL_CAST_RESULT();
                p2.ActorID = Character.ActorID;
                p2.Result = 20;
                this.netIO.SendPacket(p2);
            }
        }

        public void OnSkillCast(Packets.Client.CSMG_SKILL_CAST p)
        {
            OnSkillCast(p, true);
        }

        bool checkSkill(uint skillID, byte skillLV)
        {
            Packets.Server.SSMG_SKILL_LIST p = new SagaMap.Packets.Server.SSMG_SKILL_LIST();
            Dictionary<uint, byte> skills;
            Dictionary<uint, byte> skills2X;
            Dictionary<uint, byte> skills2T;
            Dictionary<uint, byte> skills3;
            Dictionary<uint, byte> skillsHeat;
            List<SagaDB.Skill.Skill> list = new List<SagaDB.Skill.Skill>();
            skills = SkillFactory.Instance.CheckSkillList(this.Character, SkillFactory.SkillPaga.p1);
            skills2X = SkillFactory.Instance.CheckSkillList(this.Character, SkillFactory.SkillPaga.p21);
            skills2T = SkillFactory.Instance.CheckSkillList(this.Character, SkillFactory.SkillPaga.p22);
            skills3 = SkillFactory.Instance.CheckSkillList(this.Character, SkillFactory.SkillPaga.p3);
            skillsHeat = SkillFactory.Instance.CheckSkillList(this.Character, SkillFactory.SkillPaga.none);

            if (this.chara.Skills.ContainsKey(skillID))
            {
                if (this.chara.Skills[skillID].Level >= skillLV)
                    return true;
            }
            if (this.chara.Skills2.ContainsKey(skillID))
            {
                if (this.chara.Skills2[skillID].Level >= skillLV)
                    return true;
            }
            if (this.chara.Skills2_1.ContainsKey(skillID))
            {
                if (this.chara.Skills2_1[skillID].Level >= skillLV)
                    return true;
            }
            if (this.chara.Skills2_2.ContainsKey(skillID))
            {
                if (this.chara.Skills2_2[skillID].Level >= skillLV)
                    return true;
            }
            if (this.chara.Skills2.ContainsKey(skillID))
            {
                if (this.chara.Skills2[skillID].Level >= skillLV)
                    return true;
            }
            if (this.chara.Skills3.ContainsKey(skillID))
            {
                if (this.chara.Skills3[skillID].Level >= skillLV)
                    return true;
            }
            if (this.chara.SkillsReserve.ContainsKey(skillID))
            {
                if (this.chara.SkillsReserve[skillID].Level >= skillLV)
                    return true;
            }
            if (this.chara.SkillsReserve.ContainsKey(skillID) && this.Character.DominionReserveSkill)
            {
                if (this.chara.SkillsReserve[skillID].Level >= skillLV)
                    return true;
            }
            if (this.Character.JobJoint != PC_JOB.NONE)
            {
                {
                    var skill =
                        from c in SkillFactory.Instance.SkillList(this.Character.JobJoint)
                        where c.Value <= this.Character.JointJobLevel
                        select c;
                    foreach (KeyValuePair<uint, byte> i in skill)
                    {
                        if (i.Key == skillID && this.chara.JointJobLevel >= i.Value)
                            return true;
                    }
                }
            }
            return false;
        }

        public void OnSkillCast(Packets.Client.CSMG_SKILL_CAST p, bool useMPSP)
        {
            OnSkillCast(p, useMPSP, false);
        }

        /// <summary>
        /// 检查技能是否符合使用条件
        /// </summary>
        /// <param name="skill">技能数据</param>
        /// <param name="arg">技能参数</param>
        /// <param name="mp">mp</param>
        /// <param name="sp">sp</param>
        /// <param name="ep">ep</param>
        /// <returns>结果</returns>
        private short CheckSkillUse(SagaDB.Skill.Skill skill, SkillArg arg, ushort mp, ushort sp, ushort ep)
        {
            //ECOKEY 暫時禁止騎士團演習使用2-2技能，未來需刪除
            if (this.Character.Mode == PlayerMode.KNIGHT_EAST ||
                    this.Character.Mode == PlayerMode.KNIGHT_WEST ||
                    this.Character.Mode == PlayerMode.KNIGHT_SOUTH ||
                    this.Character.Mode == PlayerMode.KNIGHT_NORTH)
            {
              /*  if (this.Character.Job == this.Character.Job2T)//玩家當前是2-2
                {
                    if (this.Character.Skills2.ContainsKey(skill.ID) || this.Character.Skills2_2.ContainsKey(skill.ID) ||
                    this.Character.MapID == 20080010 ||
                    this.Character.MapID == 20080009 ||
                    this.Character.MapID == 20080008 ||
                    this.Character.MapID == 20080007)
                    {
                        this.SendSystemMessage("暫時禁止騎士團演習使用2-2技能");
                        return -46;
                    }
                }
                if (this.Character.Job == this.Character.Job2X)//玩家當前是2-1
                {
                    if (this.Character.SkillsReserve.ContainsKey(skill.ID) || this.Character.Skills2_2.ContainsKey(skill.ID) ||
                    this.Character.MapID == 20080010 ||
                    this.Character.MapID == 20080009 ||
                    this.Character.MapID == 20080008 ||
                    this.Character.MapID == 20080007)
                    {
                        this.SendSystemMessage("暫時禁止騎士團演習使用2-2技能");
                        return -46;
                    }
                }*/
                if (skill.BaseData.eFlag2 == 7)
                {
                    return -46;
                }
            }

            if (SingleCDLst.ContainsKey(arg.skill.ID) && DateTime.Now < SingleCDLst[arg.skill.ID] && !this.nextCombo.Contains(arg.skill.ID))
                return -30;
            if (arg.skill.ID == 3372)
            {
                SingleCDLst.Clear();
                return 0;
            }
            if (DateTime.Now < skillDelay && !this.nextCombo.Contains(arg.skill.ID))
                return -30;
            if (this.Character.SP < sp || this.Character.MP < mp || this.Character.EP < ep)
            {
                if (this.Character.SP < sp && this.Character.MP < mp)
                    return -1;
                else if (this.Character.SP < sp)
                    return -16;
                else if (this.Character.MP < mp)
                    return -15;
                else
                    return -62;
            }

            if (!SkillHandler.Instance.CheckSkillCanCastForWeapon(this.chara, arg))
                return -5;

            if (this.Character.Status.Additions.ContainsKey("Silence"))
                return -7;

            if (this.Character.Status.Additions.ContainsKey("居合模式"))
            {
                if (arg.skill.ID != 2129)
                    return -7;
                else
                {
                    this.Character.Status.Additions["居合模式"].AdditionEnd();
                    this.Character.Status.Additions.Remove("居合模式");
                }
            }
            if (this.GetPossessionTarget() != null)
            {
                if (this.GetPossessionTarget().Buff.Dead && arg.skill.ID != 3055)
                    return -27;
            }
            if (this.scriptThread != null)
            {
                return -59;
            }
            if (skill.NoPossession)
            {
                if (this.chara.PossessionTarget != 0)
                {
                    return -25;
                }
            }
            if (skill.NotBeenPossessed)
            {
                if (this.chara.PossesionedActors.Count > 0)
                {
                    return -24;
                }
            }
            if (this.Character.Tasks.ContainsKey("SkillCast"))
            {
                if (arg.skill.ID == 3311)
                    return 0;
                else
                    return -8;
            }

            //ECOKEY 混沌封印
            if (this.Character.Status.Additions.ContainsKey("LightSeal") && skill.Magical)
            {
                List<uint> noskill = new List<uint>() { 3054, 3055, 3056, 3073, 3075, 3076, 3078, 3080, 3082, 3146, 3170, 3266 };
                //int[] noskill = { 3054, 3055, 3056, 3073, 3075, 3076, 3078, 3080, 3082, 3146, 3170, 3266 };
                if (noskill.Contains(skill.ID))
                {
                    return -13;
                }
            }
            //ECOKEY CA的封印
            if (this.Character.Buff.MagicSealed && skill.Magical)
                return -13;
            //ECOKEY CA的魔法封印
            if (this.Character.Buff.Sealed && skill.Physical)
                return -13;

            //ECOKEY 騎士團禁止使用
            if (this.Character.MapID == 10023001 ||
                this.Character.MapID == 10032001 ||
                this.Character.MapID == 10034001 ||
                this.Character.MapID == 10042001 ||
                this.Character.MapID == 10056001 ||
                this.Character.MapID == 10064001 ||
                this.Character.MapID == 20020001)
            {
               /* if (this.Character.Skills2_2.ContainsKey(skill.ID))
                {
                    this.SendSystemMessage("暫時禁止騎士團演習使用2-2技能");
                    return -100;
                }*/
                if (skill.BaseData.eFlag2 == 7)
                {
                    return -46;
                }
            }


            short res = 0;
            if (this.Character.Status.Additions.ContainsKey("SkillForbid"))
            {
                res = -7;
                this.SendSystemMessage("目前正处于技能封印状态");
            }
            else
                res = (short)SkillHandler.Instance.TryCast(this.Character, this.Map.GetActor(arg.dActor), arg);
            if (res < 0)
                return res;
            return 0;
        }

        public void OnSkillCast(Packets.Client.CSMG_SKILL_CAST p, bool useMPSP, bool nocheck)
        {
            if (((!checkSkill(p.SkillID, p.SkillLv) && this.chara.Account.GMLevel < 2) ||
                (p.Random == this.lastCastRandom && this.chara.Account.GMLevel < 2)) && !nocheck)
            {
                SendHack();
                if (hackCount > 2)
                    return;
            }

            //断掉自动放技能
            Character.AutoAttack = false;
            if (main != null)
                ClientManager.RemoveThread(main.Name);

            this.lastCastRandom = p.Random;
            SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(p.SkillID, p.SkillLv);
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding") && p.SkillID != 2384)//ECOKEY 新增奇襲判斷
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
                this.Character.Status.Additions.Remove("Hiding");
            }
            if (this.Character.Status.Additions.ContainsKey("fish"))
            {
                this.Character.Status.Additions["fish"].AdditionEnd();
                this.Character.Status.Additions.Remove("fish");
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking") && p.SkillID != 2384)//ECOKEY 新增奇襲判斷
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
                this.Character.Status.Additions.Remove("Cloaking");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }
            if (this.Character.Tasks.ContainsKey("Regeneration"))
            {
                this.Character.Tasks["Regeneration"].Deactivate();
                this.Character.Tasks.Remove("Regeneration");
            }

            SkillArg arg = new SkillArg();
            arg.sActor = this.Character.ActorID;
            arg.dActor = p.ActorID;
            arg.skill = skill;
            arg.x = p.X;
            arg.y = p.Y;
            arg.argType = SkillArg.ArgType.Cast;
            ushort sp, mp, ep;
            //凭依时消耗加倍
            try
            {
                if (this.Character.PossessionTarget != 0)
            {
                sp = (ushort)(skill.SP * 2);
                mp = (ushort)(skill.MP * 2);
            }
            else
            {
                sp = skill.SP;
                mp = skill.MP;
            }
            if (this.Character.Status.Additions.ContainsKey("SwordEaseSp"))
            {
                //sp = (ushort)(skill.SP * 2);
                //mp = (ushort)(skill.MP * 2);
                sp = (ushort)(skill.SP * 0.7);
                //mp = (ushort)(skill.MP * 0.7);
            }
            if (Character.Status.Additions.ContainsKey("元素解放"))
            {
                sp = (ushort)(skill.SP * 2);
                mp = (ushort)(skill.MP * 2);
            }

            if (this.Character.Status.zenList.Contains((ushort)skill.ID) || this.Character.Status.darkZenList.Contains((ushort)skill.ID))
                mp = (ushort)(mp * 2);

            if (this.Character.Status.Additions.ContainsKey("EnergyExcess"))//能量增幅耗蓝加深
            {
                float[] rate = { 0, 0.05f, 0.16f, 0.28f, 0.4f, 0.65f };
                mp += (ushort)(skill.MP * rate[this.Character.TInt["EnergyExcess"]]);
            }
            if (!useMPSP)
            {
                sp = 0;
                mp = 0;
            }
            ep = skill.EP;
            arg.useMPSP = useMPSP;
            //检查技能是否复合使用条件 0为符合, 其他为使用失败
            arg.result = CheckSkillUse(skill, arg, mp, sp, ep);

            if (arg.result == 0)
            {
                //使物理技能的读条时间受aspd影响,法系读条受cspd影响.
                //2018.07.13 现在魔法系职业的读条时间不可能小于0.5秒.
                if (skill.BaseData.flag.Test(SkillFlags.PHYSIC))
                    arg.delay = (uint)((float)skill.CastTime * (1.0f - (float)this.Character.Status.aspd / 1000.0f));
                else
                    arg.delay = (uint)Math.Max(((float)skill.CastTime * (1.0f - (float)this.Character.Status.cspd / 1000.0f)), 500);
                if (arg.skill.ID == 2559)
                {
                    if (this.Character.Gold >= 90000000)
                        arg.delay = (uint)((float)arg.delay * 0.5f);
                    else if (this.Character.Gold >= 50000000)
                        arg.delay = (uint)((float)arg.delay * 0.6f);
                    else if (this.Character.Gold >= 5000000)
                        arg.delay = (uint)((float)arg.delay * 0.7f);
                    else if (this.Character.Gold >= 500000)
                        arg.delay = (uint)((float)arg.delay * 0.8f);
                    else if (this.Character.Gold >= 50000)
                        arg.delay = (uint)((float)arg.delay * 0.9f);
                }

                if (this.Character.Status.delayCancelList.ContainsKey((ushort)arg.skill.ID))
                {
                    int rate = this.Character.Status.delayCancelList[(ushort)arg.skill.ID];
                    arg.delay = (uint)(arg.delay * (1f - ((float)rate / 100.0f)));
                }
                //bool get = Character.Status.Additions.ContainsKey("EaseCt");
                if (Character.Status.Additions.ContainsKey("EaseCt") && arg.skill.ID != 2238)//杀界模块
                {
                    float eclv = new float[] { 0f, 0.5f, 0.7f, 0.8f, 0.9f, 1.0f }[Character.Status.EaseCt_lv];
                    arg.delay = (uint)(arg.delay * (1.0f - eclv));
                    SkillHandler.RemoveAddition(Character, "EaseCt");
                }
                    //ECOKEY 流水之弓
                    if (Character.Status.Additions.ContainsKey("BowCastCancelOne") && arg.skill.BaseData.EquipFlagValue == 128)
                    {
                        float eclv = new float[] { 0f, 0.2f, 0.35f, 0.50f, 0.65f, 0.8f }[Character.Status.EaseCt_lv];
                        arg.delay = (uint)(arg.delay * (1.0f - eclv));
                        SkillHandler.RemoveAddition(Character, "BowCastCancelOne");
                    }
                    //ECOKEY 演奏專精
                    if (Character.Status.Additions.ContainsKey("MusicalDamUp") && arg.skill.BaseData.EquipFlagValue == 196608)
                    {
                        float castlv = new float[] { 0f, 0.1f, 0.25f, 0.25f, 0.4f, 0.4f }[Character.Status.EaseCt_lv];
                        arg.delay = (uint)(arg.delay * (1.0f - castlv));
                    }

                    this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Character, true);

                    if (skill.CastTime > 0 && !this.nextCombo.Contains(arg.skill.ID))
                    {
                        Tasks.PC.SkillCast task = new SagaMap.Tasks.PC.SkillCast(this, arg);
                        this.Character.Tasks.Add("SkillCast", task);

                        //ECOKEY 記錄詠唱的技能是魔法還是物理
                        if (skill.Physical)
                            Character.TInt["skilltype"] = 1;
                        else if (skill.Magical)
                            Character.TInt["skilltype"] = 2;

                        task.Activate();
                        this.nextCombo.Clear(); ;
                    }
                    else
                {
                    this.nextCombo.Clear();
                    OnSkillCastComplete(arg);
                }
                if (this.Character.Status.Additions.ContainsKey("Parry"))
                    arg.delay = (uint)arg.skill.BaseData.delay;
            }
            else
            {
                this.Character.e.OnActorSkillUse(this.Character, arg);
            }
            }
            catch (Exception ex)
            {
                //强行返回-100错误代码，防止卡技能
                arg.result = -100;
                Character.e.OnActorSkillUse(Character, arg);
                Logger.ShowError(ex);
            }
        }

        public void OnSkillCastComplete(SkillArg skill)
        {
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
                this.Character.Status.Additions.Remove("Hiding");
            }
            if (this.Character.Status.Additions.ContainsKey("fish"))
            {
                this.Character.Status.Additions["fish"].AdditionEnd();
                this.Character.Status.Additions.Remove("fish");
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
                this.Character.Status.Additions.Remove("Cloaking");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }

            if (this.Character.PossessionTarget != 0)
            {
                var act = Map.GetActor(this.Character.PossessionTarget);
                if (act is ActorPC)
                {
                    act = act as ActorPC;
                    if (act.Status.Additions.ContainsKey("Meditatioon"))
                    {
                        act.Status.Additions["Meditatioon"].AdditionEnd();
                        act.Status.Additions.Remove("Meditatioon");
                    }
                    if (act.Status.Additions.ContainsKey("Hiding"))
                    {
                        act.Status.Additions["Hiding"].AdditionEnd();
                        act.Status.Additions.Remove("Hiding");
                    }
                    if (act.Status.Additions.ContainsKey("fish"))
                    {
                        act.Status.Additions["fish"].AdditionEnd();
                        act.Status.Additions.Remove("fish");
                    }
                    if (act.Status.Additions.ContainsKey("Cloaking"))
                    {
                        act.Status.Additions["Cloaking"].AdditionEnd();
                        act.Status.Additions.Remove("Cloaking");
                    }
                    if (act.Status.Additions.ContainsKey("IAmTree"))
                    {
                        act.Status.Additions["IAmTree"].AdditionEnd();
                        act.Status.Additions.Remove("IAmTree");
                    }
                    if (act.Status.Additions.ContainsKey("Invisible"))
                    {
                        act.Status.Additions["Invisible"].AdditionEnd();
                        act.Status.Additions.Remove("Invisible");
                    }
                }
            }

            if (skill.dActor != 0xFFFFFFFF)
            {
                Actor dActor = this.Map.GetActor(skill.dActor);
                if (dActor != null)
                {
                    skill.argType = SkillArg.ArgType.Active;
                    PartnerTalking(Character.Partner, TALK_EVENT.BATTLE, 1, 20000);
                    if (skill.useMPSP)
                    {
                        uint mpCost = skill.skill.MP;
                        uint spCost = skill.skill.SP;
                        uint epCost = skill.skill.EP;
                        if (Character.Status.sp_rate_down_iris < 100)
                        {
                            spCost = (uint)(spCost * (float)(Character.Status.sp_rate_down_iris / 100.0f));
                        }
                        if (Character.Status.mp_rate_down_iris < 100)
                        {
                            mpCost = (uint)(mpCost * (float)(Character.Status.mp_rate_down_iris / 100.0f));
                        }

                        if (this.Character.Status.doubleUpList.Contains((ushort)skill.skill.ID))
                            spCost = (ushort)(spCost * 2);

                        //ECOKEY 流水攻勢只對有武器的技能有效
                        if (this.Character.Status.Additions.ContainsKey("SwordEaseSp") && skill.skill.BaseData.EquipFlagValue != 0)
                        {
                            //mpCost = (ushort)(mpCost*0.7);
                            spCost = (ushort)(spCost * 0.7);
                        }
                        if (this.Character.Status.Additions.ContainsKey("HarvestMaster"))
                        {
                            mpCost = (ushort)(mpCost * (float)(1.0f - Character.Status.HarvestMaster_Lv * 0.05));
                            spCost = (ushort)(spCost * (float)(1.0f - Character.Status.HarvestMaster_Lv * 0.05));
                        }
                        if (skill.skill.ID == 2527 && (Character.Skills2_2.ContainsKey(2355) || Character.DualJobSkill.Exists(x => x.ID == 2355)))//当技能为神速斩
                        {
                            //这里取副职的拔刀斩等级
                            var duallv = 0;
                            if (Character.DualJobSkill.Exists(x => x.ID == 2355))
                                duallv = Character.DualJobSkill.FirstOrDefault(x => x.ID == 2355).Level;

                            //这里取主职的拔刀斩等级
                            var mainlv = 0;
                            if (Character.Skills2_2.ContainsKey(2355))
                                mainlv = Character.Skills2_2[2355].Level;
                            //获取最高的拔刀斩等级
                            int maxlevel = Math.Max(duallv, mainlv);
                            spCost = (ushort)(spCost - (spCost * maxlevel * 0.04f));

                        }

                        if (this.Character.PossessionTarget != 0)
                        {
                            mpCost = (ushort)(mpCost * 2);
                            spCost = (ushort)(spCost * 2);
                        }

                        //ECOKEY 極大耗魔量修復
                        if (this.Character.Status.Additions.ContainsKey("Zensss"))
                        {
                            float zenbonus = (float)((float)(this.Character.Status.Additions["Zensss"] as DefaultBuff).Variable["Zensss"] / 10.0f);
                            mpCost = (uint)(mpCost * zenbonus);
                        }
                        /*if (this.Character.Status.Additions.ContainsKey("Zensss"))
                            mpCost *= 2;*/

                        if (this.Character.Status.Additions.ContainsKey("EnergyExcess"))//能量增幅耗蓝加深
                        {
                            float[] rate = { 0, 0.05f, 0.16f, 0.28f, 0.4f, 0.65f };
                            mpCost += (ushort)(mpCost * rate[this.Character.TInt["EnergyExcess"]]);
                        }
                        if (mpCost > this.Character.MP && spCost > this.Character.SP)
                        {
                            skill.result = -1;
                            this.Character.e.OnActorSkillUse(this.Character, skill);
                            return;
                        }
                        else if (mpCost > this.Character.MP)
                        {
                            skill.result = -15;
                            this.Character.e.OnActorSkillUse(this.Character, skill);
                            return;
                        }
                        else if (spCost > this.Character.SP)
                        {
                            skill.result = -16;
                            this.Character.e.OnActorSkillUse(this.Character, skill);
                            return;
                        }
                        this.Character.MP -= mpCost;
                        if (this.Character.MP < 0)
                            this.Character.MP = 0;

                        this.Character.SP -= spCost;
                        if (this.Character.SP < 0)
                            this.Character.SP = 0;

                        this.Character.EP -= epCost;
                        if (this.Character.EP < 0)
                            this.Character.EP = 0;

                        this.SendActorHPMPSP(this.Character);
                    }
                    SkillHandler.Instance.SkillCast(this.Character, dActor, skill);
                }
                else
                {
                    skill.result = -11;
                    this.Character.e.OnActorSkillUse(this.Character, skill);
                }
            }
            else
            {
                skill.argType = SkillArg.ArgType.Active;
                if (skill.useMPSP)
                {
                    //ECOKEY 修復極大對技能範圍技能無效問題
                    uint mpCost = skill.skill.MP;
                    //ECOKEY 極大耗魔量修復，下面的this.Character.MP都改成mpCost
                    if (this.Character.Status.Additions.ContainsKey("Zensss") && !this.Character.ZenOutLst.Contains(skill.skill.ID))
                    {
                        float zenbonus = (float)((float)(this.Character.Status.Additions["Zensss"] as DefaultBuff).Variable["Zensss"] / 10.0f);
                        mpCost = (uint)(mpCost * zenbonus);
                    }
                    /*if (this.Character.Status.Additions.ContainsKey("Zensss") && !this.Character.ZenOutLst.Contains(skill.skill.ID))
                        mpCost *= 2;*/
                    if (skill.skill.MP > this.Character.MP && skill.skill.SP > this.Character.SP)
                    {
                        skill.result = -1;
                        this.Character.e.OnActorSkillUse(this.Character, skill);
                        return;
                    }
                    else if (skill.skill.MP > this.Character.MP)
                    {
                        skill.result = -15;
                        this.Character.e.OnActorSkillUse(this.Character, skill);
                        return;
                    }
                    else if (skill.skill.SP > this.Character.SP)
                    {
                        skill.result = -16;
                        this.Character.e.OnActorSkillUse(this.Character, skill);
                        return;
                    }
                    //ECOKEY 修復極大對技能範圍技能無效問題
                    this.Character.MP -= mpCost;
                    this.Character.SP -= skill.skill.SP;
                    this.SendActorHPMPSP(this.Character);
                    /*this.Character.MP -= skill.skill.MP;
                    this.Character.SP -= skill.skill.SP;
                    this.SendActorHPMPSP(this.Character);*/
                }
                SkillHandler.Instance.SkillCast(this.Character, this.Character, skill);

            }

            //ECOKEY 刪除所有舊版寵物成長
            /*if (this.Character.Pet != null)
            {
                if (this.Character.Pet.Ride)
                {
                    SkillHandler.Instance.ProcessPetGrowth(this.Character.Pet, PetGrowthReason.UseSkill);
                }
            }*/

            if (this.Character.Status.delayCancelList.ContainsKey((ushort)skill.skill.ID))
                skillDelay = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)(skill.skill.Delay * (1f - ((float)this.Character.Status.delayCancelList[(ushort)skill.skill.ID] / 100.0f))));
            else
                skillDelay = DateTime.Now + new TimeSpan(0, 0, 0, 0, skill.skill.Delay);

            //ECOKEY 狂亂時間
            if (Character.Status.Additions.ContainsKey("OverWork"))//&& !skill.skill.BaseData.flag.Test(SkillFlags.PHYSIC))//狂乱时间
            {
                int DelayTime = (Character.Status.Additions["OverWork"] as DefaultBuff).Variable["OverWork"];
                skillDelay = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)(skill.skill.Delay * (1f - ((float)DelayTime / 100.0f))));
            }

            //ECOKEY 演奏專精
            if (Character.Status.Additions.ContainsKey("MusicalDamUp") && skill.skill.BaseData.EquipFlagValue == 196608)
            {
                float cdlv = new float[] { 0f, 0.1f, 0.1f, 0.25f, 0.425f, 0.4f }[Character.Status.EaseCt_lv];
                skillDelay = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)(skill.skill.Delay * (1f - cdlv)));
            }

            if (this.Character.Status.aspd_skill_perc >= 1f)
                skillDelay = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)(skill.skill.Delay / this.Character.Status.aspd_skill_perc));

            //独立cd
            if (!SingleCDLst.ContainsKey(skill.skill.ID))
                SingleCDLst.Add(skill.skill.ID, DateTime.Now + new TimeSpan(0, 0, 0, 0, skill.skill.SinglgCD));
            else
                SingleCDLst[skill.skill.ID] = DateTime.Now + new TimeSpan(0, 0, 0, 0, skill.skill.SinglgCD);

            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, skill, this.Character, true);

            if (skill.skill.Effect != 0 && (skill.skill.Target == 4 || (skill.skill.Target == 2 && skill.sActor == skill.dActor)))
            {
                EffectArg eff = new EffectArg();
                eff.actorID = skill.dActor;
                eff.effectID = skill.skill.Effect;
                eff.x = skill.x;
                eff.y = skill.y;
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, eff, this.Character, true);
            }

            if (this.Character.Tasks.ContainsKey("AutoCast"))
                this.Character.Tasks["AutoCast"].Activate();
            else
            {
                if (skill.autoCast.Count != 0)
                {
                    this.Character.Buff.CannotMove = true;
                    this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);
                    Tasks.Skill.AutoCast task = new SagaMap.Tasks.Skill.AutoCast(this.Character, skill);
                    this.Character.Tasks.Add("AutoCast", task);
                    task.Activate();
                }
            }

            if (this.Character.Status.Additions.ContainsKey("EyeballKeeper"))
            {
                if (Global.Random.Next(1, 100) > 95)
                {
                    int sk = Global.Random.Next(1, 5);
                    SkillArg autoskill = new SkillArg();

                    SagaDB.Skill.Skill atskill = null;
                    autoskill.sActor = this.Character.ActorID;
                    autoskill.dActor = this.Character.ActorID;
                    autoskill.argType = SkillArg.ArgType.Cast;
                    autoskill.useMPSP = false;

                    switch (sk)
                    {
                        case 1:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3146, 1);
                            break;
                        case 2:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3300, 1);
                            break;
                        case 3:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3313, 1);
                            break;
                        case 4:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3254, 1);
                            break;
                        case 5:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3308, 1);
                            break;
                        default:
                            break;
                    }
                    autoskill.skill = atskill;
                    MapClient.FromActorPC(this.Character).OnSkillCastComplete(autoskill);
                }
            }
            if (this.Character.Status.Additions.ContainsKey("EyeballRaiders"))
            {
                if (Global.Random.Next(1, 100) > 95)
                {
                    int skill1 = Global.Random.Next(1, 5);
                    SkillArg autoheal = new SkillArg();

                    SagaDB.Skill.Skill atskill = null;
                    autoheal.sActor = this.Character.ActorID;
                    autoheal.dActor = this.Character.ActorID;
                    autoheal.argType = SkillArg.ArgType.Cast;
                    autoheal.useMPSP = false;
                    Map map = Manager.MapManager.Instance.GetMap(this.Character.MapID);
                    List<Actor> affected = map.GetActorsArea(this.Character, 200, false);
                    switch (skill1)
                    {
                        case 1:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3054, 1);
                            break;
                        case 2:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2001, 1);
                            break;
                        case 3:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6653, 1);
                            break;
                        case 4:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6654, 1);
                            break;
                        case 5:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6655, 1);
                            break;
                        default:
                            break;
                    }
                    autoheal.skill = atskill;
                    MapClient.FromActorPC(this.Character as ActorPC).OnSkillCastComplete(autoheal);
                }
            }
            if (this.Character.Status.Additions.ContainsKey("EyeballUsual"))
            {
                if (Global.Random.Next(1, 100) > 95)
                {
                    int skill2 = Global.Random.Next(1, 5);
                    SkillArg autoheal = new SkillArg();

                    SagaDB.Skill.Skill atskill = null;
                    autoheal.sActor = this.Character.ActorID;
                    autoheal.dActor = this.Character.ActorID;
                    autoheal.argType = SkillArg.ArgType.Cast;
                    autoheal.useMPSP = false;

                    switch (skill2)
                    {
                        case 1:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3054, 1);
                            break;
                        case 2:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2002, 2);
                            break;
                        case 3:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(2004, 2);
                            break;
                        case 4:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(6654, 1);
                            break;
                        case 5:
                            atskill = SagaDB.Skill.SkillFactory.Instance.GetSkill(3300, 1);
                            break;
                        default:
                            break;
                    }
                    autoheal.skill = atskill;
                    MapClient.FromActorPC(this.Character as ActorPC).OnSkillCastComplete(autoheal);
                }
            }
        }

        public void SendChangeStatus()
        {
            if (this.Character.Tasks.ContainsKey("Regeneration"))
            {
                this.Character.Tasks["Regeneration"].Deactivate();
                this.Character.Tasks.Remove("Regeneration");
            }
            if (this.Character.Motion != MotionType.NONE && this.Character.Motion != MotionType.DEAD)
            {
                this.Character.Motion = MotionType.NONE;
                this.Character.MotionLoop = false;
            }
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_STATUS, null, this.Character, true);
        }
        /*  public void SendChangeStatus()
          {
              //ECOKEY 坐下優化
              if (this.Character.Motion != MotionType.SIT)
              {
                  if (this.Character.Tasks.ContainsKey("Regeneration"))
                  {
                      this.Character.Tasks["Regeneration"].Deactivate();
                      this.Character.Tasks.Remove("Regeneration");
                  }
              }
              if (this.Character.Motion != MotionType.NONE && this.Character.Motion != MotionType.DEAD)
              {
                  this.Character.Motion = MotionType.NONE;
                  this.Character.MotionLoop = false;
              }
              this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_STATUS, null, this.Character, true);
          }*/

        public void SendRevive(byte level)
        {
            this.Character.Buff.Dead = false;
            this.Character.Buff.TurningPurple = false;
            this.Character.Motion = MotionType.STAND;
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);

            float factor = 0;
            //ECOKEY 暗黑騎士的自動復活
            if (this.Character.TInt["DarkKnightRevive"] != 0)
            {
                level = (byte)this.Character.TInt["DarkKnightRevive"];
                this.Character.TInt["DarkKnightRevive"] = 0; //加這句
                SagaMap.Skill.Additions.Global.Undead skill = new SagaMap.Skill.Additions.Global.Undead(this.Character);
                SagaMap.Skill.SkillHandler.ApplyAddition(this.Character, skill);
                switch (level)
                {
                    case 1:
                        factor = 0.4f;
                        break;
                    case 2:
                        factor = 0.5f;
                        break;
                    case 3:
                        factor = 0.6f;
                        break;
                }
            }
            else
            {
                switch (level)
                {
                    case 1:
                        factor = 0.1f;
                        break;
                    case 2:
                        factor = 0.2f;
                        break;
                    case 3:
                        factor = 0.45f;
                        break;
                    case 4:
                        factor = 0.5f;
                        break;
                    case 5:
                        factor = 0.75f;
                        break;
                    case 6:
                        factor = 1f;
                        break;
                }
            }

            this.Character.HP = (uint)(this.Character.MaxHP * factor);
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.Character, true);
            SkillArg arg = new SkillArg();
            arg.sActor = this.Character.ActorID;
            arg.dActor = 0;
            arg.skill = SkillFactory.Instance.GetSkill(10002, level);
            arg.x = 0;
            arg.y = 0;
            arg.hp = new List<int>();
            arg.sp = new List<int>();
            arg.mp = new List<int>();
            arg.hp.Add((int)(-this.Character.MaxHP * factor));
            arg.sp.Add(0);
            arg.mp.Add(0);
            arg.flag.Add(AttackFlag.HP_HEAL);
            arg.affectedActors.Add(this.Character);
            arg.argType = SkillArg.ArgType.Active;
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Character, true);

            if (!this.Character.Tasks.ContainsKey("AutoSave"))
            {
                Tasks.PC.AutoSave task = new SagaMap.Tasks.PC.AutoSave(this.Character);
                this.Character.Tasks.Add("AutoSave", task);
                task.Activate();
            }
            if (!Character.Tasks.ContainsKey("Recover"))//自然恢复
            {
                Tasks.PC.Recover reg = new Tasks.PC.Recover(FromActorPC(Character));
                Character.Tasks.Add("Recover", reg);
                reg.Activate();
            }
            //ECOKEY 避免心PE倒數消失
            if (this.Character.TamaireRental != null && this.Character.TamaireRental.CurrentLender != 0 && !this.Character.Tasks.ContainsKey("HeartPossession"))
            {
                int t = (int)((this.Character.TamaireRental.RentDue - DateTime.Now).TotalSeconds) * 1000;
                Tasks.PC.HeartPossession task = new SagaMap.Tasks.PC.HeartPossession(this.Character, t);
                this.Character.Tasks.Add("HeartPossession", task);
                task.Activate();
            }

            SkillHandler.Instance.CastPassiveSkills(this.Character);
            SendPlayerInfo();

            //ECOKEY 騎士團復活紀錄
            if (Character.Mode == PlayerMode.KNIGHT_EAST ||
                Character.Mode == PlayerMode.KNIGHT_WEST ||
                Character.Mode == PlayerMode.KNIGHT_SOUTH ||
                Character.Mode == PlayerMode.KNIGHT_NORTH)
            {
                if (Character.TStr["复活者"] != null)
                {
                    foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(Character.MapID).Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)j;
                            if (pc.Name == Character.TStr["复活者"])
                            {
                                KnightWarManager.Instance.KnightWarPrize(pc, "KNIGHTWAR_Heal", (int)(this.Character.MaxHP * factor));
                                KnightWarManager.Instance.GetPoint(pc, 5, pc.Mode.ToString());
                                KnightWarManager.Instance.GetPointSelf(pc, 5, "KNIGHTWAR_Score");
                            }
                        }

                    }
                }
            }
        }

        public void SendSkillList()
        {
            Packets.Server.SSMG_SKILL_LIST p = new SagaMap.Packets.Server.SSMG_SKILL_LIST();
            Dictionary<uint, byte> skills;
            Dictionary<uint, byte> skills2X;
            Dictionary<uint, byte> skills2T;
            Dictionary<uint, byte> skills3;
            List<SagaDB.Skill.Skill> list = new List<SagaDB.Skill.Skill>();
            bool ifDominion = this.map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion);
            if (ifDominion)
            {
                skills = new Dictionary<uint, byte>();
                skills2X = new Dictionary<uint, byte>();
                skills2T = new Dictionary<uint, byte>();
                skills3 = new Dictionary<uint, byte>();
            }
            else
            {
                skills = SkillFactory.Instance.SkillList(this.Character.JobBasic);
                skills2X = SkillFactory.Instance.SkillList(this.Character.Job2X);
                skills2T = SkillFactory.Instance.SkillList(this.Character.Job2T);
                skills3 = SkillFactory.Instance.SkillList(this.Character.Job3);
            }
            {
                var skill =
                    from c in skills.Keys
                    where !this.Character.Skills.ContainsKey(c)
                    select c;
                foreach (uint i in skill)
                {
                    SagaDB.Skill.Skill sk = SkillFactory.Instance.GetSkill(i, 0);
                    list.Add(sk);
                }
                foreach (SagaDB.Skill.Skill i in this.Character.Skills.Values)
                {
                    list.Add(i);
                }
            }
            p.Skills(list, 0, this.Character.JobBasic, ifDominion, this.Character);
            this.netIO.SendPacket(p);
            if (this.Character.Rebirth || this.Character.Job == this.Character.Job3)
            {
                {
                    list.Clear();
                    {
                        var skill =
                            from c in skills2X.Keys
                            where !this.Character.Skills2_1.ContainsKey(c)
                            select c;
                        foreach (uint i in skill)
                        {
                            SagaDB.Skill.Skill sk = SkillFactory.Instance.GetSkill(i, 0);
                            list.Add(sk);
                        }
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills2_1.Values)
                        {
                            list.Add(i);
                        }
                    }

                    p.Skills(list, 1, this.Character.Job2X, ifDominion, this.Character);
                    this.netIO.SendPacket(p);
                }
                {
                    list.Clear();
                    {
                        var skill =
                            from c in skills2T.Keys
                            where !this.Character.Skills2_2.ContainsKey(c)
                            select c;
                        foreach (uint i in skill)
                        {
                            SagaDB.Skill.Skill sk = SkillFactory.Instance.GetSkill(i, 0);
                            list.Add(sk);
                        }
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills2_2.Values)
                        {
                            list.Add(i);
                        }
                    }
                    p.Skills(list, 2, this.Character.Job2T, ifDominion, this.Character);
                    this.netIO.SendPacket(p);
                }
                {
                    list.Clear();
                    {
                        var skill =
                            from c in skills3.Keys
                            where !this.Character.Skills3.ContainsKey(c)
                            select c;
                        foreach (uint i in skill)
                        {
                            SagaDB.Skill.Skill sk = SkillFactory.Instance.GetSkill(i, 0);
                            list.Add(sk);
                        }
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills3.Values)
                        {
                            list.Add(i);
                        }
                    }

                    p.Skills(list, 3, this.Character.Job3, ifDominion, this.Character);
                    this.netIO.SendPacket(p);
                }

            }
            else
            {
                if (this.Character.Job == this.Character.Job2X)
                {
                    list.Clear();
                    {
                        var skill =
                            from c in skills2X.Keys
                            where !this.Character.Skills2.ContainsKey(c)
                            select c;
                        foreach (uint i in skill)
                        {
                            SagaDB.Skill.Skill sk = SkillFactory.Instance.GetSkill(i, 0);
                            list.Add(sk);
                        }
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills2.Values)
                        {
                            list.Add(i);
                        }
                    }

                    p.Skills(list, 1, this.Character.Job2X, ifDominion, this.Character);
                    this.netIO.SendPacket(p);
                }
                if (this.Character.Job == this.Character.Job2T)
                {
                    list.Clear();
                    {
                        var skill =
                            from c in skills2T.Keys
                            where !this.Character.Skills2.ContainsKey(c)
                            select c;
                        foreach (uint i in skill)
                        {
                            SagaDB.Skill.Skill sk = SkillFactory.Instance.GetSkill(i, 0);
                            list.Add(sk);
                        }
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills2.Values)
                        {
                            list.Add(i);
                        }
                    }
                    p.Skills(list, 2, this.Character.Job2T, ifDominion, this.Character);
                    this.netIO.SendPacket(p);
                }
                if (this.map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                {
                    if (this.Character.DominionReserveSkill)
                    {
                        Packets.Server.SSMG_SKILL_RESERVE_LIST p2 = new SagaMap.Packets.Server.SSMG_SKILL_RESERVE_LIST();
                        p2.Skills = this.Character.SkillsReserve.Values.ToList();
                        this.netIO.SendPacket(p2);
                    }
                    else
                    {
                        Packets.Server.SSMG_SKILL_RESERVE_LIST p2 = new SagaMap.Packets.Server.SSMG_SKILL_RESERVE_LIST();
                        p2.Skills = new List<SagaDB.Skill.Skill>();
                        this.netIO.SendPacket(p2);
                    }
                }
                else
                {
                    Packets.Server.SSMG_SKILL_RESERVE_LIST p2 = new SagaMap.Packets.Server.SSMG_SKILL_RESERVE_LIST();
                    p2.Skills = this.Character.SkillsReserve.Values.ToList();
                    this.netIO.SendPacket(p2);
                }
            }


            if (this.Character.JobJoint != PC_JOB.NONE)
            {
                list.Clear();
                {
                    var skill =
                        from c in SkillFactory.Instance.SkillList(this.Character.JobJoint)
                        where c.Value <= this.Character.JointJobLevel
                        select c;
                    foreach (KeyValuePair<uint, byte> i in skill)
                    {
                        SagaDB.Skill.Skill sk = SkillFactory.Instance.GetSkill(i.Key, 1);
                        list.Add(sk);
                    }
                }
                Packets.Server.SSMG_SKILL_JOINT_LIST p2 = new SagaMap.Packets.Server.SSMG_SKILL_JOINT_LIST();
                p2.Skills = list;
                this.netIO.SendPacket(p2);
            }
            else
            {
                Packets.Server.SSMG_SKILL_JOINT_LIST p2 = new SagaMap.Packets.Server.SSMG_SKILL_JOINT_LIST();
                p2.Skills = new List<SagaDB.Skill.Skill>();
                this.netIO.SendPacket(p2);
            }
        }

        public void SendAnotherSkillList()
        {
            if (this.Character.UsingPaperID == 0)
                return;
            //this.Character.AnotherPapers[this.Character.UsingPaperID].
            List<SagaDB.Skill.Skill> lst = new List<SagaDB.Skill.Skill>();

            foreach (var item in AnotherFactory.Instance.Items[this.Character.UsingPaperID][this.Character.AnotherPapers[this.Character.UsingPaperID].Level].Skills)
            {
                var key = item.Key;
                var value = item.Value;
                var lv = this.Character.AnotherPapers[this.Character.UsingPaperID].Level;
                SagaDB.Skill.Skill sk;
                if (item.Key != 0)
                    lst.Add(SkillFactory.Instance.GetSkill(item.Key, item.Value[0]));
                else
                {
                    sk = SkillFactory.Instance.GetSkill(AnotherFactory.Instance.Items[this.Character.UsingPaperID][lv].AwakeSkillID, AnotherFactory.Instance.Items[this.Character.UsingPaperID][lv].AwakeSkillMaxLV);
                    lst.Add(sk);
                }
            }
            Packets.Server.SSMG_ANO_SKILL_LIST p1 = new Packets.Server.SSMG_ANO_SKILL_LIST();

            List<ushort> sklist = new List<ushort>();
            List<byte> sklevel = new List<byte>();
            foreach (var item in lst)
            {
                sklist.Add((ushort)item.ID);
                sklevel.Add(item.Level);
            }

            p1.SkillList = sklist;
            p1.SkillLevel = sklevel;
            this.netIO.SendPacket(p1);
        }

        public void SendSkillDummy()
        {
            SendSkillDummy(3311, 1);
        }

        public void SendSkillDummy(uint skillid, byte level)
        {
            if (this.Character.Tasks.ContainsKey("SkillCast"))
            {
                if (this.Character.Tasks["SkillCast"].getActivated())
                {
                    this.Character.Tasks["SkillCast"].Deactivate();
                    this.Character.Tasks.Remove("SkillCast");
                }

                SkillArg arg = new SkillArg();
                arg.sActor = this.Character.ActorID;
                arg.dActor = 0;
                arg.skill = SkillFactory.Instance.GetSkill(skillid, level);
                arg.x = 0;
                arg.y = 0;
                arg.hp = new List<int>();
                arg.sp = new List<int>();
                arg.mp = new List<int>();
                arg.hp.Add(0);
                arg.sp.Add(0);
                arg.mp.Add(0);
                arg.flag.Add(AttackFlag.NONE);
                //arg.affectedActors.Add(this.Character);
                arg.argType = SkillArg.ArgType.Active;
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Character, true);
            }
        }
        public void OnSkillRangeAttack(Packets.Client.CSMG_SKILL_RANGE_ATTACK p)
        {
            Packets.Server.SSMG_SKILL_RANGEA_RESULT p2 = new Packets.Server.SSMG_SKILL_RANGEA_RESULT();
            p2.ActorID = p.ActorID;
            if (!Character.Status.Additions.ContainsKey("自由射击"))
                p2.Speed = 410;
            else
                p2.Speed = 0;
            netIO.SendPacket(p2);
            Character.TTime["远程蓄力"] = DateTime.Now;

            if (Character.Tasks.ContainsKey("RangeAttack"))
            {
                Character.Tasks["RangeAttack"].Deactivate();
                Character.Tasks.Remove("RangeAttack");
            }
            Tasks.PC.RangeAttack ra = new Tasks.PC.RangeAttack(this);
            Character.Tasks.Add("RangeAttack", ra);
            ra.Activate();
        }

        /// <summary>
        /// 重置技能
        /// </summary>
        /// <param name="job">1为1转，2为2转</param>
        public void ResetSkill(byte job)
        {
            int totalPoints = 0;
            List<uint> delList = new List<uint>();
            switch (job)
            {
                case 1:
                    foreach (SagaDB.Skill.Skill i in this.Character.Skills.Values)
                    {
                        if (SkillFactory.Instance.SkillList(this.Character.JobBasic).ContainsKey(i.ID))
                        {
                            totalPoints += (i.Level + 2);
                            delList.Add(i.ID);
                        }
                    }
                    this.Character.SkillPoint += (ushort)totalPoints;
                    foreach (uint i in delList)
                    {
                        this.Character.Skills.Remove(i);
                    }
                    break;
                case 2:
                    if (!this.Character.Rebirth)
                    {
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills2.Values)
                        {
                            if (SkillFactory.Instance.SkillList(this.Character.Job).ContainsKey(i.ID))
                            {
                                totalPoints += (i.Level + 2);
                                delList.Add(i.ID);
                            }
                        }
                        foreach (uint i in delList)
                        {
                            this.Character.Skills2.Remove(i);
                        }
                        if (this.Character.Job == this.Character.Job2X)
                            this.Character.SkillPoint2X += (ushort)totalPoints;
                        if (this.Character.Job == this.Character.Job2T)
                            this.Character.SkillPoint2T += (ushort)totalPoints;
                    }
                    else
                    {
                        this.Character.SkillPoint2X = 0;
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills2_1.Values)
                        {
                            if (SkillFactory.Instance.SkillList(this.Character.Job2X).ContainsKey(i.ID))
                            {
                                totalPoints += (i.Level + 2);
                                delList.Add(i.ID);
                            }
                        }
                        foreach (uint i in delList)
                        {
                            this.Character.Skills2_1.Remove(i);
                            this.Character.Skills2.Remove(i);
                        }
                        this.Character.SkillPoint2X += (ushort)totalPoints;

                        totalPoints = 0;
                        delList.Clear();
                        this.Character.SkillPoint2T = 0;
                        foreach (SagaDB.Skill.Skill i in this.Character.Skills2_2.Values)
                        {
                            if (SkillFactory.Instance.SkillList(this.Character.Job2T).ContainsKey(i.ID))
                            {
                                totalPoints += (i.Level + 2);
                                delList.Add(i.ID);
                            }
                        }
                        foreach (uint i in delList)
                        {
                            this.Character.Skills2_2.Remove(i);
                            this.Character.Skills2.Remove(i);
                        }
                        this.Character.SkillPoint2T += (ushort)totalPoints;

                    }
                    break;
                case 3:
                    foreach (SagaDB.Skill.Skill i in this.Character.Skills3.Values)
                    {
                        if (SkillFactory.Instance.SkillList(this.Character.Job3).ContainsKey(i.ID))
                        {
                            totalPoints += (i.Level + 2);
                            delList.Add(i.ID);
                        }
                    }
                    this.Character.SkillPoint3 += (ushort)totalPoints;
                    foreach (uint i in delList)
                    {
                        this.Character.Skills3.Remove(i);
                    }
                    break;
            }
            SkillHandler.Instance.CastPassiveSkills(this.Character);
        }

        public void OnFishBaitsEquip(Packets.Client.CSMG_FF_FISHBAIT_EQUIP p)
        {
            if (p.InventorySlot == 0)
            {
                this.Character.EquipedBaitID = 0;

                Packets.Server.SSMG_FISHBAIT_EQUIP_RESULT p2 = new Packets.Server.SSMG_FISHBAIT_EQUIP_RESULT();
                p2.InventoryID = 0;
                p2.IsEquip = 1;
                this.netIO.SendPacket(p2);
            }
            else
            {
                SagaDB.Item.Item item = this.Character.Inventory.GetItem(p.InventorySlot);
                if (item.ItemID >= 10104900 || item.ItemID <= 10104906)
                {
                    this.Character.EquipedBaitID = item.ItemID;

                    Packets.Server.SSMG_FISHBAIT_EQUIP_RESULT p2 = new Packets.Server.SSMG_FISHBAIT_EQUIP_RESULT();
                    p2.InventoryID = p.InventorySlot;
                    p2.IsEquip = 0;
                    this.netIO.SendPacket(p2);
                }
            }
        }
    }
}
