using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
using static SagaMap.Skill.SkillHandler;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 小丑-魔法
    /// </summary>
    public class Joker_Magic : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = new float[] { 0, 1.8f, 2.1f, 2.4f, 2.7f, 3.0f }[level];
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 250, new Actor[] { });
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            List<float> Dmgnumber = new List<float>();
            int AttackAffect = 0;
            foreach (Actor i in realAffected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {

                    int FireDamage = SkillHandler.Instance.CalcDamage(false, sActor, i, args, SkillHandler.DefType.MDef, SagaLib.Elements.Fire, 100, factor);
                    int WaterDamage = SkillHandler.Instance.CalcDamage(false, sActor, i, args, SkillHandler.DefType.MDef, SagaLib.Elements.Water, 100, factor);
                    int WindDamage = SkillHandler.Instance.CalcDamage(false, sActor, i, args, SkillHandler.DefType.MDef, SagaLib.Elements.Wind, 100, factor);
                    int EarthDamage = SkillHandler.Instance.CalcDamage(false, sActor, i, args, SkillHandler.DefType.MDef, SagaLib.Elements.Earth, 100, factor);
                    int DarkDamage = SkillHandler.Instance.CalcDamage(false, sActor, i, args, SkillHandler.DefType.MDef, SagaLib.Elements.Dark, 100, factor);
                    int HolyDamage = SkillHandler.Instance.CalcDamage(false, sActor, i, args, SkillHandler.DefType.MDef, SagaLib.Elements.Holy, 100, factor);
                    AttackAffect = FireDamage + WaterDamage + WindDamage + EarthDamage + DarkDamage + HolyDamage;
                    SkillHandler.Instance.CauseDamage(sActor, i, AttackAffect);
                    SkillHandler.Instance.ShowVessel(i, AttackAffect);
                }
            }
        }
        #endregion
    }
}
