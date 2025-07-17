using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Vates
{
    public class sakura : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (dActor.type == ActorType.MOB && sActor.type == ActorType.PC)
            {
                if (dActor.HP <= dActor.MaxHP / 4)
                {
                    ActorMob m = (ActorMob)dActor;
                    if (m.MobID == 92400004)
                    {
                        SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000064, 1, true);
                    }
                    int maxKey = 0;
                    SagaLib.Elements max = SagaLib.Elements.Neutral;
                    foreach (SagaLib.Elements e in m.Elements.Keys)
                    {
                        if (m.Elements[e] > maxKey)
                        {
                            maxKey = m.Elements[e];
                            max = e;
                        }
                    }
                    if (maxKey == 0)
                    {
                        SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000065, 1, true);
                    }
                    else
                    {
                        switch (max)
                        {
                            case SagaLib.Elements.Neutral:
                                SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000065, 1, true);
                                break;
                            case SagaLib.Elements.Fire:
                                SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000066, 1, true);
                                break;
                            case SagaLib.Elements.Water:
                                SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000067, 1, true);
                                break;
                            case SagaLib.Elements.Earth:
                                SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000068, 1, true);
                                break;
                            case SagaLib.Elements.Wind:
                                SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000069, 1, true);
                                break;
                            case SagaLib.Elements.Holy:
                                SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000070, 1, true);
                                break;
                            case SagaLib.Elements.Dark:
                                SagaMap.Skill.SkillHandler.Instance.GiveItem((ActorPC)sActor, 93000071, 1, true);
                                break;
                        }
                    }



                    dActor.HP = 0;
                    dActor.e.OnDie();
                    args.affectedActors.Add(dActor);
                    args.Init();
                    args.flag[0] = SagaLib.AttackFlag.DIE;
                    Manager.MapManager.Instance.GetMap(dActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, dActor, true);
                }
            }


        }
        #endregion
    }
}
