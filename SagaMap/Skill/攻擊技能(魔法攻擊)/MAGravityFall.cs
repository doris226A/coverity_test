using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Enchanter
{
    /// <summary>
    /// 重力刃 (グラヴィティフォール)
    /// </summary>
    public class MAGravityFall : ISkill
    {
        bool MobUse;
        public MAGravityFall()
        {
            this.MobUse = false;
        }
        public MAGravityFall(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (MobUse)
            {
                level = 5;
            }

            int rate = 50;
            int lifetime = 6000;
            short[] pos = new short[2];
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ActorSkill actor = new ActorSkill(args.skill, sActor);
            //设定技能体位置
            actor.MapID = sActor.MapID;
            actor.X = SagaLib.Global.PosX8to16(args.x, map.Width);
            actor.Y = SagaLib.Global.PosY8to16(args.y, map.Height);
            //设定技能体的事件处理器，由于技能体不需要得到消息广播，因此创建个空处理器
            actor.e = new ActorEventHandlers.NullEventHandler();
            List<Actor> affected = map.GetActorsArea(actor, 250, false);
            List<Actor> realAffected = new List<Actor>();
            List<Actor> onlydamage = new List<Actor>();
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, dActor))
                {
                    if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.鈍足, rate))
                    {
                        Additions.Global.MoveSpeedDown skill = new SagaMap.Skill.Additions.Global.MoveSpeedDown(args.skill, dActor, lifetime);
                        SkillHandler.ApplyAddition(dActor, skill);
                    }
                    realAffected.Add(dActor);
            }
        }
        #endregion
    }
}
