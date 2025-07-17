
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Skill;
using SagaMap.Network.Client;
using SagaMap.Scripting;
namespace SagaMap.Skill.SkillDefinations.Bard
{
    /// <summary>
    /// 軍隊音樂會（ゲリラライブ）
    /// </summary>
    public class ChangeMusic : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (Skill.SkillHandler.Instance.isEquipmentRight(sActor, SagaDB.Item.ItemType.STRINGS) || sActor.Inventory.GetContainer(SagaDB.Item.ContainerType.RIGHT_HAND2).Count > 0)
            {
                return 0;
            }
            return -5;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = -1;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "ChangeMusic", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            // ActorPC pc = (ActorPC)actor;
            ActorPC pc = MapClient.FromActorPC((ActorPC)actor).Character;
            if (pc.PossessionTarget != 0)
            {
                return;
            }
            else
            {
                switch (skill.skill.Level)
                {
                    case 1:
                        if ((pc.MapID / 10) == 7000000 || (pc.MapID / 10) == 7500000)
                        {
                            int soundID = SagaLib.Global.Random.Next(1, 4);
                            if (soundID == 1)
                            {
                                soundID = 1081;
                            }
                            if (soundID == 2)
                            {
                                soundID = 1088;
                            }
                            if (soundID == 3)
                            {
                                soundID = 1140;
                            }
                            if (soundID == 4)
                            {
                                soundID = 1150;
                            }
                            MapClient.FromActorPC((ActorPC)actor).SendChangeBGM((uint)soundID, 1, 100, 50);
                        }
                        else
                        {
                            MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("請在飛空庭上使用");
                        }
                        break;
                    case 2:
                        if ((pc.MapID / 10) == 7000000 || (pc.MapID / 10) == 7500000)
                        {
                            int soundID = SagaLib.Global.Random.Next(1, 3);
                            if (soundID == 1)
                            {
                                soundID = 1142;
                            }
                            if (soundID == 2)
                            {
                                soundID = 1144;
                            }
                            if (soundID == 3)
                            {
                                soundID = 1145;
                            }
                            MapClient.FromActorPC((ActorPC)actor).SendChangeBGM((uint)soundID, 1, 100, 50);
                        }
                        else
                        {
                            MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("請在飛空庭上使用");
                        }
                        break;
                    case 3:
                        if ((pc.MapID / 10) == 7000000 || (pc.MapID / 10) == 7500000)
                        {
                            int soundID = SagaLib.Global.Random.Next(1, 3);
                            if (soundID == 1)
                            {
                                soundID = 1147;
                            }
                            if (soundID == 2)
                            {
                                soundID = 1148;
                            }
                            if (soundID == 3)
                            {
                                soundID = 1149;
                            }
                            MapClient.FromActorPC((ActorPC)actor).SendChangeBGM((uint)soundID, 1, 100, 50);
                        }
                        else
                        {
                            MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("請在飛空庭上使用");
                        }
                        break;
                    case 4:
                        if ((pc.MapID / 10) == 7000000 || (pc.MapID / 10) == 7500000)
                        {
                            int soundID = SagaLib.Global.Random.Next(1, 3);
                            if (soundID == 1)
                            {
                                soundID = 1171;
                            }
                            if (soundID == 2)
                            {
                                soundID = 1170;
                            }
                            if (soundID == 3)
                            {
                                soundID = 1172;
                            }
                            MapClient.FromActorPC((ActorPC)actor).SendChangeBGM((uint)soundID, 1, 100, 50);
                        }
                        else
                        {
                            MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("請在飛空庭上使用");
                        }
                        break;
                    case 5:
                        if ((pc.MapID / 10) == 7000000 || (pc.MapID / 10) == 7500000)
                        {
                            int soundID = SagaLib.Global.Random.Next(1, 3);
                            if (soundID == 1)
                            {
                                soundID = 1053;
                            }
                            if (soundID == 2)
                            {
                                soundID = 1015;
                            }
                            if (soundID == 3)
                            {
                                soundID = 1054;
                            }
                            MapClient.FromActorPC((ActorPC)actor).SendChangeBGM((uint)soundID, 1, 100, 50);
                        }
                        else
                        {
                            MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("請在飛空庭上使用");
                        }
                        break;


                }
            }


        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {

        }

        #endregion
    }
}