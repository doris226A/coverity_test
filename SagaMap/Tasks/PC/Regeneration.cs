using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class Regeneration : MultiRunTask
    {
        MapClient client;
        public Regeneration(MapClient client)
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
                if (this.client != null)
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
                            if (this.client.Character.Status.Additions.ContainsKey("PartyBivouac"))
                            {
                                this.client.Character.HP += (uint)(this.client.Character.MaxHP * this.client.Character.Status.hp_recover / 500);
                                if (this.client.Character.HP > this.client.Character.MaxHP) this.client.Character.HP = this.client.Character.MaxHP;
                                this.client.Character.MP += (uint)(this.client.Character.MaxMP * this.client.Character.Status.mp_recover / 500);
                                if (this.client.Character.MP > this.client.Character.MaxMP) this.client.Character.MP = this.client.Character.MaxMP;
                                this.client.Character.SP += (uint)(this.client.Character.MaxSP * this.client.Character.Status.sp_recover / 500);
                                if (this.client.Character.SP > this.client.Character.MaxSP) this.client.Character.SP = this.client.Character.MaxSP;
                            }
                            this.client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character, true);
                        }
                    }

                    if (this.client.Character.Partner != null)
                    {
                        if (!this.client.Character.Partner.Buff.NoRegen)
                        {
                            //ECOKEY 寵物離開位置就不回血
                            if (this.client.Character.TInt.ContainsKey("PartnerX"))
                            {
                                if (this.client.Character.Partner.X == this.client.Character.TInt["PartnerX"] && this.client.Character.Partner.Y == this.client.Character.TInt["PartnerY"])
                                {
                                    /*this.client.Character.Partner.HP += (uint)(0.1f * this.client.Character.Partner.MaxHP);
                                    if (this.client.Character.Partner.HP > this.client.Character.Partner.MaxHP)
                                        this.client.Character.Partner.HP = this.client.Character.Partner.MaxHP;
                                    this.client.Character.Partner.MP += (uint)(0.1f * this.client.Character.Partner.MaxMP);
                                    if (this.client.Character.Partner.MP > this.client.Character.Partner.MaxMP)
                                        this.client.Character.Partner.MP = this.client.Character.Partner.MaxMP;
                                    this.client.Character.Partner.SP += (uint)(0.1f * this.client.Character.Partner.MaxSP);
                                    if (this.client.Character.Partner.SP > this.client.Character.Partner.MaxSP)
                                        this.client.Character.Partner.SP = this.client.Character.Partner.MaxSP;
                                    this.client.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character.Partner, true);
                                    */

                                    this.client.Character.Partner.HP += (uint)(this.client.Character.Partner.MaxHP * this.client.Character.Partner.Status.hp_recover / 2000);
                                    if (this.client.Character.Partner.HP > this.client.Character.Partner.MaxHP) this.client.Character.Partner.HP = this.client.Character.Partner.MaxHP;
                                    this.client.Character.Partner.MP += (uint)(this.client.Character.Partner.MaxMP * this.client.Character.Partner.Status.mp_recover / 2000);
                                    if (this.client.Character.Partner.MP > this.client.Character.Partner.MaxMP) this.client.Character.Partner.MP = this.client.Character.Partner.MaxMP;
                                    this.client.Character.Partner.SP += (uint)(this.client.Character.Partner.MaxSP * this.client.Character.Partner.Status.sp_recover / 2000);
                                    if (this.client.Character.Partner.SP > this.client.Character.Partner.MaxSP) this.client.Character.Partner.SP = this.client.Character.Partner.MaxSP;
                                    if (this.client.Character.Status.Additions.ContainsKey("PartyBivouac"))
                                    {
                                        this.client.Character.Partner.HP += (uint)(this.client.Character.Partner.MaxHP * this.client.Character.Partner.Status.hp_recover / 500);
                                        if (this.client.Character.Partner.HP > this.client.Character.Partner.MaxHP) this.client.Character.Partner.HP = this.client.Character.Partner.MaxHP;
                                        this.client.Character.Partner.MP += (uint)(this.client.Character.Partner.MaxMP * this.client.Character.Partner.Status.mp_recover / 500);
                                        if (this.client.Character.Partner.MP > this.client.Character.Partner.MaxMP) this.client.Character.Partner.MP = this.client.Character.Partner.MaxMP;
                                        this.client.Character.Partner.SP += (uint)(this.client.Character.Partner.MaxSP * this.client.Character.Partner.Status.sp_recover / 500);
                                        if (this.client.Character.Partner.SP > this.client.Character.Partner.MaxSP) this.client.Character.Partner.SP = this.client.Character.Partner.MaxSP;
                                    }
                                    this.client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character.Partner, true);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.Deactivate();
                this.client.Character.Tasks.Remove("Regeneration");
                this.client.Character.Partner.Tasks.Remove("Regeneration");
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}

/*using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class Regeneration : MultiRunTask
    {
        MapClient client;
        public Regeneration(MapClient client)
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
                if (client.Character.Mode == PlayerMode.KNIGHT_EAST)//除夕活动
                {
                    this.Deactivate();
                    this.client.Character.Tasks.Remove("Regeneration");
                }
                if (this.client != null)
                {
                    this.client.Character.HP += (uint)(0.1f * this.client.Character.MaxHP);
                    if (this.client.Character.HP > this.client.Character.MaxHP)
                        this.client.Character.HP = this.client.Character.MaxHP;
                    this.client.Character.MP += (uint)(0.1f * this.client.Character.MaxMP);
                    if (this.client.Character.MP > this.client.Character.MaxMP)
                        this.client.Character.MP = this.client.Character.MaxMP;
                    this.client.Character.SP += (uint)(0.1f * this.client.Character.MaxSP);
                    if (this.client.Character.SP > this.client.Character.MaxSP)
                        this.client.Character.SP = this.client.Character.MaxSP;
                    this.client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character, true);
                    if (this.client.Character.Partner != null)
                    {
                        //ECOKEY 寵物離開位置就不回血
                        if (this.client.Character.TInt.ContainsKey("PartnerX"))
                        {
                            if (this.client.Character.Partner.X == this.client.Character.TInt["PartnerX"] && this.client.Character.Partner.Y == this.client.Character.TInt["PartnerY"])
                            {
                                this.client.Character.Partner.HP += (uint)(0.1f * this.client.Character.Partner.MaxHP);
                                if (this.client.Character.Partner.HP > this.client.Character.Partner.MaxHP)
                                    this.client.Character.Partner.HP = this.client.Character.Partner.MaxHP;
                                this.client.Character.Partner.MP += (uint)(0.1f * this.client.Character.Partner.MaxMP);
                                if (this.client.Character.Partner.MP > this.client.Character.Partner.MaxMP)
                                    this.client.Character.Partner.MP = this.client.Character.Partner.MaxMP;
                                this.client.Character.Partner.SP += (uint)(0.1f * this.client.Character.Partner.MaxSP);
                                if (this.client.Character.Partner.SP > this.client.Character.Partner.MaxSP)
                                    this.client.Character.Partner.SP = this.client.Character.Partner.MaxSP;
                                this.client.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character.Partner, true);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.Deactivate();
                this.client.Character.Tasks.Remove("Regeneration");
                this.client.Character.Partner.Tasks.Remove("Regeneration");
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
*/
/*using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class Regeneration : MultiRunTask
    {
        MapClient client;
        public Regeneration(MapClient client)
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
                if (client.Character.Mode == PlayerMode.KNIGHT_EAST)//除夕活动
                {
                    this.Deactivate();
                    this.client.Character.Tasks.Remove("Regeneration");
                }
                if (this.client != null)
                {
                    this.client.Character.HP += (uint)(0.1f * this.client.Character.MaxHP);
                    if (this.client.Character.HP > this.client.Character.MaxHP)
                        this.client.Character.HP = this.client.Character.MaxHP;
                    this.client.Character.MP += (uint)(0.1f * this.client.Character.MaxMP);
                    if (this.client.Character.MP > this.client.Character.MaxMP)
                        this.client.Character.MP = this.client.Character.MaxMP;
                    this.client.Character.SP += (uint)(0.1f * this.client.Character.MaxSP);
                    if (this.client.Character.SP > this.client.Character.MaxSP)
                        this.client.Character.SP = this.client.Character.MaxSP;
                    this.client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character, true);
                    if (this.client.Character.Partner != null)
                    {
                        //ECOKEY 寵物離開位置就不回血
                        if (this.client.Character.TInt.ContainsKey("PartnerX"))
                        {
                            if (this.client.Character.Partner.X == this.client.Character.TInt["PartnerX"] && this.client.Character.Partner.Y == this.client.Character.TInt["PartnerY"])
                            {
                                this.client.Character.Partner.HP += (uint)(0.1f * this.client.Character.Partner.MaxHP);
                                if (this.client.Character.Partner.HP > this.client.Character.Partner.MaxHP)
                                    this.client.Character.Partner.HP = this.client.Character.Partner.MaxHP;
                                this.client.Character.Partner.MP += (uint)(0.1f * this.client.Character.Partner.MaxMP);
                                if (this.client.Character.Partner.MP > this.client.Character.Partner.MaxMP)
                                    this.client.Character.Partner.MP = this.client.Character.Partner.MaxMP;
                                this.client.Character.Partner.SP += (uint)(0.1f * this.client.Character.Partner.MaxSP);
                                if (this.client.Character.Partner.SP > this.client.Character.Partner.MaxSP)
                                    this.client.Character.Partner.SP = this.client.Character.Partner.MaxSP;
                                this.client.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character.Partner, true);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.Deactivate();
                this.client.Character.Tasks.Remove("Regeneration");
                this.client.Character.Partner.Tasks.Remove("Regeneration");
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
*/