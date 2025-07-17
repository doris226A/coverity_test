using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.Network.Client;

namespace SagaMap.Skill.SkillDefinations.Harvest
{
    /// <summary>
    /// ハーヴェスト
    /// </summary>
    public class Twine : ISkill
    {
        #region ISkill Members

        Actor SkillPlayer;
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            SkillPlayer = sActor;
            int lifetime = 30000 + level * 30000;
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            //获取设置中心9*9范围的怪物
            List<Actor> actors = map.GetActorsArea(sActor, 400, true);
            List<Actor> affected = new List<Actor>();
            args.affectedActors.Clear();
            foreach (Actor i in actors)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, i))
                {
                    TwineBuff skill = new TwineBuff(args, sActor, i, lifetime, 2000);
                    SkillHandler.ApplyAddition(i, skill);


                }
            }

        }
        public class TwineBuff : DefaultBuff
        {
            SkillArg args;
            Actor sActor;
            Map map;
            public TwineBuff(SkillArg args, Actor sActor, Actor actor, int lifetime, int period)
                : base(args.skill, actor, "Twine", lifetime, period)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.OnUpdate += this.UpdateTimeHandler;
                this.Period = 10;
                this.args = args.Clone();
                this.sActor = sActor;
                map = Manager.MapManager.Instance.GetMap(actor.MapID);
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
                //if (sActor.type == ActorType.PC)
                //{
                //    ActorPC pc = (ActorPC)sActor;
                //    MapClient.FromActorPC(pc).SendSystemMessage("初始化");
                //}
                int level = skill.skill.Level;
                int min_atk1_down = (int)(actor.Status.min_atk1 * 0.5f);
                int min_atk2_down = (int)(actor.Status.min_atk2 * 0.5f);
                int min_atk3_down = (int)(actor.Status.min_atk3 * 0.5f);
                int max_atk1_down = (int)(actor.Status.max_atk1 * 0.5f);
                int max_atk2_down = (int)(actor.Status.max_atk2 * 0.5f);
                int max_atk3_down = (int)(actor.Status.max_atk3 * 0.5f);
                int min_matk_down = (int)(actor.Status.min_matk * 0.5f);
                int max_matk_down = (int)(actor.Status.max_matk * 0.5f);
                if (skill.Variable.ContainsKey("Twine_min_atk1"))
                    skill.Variable.Remove("Twine_min_atk1");
                skill.Variable.Add("Twine_min_atk1", min_atk1_down);
                actor.Status.min_atk1_skill -= (short)min_atk1_down;

                if (skill.Variable.ContainsKey("Twine_min_atk2"))
                    skill.Variable.Remove("Twine_min_atk2");
                skill.Variable.Add("Twine_min_atk2", min_atk2_down);
                actor.Status.min_atk2_skill -= (short)min_atk2_down;

                if (skill.Variable.ContainsKey("Twine_min_atk3"))
                    skill.Variable.Remove("Twine_min_atk3");
                skill.Variable.Add("Twine_min_atk3", min_atk3_down);
                actor.Status.min_atk3_skill -= (short)min_atk3_down;

                if (skill.Variable.ContainsKey("Twine_max_atk1"))
                    skill.Variable.Remove("Twine_max_atk1");
                skill.Variable.Add("Twine_max_atk1", max_atk1_down);
                actor.Status.max_atk1_skill -= (short)max_atk1_down;

                if (skill.Variable.ContainsKey("Twine_max_atk2"))
                    skill.Variable.Remove("Twine_max_atk2");
                skill.Variable.Add("Twine_max_atk2", max_atk2_down);
                actor.Status.max_atk2_skill -= (short)max_atk2_down;

                if (skill.Variable.ContainsKey("Twine_max_atk3"))
                    skill.Variable.Remove("Twine_max_atk3");
                skill.Variable.Add("Twine_max_atk3", max_atk3_down);
                actor.Status.max_atk3_skill -= (short)max_atk3_down;

                if (skill.Variable.ContainsKey("Twine_min_matk"))
                    skill.Variable.Remove("Twine_min_matk");
                skill.Variable.Add("Twine_min_matk", min_matk_down);
                actor.Status.min_matk_skill -= (short)min_matk_down;

                if (skill.Variable.ContainsKey("Twine_max_matk"))
                    skill.Variable.Remove("Twine_max_matk");
                skill.Variable.Add("Twine_max_matk", max_matk_down);
                actor.Status.max_matk_skill -= (short)max_matk_down;
                actor.Buff.MinAtkDown = true;
                actor.Buff.MaxAtkDown = true;
                actor.Buff.MinMagicAtkDown = true;
                actor.Buff.MaxMagicAtkDown = true;
                actor.Buff.三轉纏繞吸收HP = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);


            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.min_atk1_skill += (short)skill.Variable["Twine_min_atk1"];
                actor.Status.min_atk2_skill += (short)skill.Variable["Twine_min_atk2"];
                actor.Status.min_atk3_skill += (short)skill.Variable["Twine_min_atk3"];

                actor.Status.max_atk1_skill += (short)skill.Variable["Twine_max_atk1"];
                actor.Status.max_atk2_skill += (short)skill.Variable["Twine_max_atk2"];
                actor.Status.max_atk3_skill += (short)skill.Variable["Twine_max_atk3"];
                actor.Status.min_matk_skill += (short)skill.Variable["Twine_min_matk"];
                actor.Status.max_matk_skill += (short)skill.Variable["Twine_max_matk"];
                actor.Buff.MinAtkDown = false;
                actor.Buff.MaxAtkDown = false;
                actor.Buff.MinMagicAtkDown = false;
                actor.Buff.MaxMagicAtkDown = false;
                actor.Buff.三轉纏繞吸收HP = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
            void UpdateTimeHandler(Actor actor, DefaultBuff skill)
            {
                //if(sActor.type==ActorType.PC)
                //{
                //    ActorPC pc = (ActorPC)sActor;
                //    MapClient.FromActorPC(pc).SendSystemMessage("更新");
                //}
                int HpChange = 80 + 20 * skill.skill.Level;
                if (actor.HP >= 2 && !actor.Buff.Dead)
                {

                    //SkillHandler.Instance.FixAttack(sActor, actor, null, SagaLib.Elements.Neutral, HpChange);
                    if (actor.HP > HpChange)
                    {
                        actor.HP -= (uint)HpChange;
                        //SkillHandler.Instance.ShowVessel(actor, HpChange);
                    }
                    else
                    {
                        HpChange = (int)(actor.HP - 1);
                        actor.HP -= (uint)HpChange;
                        //SkillHandler.Instance.ShowVessel(actor, HpChange);
                    }
                    if (sActor != null && !sActor.Buff.Dead)
                    {

                        sActor.HP += (uint)HpChange;
                        if (sActor.HP > sActor.MaxHP)
                        {
                            sActor.HP = sActor.MaxHP;
                        }
                        SkillHandler.Instance.ShowVessel(sActor, -HpChange);
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, sActor, false);
                    }
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, false);

                }
                else
                {
                    actor.Status.Additions["Twine"].AdditionEnd();
                    actor.Status.Additions.TryRemove("Twine", out _);
                }
            }
        }
        #endregion
    }
}
