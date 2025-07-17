using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class CityRecover : MultiRunTask
    {
        MapClient client;
        public CityRecover(MapClient client)
        {
            this.dueTime = 5000;
            this.period = 5000;
            this.client = client;
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                if (!this.client.Character.Buff.NoRegen)
                {
                    if (this.client.Character.HP < this.client.Character.MaxHP || this.client.Character.MP < this.client.Character.MaxMP ||
                this.client.Character.SP < this.client.Character.MaxSP)
                    {
                        this.client.Character.HP += (uint)(this.client.Character.MaxHP * this.client.Character.Status.hp_recover / 2000);
                        if (this.client.Character.HP > this.client.Character.MaxHP) this.client.Character.HP = this.client.Character.MaxHP;
                        this.client.Character.MP += (uint)(this.client.Character.MaxMP * this.client.Character.Status.mp_recover / 2000);
                        if (this.client.Character.MP > this.client.Character.MaxMP) this.client.Character.MP = this.client.Character.MaxMP;
                        this.client.Character.SP += (uint)(this.client.Character.MaxSP * this.client.Character.Status.sp_recover / 2000);
                        if (this.client.Character.SP > this.client.Character.MaxSP) this.client.Character.SP = this.client.Character.MaxSP;
                        this.client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.client.Character.Tasks.Remove("CityRecover");
                this.Deactivate();
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
