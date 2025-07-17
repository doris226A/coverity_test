using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaDB.Quests
{
    public class QuestRewardInfo
    {
        uint id;
        uint exp, jexp, gold;
        uint ep;

        /// <summary>
        /// 任务ID
        /// </summary>
        public uint ID { get { return this.id; } set { this.id = value; } }

        /// <summary>
        /// 任务奖励经验值
        /// </summary>
        public uint EXP { get { return this.exp; } set { this.exp = value; } }

        /// <summary>
        /// 任务奖励职业经验值
        /// </summary>
        public uint JEXP { get { return this.jexp; } set { this.jexp = value; } }

        /// <summary>
        /// 任务奖励金钱
        /// </summary>
        public uint Gold { get { return this.gold; } set { this.gold = value; } }

        /// <summary>
        /// ECOKEY 任务奖励EP
        /// </summary>
        public uint EP { get { return this.ep; } set { this.ep = value; } }

    }
}
