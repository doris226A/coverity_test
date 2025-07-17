using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
using SagaDB.Skill;
using SagaDB;
using System.Linq;
using System.Xml.Linq;
using System.Security.Policy;
namespace SagaMap.Tasks.PC
{
    public class TimerItem : MultiRunTask
    {
        ActorPC pc;
        public TimerItem(ActorPC pc)
        {
            this.dueTime = 1000;
            this.period = 1000;
            this.pc = pc;
        }

        public override void CallBack()
        {
            if (pc == null)
            {
                //this.Deactivate();
                return;
            }
            if (!pc.Online)
            {
                //this.Deactivate();
                return;
            }
            try
            {
                foreach (string name in pc.TimerItems.Keys.ToArray())
                {
                    pc.TimerItems[name].StartTime++;
                    pc.TimerItems[name].Duration--;
                    var startTime = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    if (!pc.Buff.Dead && !pc.Buff.NoRegen)
                    {
                        switch (name)
                        {
                            case "REGEN_UP_HP":
                                if (pc.HP < pc.MaxHP)
                                {
                                    uint heal = (uint)(pc.MaxHP * 0.001);
                                    if (heal <= 0) heal = 1;
                                    pc.HP += heal;
                                    if (pc.HP > pc.MaxHP) pc.HP = pc.MaxHP;
                                    ((SagaMap.ActorEventHandlers.PCEventHandler)pc.e).Client.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, pc, true);
                                }
                                break;
                            case "REGEN_UP_MP":
                                if (pc.MP < pc.MaxMP)
                                {
                                    uint heal = (uint)(pc.MaxMP * 0.001);
                                    if (heal <= 0) heal = 1;
                                    pc.MP += heal;
                                    if (pc.MP > pc.MaxMP) pc.MP = pc.MaxMP;
                                    ((SagaMap.ActorEventHandlers.PCEventHandler)pc.e).Client.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, pc, true);
                                }
                                break;
                            case "REGEN_UP_SP":
                                if (pc.SP < pc.MaxSP)
                                {
                                    uint heal = (uint)(pc.MaxSP * 0.001);
                                    if (heal <= 0) heal = 1;
                                    pc.SP += heal;
                                    if (pc.SP > pc.MaxSP) pc.SP = pc.MaxSP;
                                    ((SagaMap.ActorEventHandlers.PCEventHandler)pc.e).Client.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, pc, true);
                                }
                                break;
                        }
                    }
                    if (startTime >= pc.TimerItems[name].EndTime)
                    {
                        MapServer.charDB.DeleteTimerItem(pc.CharID, name);
                        pc.TimerItems.Remove(name);
                        string item_name = "";
                        switch (name)
                        {
                            case "EXP_ALL_BOUNS":
                                item_name = "經驗戒指";
                                break;
                            case "EXP_JOB_BOUNS":
                                item_name = "職業經驗戒指";
                                break;
                            case "ITEM_DROP_BOUNS":
                                item_name = "富豪的象徵";
                                break;
                            case "PARTNER_GET_EXP":
                                item_name = "寵物的協助";
                                break;
                            case "REGEN_UP_HP":
                                item_name = "充滿體力";
                                break;
                            case "REGEN_UP_MP":
                                item_name = "充滿魔力";
                                break;
                            case "REGEN_UP_SP":
                                item_name = "充滿精力";
                                break;
                            case "ADDON_PAYL_CAPA":
                                item_name = "泰迪的秘傳收納術";
                                break;
                            case "ADDON_CSPD":
                                item_name = "施放藥水";
                                break;
                            case "RELIABILITY_BOUNS":
                                item_name = "夥伴信賴度提升";
                                break;
                            case "EXP_PARTNER_BOUNS":
                                item_name = "超級蔬菜棒";
                                break;
                        }
                        //MapClient.FromActorPC((ActorPC)pc).SendSystemMessage("【寵物的協助】效果消失了！");
                        MapClient.FromActorPC((ActorPC)pc).SendSystemMessage("【" + item_name + "】效果消失了！");
                        //pc.Tasks_Online.Remove(name);

                        if (pc.TimerItems.Count == 0)
                        {
                            this.Deactivate();
                        }
                        break;
                    }
                }
            }
            catch
            {
                this.Deactivate();
            }
        }
    }
}