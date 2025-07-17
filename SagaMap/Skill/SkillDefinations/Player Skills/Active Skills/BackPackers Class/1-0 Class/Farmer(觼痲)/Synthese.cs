using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;
using SagaMap.Network.Client;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 各种制作技能
    /// </summary>
    public class Synthese : SkillEvent
    {
        protected override void RunScript(SkillEvent.Parameter para)
        {
            if (para.dActor.type == ActorType.PC)
            {
                Scripting.SkillEvent.Instance.Synthese((ActorPC)para.dActor, (ushort)para.args.skill.ID, para.level, true);
            }
        }
    }
    }

