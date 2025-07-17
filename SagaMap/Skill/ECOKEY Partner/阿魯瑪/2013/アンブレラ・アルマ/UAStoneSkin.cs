
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Cabalist
{
    /// <summary>
    /// 石化皮膚（メデューサスキン）
    /// </summary>
    public class UAStoneSkin : ISkill
    {
        bool MobUse;
        public UAStoneSkin()
        {
            this.MobUse = false;
        }
        public UAStoneSkin(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            dActor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
            if (MobUse)
            {
                level = 5;
            }
            int[] lifetime = { 0, 30000, 30000, 45000, 45000, 60000 };
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "StoneSkin", lifetime[level]);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            if (skill.Variable.ContainsKey("StoneSkin"))
                skill.Variable.Remove("StoneSkin");
            skill.Variable.Add("StoneSkin", skill.skill.Level);
            if (actor.type == ActorType.PC)
            {
                MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("已進入美杜莎皮膚狀態");
            }

        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (skill.Variable.ContainsKey("StoneSkin"))
                skill.Variable.Remove("StoneSkin");
            if (actor.type == ActorType.PC)
            {
                MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("已結束美杜莎皮膚狀態");
            }
        }
        #endregion
    }
}
