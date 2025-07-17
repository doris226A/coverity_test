using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Druid
{
    /// <summary>
    /// 強身健體（ラウズボディ）
    /// </summary>
    public class BMAG_INT_DEX_UP : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //SkillArg SkillFireBolt = new SkillArg();
            //int[] lifetime ={15, 20, 25, 27, 30};
            //DefaultBuff skill = new DefaultBuff(args.skill, dActor, "STR_VIT_AGI_UP", lifetime[level - 1] * 1000);
            int lifetime = 25000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "MAG_INT_DEX_UP", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            if (dActor == sActor)
            {
                Map map = Manager.MapManager.Instance.GetMap(dActor.MapID);
                EffectArg arg2 = new EffectArg();
                arg2.effectID = 5178;
                arg2.actorID = dActor.ActorID;
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg2, dActor, true);
            }
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int level = skill.skill.Level;
            short[] DEX = { 6, 8, 10, 12, 14 };
            short[] INT = { 6, 7, 8, 10, 11 };
            short[] MAG = { 5, 6, 7, 9, 10 };
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                short hit_melee = (short)(DEX[level - 1] + (short)Math.Floor((double)((float)DEX[level - 1] / 10.0f)) * 11 + actor.Level + 3);
                short cspd = (short)(DEX[level - 1] * 3 + Math.Floor(Math.Pow((short)((float)(DEX[level - 1] + 63) / 9.0f), 2)));
                actor.Status.hit_melee_skill += hit_melee;
                actor.Status.cspd_skill += cspd;

                short minmatk = (short)Math.Floor((double)MAG[level - 1] + Math.Pow(Math.Floor((double)((float)(MAG[level - 1] + 9) / 8)), 2) * (1.0f + Math.Floor((double)(INT[level - 1] * 1.2f)) / 320.0f));
                short maxmatk = (short)(MAG[level - 1] + Math.Pow(Math.Floor((double)((float)(MAG[level - 1] + 17) / 6.0f)), 2));
                short hit_ranged = (short)(INT[level - 1] + (short)Math.Floor((double)((float)INT[level - 1] / 10.0f)) * 11 + actor.Level + 3);
                short avoid_ranged = (short)((float)INT[level - 1] * 5.0f / 3.0f);

                actor.Status.min_matk_skill += minmatk;
                actor.Status.max_matk_skill += maxmatk;
                actor.Status.hit_ranged_skill += hit_ranged;
                actor.Status.avoid_ranged_skill += avoid_ranged;

                short mp = (short)(actor.MaxMP * (float)(MAG[level - 1] / 1000.0f * 5.0f));
                actor.Status.mp_skill += mp;

                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_hit_melee"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_hit_melee");
                skill.Variable.Add("MAG_INT_DEX_UP_hit_melee", hit_melee);
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_cspd"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_cspd");
                skill.Variable.Add("MAG_INT_DEX_UP_cspd", cspd);
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_minmatk"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_minmatk");
                skill.Variable.Add("MAG_INT_DEX_UP_minmatk", minmatk);
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_maxmatk"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_maxmatk");
                skill.Variable.Add("MAG_INT_DEX_UP_maxmatk", maxmatk);
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_hit_ranged"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_hit_ranged");
                skill.Variable.Add("MAG_INT_DEX_UP_hit_ranged", hit_ranged);
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_avoid_ranged"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_avoid_ranged");
                skill.Variable.Add("MAG_INT_DEX_UP_avoid_ranged", avoid_ranged);
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_mp"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_mp");
                skill.Variable.Add("MAG_INT_DEX_UP_mp", mp);
            }
            else
            {
                //DEX
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_DEX"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_DEX");
                skill.Variable.Add("MAG_INT_DEX_UP_DEX", DEX[level - 1]);
                actor.Status.dex_skill += DEX[level - 1];
                //INT
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_INT"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_INT");
                skill.Variable.Add("MAG_INT_DEX_UP_INT", INT[level - 1]);
                actor.Status.int_skill += INT[level - 1];
                //MAG
                if (skill.Variable.ContainsKey("MAG_INT_DEX_UP_MAG"))
                    skill.Variable.Remove("MAG_INT_DEX_UP_MAG");
                skill.Variable.Add("MAG_INT_DEX_UP_MAG", MAG[level - 1]);
                actor.Status.mag_skill += MAG[level - 1];
            }
            actor.Buff.MagUp = true;
            actor.Buff.INTUp = true;
            actor.Buff.DEXUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //ECOKEY 寵物用
            if (actor.type == ActorType.PARTNER)
            {
                actor.Status.hit_melee_skill -= (short)skill.Variable["MAG_INT_DEX_UP_hit_melee"];
                actor.Status.cspd_skill -= (short)skill.Variable["MAG_INT_DEX_UP_cspd"];

                actor.Status.min_matk_skill -= (short)skill.Variable["MAG_INT_DEX_UP_minmatk"];
                actor.Status.max_matk_skill -= (short)skill.Variable["MAG_INT_DEX_UP_maxmatk"];
                actor.Status.hit_ranged_skill -= (short)skill.Variable["MAG_INT_DEX_UP_hit_ranged"];
                actor.Status.avoid_ranged_skill -= (short)skill.Variable["MAG_INT_DEX_UP_avoid_ranged"];

                actor.Status.mp_skill -= (short)skill.Variable["MAG_INT_DEX_UP_mp"];
            }
            else
            {
                actor.Status.dex_skill -= (short)skill.Variable["MAG_INT_DEX_UP_DEX"];
                actor.Status.int_skill -= (short)skill.Variable["MAG_INT_DEX_UP_INT"];
                actor.Status.mag_skill -= (short)skill.Variable["MAG_INT_DEX_UP_MAG"];
            }
            actor.Buff.MagUp = false;
            actor.Buff.INTUp = false;
            actor.Buff.DEXUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}

