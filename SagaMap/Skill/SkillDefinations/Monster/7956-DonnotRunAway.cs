using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// こら……逃げるなよ
    /// </summary>
    public class DonnotRunAway : ISkill
    {
        #region ISkill 成員

        public int TryCast(SagaDB.Actor.ActorPC sActor, SagaDB.Actor.Actor dActor, SkillArg args)
        {
            if (sActor.Status.Additions.ContainsKey("SpeedHit"))
                return -30;
            else
                return 0;
        }
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            int[] lifetime = { 0, 1000, 1250, 1500, 1750, 2000 };
            DefaultBuff skill2 = new DefaultBuff(args.skill, sActor, "SpeedHit", lifetime[level]);
            SkillHandler.ApplyAddition(sActor, skill2);
            args.type = ATTACK_TYPE.SLASH;
            float factor = 5f;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);

            short[] pos = new short[2];
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            pos[0] = dActor.X;
            pos[1] = dActor.Y;
            //map.MoveActor(Map.MOVE_TYPE.START, sActor, pos, 20000, 1000, true);

            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Stun, 50))
            {
                Additions.Global.Stun skill = new SagaMap.Skill.Additions.Global.Stun(args.skill, dActor, 1000);
                SkillHandler.ApplyAddition(dActor, skill);
            }

            map.MoveActor(Map.MOVE_TYPE.START, sActor, pos, sActor.Dir, 20000, true, SagaLib.MoveType.BATTLE_MOTION);
            int a = SagaLib.Global.Random.Next(0, 99);
            if (a < 50)
                args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(7957, level, 2000));
            else
                args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(7764, level, 2000));

        }
        #endregion
    }
}
