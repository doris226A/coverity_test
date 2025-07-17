using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.AnotherBookPaper
{
    /// <summary>
    /// コンチェルタート
    /// </summary>
    public class Concerttate : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            //float firstfactor = 0, conbifactor = 0;
            ////args.type = ATTACK_TYPE.SLASH;
            //firstfactor = new float[] { 0, 10.0f, 10.0f, 10.0f, 13.0f, 13.0f, 13.0f, 14.0f, 14.0f, 14.0f, 15.0f, 15.0f, 15.0f, 17.0f, 18.0f, 20.0f }[level];
            //conbifactor = new float[] { 0, 10.0f, 10.0f, 10.0f, 15.0f, 15.0f, 15.0f, 20.0f, 20.0f, 20.0f, 24.0f, 24.0f, 24.0f, 27.0f, 27.0f, 30.0f }[level];
            //List<Actor> dest = new List<Actor>();
            //for (int i = 0; i < 7; i++)
            //    dest.Add(dActor);
            //args.delayRate = 4.5f;
            //SkillHandler.Instance.PhysicalAttack(sActor, dest, args, sActor.WeaponElement, conbifactor);

            float firstfactor = 0, conbifactor = 0;
            firstfactor = new float[] { 0, 10.0f, 10.0f, 10.0f, 13.0f, 13.0f, 13.0f, 14.0f, 14.0f, 14.0f, 15.0f, 15.0f, 15.0f, 17.0f, 18.0f, 20.0f }[level];

            float endfactor = new float[] { 0, 22.0f, 23.0f, 24.0f, 26.0f, 27.0f, 28.0f, 30.0f, 31.0f, 33.0f, 35.0f, 37.0f, 39.0f, 43.0f, 46.0f, 50.0f }[level];
            sActor.MuSoUCount = 0;

            for (int i = 1; i <= 9; i++)
            {
                AutoCastInfo aci = SkillHandler.Instance.CreateAutoCastInfo(2589, level, 100);
                args.autoCast.Add(aci);
            }
            //SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, firstfactor);
        }

        #endregion
    }
}
