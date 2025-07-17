
using System;
using System.Collections.Generic;
using System.Text;
using SagaLib;
using SagaDB.Actor;
using SagaDB.Item;
using SagaMap.Scripting;
using SagaMap.ActorEventHandlers;
using SagaMap.Mob;
using SagaDB.Mob;
namespace SagaMap.Tasks.System
{
    public partial class 活動怪物
    {
        public static ActorMob.MobInfo 巨大皮路路Info()
            {
                ActorMob.MobInfo info = new ActorMob.MobInfo();
            info.name = "巨大皮露露";
            info.level = 50;
            info.maxhp = 74000;
            info.speed = 305;
            info.atk_min = 25;
            info.atk_max = 36;
            info.matk_min = 28;
            info.matk_max = 45;
            info.def = 28;
            info.def_add = 12;
            info.mdef = 33;
            info.mdef_add = 8;
            info.hit_critical = 14;
            info.hit_magic = 70;
            info.hit_melee = 42;
            info.hit_ranged = 79;
            info.avoid_critical = 14;
            info.avoid_magic = 200;
            info.avoid_melee = 101;
            info.avoid_ranged = 58;
            info.Aspd = 289;
            info.Cspd = 200;
            info.elements[SagaLib.Elements.Neutral] = 255;
            info.elements[SagaLib.Elements.Fire] = 255;
            info.elements[SagaLib.Elements.Water] = 255;
            info.elements[SagaLib.Elements.Wind] = 255;
            info.elements[SagaLib.Elements.Earth] = 255;
            info.elements[SagaLib.Elements.Holy] = 255;
            info.elements[SagaLib.Elements.Dark] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Confused] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Frosen] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Paralyse] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Poisen] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Silence] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Sleep] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Stone] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.Stun] = 255;
            info.abnormalstatus[SagaLib.AbnormalStatus.MoveSpeedDown] = 255;

            return info;
        }
        public static AIMode 巨大皮路路AI()
        {
            AIMode ai = new AIMode(0); ai.MobID = 10000000; ai.isNewAI = true;//1為主動，0為被動
            ai.MobID = 92400000;//怪物ID
            ai.isNewAI = true;//使用的是TT AI
            ai.Distance = 3;//遠程進程切換距離，與敵人3格距離切換
            ai.ShortCD = 3;//進程技能表最短釋放間隔，3秒一次
            ai.LongCD = 3;//遠程技能表最短釋放間隔，3秒一次
            AIMode.SkilInfo skillinfo = new AIMode.SkilInfo();

            /*---------死亡進行曲---------*/
            skillinfo.CD = 50;//技能CD
            skillinfo.Rate = 25;//釋放概率
            skillinfo.MaxHP = 100;//低於100%血量的時候才會釋放
            skillinfo.MinHP = 0;//高於0%血量的時候才會釋放
            ai.SkillOfShort.Add(3323, skillinfo);//將這個技能加進進程技能表
            return ai;
        }
    }
}

