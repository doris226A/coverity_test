using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.SoulTaker
{
    public class GDTMMegaDarkBlaze : ISkill
    {
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.PossessionTarget > 0)
            {
                return -25;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 2f + 2f * level;
            //祥奪岆翋眥遜岆萵眥, 硐猁炾腕窪做鳶栭撮夔ㄛ輛俴瓚剿
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc = sActor as ActorPC;
                if (pc.Skills2_1.ContainsKey(3310) || pc.DualJobSkill.Exists(x => x.ID == 3310))
                {
                    //涴爵萵眥腔窪做鳶栭脹撰
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 3310))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 3310).Level;

                    //涴爵翋眥腔窪做鳶栭脹撰
                    var mainlv = 0;
                    if (pc.Skills2_1.ContainsKey(3310))
                        mainlv = pc.Skills2_1[3310].Level;

                    //涴爵脹撰郔詢腔窪做鳶栭脹撰蚚懂捷薹樓傖
                    factor += (2.5f + 0.5f * Math.Max(duallv, mainlv));
                }
            }

            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 550, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(sActor, act))
                {
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.MagicAttack(sActor, realAffected, args, SagaLib.Elements.Dark, factor);
        }

    }
}