using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaMap.Skill.Additions.Global;
using SagaLib;
using SagaMap.Mob;

namespace SagaMap.Skill.SkillDefinations.BladeMaster
{
    /// <summary>
    /// 無拍子（無拍子）
    /// </summary>
    public class Transporter : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {

            //map.MoveActor(Map.MOVE_TYPE.START, sActor, pos, sActor.Dir, 20000, true, SagaLib.MoveType.QUICKEN);
            short[] pos = new short[2];
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            pos[0] = SagaLib.Global.PosX8to16(args.x, map.Width);
            pos[1] = SagaLib.Global.PosY8to16(args.y, map.Height);

            Activator timer = new Activator(sActor, pos);
            timer.Activate();
        }

        #endregion

        private class Activator : MultiRunTask
        {
            Map map;
            Actor sActor;
            short[] pos;

            public Activator(Actor caster, short[] pos)
            {
                this.sActor = caster;
                this.pos = pos;
                map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                this.period = 2000;
                this.dueTime = 400;
            }

            public override void CallBack()
            {
                try
                {
                    map.MoveActor(Map.MOVE_TYPE.START, sActor, pos, sActor.Dir, 10000, true, SagaLib.MoveType.QUICKEN);
                    this.Deactivate();
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }
        }
    }
}
