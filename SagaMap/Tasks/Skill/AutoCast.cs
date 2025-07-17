using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.Skill
{
    public class AutoCast : MultiRunTask
    {
        Actor caster;
        SkillArg arg;
        public AutoCast(Actor pc, SkillArg arg)
        {
            this.period = 600000;
            this.dueTime = 0;
            this.caster = pc;
            this.arg = arg;
        }

        public override void CallBack()
        {
            try
            {
                this.Deactivate();
                AutoCastInfo info = null;
                foreach (AutoCastInfo i in arg.autoCast)
                {
                    info = i;
                    break;
                }
                if (info != null)
                {
                    arg.x = info.x;
                    arg.y = info.y;
                    arg.autoCast.Remove(info);
                    switch (caster.type)
                    {
                        case ActorType.PC:
                            {
                                ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)caster.e;
                                eh.Client.SkillDelay = DateTime.Now;
                                Packets.Client.CSMG_SKILL_CAST p1 = new SagaMap.Packets.Client.CSMG_SKILL_CAST();
                                p1.ActorID = arg.dActor;
                                p1.SkillID = (ushort)info.skillID;
                                p1.SkillLv = info.level;
                                p1.X = arg.x;
                                p1.Y = arg.y;
                                p1.Random = (short)Global.Random.Next();
                                eh.Client.OnSkillCast(p1, false, true);
                            }
                            break;
                        case ActorType.MOB:
                            ActorEventHandlers.MobEventHandler eh2 = (ActorEventHandlers.MobEventHandler)caster.e;
                            eh2.AI.CastSkill(info.skillID, info.level, arg.dActor, Global.PosX8to16(arg.x, eh2.AI.map.Width), Global.PosY8to16(arg.y, eh2.AI.map.Height));
                            break;
                        //ECOKEY 新增召喚物
                        case ActorType.SHADOW:
                            ActorEventHandlers.PetEventHandler eh4 = (ActorEventHandlers.PetEventHandler)caster.e;
                            eh4.AI.CastSkill(info.skillID, info.level, arg.dActor, Global.PosX8to16(arg.x, eh4.AI.map.Width), Global.PosY8to16(arg.y, eh4.AI.map.Height));
                            break;
                        //ECOKEY 新增寵物
                        case ActorType.PARTNER:
                            ActorEventHandlers.PartnerEventHandler eh3 = (ActorEventHandlers.PartnerEventHandler)caster.e;
                            eh3.AI.CastSkill(info.skillID, info.level, arg.dActor, Global.PosX8to16(arg.x, eh3.AI.map.Width), Global.PosY8to16(arg.y, eh3.AI.map.Height));
                            //eh3.AI.CustomCastSkill(info.skillID, info.level, 0, arg.dActor);
                            /*SkillArg argA = new SkillArg();
                            argA.sActor = caster.ActorID;
                            argA.skill = arg.skill;
                            argA.argType = SkillArg.ArgType.Cast;
                            argA.delay = (uint)arg.skill.CastTime;
                            argA.dActor = arg.dActor;
                            eh3.AI.OnSkillCastComplete(argA);*/
                            break;
                    }
                    this.dueTime = info.delay;
                }
                else
                {
                    caster.Tasks.Remove("AutoCast");
                    caster.Buff.CannotMove = false;
                    if (caster.type == ActorType.PC)
                    {
                        ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)caster.e;
                        eh.Client.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, caster, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }
    }
}
