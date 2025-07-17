
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Breeder
{
    /// <summary>
    /// メタモルフォーゼ（メタモルフォーゼ）寵物化身
    /// </summary>
    public class Metamorphosis : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(sActor);
            ActorPartner partner = SkillHandler.Instance.GetPartner(sActor);
            ActorPC pc = (ActorPC)sActor;
            MapClient client = MapClient.FromActorPC(pc);
            if (pc.TranceID != 0)
            {
                pc.TranceID = 0;
            }
            else
            {
                if (partner != null && pet == null)
                {
                    if (partner.PictID == 0)
                        pc.TranceID = partner.BaseData.pictid;
                    else
                        pc.TranceID = partner.PictID;
                }
            }
            client.SendCharInfoUpdate();
        }
        #endregion
    }
}