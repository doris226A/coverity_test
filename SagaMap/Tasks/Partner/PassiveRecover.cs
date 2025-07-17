using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.Partner
{
    public class PassiveRecover : MultiRunTask
    {
        ActorPartner partner;
        int HPRecover;
        int MPRecover;
        int SPRecover;
        SagaMap.Partner.PartnerAI partnerai;
        public PassiveRecover(ActorPartner partner, int HPRecover, int MPRecover, int SPRecover, SagaMap.Partner.PartnerAI partnerai)
        {
            this.dueTime = 5000;
            this.period = 5000;
            this.partner = partner;
            this.HPRecover = HPRecover;
            this.MPRecover = MPRecover;
            this.SPRecover = SPRecover;
            this.partnerai = partnerai;
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                if (partner.HP < partner.MaxHP)
                {
                    partner.HP = partner.HP + (uint)HPRecover;
                }
                if (partner.MP < partner.MaxMP)
                {
                    partner.MP = partner.MP + (uint)MPRecover;
                }
                if (partner.SP < partner.MaxSP)
                {
                    partner.SP = partner.SP + (uint)SPRecover;
                }
                partnerai.PartnerHPMPSP();
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.partner.Tasks.Remove("PassiveRecover");
                this.Deactivate();
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
