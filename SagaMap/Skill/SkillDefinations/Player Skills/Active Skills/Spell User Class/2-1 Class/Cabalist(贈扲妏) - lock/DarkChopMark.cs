
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Cabalist
{
    /// <summary>
    /// 黑暗苦痛（ダークペイン）
    /// </summary>
    public class DarkChopMark : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.Slave.Count == 0)
            {
                return -12;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            float factor = 1.5f + 0.3f * level;
            List<Actor> actors = new List<Actor>();
            foreach (Actor i in sActor.Slave.ToArray())
            {
                if (i.type != ActorType.SKILL)
                    continue;
                foreach (Actor j in map.GetActorsArea(i, 100, false))
                {
                    actors.Add(j);
                }
                Manager.MapManager.Instance.GetMap(i.MapID).SendEffect(i, PosX16to8W(i.X, map.Width), PosY16to8H(i.Y, map.Height), 5043);
                map.DeleteActor(i);
            }
            List<Actor> affected = new List<Actor>();
            foreach (Actor i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {
                    affected.Add(i);

                }
            }
            sActor.Slave.Clear();
            SkillHandler.Instance.MagicAttack(sActor, affected, args, SkillHandler.DefType.IgnoreAll, SagaLib.Elements.Dark, factor);

        }
        byte PosX16to8W(short pos, ushort width)
        {
            double val = ((double)width / 2);
            double tmp = (((float)(pos - 50) / 100) + val);
            if (tmp < 0)
                tmp = 0;
            return (byte)tmp;
        }

        byte PosY16to8H(short pos, ushort height)
        {
            double val = ((double)height / 2);
            double tmp = (((float)-(pos + 50) / 100) + val);
            if (tmp < 0)
                tmp = 0;
            return (byte)tmp;
        }
        #endregion
    }
}