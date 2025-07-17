﻿using System;
using System.Collections.Generic;
using System.Text;


using SagaDB.Actor;

namespace SagaDB
{
    public interface ActorDB
    {
        void AJIClear();

        /// <summary>
        /// Write the given character to the database.
        /// </summary>
        /// <param name="aChar">Character that needs to be writen.</param>
        void SaveChar(ActorPC aChar);
        void SavePaper(ActorPC aChar);
        void SaveChar(ActorPC aChar, bool fullinfo);
        void SaveItem(ActorPC pc);
        void SaveChar(ActorPC aChar, bool itemInfo, bool fullinfo);

        void CreateChar(ActorPC aChar, int account_id);

        void SaveVar(ActorPC aChar);
        
        void GetVar(ActorPC pc);

        void DeleteChar(ActorPC aChar);

        ActorPC GetChar(uint charID);

        ActorPC GetChar(uint charID, bool fullinfo);

        List<ActorPC> GetChars(uint accountid);
        //ECOKEY 用名字尋找AccountID
        uint getAccountID(string name);
        //ECOKEY 為了好友列表新增的儲存功能，存地圖和(偽)限時動態
        void SaveCharfriend(ActorPC aChar, uint mapid, string Memo);

        bool CharExists(string name);

        uint GetAccountID(ActorPC pc);

        uint GetAccountID(uint charID);

        uint[] GetCharIDs(int account_id);

        string GetCharName(uint id);

        bool Connect();

        bool isConnected();

        /// <summary>
        /// 取得指定玩家的好友列表
        /// </summary>
        /// <param name="pc">玩家</param>
        /// <returns>好友列表</returns>
        List<ActorPC> GetFriendList(ActorPC pc);

        /// <summary>
        /// 取得添加指定玩家为好友的玩家列表
        /// </summary>
        /// <param name="pc">玩家</param>
        /// <returns>玩家列表</returns>
        List<ActorPC> GetFriendList2(ActorPC pc);
        // void DeleteTimerItem(uint itemID);
        void AddFriend(ActorPC pc, uint charID);

        bool IsFriend(uint char1, uint char2);

        void DeleteFriend(uint char1, uint char2);

        Party.Party GetParty(uint id);

        void NewParty(Party.Party party);

        void SaveParty(Party.Party party);

        void DeleteParty(Party.Party party);

        Ring.Ring GetRing(uint id);

        void NewRing(Ring.Ring ring);
        //ECOKEY 換團長
        void ChangeRingMaster(Ring.Ring ring, ActorPC oldmaster, ActorPC newmaster);


        void SaveRing(Ring.Ring ring, bool saveMembers);

        void DeleteRing(Ring.Ring ring);

        void RingEmblemUpdate(Ring.Ring ring, byte[] buf);

        byte[] GetRingEmblem(uint ring_id, DateTime date, out bool needUpdate, out DateTime newTime);

        List<BBS.Post> GetBBS(uint bbsID);

        List<BBS.Post> GetBBSPage(uint bbsID, int page);

        List<BBS.Mail> GetMail(ActorPC pc);

        bool BBSNewPost(ActorPC poster, uint bbsID, string title, string content);

        ActorPC LoadServerVar();

        void SaveServerVar(ActorPC fakepc);

        void SaveFGarden(ActorPC pc);

        void GetVShop(ActorPC pc);
        //ECOKEY NC
        void GetNCShop(ActorPC pc);

        void SaveSkill(ActorPC pc);

        void GetSkill(ActorPC pc);

        void SaveVShop(ActorPC pc);
        //ECOKEY NC
        void SaveNCShop(ActorPC pc);


        uint CreatePartner(Item.Item partnerItem);

        void SavePartner(ActorPartner ap);

        void SavePartnerEquip(ActorPartner ap);

        void SavePartnerCube(ActorPartner ap);

        void SavePartnerAI(ActorPartner ap);

        ActorPartner GetActorPartner(uint ActorPartnerID, Item.Item partneritem);

        void GetPartnerEquip(ActorPartner ap);

        void GetPartnerCube(ActorPartner ap);

        void GetPartnerAI(ActorPartner ap);

        void SaveWRP(ActorPC pc);

        List<ActorPC> GetWRPRanking();

        List<SagaDB.FFarden.FFarden> GetFFList();

        void SaveFF(Ring.Ring ring);

        void SaveSerFF(Server.Server ser);

        void GetSerFFurniture(Server.Server ser);

        void SaveFFCopy(Dictionary<SagaDB.FFarden.FurniturePlace, List<ActorFurniture>> Furnitures);

        void CreateFF(ActorPC pc);

        void GetFF(ActorPC pc);

        uint GetFFRindID(uint ffid);

        void GetFFurniture(SagaDB.Ring.Ring ring);

        void GetFFurnitureCopy(Dictionary<SagaDB.FFarden.FurniturePlace, List<ActorFurniture>> Furnitures);

        void SavaLevelLimit();

        void GetLevelLimit();

        List<BBS.Gift> GetGifts(ActorPC pc);

        bool DeleteGift(BBS.Gift gift);

        bool DeleteMail(BBS.Mail mail);

        uint AddNewGift(BBS.Gift gift);

        List<Tamaire.TamaireLending> GetTamaireLendings();

        void GetTamaireLending(ActorPC pc);

        void CreateTamaireLending(Tamaire.TamaireLending tamaireLending);

        void DeleteTamaireLending(Tamaire.TamaireLending tamaireLending);

        void SaveTamaireLending(Tamaire.TamaireLending tamaireLending);

        void GetTamaireRental(ActorPC pc);

        void CreateTamaireRental(Tamaire.TamaireRental tamaireRental);

        void SaveTamaireRental(Tamaire.TamaireRental tamaireRental);

        void DeleteTamaireRental(Tamaire.TamaireRental tamaireRental);

        // void SaveNPCStates(ActorPC pc, uint npcID);

        void SaveStamp(ActorPC pc, StampGenre genre);

        void SaveMosterGuide(ActorPC pc, uint mobID, bool state);

        void GetMosterGuide(ActorPC pc);

        void SaveTimerItem(ActorPC pc);

        void GetTimerItem(ActorPC pc);

        void DeleteTimerItem(uint id, string name);

        #region 副职相关
        void GetDualJobInfo(ActorPC pc);

        void SaveDualJobInfo(ActorPC pc, bool allinfo);

        void GetDualJobSkill(ActorPC pc);

        void SaveDualJobSkill(ActorPC pc);

        #endregion

        #region Golem相关

        void SaveGolemInfo(ActorGolem golem);

        void SaveGolemItem(ActorGolem golem);

        void SaveGolemTransactions(ActorGolem golem);

        void LoadGolemInfo(ActorPC pc);

        void LoadGolemItem(ActorGolem golem);

        void LoadGolemTransactions(ActorPC pc);

        #endregion

        #region 镜子相关

        void SaveMirrorInfo(ActorPC pc);

        void GetMirrorInfo(ActorPC pc);

        void DeleteMirrorInfo(ActorPC pc);

        #endregion

        //IP限制
        void SaveIpLimit(ActorPC pc);

        bool GetIpLimit(ActorPC pc, int limit);

        void DeleteIpLimit(int accountID);
    }
}
