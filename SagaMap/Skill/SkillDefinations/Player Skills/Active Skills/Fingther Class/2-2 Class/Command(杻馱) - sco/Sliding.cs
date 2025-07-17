

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Mob;

namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 滑動追擊（スライディング）
    /// </summary>
    public class Sliding : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float[] factors = { 0, 5.0f, 3.5f, 5.5f, 4.0f, 6.0f };
            short[] pos = new short[2];
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            if (sActor != dActor)
            {
                pos[0] = dActor.X;
                pos[1] = dActor.Y;
            }
            else
            {
                pos[0] = SagaLib.Global.PosX8to16(args.x, map.Width);
                pos[1] = SagaLib.Global.PosY8to16(args.y, map.Height);
            }
            List<SkillHandler.Point> path2 = SkillHandler.Instance.GetStraightPath(SagaLib.Global.PosX16to8(sActor.X, map.Width), SagaLib.Global.PosY16to8(sActor.Y, map.Height), SagaLib.Global.PosX16to8(pos[0], map.Width), SagaLib.Global.PosY16to8(pos[1], map.Height));

            SkillHandler.Point nodea = new SkillHandler.Point();
            nodea.x = SagaLib.Global.PosX16to8(pos[0], map.Width);
            nodea.y = SagaLib.Global.PosY16to8(pos[1], map.Height);
            path2.Add(nodea);
            bool find = false;
            List<Actor> realAffected = new List<Actor>();
            foreach (SkillHandler.Point i in path2)
            {
                if (find)
                    break;
                List<Actor> affected = map.GetActorsArea(SagaLib.Global.PosX8to16(i.x, map.Width), SagaLib.Global.PosY8to16(i.y, map.Height), 80, false);
                foreach (Actor act in affected)
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        realAffected.Add(act);
                        find = true;
                        pos[0] = SagaLib.Global.PosX8to16(i.x, map.Width);
                        pos[1] = SagaLib.Global.PosY8to16(i.y, map.Height);
                    }
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factors[level]);
            map.MoveActor(Map.MOVE_TYPE.START, sActor, pos, sActor.Dir, 20000, true, SagaLib.MoveType.QUICKEN);
        }
        #endregion
    }
}