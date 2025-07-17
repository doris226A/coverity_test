
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.BladeMaster
{
    /// <summary>
    ///  狂戰士（バーサーク）
    /// </summary>
    public class P_BERSERK : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
        }
        /*   #region ISkill Members
           public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
           {
               return 0;
           }
           /*  public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
             {
                 int lowHP = (int)(sActor.MaxHP * (0.05f + 0.05f * level));
                   bool active = false;
                   if (sActor.HP <= lowHP)
                   {
                       active = true;
                 }
                 DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "P_BERSERK", active);
                 skill.OnAdditionStart += this.StartEventHandler;
                 skill.OnAdditionEnd += this.EndEventHandler;
                 SkillHandler.ApplyAddition(sActor, skill);
             }*/
        /*    public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
            {
                int lowHP = (int)(sActor.MaxHP * (0.05f + 0.05f * level));
                bool active = false;
                if (sActor.HP <= lowHP)
                {
                    active = true;
                }
                DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "P_BERSERK", active);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(sActor, skill);
            }
            void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
            {
                if (skill.Activated)
                {
                    Berserk bs = new Berserk(skill.skill, actor, 30000);
                    SkillHandler.ApplyAddition(actor, bs);
                }
            }
            void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
            {
            }
            /*  void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
              {
                  if (skill.Activated)
                  {
                      Berserk bs = new Berserk(skill.skill, actor, 30000);
                      SkillHandler.ApplyAddition(actor, bs);

                      // 在狂戰士狀態下，添加亂走邏輯
                      if (actor is ActorPC)
                      {
                          ActorPC pc = (ActorPC)actor;

                          byte x, y;
                          SkillHandler.Instance.GetTBackPos(Manager.MapManager.Instance.GetMap(actor.MapID), actor, out x, out y);

                          short[] pos = new short[2];
                          Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                          pos[0] = SagaLib.Global.PosX8to16(x, map.Width);
                          pos[1] = SagaLib.Global.PosY8to16(y, map.Height);
                          map.MoveActor(Map.MOVE_TYPE.START, actor, pos, (ushort)(actor.Dir / 45), 20000, true, SagaLib.MoveType.WALK);
                       //   mob.AI.OnPathInterupt();

                         // Stun skill = new Stun(args.skill, act, lifetime);
                        //  SkillHandler.ApplyAddition(act, skill);




                          // 實現亂走的邏輯，可以根據遊戲需求調整
                          Random random = new Random();
                          short[] randomPos = new short[2];
                          randomPos[0] = (short)(actor.X + random.Next(-10, 10));
                          randomPos[1] = (short)(actor.Y + random.Next(-10, 10));


                          // 移動到隨機位置
                          // MoveActor(randomPos);
                        //  Map map;
                          map = Manager.MapManager.Instance.GetMap(actor.MapID);
                          map.MoveActor(Map.MOVE_TYPE.START, actor, randomPos, 0, actor.Speed);

                        //  Stun skill = new Stun(args.skill, act, lifetime);
                         // SkillHandler.ApplyAddition(act, skill);
                      }
                  }
              }
              void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
              {
              }*/
        //#endregion
    }
}

        /*   #region ISkill Members
           public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
           {
               return 0;
           }
           /*  public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
             {
                 int lowHP = (int)(sActor.MaxHP * (0.05f + 0.05f * level));
                   bool active = false;
                   if (sActor.HP <= lowHP)
                   {
                       active = true;
                 }
                 DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "P_BERSERK", active);
                 skill.OnAdditionStart += this.StartEventHandler;
                 skill.OnAdditionEnd += this.EndEventHandler;
                 SkillHandler.ApplyAddition(sActor, skill);
             }*/
        /*    public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
            {
                int lowHP = (int)(sActor.MaxHP * (0.05f + 0.05f * level));
                bool active = false;
                if (sActor.HP <= lowHP)
                {
                    active = true;
                }
                DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "P_BERSERK", active);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(sActor, skill);
            }
            void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
            {
                if (skill.Activated)
                {
                    Berserk bs = new Berserk(skill.skill, actor, 30000);
                    SkillHandler.ApplyAddition(actor, bs);
                }
            }
            void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
            {
            }
            /*  void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
              {
                  if (skill.Activated)
                  {
                      Berserk bs = new Berserk(skill.skill, actor, 30000);
                      SkillHandler.ApplyAddition(actor, bs);

                      // 在狂戰士狀態下，添加亂走邏輯
                      if (actor is ActorPC)
                      {
                          ActorPC pc = (ActorPC)actor;

                          byte x, y;
                          SkillHandler.Instance.GetTBackPos(Manager.MapManager.Instance.GetMap(actor.MapID), actor, out x, out y);

                          short[] pos = new short[2];
                          Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                          pos[0] = SagaLib.Global.PosX8to16(x, map.Width);
                          pos[1] = SagaLib.Global.PosY8to16(y, map.Height);
                          map.MoveActor(Map.MOVE_TYPE.START, actor, pos, (ushort)(actor.Dir / 45), 20000, true, SagaLib.MoveType.WALK);
                       //   mob.AI.OnPathInterupt();

                         // Stun skill = new Stun(args.skill, act, lifetime);
                        //  SkillHandler.ApplyAddition(act, skill);




                          // 實現亂走的邏輯，可以根據遊戲需求調整
                          Random random = new Random();
                          short[] randomPos = new short[2];
                          randomPos[0] = (short)(actor.X + random.Next(-10, 10));
                          randomPos[1] = (short)(actor.Y + random.Next(-10, 10));


                          // 移動到隨機位置
                          // MoveActor(randomPos);
                        //  Map map;
                          map = Manager.MapManager.Instance.GetMap(actor.MapID);
                          map.MoveActor(Map.MOVE_TYPE.START, actor, randomPos, 0, actor.Speed);

                        //  Stun skill = new Stun(args.skill, act, lifetime);
                         // SkillHandler.ApplyAddition(act, skill);
                      }
                  }
              }
              void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
              {
              }*/
        //#endregion
