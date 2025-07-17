using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SagaDB.Actor
{
    public class ActorShadow : ActorPet
    {
        public ActorShadow(ActorPC creator)
        {
            this.baseData = new SagaDB.Mob.MobData();
            this.baseData.level = creator.Level;
            this.Status.attackType = creator.Status.attackType;
            this.Status.aspd = creator.Status.aspd;
            this.Status.def = creator.Status.def;
            this.Status.def_add = creator.Status.def_add;
            this.Status.mdef = creator.Status.mdef;
            this.Status.mdef_add = creator.Status.mdef_add;
            this.Status.min_atk1 = creator.Status.min_atk1;
            this.Status.max_atk1 = creator.Status.max_atk1;
            this.Status.min_atk2 = creator.Status.min_atk2;
            this.Status.max_atk2 = creator.Status.max_atk2;
            this.Status.min_atk3 = creator.Status.min_atk3;
            this.Status.max_atk3 = creator.Status.max_atk3;
            this.Status.min_matk = creator.Status.min_matk;
            this.Status.max_matk = creator.Status.max_matk;

            this.Status.hit_melee = creator.Status.hit_melee;
            this.Status.hit_ranged = creator.Status.hit_ranged;
            this.Status.avoid_melee = creator.Status.avoid_melee;
            this.Status.avoid_ranged = creator.Status.avoid_ranged;
            this.MaxHP = 1;
            this.HP = 1;
            this.type = ActorType.SHADOW;
            this.sightRange = 1500;
            this.Owner = creator;
            this.Speed = 100;
        }
        //ECOKEY 召喚物專用
        public ActorShadow(uint mobID)
        {
            this.type = ActorType.SHADOW;
            this.baseData = Mob.MobFactory.Instance.GetMobData(mobID);
            this.Level = this.BaseData.level;
            this.MaxHP = this.baseData.hp;
            this.HP = this.MaxHP;
            this.MaxMP = this.baseData.mp;
            this.MP = this.MaxMP;
            this.MaxSP = this.baseData.sp;
            this.SP = this.MaxSP;
            this.Name = this.baseData.name;
            //this.Speed = this.baseData.speed;
            this.Status.attackType = this.baseData.attackType;
            this.Status.aspd = this.baseData.aspd;
            this.Status.cspd = this.baseData.cspd;
            this.Status.def = this.baseData.def;
            this.Status.def_add = (short)this.baseData.def_add;
            this.Status.mdef = this.baseData.mdef;
            this.Status.mdef_add = (short)this.baseData.mdef_add;
            this.Status.min_atk1 = this.baseData.atk_min;
            this.Status.max_atk1 = this.baseData.atk_max;
            this.Status.min_atk2 = this.baseData.atk_min;
            this.Status.max_atk2 = this.baseData.atk_max;
            this.Status.min_atk3 = this.baseData.atk_min;
            this.Status.max_atk3 = this.baseData.atk_max;
            this.Status.min_matk = this.baseData.matk_min;
            this.Status.max_matk = this.baseData.matk_max;
            this.Status.hit_critical = this.baseData.cri;
            this.Status.avoid_critical = this.baseData.criavd;
            this.Status.hit_magic = this.baseData.hit_magic;
            this.Status.avoid_magic = this.baseData.avoid_magic;
            this.Race = this.baseData.race;
            foreach (SagaLib.Elements i in baseData.elements.Keys)
            {
                this.Elements[i] = this.baseData.elements[i];
                this.AttackElements[i] = 0;
            }
            foreach (SagaLib.AbnormalStatus i in baseData.abnormalStatus.Keys)
            {
                this.AbnormalStatus[i] = this.baseData.abnormalStatus[i];
            }
            this.Status.hit_melee = this.baseData.hit_melee;
            this.Status.hit_ranged = this.baseData.hit_ranged;
            this.Status.avoid_melee = this.baseData.avoid_melee;
            this.Status.avoid_ranged = this.baseData.avoid_ranged;
            this.Status.undead = this.baseData.undead;

            this.PictID = this.baseData.pictid;

        }
    }
}
