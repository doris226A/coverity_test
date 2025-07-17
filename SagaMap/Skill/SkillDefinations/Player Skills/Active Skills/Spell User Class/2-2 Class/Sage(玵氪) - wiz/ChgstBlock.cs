using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaDB.Mob;
using SagaMap.Network.Client;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Sage
{
    /// <summary>
    /// 解放異常狀態（エンチャントブロック）
    /// </summary>
    public class ChgstBlock : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (dActor.type == ActorType.PC)
            {
                return 0;
            }
            List<ActorMob> actorsInRange = new List<ActorMob>();
            ActorMob mob = (ActorMob)dActor;
            if (SkillHandler.Instance.isBossMob(mob))
            {
                return -14;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            string[] no = { "ChgstBlock", "FireWeapon", "WindWeapon", "WaterWeapon", "EarthWeapon", "DarkWeapon", "HolyWeapon", "EarthShield", "FireShield", "WaterShield", "WindShield", "DarkShield", "HolyShield" };
            int rate = 30 + 10 * level;
            if (SagaLib.Global.Random.Next(0, 99) < rate)
            {
                List<string> adds = new List<string>();
                foreach (System.Collections.Generic.KeyValuePair<string, SagaDB.Actor.Addition> ad in dActor.Status.Additions)
                {
                    if (!(ad.Value is DefaultPassiveSkill))
                    {
                        adds.Add(ad.Value.Name);
                        foreach (string i in no)
                        {
                            if (ad.Value.Name == i)
                                adds.Remove(ad.Value.Name);
                        }
                    }
                }
                foreach (string adn in adds)
                {
                    SkillHandler.RemoveAddition(dActor, adn);
                }

                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "ChgstBlock", 10000);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.EnchantmentBlock = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.EnchantmentBlock = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
