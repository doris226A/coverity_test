using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Actor;
using SagaDB.Marionette;
using SagaDB.Skill;
using SagaLib;
using SagaMap;
using SagaMap.Manager;
using SagaMap.Skill;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public void MarionetteActivate(uint marionetteID)
        {
            MarionetteActivate(marionetteID, true, true);
        }

        public void MarionetteActivate(uint marionetteID, bool delay,bool duration)
        {
            Marionette marionette = MarionetteFactory.Instance[marionetteID];
            Character.Status.str_mario = marionette.str;
            Character.Status.agi_mario = marionette.agi;
            Character.Status.vit_mario = marionette.vit;
            Character.Status.int_mario = marionette.intel;
            Character.Status.mag_mario = marionette.mag;
            Character.Status.dex_mario = marionette.dex;

            Character.Status.hp_mario = marionette.hp;
            Character.Status.mp_mario = marionette.mp;
            Character.Status.sp_mario = marionette.sp;
            Character.Status.speed_mario = marionette.move_speed;
            Character.Status.aspd_mario = marionette.aspd;
            Character.Status.cspd_mario = marionette.cspd;

            Character.Status.min_atk1_mario = marionette.min_atk1;
            Character.Status.max_atk1_mario = marionette.max_atk1;
            Character.Status.min_atk2_mario = marionette.min_atk2;
            Character.Status.max_atk2_mario = marionette.max_atk2;
            Character.Status.min_atk3_mario = marionette.min_atk3;
            Character.Status.max_atk3_mario = marionette.max_atk3;
            Character.Status.min_matk_mario = marionette.min_matk;
            Character.Status.max_matk_mario = marionette.max_matk;

            Character.Status.def_mario = marionette.def;
            Character.Status.def_add_mario = marionette.def_add;
            Character.Status.mdef_add_mario = marionette.mdef;
            Character.Status.mdef_add_mario = marionette.mdef_add;

            Character.Status.hit_melee_mario = marionette.hit_melee;
            Character.Status.avoid_melee_mario = marionette.avoid_melee;
            Character.Status.hit_ranged_mario = marionette.hit_ranged;
            Character.Status.avoid_ranged_mario = marionette.avoid_ranged;
            Character.Status.avoid_magic_mario = marionette.avoid_magic;
            Character.Status.cri_mario = marionette.hit_cri;
            Character.Status.cri_avoid_mario = marionette.avoid_cri;


            //ECOKEY 活動木偶強化被動
            SkillHandler.Instance.CastPassiveSkills(this.Character);
            if (marionette != null)
            {
                Tasks.PC.Marionette task = new SagaMap.Tasks.PC.Marionette(this, marionette.Duration);
                if (this.Character.Tasks.ContainsKey("Marionette") && duration)
                {
                    MarionetteDeactivate();
                    this.Character.Tasks["Marionette"].Deactivate();
                    this.Character.Tasks.Remove("Marionette");
                }
                if (!duration && this.Character.Marionette != null)
                {
                    foreach (uint i in this.Character.Marionette.skills)
                    {
                        SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(i, 1);
                        if (skill != null)
                        {
                            if (this.Character.Skills.ContainsKey(i))
                            {
                                this.Character.Skills.Remove(i);
                            }
                        }
                    }
                    SkillHandler.Instance.CastPassiveSkills(this.Character);
                }
                if (!this.Character.Tasks.ContainsKey("Marionette"))
                {
                    this.Character.Tasks.Add("Marionette", task);
                    task.Activate();
                }
                if (delay)
                {
                    if (!this.Character.Status.Additions.ContainsKey("MarioTimeUp"))
                        this.Character.NextMarionetteTime = DateTime.Now + new TimeSpan(0, 0, marionette.Delay);
                    else
                        this.Character.NextMarionetteTime = DateTime.Now + new TimeSpan(0, 0, (int)(marionette.Delay * 0.6f));
                }
                this.Character.Marionette = marionette;
                SendCharInfoUpdate();
                foreach (uint i in marionette.skills)
                {
                    SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(i, 1);
                    if (skill != null)
                    {
                        if (!this.Character.Skills.ContainsKey(i))
                        {
                            skill.NoSave = true;
                            this.Character.Skills.Add(i, skill);
                            if (!skill.BaseData.active)
                            {
                                SkillArg arg = new SkillArg();
                                arg.skill = skill;
                                SkillHandler.Instance.SkillCast(this.Character, this.Character, arg);
                            }
                        }
                    }
                }
                SkillHandler.Instance.CastPassiveSkills(this.Character);
                PC.StatusFactory.Instance.CalcStatus(this.Character);
                SendPlayerInfo();
            }
        }

        public void MarionetteDeactivate()
        {
            MarionetteDeactivate(false);
        }

        public void MarionetteDeactivate(bool disconnecting)
        {
            if (this.Character.Marionette == null)
                return;
            Marionette marionette = this.Character.Marionette;
            Character.Status.str_mario = 0;
            Character.Status.agi_mario = 0;
            Character.Status.vit_mario = 0;
            Character.Status.int_mario = 0;
            Character.Status.mag_mario = 0;
            Character.Status.dex_mario = 0;

            Character.Status.hp_mario = 0;
            Character.Status.mp_mario = 0;
            Character.Status.sp_mario = 0;
            Character.Status.speed_mario = 0;
            Character.Status.aspd_mario = 0;
            Character.Status.cspd_mario = 0;

            Character.Status.min_atk1_mario = 0;
            Character.Status.max_atk1_mario = 0;
            Character.Status.min_atk2_mario = 0;
            Character.Status.max_atk2_mario = 0;
            Character.Status.min_atk3_mario = 0;
            Character.Status.max_atk3_mario = 0;
            Character.Status.min_matk_mario = 0;
            Character.Status.max_matk_mario = 0;

            Character.Status.def_mario = 0;
            Character.Status.def_add_mario = 0;
            Character.Status.mdef_add_mario = 0;
            Character.Status.mdef_add_mario = 0;

            Character.Status.hit_melee_mario = 0;
            Character.Status.avoid_melee_mario = 0;
            Character.Status.hit_ranged_mario = 0;
            Character.Status.avoid_ranged_mario = 0;
            Character.Status.avoid_magic_mario = 0;
            Character.Status.cri_mario = 0;
            Character.Status.cri_avoid_mario = 0;
            this.Character.Marionette = null;
            if (!disconnecting) SendCharInfoUpdate();
            foreach (uint i in marionette.skills)
            {
                SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(i, 1);
                if (skill != null)
                {
                    if (this.Character.Skills.ContainsKey(i))
                    {
                        this.Character.Skills.Remove(i);
                    }
                }
            }
            SkillHandler.Instance.CastPassiveSkills(this.Character);
            PC.StatusFactory.Instance.CalcStatus(this.Character);
            if (!disconnecting)
            {
                SendPlayerInfo();
                SendMotion(MotionType.JOY, 0);
            }
        }
    }
}
