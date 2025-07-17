using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    /// <summary>
    /// コンチェルタート[接续技能]
    /// </summary>
    public class ConcerttateSEQ : ISkill
    {
        #region ISkill 成員

        public int TryCast(SagaDB.Actor.ActorPC sActor, SagaDB.Actor.Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            float firstfactor = new float[] { 0, 10.0f, 10.0f, 10.0f, 15.0f, 15.0f, 15.0f, 20.0f, 20.0f, 20.0f, 24.0f, 24.0f, 24.0f, 27.0f, 27.0f, 30.0f }[level];
            float conbifactor = new float[] { 0, 10.0f, 10.0f, 10.0f, 15.0f, 15.0f, 15.0f, 20.0f, 20.0f, 20.0f, 24.0f, 24.0f, 24.0f, 27.0f, 27.0f, 30.0f }[level];
            float factor = new float[] { 0, 22.0f, 23.0f, 24.0f, 26.0f, 27.0f, 28.0f, 30.0f, 31.0f, 33.0f, 35.0f, 37.0f, 39.0f, 43.0f, 46.0f, 50.0f }[level];
            if(sActor.MuSoUCount==0)
                SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, firstfactor);
            if (sActor.MuSoUCount == 8)
            {
                SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
                short[] pos = new short[2];
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                pos[0] = dActor.X;
                pos[1] = dActor.Y;
                map.MoveActor(Map.MOVE_TYPE.START, sActor, pos, sActor.Dir, 20000, true, SagaLib.MoveType.BATTLE_MOTION);
            }
            else
                SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, conbifactor);
            //SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            sActor.MuSoUCount++;


        }
        #endregion
    }
}
