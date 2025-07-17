
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Cabalist
{
    /// <summary>
    /// 吸收靈魂（ソウルスティール）
    /// </summary>
    public class ARSoulSteal : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
                return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.0f;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, SagaLib.Elements.Dark, factor);
            if (sActor.type == ActorType.PC || sActor.type == ActorType.PARTNER || sActor.type == ActorType.PET || sActor.type == ActorType.SHADOW)
            {
               // ActorPC pc = (ActorPC)sActor;
               // pc = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
                //吸收MP恢復
                int mp_recovery = 0;
                foreach (int hp in args.hp)
                {
                    mp_recovery += (int)(hp);
                }
                uint mp_add = (uint)(mp_recovery * (0.15f + 0.03f * level));
                sActor.MP += mp_add;
                if (sActor.MP > sActor.MaxMP)
                {
                    sActor.MP = sActor.MaxMP;
                }
                SkillHandler.Instance.ShowVessel(sActor, 0, -(int)(mp_add), 0);
                Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, sActor, true);
            }

        }
        #endregion
    }
}