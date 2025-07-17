using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class Possession : MultiRunTask
    {
        MapClient client;
        ActorPC target;
        PossessionPosition pos;
        string comment;
        public Possession(MapClient client, ActorPC target, PossessionPosition position, string comment, int reduce)
        {
            if (reduce > Configuration.Instance.PossessionRequireTimeMaxReduce)
                reduce = Configuration.Instance.PossessionRequireTimeMaxReduce;
            this.dueTime = Configuration.Instance.PossessionRequireTime - reduce * 1000;
            this.period = Configuration.Instance.PossessionRequireTime - reduce * 1000;
            this.client = client;
            this.target = target;
            this.pos = position;
            this.comment = comment;
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                client.Character.Buff.GetReadyPossession = false;
                client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, client.Character, true);
                client.PossessionPerform(target, pos, comment);
                if (client.Character.Tasks.ContainsKey("Possession"))
                    client.Character.Tasks.Remove("Possession");
                this.Deactivate();

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.Deactivate();
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
    public class Possession : MultiRunTask
    {
        MapClient client;
        ActorPC target;
        PossessionPosition pos;
        string comment;
        public Possession(MapClient client, ActorPC target, PossessionPosition position, string comment, int reduce)
        {
            if (reduce > Configuration.Instance.PossessionRequireTimeMaxReduce)
                reduce = Configuration.Instance.PossessionRequireTimeMaxReduce;
            this.dueTime = Configuration.Instance.PossessionRequireTime - reduce * 1000;
            this.period = Configuration.Instance.PossessionRequireTime - reduce * 1000;
            this.client = client;
            this.target = target;
            this.pos = position;
            this.comment = comment;
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                client.Character.Buff.GetReadyPossession = false;
                client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, client.Character, true);
                client.PossessionPerform(target, pos, comment);
                if (client.Character.Tasks.ContainsKey("Possession"))
                    client.Character.Tasks.Remove("Possession");

                //ECOKEY PE進度條修復
                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10023500);
                SkillArg arg = new SkillArg();
                arg.sActor = client.Character.ActorID;
                arg.dActor = client.Character.ActorID;
                arg.item = item;
                arg.x = (byte)client.Character.X;
                arg.y = (byte)client.Character.Y;
                arg.argType = SkillArg.ArgType.Item_Cast;

                Packets.Server.SSMG_ITEM_ACTIVE_SELF p3 = new SagaMap.Packets.Server.SSMG_ITEM_ACTIVE_SELF((byte)arg.affectedActors.Count);
                p3.ActorID = client.Character.ActorID;
                p3.AffectedID = arg.affectedActors;
                p3.AttackFlag(arg.flag);
                p3.ItemID = arg.item.ItemID;
                p3.SetHP(arg.hp);
                p3.SetMP(arg.mp);
                p3.SetSP(arg.sp);
                MapClient.FromActorPC((ActorPC)client.Character).netIO.SendPacket(p3);

                this.Deactivate();

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.Deactivate();
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
*/