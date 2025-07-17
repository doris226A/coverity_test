using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Item;
using SagaDB.Actor;
using SagaDB.Npc;
using SagaDB.Quests;
using SagaDB.Party;
using SagaLib;
using SagaMap;
using SagaMap.Manager;
using SagaDB.Tamaire;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        //租心
        public void OnTamaireRentalRequest(Packets.Client.CSMG_TAMAIRE_RENTAL_REQUEST p)
        {
            if (this.Character.Gold < 100)
            {
                SendSystemMessage("金錢不足");
                return;
            }
            ActorPC lender = MapServer.charDB.GetChar(p.Lender);
            if (lender.TamaireLending.Renters.Count >= lender.TamaireLending.MaxLendings)
            {
                SendSystemMessage("此心魂借用已達到上限");
                return;
            }
            if (this.Character.TamaireRental != null && this.Character.TamaireRental.CurrentLender != 0)
            {
                TamaireRentalManager.Instance.TerminateRental(this.Character, 1);
            }
            TamaireRentalManager.Instance.ProcessRental(this.Character, lender);
            SendTamaire();
            PC.StatusFactory.Instance.CalcStatus(this.Character);
            SendPlayerInfo();
            this.Character.Gold -= 100;
        }

        //過圖or上線
        public void SendTamaire()
        {
            if (this.Character.TamaireRental == null)
                return;
            TamaireRentalManager.Instance.CheckRentalExpiry(this.Character);
            if (this.Character.TamaireRental.CurrentLender == 0)
                return;
            Packets.Server.SSMG_TAMAIRE_RENTAL p = new Packets.Server.SSMG_TAMAIRE_RENTAL();
            ActorPC lender = MapServer.charDB.GetChar(this.Character.TamaireRental.CurrentLender);
            p.JobType = lender.TamaireLending.JobType;
            p.RentalDue = this.Character.TamaireRental.RentDue - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            p.Factor = (short)((1f - TamaireRentalManager.Instance.CalcFactor(lender.TamaireLending.Baselv - this.Character.Level)) * 1000);
            this.netIO.SendPacket(p);
            int t = (int)((this.Character.TamaireRental.RentDue - DateTime.Now).TotalSeconds) * 1000;
            if (!this.Character.Tasks.ContainsKey("HeartPossession"))
            {
                Tasks.PC.HeartPossession task = new SagaMap.Tasks.PC.HeartPossession(this.Character, t);
                this.Character.Tasks.Add("HeartPossession", task);
                task.Activate();
            }
        }
        //不用了，沒用到
        //新增，過圖專用
        /*public void SendTamaireMapLoad()
        {
            if (this.Character.TamaireRental == null)
                return;
            TamaireRentalManager.Instance.CheckRentalExpiry(this.Character);
            if (this.Character.TamaireRental.CurrentLender == 0)
                return;
            Packets.Server.SSMG_TAMAIRE_RENTAL p = new Packets.Server.SSMG_TAMAIRE_RENTAL();
            ActorPC lender = MapServer.charDB.GetChar(this.Character.TamaireRental.CurrentLender);
            p.JobType = lender.TamaireLending.JobType;
            p.RentalDue = this.Character.TamaireRental.RentDue - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            p.Factor = (short)((1f - TamaireRentalManager.Instance.CalcFactor(lender.TamaireLending.Baselv - this.Character.Level)) * 1000);
            this.netIO.SendPacket(p);
        }*/
        //新增，登入專用
        public void SendTamaireOnLogin()
        {
            if (this.Character.TamaireLending != null)
            {
                TamaireExperienceMagager.Instance.GiveReward(this.Character);
            }
            if (this.Character.TamaireRental == null)
                return;
            TamaireRentalManager.Instance.CheckRentalExpiry(this.Character);
            if (this.Character.TamaireRental.CurrentLender == 0)
                return;
            ActorPC lender = MapServer.charDB.GetChar(this.Character.TamaireRental.CurrentLender);
            int leveldiff = lender.TamaireLending.Baselv - this.Character.Level;
            TamaireRentalManager.Instance.ProcessRentalStatus(this.Character, leveldiff, lender.TamaireLending.JobType);

            SendTamaire();
        }
        //終止心PE
        public void OnTamaireRentalTerminateRequest(Packets.Client.CSMG_TAMAIRE_RENTAL_TERMINATE_REQUEST p)
        {
            TamaireRentalManager.Instance.TerminateRental(this.Character, 1);
            //OnTamaireRentalTerminate(1);
        }
        //終止心PE
        public void OnTamaireRentalTerminate(byte reason)
        {
            Packets.Server.SSMG_TAMAIRE_RENTAL_TERMINATE p = new Packets.Server.SSMG_TAMAIRE_RENTAL_TERMINATE();
            p.Reason = reason;
            this.netIO.SendPacket(p);
            PC.StatusFactory.Instance.CalcStatus(this.Character);
            SendPlayerInfo();
        }

        //開心憑依列表
        public void OpenTamaireListUI()
        {
            Packets.Server.SSMG_TAMAIRE_LIST_UI p = new Packets.Server.SSMG_TAMAIRE_LIST_UI();
            this.netIO.SendPacket(p);
        }

        //關心憑依列表
        public void CloseTamaireListUI()
        {
            this.changeDualJob = false;
        }
    }
}