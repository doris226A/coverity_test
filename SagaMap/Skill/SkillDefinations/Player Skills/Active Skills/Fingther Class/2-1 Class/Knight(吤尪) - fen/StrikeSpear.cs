
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;

using SagaLib;

namespace SagaMap.Skill.SkillDefinations.Knight
{
    public class StrikeSpear : ISkill
    {
        /// <summary>
        /// Defines the range.
        /// </summary>
        public Dictionary<SkillHandler.ActorDirection, List<int>> range = new Dictionary<SkillHandler.ActorDirection, List<int>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StrikeSpear"/> class.
        /// </summary>
        public StrikeSpear()
        {
            for (int index = 0; index < 8; ++index)
                this.range.Add((SkillHandler.ActorDirection)index, new List<int>());
            this.range[SkillHandler.ActorDirection.North].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(0, 1, 3));
            this.range[SkillHandler.ActorDirection.North].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(0, 2, 3));
            this.range[SkillHandler.ActorDirection.North].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(0, 3, 3));
            this.range[SkillHandler.ActorDirection.NorthEast].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(1, 1, 3));
            this.range[SkillHandler.ActorDirection.NorthEast].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(2, 2, 3));
            this.range[SkillHandler.ActorDirection.NorthEast].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(3, 3, 3));
            this.range[SkillHandler.ActorDirection.East].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(1, 0, 3));
            this.range[SkillHandler.ActorDirection.East].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(2, 0, 3));
            this.range[SkillHandler.ActorDirection.East].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(3, 0, 3));
            this.range[SkillHandler.ActorDirection.SouthEast].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(1, -1, 3));
            this.range[SkillHandler.ActorDirection.SouthEast].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(2, -2, 3));
            this.range[SkillHandler.ActorDirection.SouthEast].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(3, -3, 3));
            this.range[SkillHandler.ActorDirection.South].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(0, -1, 3));
            this.range[SkillHandler.ActorDirection.South].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(0, -2, 3));
            this.range[SkillHandler.ActorDirection.South].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(0, -3, 3));
            this.range[SkillHandler.ActorDirection.SouthWest].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-1, -1, 3));
            this.range[SkillHandler.ActorDirection.SouthWest].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-2, -2, 3));
            this.range[SkillHandler.ActorDirection.SouthWest].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-3, -3, 3));
            this.range[SkillHandler.ActorDirection.West].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-1, 0, 3));
            this.range[SkillHandler.ActorDirection.West].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-2, 0, 3));
            this.range[SkillHandler.ActorDirection.West].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-3, 0, 3));
            this.range[SkillHandler.ActorDirection.NorthWest].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-1, 1, 3));
            this.range[SkillHandler.ActorDirection.NorthWest].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-2, 2, 3));
            this.range[SkillHandler.ActorDirection.NorthWest].Add(Singleton<SkillHandler>.Instance.CalcPosHashCode(-3, 3, 3));
        }

        /// <summary>
        /// The TryCast.
        /// </summary>
        /// <param name="sActor">The sActor<see cref="ActorPC"/>.</param>
        /// <param name="dActor">The dActor<see cref="SagaDB.Actor.Actor"/>.</param>
        /// <param name="args">The args<see cref="SkillArg"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int TryCast(ActorPC sActor, SagaDB.Actor.Actor dActor, SkillArg args)
        {
            if (Skill.SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.SPEAR) || Skill.SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.RAPIER) || sActor.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
            {
                return 0;
            }
            return -5;
        }

        /// <summary>
        /// The Proc.
        /// </summary>
        /// <param name="sActor">The sActor<see cref="SagaDB.Actor.Actor"/>.</param>
        /// <param name="dActor">The dActor<see cref="SagaDB.Actor.Actor"/>.</param>
        /// <param name="args">The args<see cref="SkillArg"/>.</param>
        /// <param name="level">The level<see cref="byte"/>.</param>
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            float ATKBonus = (float)(1.79999995231628 + 0.200000002980232 * (double)level);
            uint key = 2138;
            ActorPC actorPc = (ActorPC)sActor;
            if (actorPc.Skills.ContainsKey(key) && actorPc.Skills[key].Level == (byte)5)
                ATKBonus = (float)(2.29999995231628 + 0.200000002980232 * (double)level);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<SagaDB.Actor.Actor> actorsArea = map.GetActorsArea(sActor, (short)300, false);
            List<SagaDB.Actor.Actor> dActor1 = new List<SagaDB.Actor.Actor>();
            SkillHandler.ActorDirection direction = Singleton<SkillHandler>.Instance.GetDirection(sActor);
            foreach (SagaDB.Actor.Actor dActor2 in actorsArea)
            {
                if (Singleton<SkillHandler>.Instance.CheckValidAttackTarget(sActor, dActor2))
                {
                    int XDiff;
                    int YDiff;
                    Singleton<SkillHandler>.Instance.GetXYDiff(map, sActor, dActor2, out XDiff, out YDiff);
                    if (this.range[direction].Contains(Singleton<SkillHandler>.Instance.CalcPosHashCode(XDiff, YDiff, 3)))
                        dActor1.Add(dActor2);
                }
            }
            Singleton<SkillHandler>.Instance.PhysicalAttack(sActor, dActor1, args, Elements.Neutral, ATKBonus);
        }
    }

    /// <summary>
    /// 比卡利之槍（ストライクスピア）
    /// </summary>
   /* public class StrikeSpear : ISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 2.0f+0.4f*level;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //向右的判定矩型
            short ox1 = 0;
            short oy1 = 100;
            short ox2 = 0;
            short oy2 = -100;
            short ox3 = 400;
            short oy3 = -100;
            short ox4 = 400;
            short oy4 = 100;
            //矩阵旋转
            double angel = map.DirChange(sActor.Dir) * Math.PI / 180;
            short x1 = (short)(ox1 * Math.Cos(angel) - oy1 * Math.Sin(angel));
            short y1 = (short)(ox1 * Math.Sin(angel) + oy1 * Math.Cos(angel));
            short x2 = (short)(ox2 * Math.Cos(angel) - oy2 * Math.Sin(angel));
            short y2 = (short)(ox2 * Math.Sin(angel) + oy2 * Math.Cos(angel));
            short x3 = (short)(ox3 * Math.Cos(angel) - oy3 * Math.Sin(angel));
            short y3 = (short)(ox3 * Math.Sin(angel) + oy3 * Math.Cos(angel));
            short x4 = (short)(ox4 * Math.Cos(angel) - oy4 * Math.Sin(angel));
            short y4 = (short)(ox4 * Math.Sin(angel) + oy4 * Math.Cos(angel));

            List<Actor> actors = map.GetRectAreaActors(
                (short)(x1 + sActor.X), (short)(y1 + sActor.Y),
                (short)(x2 + sActor.X), (short)(y2 + sActor.Y),
                (short)(x3 + sActor.X), (short)(y3 + sActor.Y),
                (short)(x4 + sActor.X), (short)(y4 + sActor.Y));
            //Logger.ShowError(actors.Count.ToString());
            List<Actor> affected = new List<Actor>();
            foreach (Actor i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                    affected.Add(i);
            }

            
            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, sActor.WeaponElement, factor);

        }
    }*/
}