
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// ラッシュボム（ラッシュボム）[接續技能]
    /// </summary>
    public class RushBomSeq2 : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*float[] factors = { 0f, 1.1f, 1.1f, 2.2f, 3.3f, 4.4f };
            float factor = factors[level];
            Map map = Manager.MapManager.Instance.GetMap(dActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factor);*/

            ActivatorA timer = new ActivatorA(sActor, dActor, args);
            timer.Activate();
        }
        #endregion

        private class ActivatorA : MultiRunTask
        {
            Map map;
            Actor sActor;
            Actor dActor;
            SkillArg args;
            public ActivatorA(Actor caster, Actor actor, SkillArg args)
            {
                this.sActor = caster;
                this.dActor = actor;
                this.args = args;
                map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                this.period = 2000;
                this.dueTime = 2000;
            }

            public override void CallBack()
            {
                try
                {
                    if (dActor.HP > 0)
                    {
                        float[] factors = { 0f, 1.1f, 1.1f, 2.2f, 3.3f, 4.4f };
                        List<Actor> affected = map.GetActorsArea(dActor, 150, false);
                        List<Actor> realAffected = new List<Actor>();
                        foreach (Actor act in affected)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                            {
                                realAffected.Add(act);
                            }
                        }
                        realAffected.Add(dActor);
                      //  map.SendEffect(dActor, 5206);
                        SkillHandler.Instance.PhysicalAttack(sActor, realAffected, args, sActor.WeaponElement, factors[args.skill.Level]);
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, args, sActor, true);

                    }
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