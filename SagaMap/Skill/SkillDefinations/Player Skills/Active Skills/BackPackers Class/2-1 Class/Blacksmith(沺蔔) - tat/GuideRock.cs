
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Mob;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Blacksmith
{
    /// <summary>
    /// 探礦感應（鍛冶屋の鼻）
    /// </summary>
    public class GuideRock : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int[] range = { 0, 5000, 7000, 10000 };
            SagaMap.Network.Client.MapClient client = SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)sActor);
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, (short)range[level], false);

            int i = 0;


            byte arrX = 255;
            byte arrY = 255;
            double length = 100000;

            foreach (Actor act in affected)
            {
                if (act.type == ActorType.MOB)
                {
                    ActorMob m = (ActorMob)act;
                    if (m.BaseData.mobType == SagaDB.Mob.MobType.ROCK_MATERIAL)
                    {
                        if (m.HP <= 0) continue;
                        i++;
                        if (map.GetLengthD(sActor.X, sActor.Y, act.X, act.Y) <= length)
                        {
                            length = map.GetLengthD(sActor.X, sActor.Y, act.X, act.Y);
                            arrX = SagaLib.Global.PosX16to8(act.X, map.Width);
                            arrY = SagaLib.Global.PosY16to8(act.Y, map.Height);
                        }
                    }
                }
            }

            if (i <= 0)
            {
                client.SendSystemMessage("沒有對象於搜索範圍內。");
                Packets.Server.SSMG_NPC_NAVIGATION_CANCEL p = new SagaMap.Packets.Server.SSMG_NPC_NAVIGATION_CANCEL();
                MapClient.FromActorPC((ActorPC)sActor).netIO.SendPacket(p);
                return;
            }
            else
            {
                Packets.Server.SSMG_NPC_NAVIGATION p = new SagaMap.Packets.Server.SSMG_NPC_NAVIGATION();
                p.X = arrX;
                p.Y = arrY;
                //p.Type = 0;
                client.netIO.SendPacket(p);
            }
            /*bool check = false;
            short factor = new short[] { 0, 2000, 3000, 4000 }[level];
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, factor, false);
            foreach (Actor act in affected)
            {
                if (act.type == ActorType.MOB)
                {
                    ActorMob mob = (ActorMob)act;
                    if (mob.BaseData.mobType == MobType.ROCK_MATERIAL)
                    {
                        check = true;
                        Packets.Server.SSMG_NPC_NAVIGATION p = new SagaMap.Packets.Server.SSMG_NPC_NAVIGATION();
                        p.X = SagaLib.Global.PosX16to8(mob.X, map.Width);
                        p.Y = SagaLib.Global.PosY16to8(mob.Y, map.Height);
                        MapClient.FromActorPC((ActorPC)sActor).netIO.SendPacket(p);
                        break;
                    }
                }
            }
            if (!check)
            {
                MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("範圍內找不到礦石");
                Packets.Server.SSMG_NPC_NAVIGATION_CANCEL p = new SagaMap.Packets.Server.SSMG_NPC_NAVIGATION_CANCEL();
                MapClient.FromActorPC((ActorPC)sActor).netIO.SendPacket(p);
            }*/


        }
        #endregion
    }
}
