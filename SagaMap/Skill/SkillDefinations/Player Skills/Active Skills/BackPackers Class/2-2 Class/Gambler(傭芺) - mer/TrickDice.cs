
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 欺詐骰子（トリックダイス）
    /// </summary>
    public class TrickDice : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            int num = SagaLib.Global.Random.Next(1, 6);
            switch (num)
            {
                case 1:
                    map.SendEffect(sActor, args.x, args.y, 4523);
                    switch (SagaLib.Global.Random.Next(1, 6))
                    {
                        case 1:
                            ALL_A(sActor, args, 7664);
                            break;
                        case 2:
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(9000, 1, 0));//濕滑陷阱
                            ONE_濕滑陷阱(sActor, args);
                            break;
                        case 3:
                            ONE_交叉火力(sActor, args);
                            break;
                        case 4:
                            ONE_毒氣泡(sActor, args);
                            break;
                        case 5:
                            ONE_劇毒爆炸(sActor, args);
                            break;
                        case 6:
                            ONE_猛毒眩暈氣體(sActor, args);
                            break;
                    }
                    break;
                case 2:
                    map.SendEffect(sActor, args.x, args.y, 4524);
                    switch (SagaLib.Global.Random.Next(1, 6))
                    {
                        case 1:
                            ALL_A(sActor, args, 7665);
                            break;
                        case 2:
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2352, 1, 0));//星辰亂舞
                            TWO_星辰亂舞(sActor, args);
                            break;
                        case 3:
                            TWO_鬼哭神號(sActor, args);
                            break;
                        case 4:
                            TWO_劫雷滅世(sActor, args);
                            break;
                        case 5:
                            TWO_衝擊波(sActor, args);
                            break;
                        case 6:
                            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, SagaLib.Global.PosX16to8(sActor.X, map.Width), SagaLib.Global.PosY16to8(sActor.Y, map.Width), 9916);
                            break;
                    }
                    break;
                case 3:
                    map.SendEffect(sActor, args.x, args.y, 4525);
                    switch (SagaLib.Global.Random.Next(1, 6))
                    {
                        case 1:
                            ALL_A(sActor, args, 7666);
                            break;
                        case 2:
                            THREE_冰霜之翼破(sActor, args);
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(7529, 1, 0));//方陣
                            break;
                        case 3:
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(6312, 1, 0));//催眠曲ララバイ
                            THREE_催眠曲(sActor, args);
                            break;
                        case 4:
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3274, 3, 0));//束手就擒ヒンダーハンド
                            THREE_束手就擒(sActor, args);
                            break;
                        case 5:
                            THREE_猛毒慢速氣體(sActor, args);
                            break;
                        case 6:
                            break;
                    }
                    break;
                case 4:
                    map.SendEffect(sActor, args.x, args.y, 4526);
                    switch (SagaLib.Global.Random.Next(1, 6))
                    {
                        case 1:
                            ALL_A(sActor, args, 7667);
                            break;
                        case 2:
                            FOUR_獅子吼(sActor, args);
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2366, 3, 0));//獅子吼
                            break;
                        case 3:
                            FOUR_重力刃(sActor, args);
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3318, 1, 0));//重力刃
                            break;
                        case 4:
                            FOUR_冷凍氣體(sActor, args);
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2366, 3, 0));//獅子吼
                            break;
                        case 5:
                            FOUR_交叉水氣(sActor, args);
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2366, 3, 0));//獅子吼
                            break;
                        case 6:
                            FOUR_猛毒針(sActor, args);
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2366, 3, 0));//獅子吼
                            break;
                    }
                    break;
                case 5:
                    map.SendEffect(sActor, args.x, args.y, 4527);
                    switch (SagaLib.Global.Random.Next(1, 6))
                    {
                        case 1:
                            ALL_A(sActor, args, 7668);
                            break;
                        case 2:
                            FIVE_一齊發射(sActor, args);
                            break;
                        case 3:
                            FIVE_啊(sActor, args);
                            break;
                        case 4:
                            FIVE_解除憑依(sActor, args);
                            break;
                        case 5:
                            FIVE_沉默陷阱(sActor, args);
                            break;
                        case 6:
                            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, SagaLib.Global.PosX16to8(sActor.X, map.Width), SagaLib.Global.PosY16to8(sActor.Y, map.Width), 9927);
                            break;
                    }
                    break;
                case 6:
                    map.SendEffect(sActor, args.x, args.y, 4528);
                    switch (SagaLib.Global.Random.Next(1, 6))
                    {
                        case 1:
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3310, 1, 0));//黑暗火焰
                            SIX_黑暗火焰(sActor, args);
                            break;
                        case 2:
                            //args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(3308, 5, 0));//範圍治癒
                            SIX_範圍治癒(sActor, args);
                            break;
                        case 3:
                            SIX_遲緩(sActor, args);
                            break;
                        case 4:
                            SIX_石化(sActor, args);
                            break;
                        case 5:
                            SIX_交叉土(sActor, args);
                            break;
                        case 6:
                            SIX_交叉風(sActor, args);
                            break;
                    }
                    break;
            }
        }
        #endregion
        //燃燒的路系列
        void ALL_A(Actor sActor, SkillArg args, uint skillid)
        {
            byte[] posX = new byte[3];
            byte[] posY = new byte[3];
            SkillHandler.Instance.GetRelatedPos(sActor, 0, 0, out posX[0], out posY[0]);
            switch (SkillHandler.Instance.GetDirection(sActor))
            {
                case SkillHandler.ActorDirection.North:
                case SkillHandler.ActorDirection.NorthEast:
                    SkillHandler.Instance.GetRelatedPos(sActor, 0, 3, out posX[1], out posY[1]);
                    SkillHandler.Instance.GetRelatedPos(sActor, 0, 6, out posX[2], out posY[2]);
                    break;
                case SkillHandler.ActorDirection.South:
                case SkillHandler.ActorDirection.SouthEast:
                    SkillHandler.Instance.GetRelatedPos(sActor, 0, -3, out posX[1], out posY[1]);
                    SkillHandler.Instance.GetRelatedPos(sActor, 0, -6, out posX[2], out posY[2]);
                    break;
                case SkillHandler.ActorDirection.West:
                case SkillHandler.ActorDirection.NorthWest:
                    SkillHandler.Instance.GetRelatedPos(sActor, -3, 0, out posX[1], out posY[1]);
                    SkillHandler.Instance.GetRelatedPos(sActor, -6, 0, out posX[2], out posY[2]);
                    break;
                case SkillHandler.ActorDirection.East:
                case SkillHandler.ActorDirection.SouthWest:
                    SkillHandler.Instance.GetRelatedPos(sActor, 3, 0, out posX[1], out posY[1]);
                    SkillHandler.Instance.GetRelatedPos(sActor, 6, 0, out posX[2], out posY[2]);
                    break;
            }

            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(skillid, 1, 0, posX[0], posY[0]));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(skillid, 1, 0, posX[1], posY[1]));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(skillid, 1, 0, posX[2], posY[2]));
        }

        void ONE_交叉火力(Actor sActor, SkillArg args)
        {
            for (int i = 0; i < 3; i++)
            {
                args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(7674, 1, 0));
            }
        }

        void ONE_濕滑陷阱(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    Stiff skill = new Stiff(args.skill, act, 5000);
                    SkillHandler.ApplyAddition(act, skill);
                    realAffected.Add(act);
                }
            }
            map.SendEffect(sActor, 5099);
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.1f);
        }

        void ONE_毒氣泡(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Poison, 30))
                    {
                        Additions.Global.Poison skill = new SagaMap.Skill.Additions.Global.Poison(args.skill, act, 10000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            map.SendEffect(sActor, 5093);
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.1f);
        }

        void ONE_劇毒爆炸(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                    SkillHandler.Instance.PushBack(sActor, act, 3);
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stun, 30))
                    {
                        Additions.Global.Stun skill5 = new SagaMap.Skill.Additions.Global.Stun(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill5);
                    }
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.0f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5266);
        }

        void ONE_猛毒眩暈氣體(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stun, 30))
                    {
                        Additions.Global.DeadlyPosion nskill = new SagaMap.Skill.Additions.Global.DeadlyPosion(args.skill, act, "DeadlyPosion", 10000, 2000);
                        SkillHandler.ApplyAddition(act, nskill);
                        Additions.Global.Stun skill = new SagaMap.Skill.Additions.Global.Stun(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5109);
        }

        void TWO_星辰亂舞(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Confuse, 30))
                    {
                        Additions.Global.Confuse skill5 = new SagaMap.Skill.Additions.Global.Confuse(args.skill, act, 4000);
                        SkillHandler.ApplyAddition(act, skill5);
                    }
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.0f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5268);
        }

        void TWO_鬼哭神號(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 300, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Frosen, 30))
                    {
                        Additions.Global.Freeze skill = new SagaMap.Skill.Additions.Global.Freeze(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 4007);
        }

        void TWO_劫雷滅世(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 300, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor i in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                    realAffected.Add(i);
            }
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SagaLib.Elements.Wind, 1.8f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5258);
        }

        void TWO_衝擊波(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 300, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    Stiff skill = new Stiff(args.skill, act, 5000);
                    SkillHandler.ApplyAddition(act, skill);
                    realAffected.Add(act);
                }
            }
            map.SendEffect(sActor, 5094);
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 0.8f);
        }

        //ファランクス
        void THREE_冰霜之翼破(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    Stiff skill = new Stiff(args.skill, act, 5000);
                    SkillHandler.ApplyAddition(act, skill);
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.1f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 4098);
        }
        //ララバイ
        void THREE_催眠曲(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Sleep, 30))
                    {
                        Additions.Global.Sleep skill = new SagaMap.Skill.Additions.Global.Sleep(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.1f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5095);
        }
        //束手就擒ヒンダーハンド
        void THREE_束手就擒(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.鈍足, 30))
                    {
                        MoveSpeedDown skill = new MoveSpeedDown(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.1f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5180);
        }

        void THREE_猛毒慢速氣體(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.鈍足, 30))
                    {
                        Additions.Global.DeadlyPosion nskill = new SagaMap.Skill.Additions.Global.DeadlyPosion(args.skill, act, "DeadlyPosion", 10000, 2000);
                        SkillHandler.ApplyAddition(act, nskill);
                        Additions.Global.MoveSpeedDown skill = new SagaMap.Skill.Additions.Global.MoveSpeedDown(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5109);
        }

        void FOUR_獅子吼(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 250, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stun, 30))
                    {
                        Stun skill = new Stun(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 4216);
        }

        void FOUR_重力刃(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 250, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.鈍足, 30))
                    {
                        Additions.Global.MoveSpeedDown skill = new SagaMap.Skill.Additions.Global.MoveSpeedDown(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SagaLib.Elements.Earth, 3.5f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5306);
        }

        void FOUR_冷凍氣體(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Frosen, 30))
                    {
                        Additions.Global.Freeze skill = new SagaMap.Skill.Additions.Global.Freeze(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5104);
        }

        void FOUR_交叉水氣(Actor sActor, SkillArg args)
        {
            for (int i = 0; i < 3; i++)
            {
                args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(7675, 1, 0));
            }
        }

        void FOUR_猛毒針(Actor sActor, SkillArg args)
        {
            int lifetime = 5000;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 200, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            foreach (Actor i in realAffected)
            {
                Additions.Global.Poison skill = new SagaMap.Skill.Additions.Global.Poison(args.skill, i, lifetime);
                SkillHandler.ApplyAddition(i, skill);
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5111);
        }

        void FIVE_沉默陷阱(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Silence, 30))
                    {
                        Additions.Global.Silence skill = new SagaMap.Skill.Additions.Global.Silence(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5102);
        }

        void FIVE_解除憑依(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 200, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (act.type == ActorType.PC)
                    {
                        if (SagaLib.Global.Random.Next(0, 99) < 50)
                        {
                            SkillHandler.Instance.PossessionCancel((ActorPC)act, SagaLib.PossessionPosition.NONE);
                        }
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5112);
        }

        void FIVE_啊(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 300, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Confuse, 30))
                    {
                        Additions.Global.Confuse skill = new SagaMap.Skill.Additions.Global.Confuse(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, 1.1f);
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 4020);
        }

        void FIVE_一齊發射(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 700, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        realAffected.Add(act);
                    }
                }
            }
            if (realAffected.Count > 0)
            {
                List<Actor> finalAffected = new List<Actor>();
                for (int i = 0; i < realAffected.Count; i++)
                {
                    finalAffected.Add(realAffected[SagaLib.Global.Random.Next(0, realAffected.Count - 1)]);
                }
                SkillHandler.Instance.PhysicalAttack(sActor, finalAffected, args, sActor.WeaponElement, 0.8f);
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 8012);
        }


        void SIX_黑暗火焰(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            //SkillArg arg = new SkillArg();
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, Elements.Dark, 3.0f);
            map.SendEffect(sActor, 5173);
        }

        void SIX_範圍治癒(Actor sActor, SkillArg args)
        {
            float factor = -(1f + 0.4f * 3);
            float damagefactor = 1f + 0.4f * 3;

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            List<Actor> damageaffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (act.Buff.Undead)
                {
                    damageaffected.Add(act);
                    continue;
                }
                if (act.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)act;
                    if (pc.PossessionTarget == 0)
                    {
                        realAffected.Add(act);
                    }
                }
                if (act.type == ActorType.PET || act.type == ActorType.PARTNER || act.type == ActorType.SHADOW)
                {
                    realAffected.Add(act);
                }
                if (act.type == ActorType.MOB)
                {
                    ActorMob m = (ActorMob)act;
                    if (m.BaseData.undead)
                        damageaffected.Add(act);
                }
            }

            SkillHandler.Instance.MagicAttack(sActor, damageaffected, args, SagaLib.Elements.Holy, damagefactor);
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SkillHandler.DefType.IgnoreAll, SagaLib.Elements.Holy, factor);
            map.SendEffect(sActor, 5297);
        }

        void SIX_遲緩(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.鈍足, 100))
                    {
                        Additions.Global.MoveSpeedDown skill = new SagaMap.Skill.Additions.Global.MoveSpeedDown(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5110);
        }

        void SIX_石化(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stone, 100))
                    {
                        Additions.Global.Stone skill = new SagaMap.Skill.Additions.Global.Stone(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);
                    }
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5098);
        }

        void SIX_交叉土(Actor sActor, SkillArg args)
        {
            for (int i = 0; i < 3; i++)
            {
                args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(7677, 1, 0));
            }
        }

        void SIX_交叉風(Actor sActor, SkillArg args)
        {
            for (int i = 0; i < 3; i++)
            {
                args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(7676, 1, 0));
            }
        }
    }
}