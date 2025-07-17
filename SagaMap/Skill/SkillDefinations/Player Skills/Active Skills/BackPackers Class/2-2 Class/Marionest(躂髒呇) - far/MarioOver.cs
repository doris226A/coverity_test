
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Marionest
{
    /// <summary>
    /// 活動木偶專家（マリオネットオーバーロード）
    /// </summary>
    public class MarioOver : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (dActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)dActor;
                if (pc.Marionette != null)
                {
                    if (pc.Tasks.ContainsKey("Marionette"))
                    {
                        int[] time = { 0, 300, 420, 600 };
                        if (pc.Tasks["Marionette"].period / 1000 < pc.Marionette.Duration + time[args.skill.Level])
                        {
                            return 0;
                        }
                    }

                }
            }
            return -12;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            map.SendEffect(dActor, 4205);
            //int[] time = { 0, 300000, 420000, 600000};
            int[] time = { 0, 300, 420, 600 };
            if (dActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)dActor;
                if (pc.Marionette != null)
                {
                    if (pc.Tasks.ContainsKey("Marionette"))
                    {
                        pc.Tasks["Marionette"].Deactivate();
                        pc.Tasks.Remove("Marionette");
                        ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                        Tasks.PC.Marionette task = new SagaMap.Tasks.PC.Marionette(eh.Client, pc.Marionette.Duration + time[level]);
                        pc.Tasks.Add("Marionette", task);
                        task.Activate();
                    }
                }


            }
        }
        #endregion
    }
}