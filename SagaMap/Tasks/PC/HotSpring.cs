using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;
using SagaMap.Skill;
using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public partial class HotSpring : MultiRunTask
    {
        MapClient client;
        public HotSpring(MapClient client)
        {
            this.dueTime = 180000;
            this.period = 180000;
            this.client = client;
        }

        public override void CallBack()
        {
            try
            {
                if (client != null)
                {
                    ActorPC pc = client.Character;
                    if (pc == null) return;
                    if (pc.MapID != 31303000)
                    {
                        this.Deactivate();
                        this.client.Character.Tasks.Remove("HotSpring");
                        return;
                    }
                    SagaDB.Skill.Skill skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(100, 1);
                    SagaMap.Skill.Additions.Global.HotSpring Spring = new SagaMap.Skill.Additions.Global.HotSpring(skill, pc, 1800000, 5000);
                    SagaMap.Skill.SkillHandler.ApplyAddition(pc, Spring);
                }
                else
                {
                    this.Deactivate();
                    this.client.Character.Tasks.Remove("HotSpring");
                }

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.Deactivate();
                this.client.Character.Tasks.Remove("HotSpring");
            }
        }
    }
}
