using System;
using System.Collections.Generic;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;
using SagaMap.Manager;
using SagaMap.Network.Client;
using SagaDB.Item;
namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    class Logging : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (pc.PossessionTarget != 0)
            {
                return -7;
            }
            else
            {
                return 0;
            }
            //  return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            dActor = SkillHandler.Instance.GetdActor(sActor, args);
            if (dActor == null) return;

            if (dActor.type != ActorType.MOB)
                return;

            if (sActor.type != ActorType.PC)
                return;
            ActorPC pc = (ActorPC)sActor;
            MapClient client = MapClient.FromActorPC(pc);
            Map map = MapManager.Instance.GetMap(sActor.MapID);
            int damage = 1;
            SkillHandler.Instance.CauseDamage(sActor, dActor, damage);
            SkillHandler.Instance.ShowVessel(dActor, damage);

             if (dActor.Name == "樟樹")
             {
                int itemid = SagaLib.Global.Random.Next(1, 5);
                if (itemid == 1)
                {
                    itemid = 10001201;
                }
                if (itemid == 2)
                {
                    itemid = 10016300;
                }
                if (itemid == 3)
                {
                    itemid = 10044100;
                }
                if (itemid == 4)
                {
                    itemid = 10016200;
                }
                if (itemid == 5)
                {
                    itemid = 10016600;
                }


                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid);
                 item.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 50) 
                {
                    client.AddItem(item, true);
                }
                else
                {
                    client.SendSystemMessage("你失手了……。");
                }

                int itemid2 = SagaLib.Global.Random.Next(1, 4);
                if (itemid2 == 1)
                {
                    itemid2 = 22000497;
                }
                if (itemid2 == 2)
                {
                    itemid2 = 22000498;
                }
                if (itemid2 == 3)
                {
                    itemid2 = 22000499;
                }
                if (itemid2 == 4)
                {
                    itemid2 = 22000500;
                }

                SagaDB.Item.Item item2 = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid2);
                item2.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 3)
                    client.AddItem(item2, true);
                 pc.CInt["伐木exp"]++;
                 client.SendSystemMessage("獲得了 1 點伐木經驗值。");
             }
            if (dActor.Name == "欒樹")
            {
                int itemid = SagaLib.Global.Random.Next(1,4);
                if (itemid == 1)
                {
                    itemid = 10001201;
                }
                if (itemid == 2)
                {
                    itemid = 10016400;
                }
                if (itemid == 3)
                {
                    itemid = 10044103;
                }
                if (itemid == 4)
                {
                    itemid = 10016200;
                }

                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid);
                item.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 40)
                {
                    client.AddItem(item, true);
                }
                else
                {
                    client.SendSystemMessage("你失手了……。");
                }

                int itemid2 = SagaLib.Global.Random.Next(1, 4);
                if (itemid2 == 1)
                {
                    itemid2 = 22000497;
                }
                if (itemid2 == 2)
                {
                    itemid2 = 22000498;
                }
                if (itemid2 == 3)
                {
                    itemid2 = 22000499;
                }
                if (itemid2 == 4)
                {
                    itemid2 = 22000500;
                }

                SagaDB.Item.Item item2 = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid2);
                item2.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 3)
                    client.AddItem(item2, true);
                pc.CInt["伐木exp"]++;
                client.SendSystemMessage("獲得了 1 點伐木經驗值。");
            }
            if (dActor.Name == "猴麵包樹")
            {
                int itemid = SagaLib.Global.Random.Next(1, 4);
                if (itemid == 1)
                {
                    itemid = 10001201;
                }
                if (itemid == 2)
                {
                    itemid = 10016401;
                }
                if (itemid == 3)
                {
                    itemid = 10019100;
                }
                if (itemid == 4)
                {
                    itemid = 10044101;
                }


                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid);
                item.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 30)
                {
                    client.AddItem(item, true);
                }
                else
                {
                    client.SendSystemMessage("你失手了……。");
                }

                int itemid2 = SagaLib.Global.Random.Next(1, 4);
                if (itemid2 == 1)
                {
                    itemid2 = 22000497;
                }
                if (itemid2 == 2)
                {
                    itemid2 = 22000498;
                }
                if (itemid2 == 3)
                {
                    itemid2 = 22000499;
                }
                if (itemid2 == 4)
                {
                    itemid2 = 22000500;
                }

                SagaDB.Item.Item item2 = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid2);
                item2.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 3)
                    client.AddItem(item2, true);
                pc.CInt["伐木exp"]++;
                client.SendSystemMessage("獲得了 1 點伐木經驗值。");
            }
            if (dActor.Name == "猢猻樹")
            {
                int itemid = SagaLib.Global.Random.Next(1, 4);
                if (itemid == 1)
                {
                    itemid = 10001201;
                }
                if (itemid == 2)
                {
                    itemid = 10016401;
                }
                if (itemid == 3)
                {
                    itemid = 10019100;
                }
                if (itemid == 4)
                {
                    itemid = 10044101;
                }

                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid);
                item.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 30)
                {
                    client.AddItem(item, true);
                }
                else
                {
                    client.SendSystemMessage("你失手了……。");
                }

                int itemid2 = SagaLib.Global.Random.Next(1, 4);
                if (itemid2 == 1)
                {
                    itemid2 = 22000497;
                }
                if (itemid2 == 2)
                {
                    itemid2 = 22000498;
                }
                if (itemid2 == 3)
                {
                    itemid2 = 22000499;
                }
                if (itemid2 == 4)
                {
                    itemid2 = 22000500;
                }

                SagaDB.Item.Item item2 = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid2);
                item2.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 3)
                    client.AddItem(item2, true);
                pc.CInt["伐木exp"]++;
                client.SendSystemMessage("獲得了 1 點伐木經驗值。");
            }
            if (dActor.Name == "樹幹")
            {
                int itemid = SagaLib.Global.Random.Next(1, 4);
                if (itemid == 1)
                {
                    itemid = 10016200;
                }
                if (itemid == 2)
                {
                    itemid = 10016600;
                }
                if (itemid == 3)
                {
                    itemid = 10044100;
                }
                if (itemid == 4)
                {
                    itemid = 10020207;
                }

                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid);
                item.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 70)
                {
                    client.AddItem(item, true);
                }
                else
                {
                    client.SendSystemMessage("你失手了……。");
                }
                pc.CInt["伐木exp"]++;
                client.SendSystemMessage("獲得了 1 點伐木經驗值。");
            }
            if (dActor.Name == "大榕樹")
            {
                int itemid = SagaLib.Global.Random.Next(1, 3);
                if (itemid == 1)
                {
                    itemid = 10001201;
                }
                if (itemid == 2)
                {
                    itemid = 10016350;
                }
                if (itemid == 3)
                {
                    itemid = 10044102;
                }


                SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem((uint)itemid);
                item.Stack = 1;

                if (SagaLib.Global.Random.Next(0, 100) < 20)
                {
                    client.AddItem(item, true);
                }
                else
                {
                    client.SendSystemMessage("你失手了……。");
                }
                pc.CInt["伐木exp"]++;
                client.SendSystemMessage("獲得了 1 點伐木經驗值。");
            }
        }
        #endregion
    }
}
