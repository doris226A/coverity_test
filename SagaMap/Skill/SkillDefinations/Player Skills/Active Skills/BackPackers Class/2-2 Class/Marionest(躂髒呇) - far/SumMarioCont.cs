using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Marionest
{
    /// <summary>
    /// 召喚活動木偶皇帝 [接續技能]
    /// </summary>
    public class SumMarioCont : ISkill
    {
        private Elements element;
        public SumMarioCont(Elements e)
        {
            element = e;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        ActorPC pc;
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0.7f + 3f * level;
            /*if (sActor is ActorPC)
            {
                ActorPC pc = sActor as ActorPC;

                //不管是主职还是副职, 只要习得技能
                if (pc.Skills3.ContainsKey(993) || pc.DualJobSkill.Exists(x => x.ID == 993))
                {

                    //这里取副职的等级
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 993))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 993).Level;

                    //这里取主职的等级
                    var mainlv = 0;
                    if (pc.Skills3.ContainsKey(993))
                        mainlv = pc.Skills3[993].Level;

                    //这里取等级最高的等级用来做倍率加成
                    factor += new float[] { 0, 1.0f, 1.6f, 2.4f, 3.1f, 3.8f }[Math.Max(duallv, mainlv)];
                }
            }*/

            if (sActor is ActorMob)
            {
                ActorMob mob = sActor as ActorMob;
                pc = (ActorPC)mob.Owner;
                //不管是主职还是副职, 只要习得技能
                if (pc.Skills3.ContainsKey(993) || pc.DualJobSkill.Exists(x => x.ID == 993))
                {

                    //这里取副职的等级
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 993))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 993).Level;

                    //这里取主职的等级
                    var mainlv = 0;
                    if (pc.Skills3.ContainsKey(993))
                        mainlv = pc.Skills3[993].Level;

                    //这里取等级最高的等级用来做倍率加成
                    factor += new float[] { 0, 1.0f, 1.6f, 2.4f, 3.1f, 3.8f }[Math.Max(duallv, mainlv)];
                }
            }
            if (args.skill.ID == 3341)
            {
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                List<Actor> affected = map.GetActorsArea(sActor, 200, false);
                List<Actor> realAffected = new List<Actor>();
                foreach (Actor act in affected)
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        realAffected.Add(act);
                        Additions.Global.Confuse skill = new SagaMap.Skill.Additions.Global.Confuse(args.skill, act, 5000);
                        SkillHandler.ApplyAddition(act, skill);

                    }
                }
                SkillHandler.Instance.MagicAttack(sActor, realAffected, args, element, factor);
            }
            else
            {
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                List<Actor> affected = map.GetActorsArea(dActor, 200, true);
                List<Actor> realAffected = new List<Actor>();
                foreach (Actor act in affected)
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                    {
                        realAffected.Add(act);
                    }
                }
                SkillHandler.Instance.MagicAttack(sActor, realAffected, args, element, factor);
            }
            if (sActor.type == ActorType.MOB)
            {
                ActorMob m = (ActorMob)sActor;
                Map map = Manager.MapManager.Instance.GetMap(m.MapID);
                map.DeleteActor(m);
            }

            /*ActorMob mob = (ActorMob)sActor.Slave[0];
            map.DeleteActor(mob);
            sActor.Slave.RemoveAt(0);*/
        }
        #endregion
    }
}



