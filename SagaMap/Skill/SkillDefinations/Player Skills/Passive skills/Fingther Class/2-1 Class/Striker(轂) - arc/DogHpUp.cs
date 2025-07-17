
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Striker
{
    public class DogHpUp : ISkill
    {
        private float[] HP_AddRate = { 0f, 6f, 6, 8f, 8f, 10f };
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = false;
            ActorPet pet = SkillHandler.Instance.GetPet(sActor);
            if (pet != null)
            {
                if (SkillHandler.Instance.CheckMobType(pet, "ANIMAL"))
                {
                    active = true;
                }
                DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "DogHpUp", active);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(sActor, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            float hpadd = new float[] { 0, 0.12f, 0.12f, 0.16f, 0.16f, 0.2f }[skill.skill.Level];
            int AddHP = (int)(pet.MaxHP * hpadd);
            if (skill.Variable.ContainsKey("DogHpUp_AddHP"))
                skill.Variable.Remove("DogHpUp_AddHP");
            skill.Variable.Add("DogHpUp_AddHP", AddHP);
            pet.MaxHP += (uint)AddHP;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, pet, true);
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            ActorPet pet = SkillHandler.Instance.GetPet(actor);
            pet.MaxHP -= (uint)skill.Variable["DogHpUp_AddHP"];
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, pet, true);
        }
        #endregion
    }
}

