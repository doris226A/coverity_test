
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.Manager;
using SagaDB.Map;

namespace SagaMap.Skill.SkillDefinations.Marionest
{
    /// <summary>
    /// 召喚活動木偶（マリオネット召喚）
    /// </summary>
    public class MarioCtrl : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            Map map = MapManager.Instance.GetMap(sActor.MapID);

            if (map.Info.Flag.Test(MapFlags.Healing))
            {
                return -6;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 25000;
            uint[] MobID ={0,   26320000//寵物泰迪
                               ,26280000//寵物皮諾
                               ,26290000//寵物愛伊斯
                               ,26300000//寵物塔依
                               ,26310000//寵物虎姆拉
                             };
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            ActorMob mob = map.SpawnMob(MobID[level], SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 2500, sActor);

            ActorPC pc = (ActorPC)sActor;
            if (pc.Skills2_2.ContainsKey(981) || pc.DualJobSkill.Exists(x => x.ID == 981))
            {
                float factor = 0f;
                var duallv = 0;
                if (pc.DualJobSkill.Exists(x => x.ID == 981))
                    duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 981).Level;

                var mainlv = 0;
                if (pc.Skills2_2.ContainsKey(981))
                    mainlv = pc.Skills2_2[981].Level;

                factor += (1.0f + 0.05f * Math.Max(duallv, mainlv));

                int max_atk1_add = (int)(mob.Status.max_atk_bs * factor);
                mob.Status.max_atk1_skill += (short)max_atk1_add; ;
                mob.Status.max_atk2_skill += (short)max_atk1_add;
                mob.Status.max_atk3_skill += (short)max_atk1_add;
                mob.Status.min_atk1_skill += (short)max_atk1_add;
                mob.Status.min_atk2_skill += (short)max_atk1_add;
                mob.Status.min_atk3_skill += (short)max_atk1_add;
                int min_matk_add = (int)(mob.Status.min_matk * factor);
                mob.Status.min_matk_skill += (short)min_matk_add;
            }


            ((ActorEventHandlers.MobEventHandler)mob.e).AI.Mode = new Mob.AIMode(1);//主動戰鬥
            MarioCtrlBuff skill = new MarioCtrlBuff(args.skill, sActor, lifetime, mob);
            SkillHandler.ApplyAddition(sActor, skill);
            if (!sActor.Status.Additions.ContainsKey("MarioCtrlMove"))
            {
                CannotMove cm = new CannotMove(args.skill, sActor, lifetime);
                SkillHandler.ApplyAddition(sActor, cm);
            }
        }
        public class MarioCtrlBuff : DefaultBuff
        {
            private ActorMob mob;
            public MarioCtrlBuff(SagaDB.Skill.Skill skill, Actor actor, int lifetime, ActorMob mob)
                : base(skill, actor, "MarioCtrl", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.mob = mob;
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
                ActorPC pc = (ActorPC)actor;
                switch (mob.MobID)
                {
                    case 26320000:
                        pc.TInt["MarioCtrl"] = 10050700;
                        //pc.Marionette.ID = 10050700;
                        break;
                    case 26280000:
                        pc.TInt["MarioCtrl"] = 10027000;
                        //pc.Marionette.ID = 10027000;
                        break;
                    case 26290000:
                        pc.TInt["MarioCtrl"] = 10019300;
                        //pc.Marionette.ID = 10019300;
                        break;
                    case 26300000:
                        pc.TInt["MarioCtrl"] = 10030001;
                        //pc.Marionette.ID = 10030001;
                        break;
                    case 26310000:
                        pc.TInt["MarioCtrl"] = 10021700;
                        //pc.Marionette.ID = 10021700;
                        break;
                }

            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                mob.ClearTaskAddition();
                map.DeleteActor(mob);
                ActorPC pc = (ActorPC)actor;
                //pc.Marionette.ID = 0;
                pc.TInt["MarioCtrl"] = 0;
            }
        }
        #endregion
    }
}

