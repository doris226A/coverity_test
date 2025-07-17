using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;


namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    /// <summary>
    /// ウルフオブチェイン
    /// </summary>
    class WolfeOfChain : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = new float[] { 0, 10.0f, 15.0f, 20.5f, 26.0f, 31.5f, 37.0f, 42.5f, 48.0f, 53.5f, 59.0f, 64.5f, 70.0f, 75.5f, 81.0f, 86.5f }[level];

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> actors = map.GetActorsArea(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 200, true);
            List<Actor> affected = new List<Actor>();
            short[] pos = new short[2];
            pos[0] = sActor.X;
            pos[1] = sActor.Y;
            foreach (Actor i in actors)
            {
                //这里,应该是判定为可以攻击的对象才会被拉过来.
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {
                    affected.Add(i);
                    map.MoveActor(Map.MOVE_TYPE.START, i, pos, i.Dir, 20000, true, MoveType.BATTLE_MOTION);
                }
            }

            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
