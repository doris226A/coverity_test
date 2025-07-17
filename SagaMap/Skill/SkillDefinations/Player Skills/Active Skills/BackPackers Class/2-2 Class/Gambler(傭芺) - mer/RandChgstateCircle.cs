
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gambler
{
    /// <summary>
    /// 謎（SAGA6）（エニグマ）
    /// </summary>
    public class RandChgstateCircle : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            switch (SagaLib.Global.Random.Next(1, 21))
            {
                case 1:
                    ATKUpBuff skill1 = new ATKUpBuff(args, sActor, 90000);
                    SkillHandler.ApplyAddition(sActor, skill1);
                    break;
                case 2:
                    DefUpBuff skill2 = new DefUpBuff(args, sActor, 90000);
                    SkillHandler.ApplyAddition(sActor, skill2);
                    break;
                case 3:
                    HitMeleeUpBuff skill3 = new HitMeleeUpBuff(args, sActor, 60000);
                    SkillHandler.ApplyAddition(sActor, skill3);
                    break;
                case 4:
                    LHitUpBuff skill4 = new LHitUpBuff(args, sActor, 6000);
                    SkillHandler.ApplyAddition(sActor, skill4);
                    break;
                case 5:
                    AvoidMeleeUpBuff skill5 = new AvoidMeleeUpBuff(args, sActor, 10000);
                    SkillHandler.ApplyAddition(sActor, skill5);
                    break;
                case 6:
                    CriUpBuff skill6 = new CriUpBuff(args, sActor, 10000);
                    SkillHandler.ApplyAddition(sActor, skill6);
                    break;
                case 7:
                    AtkUpOneBuff skill7 = new AtkUpOneBuff(args, sActor, 60000);
                    SkillHandler.ApplyAddition(sActor, skill7);
                    break;
                case 8:
                    OverWorkBuff skill8 = new OverWorkBuff(args, sActor, 20000);
                    SkillHandler.ApplyAddition(sActor, skill8);
                    break;
                case 9:
                    EnergyBarrierBuff skill9 = new EnergyBarrierBuff(args, sActor, 60000);
                    SkillHandler.ApplyAddition(sActor, skill9);
                    break;
                case 10:
                    MagicBarrierBuff skill10 = new MagicBarrierBuff(args, sActor, 60000);
                    SkillHandler.ApplyAddition(sActor, skill10);
                    break;
                case 11:
                    DevineBarrierBuff skill11 = new DevineBarrierBuff(args, sActor, 125000);
                    SkillHandler.ApplyAddition(sActor, skill11);
                    break;
                case 12:
                    StrVitAgiDownOneBuff skill12 = new StrVitAgiDownOneBuff(args, sActor, 18000);
                    SkillHandler.ApplyAddition(sActor, skill12);
                    break;
                case 13:
                    MagIntDexDownOneBuff skill13 = new MagIntDexDownOneBuff(args, sActor, 18000);
                    SkillHandler.ApplyAddition(sActor, skill13);
                    break;
                case 14:
                    AtkdownBuff skill14 = new AtkdownBuff(args, sActor, 10000);
                    SkillHandler.ApplyAddition(sActor, skill14);
                    break;
                case 15:
                    AvoiddownOneBuff skill15 = new AvoiddownOneBuff(args, sActor, 10000);
                    SkillHandler.ApplyAddition(sActor, skill15);
                    break;
                case 16:
                    DefdownOneBuff skill16 = new DefdownOneBuff(args, sActor, 10000);
                    SkillHandler.ApplyAddition(sActor, skill16);
                    break;
                case 17:
                    AtkdownOneBuff skill17 = new AtkdownOneBuff(args, sActor, 60000);
                    SkillHandler.ApplyAddition(sActor, skill17);
                    break;
                case 18:
                    猛毒遲緩(sActor, args);
                    break;
                case 19:
                    猛毒石化(sActor, args);
                    break;
                case 20:
                    凍結(sActor, args);
                    break;
                case 21:
                    石化(sActor, args);
                    break;
            }
        }
        #endregion
        //攻擊．煥發 アタックバースト
        public class ATKUpBuff : DefaultBuff
        {
            private SkillArg args;
            public ATKUpBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "ATKUp", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.max_atk1_skill += 5;
                actor.Status.max_atk2_skill += 5;
                actor.Status.max_atk3_skill += 5;
                actor.Buff.MaxAtkUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 4047);
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.max_atk1_skill -= 5;
                actor.Status.max_atk2_skill -= 5;
                actor.Status.max_atk3_skill -= 5;
                actor.Buff.MaxAtkUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //重裝鎧化（ディフェンス・バースト）
        public class DefUpBuff : DefaultBuff
        {
            private SkillArg args;
            public DefUpBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "重装铠化", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.def_skill += 5;
                actor.Status.def_add_skill += 10;
                actor.Status.mdef_skill += 4;
                actor.Status.mdef_add_skill += 7;
                actor.Buff.DefUp = true;
                actor.Buff.MagicDefUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 4047);
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.mdef_add_skill -= 7;
                actor.Status.mdef_skill -= 4;
                actor.Status.def_add_skill -= 10;
                actor.Status.def_skill -= 5;
                actor.Buff.DefUp = false;
                actor.Buff.MagicDefUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //集中 コンセントレート
        public class HitMeleeUpBuff : DefaultBuff
        {
            private SkillArg args;
            public HitMeleeUpBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "HitMeleeUp", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.hit_melee_skill += 6;
                actor.Buff.ShortHitUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 4005);
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.hit_melee_skill -= 6;
                actor.Buff.ShortHitUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //名射手的光环（マークスマンオーラ）
        public class LHitUpBuff : DefaultBuff
        {
            private SkillArg args;
            public LHitUpBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "MarkMannAura", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.hit_ranged_skill += 6;
                actor.Buff.LongHitUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 4097);
            }

            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.hit_ranged_skill -= 6;
                actor.Buff.LongHitUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //敏捷的動作
        public class AvoidMeleeUpBuff : DefaultBuff
        {
            private SkillArg args;
            public AvoidMeleeUpBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "AvoidBurst", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.avoid_melee_skill += 6;
                actor.Buff.ShortDodgeUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 4047);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.avoid_melee_skill -= 6;
                actor.Buff.ShortDodgeUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //會心一擊
        public class CriUpBuff : DefaultBuff
        {
            private SkillArg args;
            public CriUpBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "CriUp", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.cri_skill += 10;
                SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("會心一擊率提高已進入狀態");
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 4046);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.cri_skill -= 10;
                SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("會心一擊率提高狀態已解除");
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //應援（応援）
        public class AtkUpOneBuff : DefaultBuff
        {
            private SkillArg args;
            public AtkUpOneBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "AtkUpOne", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.max_atk1_skill += 10;
                actor.Status.max_atk2_skill += 10;
                actor.Status.max_atk3_skill += 10;
                actor.Status.min_atk1_skill += 10;
                actor.Status.min_atk2_skill += 10;
                actor.Status.min_atk3_skill += 10;
                actor.Status.max_matk_skill += 10;
                actor.Status.min_matk_skill += 10;
                actor.Buff.MinAtkUp = true;
                actor.Buff.MaxAtkUp = true;
                actor.Buff.MinMagicAtkUp = true;
                actor.Buff.MaxMagicAtkUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5077);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.max_atk1_skill -= 10;
                actor.Status.max_atk2_skill -= 10;
                actor.Status.max_atk3_skill -= 10;
                actor.Status.min_atk1_skill -= 10;
                actor.Status.min_atk2_skill -= 10;
                actor.Status.min_atk3_skill -= 10;
                actor.Status.max_matk_skill -= 10;
                actor.Status.min_matk_skill -= 10;
                actor.Buff.MinAtkUp = false;
                actor.Buff.MaxAtkUp = false;
                actor.Buff.MinMagicAtkUp = false;
                actor.Buff.MaxMagicAtkUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //狂亂時間（オーバーワーク）
        public class OverWorkBuff : DefaultBuff
        {
            private SkillArg args;
            public OverWorkBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "OverWork", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                if (skill.Variable.ContainsKey("OverWork"))
                    skill.Variable.Remove("OverWork");
                skill.Variable.Add("OverWork", 15);
                actor.Status.aspd_skill_perc += 0.15f;
                actor.Buff.OverWork = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5170);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                if (skill.Variable.ContainsKey("OverWork"))
                    skill.Variable.Remove("OverWork");
                actor.Status.aspd_skill_perc -= 0.15f;
                actor.Buff.OverWork = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //能量光壁
        public class EnergyBarrierBuff : DefaultBuff
        {
            private SkillArg args;
            public EnergyBarrierBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "EnergyBarrier", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.def_skill += 20;
                actor.Status.def_add_skill += 20;
                actor.Buff.DefUp = true;
                actor.Buff.DefRateUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5168);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.def_skill -= 20;
                actor.Status.def_add_skill -= 20;
                actor.Buff.DefUp = false;
                actor.Buff.DefRateUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //魔法光壁
        public class MagicBarrierBuff : DefaultBuff
        {
            private SkillArg args;
            public MagicBarrierBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "MagicBarrier", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.mdef_skill += 20;
                actor.Status.mdef_add_skill += 20;
                actor.Buff.MagicDefUp = true;
                actor.Buff.MagicDefRateUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5169);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.mdef_skill -= 20;
                actor.Status.mdef_add_skill -= 20;
                actor.Buff.MagicDefUp = false;
                actor.Buff.MagicDefRateUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //神圣光界
        public class DevineBarrierBuff : DefaultBuff
        {
            private SkillArg args;
            public DevineBarrierBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "DevineBarrier", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
                SkillHandler.RemoveAddition(actor, "MagicBarrier");
                SkillHandler.RemoveAddition(actor, "EnergyBarrier");
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.mdef_skill += 20;
                actor.Status.mdef_add_skill += 15;
                actor.Buff.MagicDefUp = true;
                actor.Buff.MagicDefRateUp = true;
                actor.Status.def_skill += 13;
                actor.Status.def_add_skill += 10;
                actor.Buff.DefUp = true;
                actor.Buff.DefRateUp = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 4019);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.mdef_skill -= 20;
                actor.Status.mdef_add_skill -= 15;
                actor.Buff.MagicDefUp = false;
                actor.Buff.MagicDefRateUp = false;
                actor.Status.def_skill -= 13;
                actor.Status.def_add_skill -= 10;
                actor.Buff.DefUp = false;
                actor.Buff.DefRateUp = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //弱化（ディビリテイト）
        public class StrVitAgiDownOneBuff : DefaultBuff
        {
            private SkillArg args;
            public StrVitAgiDownOneBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "StrVitAgiDownOne", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.vit_skill -= 10;
                actor.Status.agi_skill -= 13;
                actor.Status.str_skill -= 9;
                actor.Buff.VITDown = true;
                actor.Buff.AGIDown = true;
                actor.Buff.STRDown = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5171);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.vit_skill += 10;
                actor.Status.agi_skill += 13;
                actor.Status.str_skill += 9;
                actor.Buff.VITDown = false;
                actor.Buff.AGIDown = false;
                actor.Buff.STRDown = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //神經衰弱（クラッター）
        public class MagIntDexDownOneBuff : DefaultBuff
        {
            private SkillArg args;
            public MagIntDexDownOneBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "MagIntDexDownOne", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.int_skill -= 9;
                actor.Status.mag_skill -= 9;
                actor.Status.dex_skill -= 11;
                actor.Buff.INTDown = true;
                actor.Buff.DEXDown = true;
                actor.Buff.MAGDown = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5172);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.int_skill += 9;
                actor.Status.mag_skill += 9;
                actor.Status.dex_skill += 11;
                actor.Buff.INTDown = false;
                actor.Buff.DEXDown = false;
                actor.Buff.MAGDown = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //力量下降（パワーダウン）
        public class AtkdownBuff : DefaultBuff
        {
            private SkillArg args;
            public AtkdownBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "Atkdown", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                short max = (short)(actor.Status.max_atk_bs * 0.45f);
                short min = (short)(actor.Status.min_atk_bs * 0.03f);
                if (skill.Variable.ContainsKey("Atkdown_max"))
                    skill.Variable.Remove("Atkdown_max");
                skill.Variable.Add("Atkdown_max", max);
                if (skill.Variable.ContainsKey("Atkdown_min"))
                    skill.Variable.Remove("Atkdown_min");
                skill.Variable.Add("Atkdown_min", min);
                actor.Status.max_atk1_skill -= max;
                actor.Status.max_atk2_skill -= max;
                actor.Status.max_atk3_skill -= max;
                actor.Status.min_atk1_skill -= min;
                actor.Status.min_atk2_skill -= min;
                actor.Status.min_atk3_skill -= min;
                actor.Buff.MaxAtkDown = true;
                actor.Buff.MinAtkDown = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5017);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                short max = (short)skill.Variable["Atkdown_max"];
                short min = (short)skill.Variable["Atkdown_min"];
                actor.Status.max_atk1_skill += max;
                actor.Status.max_atk2_skill += max;
                actor.Status.max_atk3_skill += max;
                actor.Status.min_atk1_skill += min;
                actor.Status.min_atk2_skill += min;
                actor.Status.min_atk3_skill += min;
                actor.Buff.MaxAtkDown = false;
                actor.Buff.MinAtkDown = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //速度下降スピードダウン
        public class AvoiddownOneBuff : DefaultBuff
        {
            private SkillArg args;
            public AvoiddownOneBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "AvoiddownOne", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.avoid_melee_skill -= 6;
                actor.Status.avoid_ranged_skill -= 6;
                actor.Buff.LongDodgeDown = true;
                actor.Buff.ShortDodgeDown = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5017);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.avoid_melee_skill -= 6;
                actor.Status.avoid_ranged_skill -= 6;
                actor.Buff.LongDodgeDown = false;
                actor.Buff.ShortDodgeDown = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //生命下降（バイタリティダウン）
        public class DefdownOneBuff : DefaultBuff
        {
            private SkillArg args;
            public DefdownOneBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "DefdownOne", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.def_skill -= 5;
                actor.Status.def_add_skill -= 5;
                actor.Buff.DefDown = true;
                actor.Buff.DefRateDown = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5017);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                actor.Status.def_skill += 5;
                actor.Status.def_add_skill += 5;
                actor.Buff.DefDown = false;
                actor.Buff.DefRateDown = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        //女子是撒嬌/男子是魄力（女は愛嬌、男は度胸）
        public class AtkdownOneBuff : DefaultBuff
        {
            private SkillArg args;
            public AtkdownOneBuff(SkillArg args, Actor actor, int lifetime)
                : base(args.skill, actor, "AtkdownOne", lifetime)
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
                this.args = args.Clone();
            }
            void StartEvent(Actor actor, DefaultBuff skill)
            {
                short max = (short)(actor.Status.max_atk_bs * 0.18f);
                short min = (short)(actor.Status.min_atk_bs * 0.14f);
                if (skill.Variable.ContainsKey("AtkdownOne_max"))
                    skill.Variable.Remove("AtkdownOne_max");
                skill.Variable.Add("AtkdownOne_max", max);
                if (skill.Variable.ContainsKey("AtkdownOne_min"))
                    skill.Variable.Remove("AtkdownOne_min");
                skill.Variable.Add("AtkdownOne_min", min);
                actor.Status.max_atk1_skill -= max;
                actor.Status.max_atk2_skill -= max;
                actor.Status.max_atk3_skill -= max;
                actor.Status.min_atk1_skill -= min;
                actor.Status.min_atk2_skill -= min;
                actor.Status.min_atk3_skill -= min;
                actor.Buff.MaxAtkDown = true;
                actor.Buff.MinAtkDown = true;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5261);
            }
            void EndEvent(Actor actor, DefaultBuff skill)
            {
                short max = (short)skill.Variable["AtkdownOne_max"];
                short min = (short)skill.Variable["AtkdownOne_min"];
                actor.Status.max_atk1_skill += max;
                actor.Status.max_atk2_skill += max;
                actor.Status.max_atk3_skill += max;
                actor.Status.min_atk1_skill += min;
                actor.Status.min_atk2_skill += min;
                actor.Status.min_atk3_skill += min;
                actor.Buff.MaxAtkDown = false;
                actor.Buff.MinAtkDown = false;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            }
        }

        void 猛毒遲緩(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                Additions.Global.DeadlyPosion nskill = new SagaMap.Skill.Additions.Global.DeadlyPosion(args.skill, act, "DeadlyPosion", 16000, 2000);
                SkillHandler.ApplyAddition(act, nskill);
                if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.鈍足, 100))
                {
                    Additions.Global.MoveSpeedDown skill = new SagaMap.Skill.Additions.Global.MoveSpeedDown(args.skill, act, 5000);
                    SkillHandler.ApplyAddition(act, skill);
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5110);
        }
        void 猛毒石化(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                Additions.Global.DeadlyPosion nskill = new SagaMap.Skill.Additions.Global.DeadlyPosion(args.skill, act, "DeadlyPosion", 16000, 2000);
                SkillHandler.ApplyAddition(act, nskill);
                if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stone, 100))
                {
                    Additions.Global.Stone skill = new SagaMap.Skill.Additions.Global.Stone(args.skill, act, 5000);
                    SkillHandler.ApplyAddition(act, skill);
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5109);
        }
        void 凍結(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Frosen, 100))
                {
                    Additions.Global.Freeze skill = new SagaMap.Skill.Additions.Global.Freeze(args.skill, act, 5000);
                    SkillHandler.ApplyAddition(act, skill);
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5104);
        }
        void 石化(Actor sActor, SkillArg args)
        {
            Map map = Manager.MapManager.Instance.GetMap(sActor.MapID);
            List<Actor> affected = map.GetActorsArea(sActor, 100, true);
            List<Actor> realAffected = new List<Actor>();
            foreach (Actor act in affected)
            {
                if (SkillHandler.Instance.CanAdditionApply(sActor, act, SkillHandler.DefaultAdditions.Stone, 100))
                {
                    Additions.Global.Stone skill = new SagaMap.Skill.Additions.Global.Stone(args.skill, act, 5000);
                    SkillHandler.ApplyAddition(act, skill);
                }
            }
            Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5098);
        }
    }
}