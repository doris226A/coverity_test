
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 定時炸彈（セットボム）[接續技能]
    /// </summary>
    public class SetBomb2 : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*float factor = 0.5f + 0.5f * level;
            Map map = Manager.MapManager.Instance.GetMap(dActor.MapID);
            List<Actor> affected = map.GetActorsArea(dActor, 150, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor,act))
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
                this.period = 3000;
                this.dueTime = 3000;
            }

            public override void CallBack()
            {
                try
                {
                    if (dActor.HP > 0)
                    {
                        float[] factors = { 0, 4.0f, 4.5f, 5.0f, 5.5f, 6.0f };
                        List<Actor> affected = map.GetActorsArea(dActor, 150, true);
                        List<Actor> realAffected = new List<Actor>();
                        foreach (Actor act in affected)
                        {
                            if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                            {
                                realAffected.Add(act);
                            }
                        }
                       // map.SendEffect(dActor, 5206);
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