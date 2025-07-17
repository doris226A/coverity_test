using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Item;
using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Cabalist
{
    /// <summary>
    /// グリムリーパー
    /// </summary>
    public class GrimReaper : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (!this.CheckPossible((SagaDB.Actor.Actor)pc))
                return -5;
            return SkillHandler.Instance.CheckValidAttackTarget((SagaDB.Actor.Actor)pc, dActor) ? 0 : -14;
        }

        /// <summary>
        /// The CheckPossible.
        /// </summary>
        /// <param name="sActor">The sActor<see cref="SagaDB.Actor.Actor"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool CheckPossible(SagaDB.Actor.Actor sActor)
        {
            if (sActor.type != ActorType.PC)
                return true;
            ActorPC actorPc = (ActorPC)sActor;
            return actorPc.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND) || actorPc.Inventory.GetContainer(ContainerType.RIGHT_HAND2).Count > 0;
        }

        /// <summary>
        /// The Proc.
        /// </summary>
        /// <param name="sActor">The sActor<see cref="SagaDB.Actor.Actor"/>.</param>
        /// <param name="dActor">The dActor<see cref="SagaDB.Actor.Actor"/>.</param>
        /// <param name="args">The args<see cref="SkillArg"/>.</param>
        /// <param name="level">The level<see cref="byte"/>.</param>
        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            if (!this.CheckPossible(sActor))
                return;
            args.type = ATTACK_TYPE.BLOW;
            float ATKBonus = (float)(1.75f + 0.35f * level);
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, SagaLib.Elements.Dark, ATKBonus);
        }
        #endregion
    }
}
/*  return 0;
}

public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
{
  float factor = 0;
  args.type = ATTACK_TYPE.BLOW;

  factor = 1.75f + 0.35f * level;
  List<Actor> actors = new List<Actor>();
  if (level == 6)
  {
      EffectArg arg = new EffectArg();
      arg.effectID = 5236;
      arg.actorID = dActor.ActorID;
      Manager.MapManager.Instance.GetMap(sActor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg, (ActorPC)sActor, true);
      factor = 2f;
      for (int i = 0; i < 3; i++)
      {
          actors.Add(dActor);
      }
      args.delay = 500;
      SkillHandler.Instance.PhysicalAttack(sActor, actors, args, SkillHandler.DefType.Def, SagaLib.Elements.Dark, 0, factor, false, 0.6f, false);
  }
  else
  {
      actors.Add(dActor);
      SkillHandler.Instance.PhysicalAttack(sActor, actors, args, SkillHandler.DefType.Def, SagaLib.Elements.Dark, 0, factor, false, 0.1f, false);
  }

}

#endregion
}
}
*/