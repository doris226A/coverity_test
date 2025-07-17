using System.Text;

using SagaDB.Actor;
using SagaDB.Partner;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Global
{
    public class PRHPRecoverUP : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0; 
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*    int value = 0;
                uint itemID = 0;
                ushort cubeID = 0;
                ActCubeData cube = PartnerFactory.Instance.actcubes_db_uniqueID[cubeID]; // 獲取方塊的數據
                PartnerCubeType cubeType = cube.cubetype;

                if (cubeType == PartnerCubeType.PASSIVESKILL && itemID == 68400020)
                    value = 50;
                else if (cubeType == PartnerCubeType.PASSIVESKILL && itemID == 68400021)
                    value = 150;*/
           int value = 50;
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "HPRecoverUP", true);
            skill.OnAdditionStart += (actor, sk) => StartEventHandler(actor, sk, value);
            skill.OnAdditionEnd += (actor, sk) => EndEventHandler(actor, sk, value);
            SkillHandler.ApplyAddition(sActor, skill);
        }

        void StartEventHandler(Actor actor, DefaultPassiveSkill skill, int value)
        {
            PartnerData partner = new PartnerData();
            if (skill.Variable.ContainsKey("HPRecover"))
                skill.Variable.Remove("HPRecover");
            skill.Variable.Add("HPRecover", value);
            partner.hp_in += (uint)value;
        }

        void EndEventHandler(Actor actor, DefaultPassiveSkill skill, int value)
        {
            PartnerData partner = new PartnerData();
            partner.hp_in -= (uint)value;
        }

        #endregion
    }
}
