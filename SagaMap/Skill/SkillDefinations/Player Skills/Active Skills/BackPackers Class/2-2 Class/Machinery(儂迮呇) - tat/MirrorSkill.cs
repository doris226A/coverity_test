
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Machinery
{
    /// <summary>
    /// 自爆（自爆）
    /// </summary>
    public class MirrorSkill : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            ActorPet pet = SkillHandler.Instance.GetPet(sActor);
            if (pet == null)
            {
                return -54;//需回傳"需裝備寵物"
            }
            if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
            {
                return 0;
            }
            return -54;//需回傳"需裝備寵物"
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            /*int lifetime = 3000;
            MirrorSkillBuff skill = new MirrorSkillBuff(args, sActor, lifetime);
            SkillHandler.ApplyAddition(sActor, skill);*/

            float factor = (float)(SagaLib.Global.Random.Next(10, 400) / 100f);
            Map map = Manager.MapManager.Instance.GetMap(dActor.MapID);
            List<Actor> affected = map.GetActorsArea(dActor, 300, false);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CheckValidAttackTarget(dActor, act))
                {
                    realAffected.Add(act);
                }
            }
            SkillHandler.Instance.PhysicalAttack(dActor, realAffected, args, dActor.WeaponElement, factor);

            ActorPC pc = (ActorPC)dActor;
            if (pc.Pet != null)
            {
                if (pc.Pet.Ride)
                {
                    ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)pc.e;
                    Packets.Client.CSMG_ITEM_MOVE p = new SagaMap.Packets.Client.CSMG_ITEM_MOVE();
                    p.data = new byte[11];
                    if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.PET))
                    {
                        SagaDB.Item.Item item = pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.PET];
                        if (item.Durability != 0) item.Durability--;
                        eh.Client.SendItemInfo(item);
                        eh.Client.SendSystemMessage(string.Format(Manager.LocalManager.Instance.Strings.PET_FRIENDLY_DOWN, item.BaseData.name));
                        EffectArg arg = new EffectArg();
                        arg.actorID = eh.Client.Character.ActorID;
                        arg.effectID = 8044;
                        eh.OnShowEffect(eh.Client.Character, arg);
                        p.InventoryID = item.Slot;
                        p.Target = SagaDB.Item.ContainerType.BODY;
                        p.Count = 1;
                        eh.Client.OnItemMove(p);
                        //ECOKEY 防止玩家死掉
                        pc.HP = pc.MaxHP;
                    }
                }
            }
            int A = SagaLib.Global.Random.Next(0, 99);

            short[] pos = new short[2];
            pos[0] = (short)(dActor.X + 500);
            pos[1] = (short)(dActor.Y + 500);
            map.MoveActor(Map.MOVE_TYPE.START, dActor, pos, sActor.Dir, 10000, true, SagaLib.MoveType.QUICKEN);
        }
        public class MirrorSkillBuff : DefaultBuff
        {
            private SkillArg args;
            public MirrorSkillBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "MirrorSkill", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }

            void StartEvent(Actor actor, DefaultBuff skill)
            {
                //Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                //Actor pc = map.GetActor(args.sActor);
                //map.SendEffect(actor, 5216);
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                float factor = (float)(SagaLib.Global.Random.Next(10, 400) / 100f);
                Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
                List<Actor> affected = map.GetActorsArea(actor, 300, false);
                List<Actor> realAffected = new List<Actor>();
                foreach (Actor act in affected)
                {
                    if (SkillHandler.Instance.CheckValidAttackTarget(actor, act))
                    {
                        realAffected.Add(act);
                    }
                }
                SkillHandler.Instance.PhysicalAttack(actor, realAffected, args, actor.WeaponElement, factor);
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, args, actor, false);

                ActorPC pc = (ActorPC)map.GetActor(args.sActor);
                map.SendEffect(pc, 5207);
            }
        }
        #endregion
    }
}
