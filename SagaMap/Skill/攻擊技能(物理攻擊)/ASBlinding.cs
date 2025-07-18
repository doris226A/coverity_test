﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Explorer
{
    /// <summary>
    /// 灑灰（目潰し）
    /// </summary>
    public class ASBlinding : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.25f;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            int lifetime = 10000;
            int[] rates = { 0, 69, 78, 84 };
            if (SagaLib.Global.Random.Next(0, 99) < rates[level])
            {
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "Blinding", lifetime);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int level = skill.skill.Level;
            //近命中
            int hit_melee_add = -(int)(actor.Status.hit_melee * 0.33f);
            if (skill.Variable.ContainsKey("Blinding_hit_melee"))
                skill.Variable.Remove("Blinding_hit_melee");
            skill.Variable.Add("Blinding_hit_melee", hit_melee_add);
            actor.Status.hit_melee_skill += (short)hit_melee_add;


            //遠命中
            int hit_ranged_add = -(int)(actor.Status.hit_ranged * 0.33f);
            if (skill.Variable.ContainsKey("Blinding_hit_ranged"))
                skill.Variable.Remove("Blinding_hit_ranged");
            skill.Variable.Add("Blinding_hit_ranged", hit_ranged_add);
            actor.Status.hit_ranged_skill += (short)hit_ranged_add;

            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.LongHitDown = true;
            actor.Buff.ShortHitDown = true;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //近命中
            actor.Status.hit_melee_skill -= (short)skill.Variable["Blinding_hit_melee"];

            //遠命中
            actor.Status.hit_ranged_skill -= (short)skill.Variable["Blinding_hit_ranged"];

            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.LongHitDown = false;
            actor.Buff.ShortHitDown = false;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}

