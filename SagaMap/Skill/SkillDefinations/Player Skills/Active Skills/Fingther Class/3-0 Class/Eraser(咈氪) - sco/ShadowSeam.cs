using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB;
using SagaDB.Actor;
using SagaDB.Skill;
using SagaLib;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Eraser
{
    /// <summary>
    /// 影縫い
    /// </summary>
    public class ShadowSeam : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(SagaLib.Global.PosX8to16(args.x, map.Width), SagaLib.Global.PosY8to16(args.y, map.Height), 200, null);
            int deadran = level;
            int stiffran = 40 + level * 10;
            int stifftime_mob = new int[] { 0,3000, 4000, 5000, 6000, 8000 }[level];
            int stifftime = new int[] { 0,6000, 5000, 4000, 3000, 2000 }[level];
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    if(act.type == ActorType.MOB)
                    {
                        int ran = SagaLib.Global.Random.Next(0, 100);
                        if (ran < deadran)
                        {
                            SkillHandler.Instance.FixAttackNoref(sActor, act, args, SagaLib.Elements.Neutral, act.HP);
                        }
                        else if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stiff, stiffran))
                        {
                            Additions.Global.Stiff skill = new SagaMap.Skill.Additions.Global.Stiff(args.skill, act, stifftime_mob);
                            SkillHandler.ApplyAddition(act, skill);
                        }
                    }
                    else if (act.type == ActorType.PC)
                    {
                        if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stiff, stiffran))
                        {
                            Additions.Global.Stiff skill = new SagaMap.Skill.Additions.Global.Stiff(args.skill, act, stifftime);
                            SkillHandler.ApplyAddition(act, skill);
                        }
                    }
                }
            }

        }
        #endregion
    }
}
