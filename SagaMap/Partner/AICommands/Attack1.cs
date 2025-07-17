using System;
using System.Collections.Generic;
using System.Text;
using SagaDB.Skill;
using SagaLib;
using SagaDB.Actor;
using SagaDB.Partner;
using SagaMap;
using SagaMap.Scripting;
//ECOKEY 寵物放置判斷
using SagaDB.Item;
using SagaMap.Network.Client;

namespace SagaMap.Partner.AICommands
{
    public class Attack : AICommand
    {
        private CommandStatus status;
        private PartnerAI partnerai;
        private Actor dest;
        private bool attacking;
        PartnerAttack attacktask;
        public bool active;
        short x, y;
        int counter = 0;
        public Dictionary<byte, DateTime> ai_lastSkillCast = new Dictionary<byte, DateTime>(); //ECOKEY放技能的時間

        public Attack(PartnerAI partnerai)
        {
            //ECOKEY直接新增初始數據
            ai_lastSkillCast.Add(0, DateTime.Now);
            ai_lastSkillCast.Add(1, DateTime.Now);
            ai_lastSkillCast.Add(2, DateTime.Now);
            ai_lastSkillCast.Add(3, DateTime.Now);
            ai_lastSkillCast.Add(4, DateTime.Now);
            this.partnerai = partnerai;
            ActorPartner partner = (ActorPartner)this.partnerai.Partner;
            //ECOKEY 自定義AI追加
            if (partner.PlusAI >= 1)
            {
                ai_lastSkillCast.Add(5, DateTime.Now);
                ai_lastSkillCast.Add(6, DateTime.Now);
                ai_lastSkillCast.Add(7, DateTime.Now);
                ai_lastSkillCast.Add(8, DateTime.Now);
                ai_lastSkillCast.Add(9, DateTime.Now);
            }
            this.Status = CommandStatus.INIT;
            int aspd = 0;
            aspd = partner.Status.aspd;
            attacktask = new PartnerAttack(partnerai, dest);
            x = partnerai.Partner.X;
            y = partnerai.Partner.Y;
            //CustomPassiveSkill_once();//ECOKEY HPMPSP自然回復量計算
        }

        public string GetName() { return "Attack"; }

        private Actor CurrentTarget()
        {
            try
            {
                uint id = 0;
                uint hate = 0;
                Actor tmp = null;
                ActorPartner partner = (ActorPartner)partnerai.Partner;
                uint[] ids = new uint[partnerai.Hate.Keys.Count];
                partnerai.Hate.Keys.CopyTo(ids, 0);
                for (uint i = 0; i < partnerai.Hate.Keys.Count; i++)//Find out the actorPC with the highest hate value
                {
                    if (ids[i] == 0) continue;
                    if (ids[i] == this.partnerai.Partner.ActorID)
                        continue;
                    if (this.partnerai.Master != null)
                    {
                        if (ids[i] == this.partnerai.Master.ActorID && partnerai.Hate.Count > 1)
                            continue;
                    }
                    if (!partnerai.Hate.ContainsKey(ids[i]))
                        continue;
                    if (hate < partnerai.Hate[ids[i]])
                    {
                        hate = partnerai.Hate[ids[i]];
                        id = ids[i];
                        tmp = partnerai.map.GetActor(id);
                        if (tmp == null)
                        {
                            partnerai.Hate.Remove(id);
                            id = 0;
                            hate = 0;
                            active = false;
                            i = 0;
                            continue;
                        }
                        active = true;
                    }
                }

                if (partner.Owner.PartnerTartget != null && partner.ai_mode <= 1)
                {
                    id = partner.Owner.PartnerTartget.ActorID;
                }

                if (id != 0)//Now the id is refer to the PC with the highest hate to the Mob.现在这个ID是怪物对最高仇恨者的ID
                {
                    tmp = partnerai.map.GetActor(id);
                    if (tmp != null)
                    {
                        //ECOKEY 主人死亡時繼續執行
                        if (tmp.HP == 0 && tmp.ActorID != partner.Owner.ActorID)
                        {
                            partnerai.Hate.Remove(tmp.ActorID);
                            if (partner.Owner.PartnerTartget != null)
                                partner.Owner.PartnerTartget = null;
                            id = 0;
                            active = false;
                        }
                        /* if (tmp.HP == 0)
                         {
                             partnerai.Hate.Remove(tmp.ActorID);
                             if (partner.Owner.PartnerTartget != null)
                                 partner.Owner.PartnerTartget = null;
                             id = 0;
                             active = false;
                         }*/
                    }
                }
                if (id == 0)
                {
                    active = false;
                    return null;
                }
                try
                {
                    if (dest != null && dest.ActorID != id)
                    {
                        if (attacktask.Activated == true)
                        {
                            attacktask.Deactivate();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                /*   if (dest != null)
                   {
                       if (dest.ActorID != id) if (attacktask.Activated == true) attacktask.Deactivate();
                   }*/
                return tmp;
            }
            catch (Exception ex)
            {
                SagaLib.Logger.ShowError(ex);
                return null;
            }

        }

        private void CheckAggro()
        {
            double distance = double.MaxValue;
            Actor target = null;
            bool isSlavaOfPc = false;
            if (this.partnerai.Master != null)
            {
                if (!this.partnerai.Hate.ContainsKey(this.partnerai.Master.ActorID))
                    this.partnerai.Hate.Add(this.partnerai.Master.ActorID, 1);
                if (this.partnerai.Master.type == ActorType.PC)
                    isSlavaOfPc = true;
            }
            uint[] actorIds = partnerai.Partner.VisibleActors.ToArray(); // Convert to an array for indexed access

            for (int index = 0; index < actorIds.Length; index++) // Use a for loop with index
            {
                uint id = actorIds[index];
                Actor i = partnerai.map.GetActor(id);
                //  foreach (uint id in partnerai.Partner.VisibleActors)
                //  {
                //  Actor i = partnerai.map.GetActor(id);
                if (i == null)
                    continue;
                if (i.Buff.Transparent)
                    continue;
                if (i.MapID != this.partnerai.map.ID)
                    continue;
                if (i.Status.Additions.ContainsKey("IAmTree"))
                    continue;
                if (i.Status.Additions.ContainsKey("Through"))
                    continue;
                if (i.HP == 0)
                    continue;
                if (i.type == ActorType.MOB)
                {
                    ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)i.e;
                    //ECOKEY 寵物禁止攻擊玩家召喚物（0513再修正）
                    if (eh.AI.Master != null && eh.AI.Master.type != ActorType.MOB)
                    {
                        if (!Skill.SkillHandler.Instance.CheckValidAttackTarget(this.partnerai.Master, eh.AI.Master))
                        {
                            continue;
                        }
                    }
                    if (eh.AI.Mode.Symbol && !isSlavaOfPc)
                    {
                        partnerai.Hate.Add(i.ActorID, 20);
                        //SendAggroEffect();
                    }
                }
                if (!partnerai.Partner.Buff.Zombie && i.type != ActorType.PC && i.type != ActorType.PET && i.type != ActorType.SHADOW && !(i.type == ActorType.MOB && isSlavaOfPc))
                    continue;
                if (isSlavaOfPc && i.type == ActorType.SHADOW)
                    continue;
                if (isSlavaOfPc && i.type == ActorType.PET)
                    continue;
                if (isSlavaOfPc && i.type == ActorType.PC)
                    continue;
                if (isSlavaOfPc && i.type == ActorType.MOB)
                {
                    ActorEventHandlers.MobEventHandler ie = (ActorEventHandlers.MobEventHandler)i.e;
                    if (ie.AI.Master == this.partnerai.Master)
                        continue;
                }
                if (i.type == ActorType.PC)
                {
                    if (((ActorPC)i).PossessionTarget != 0)
                        continue;
                }
                double len = PartnerAI.GetLengthD(i.X, i.Y, partnerai.Partner.X, partnerai.Partner.Y);
                if (len < distance)
                {
                    byte x, y, x2, y2;
                    x = Global.PosX16to8(this.partnerai.Partner.X, this.partnerai.map.Width);
                    y = Global.PosY16to8(this.partnerai.Partner.Y, this.partnerai.map.Height);
                    x2 = Global.PosX16to8(i.X, this.partnerai.map.Width);
                    y2 = Global.PosY16to8(i.Y, this.partnerai.map.Height);

                    List<MapNode> path = this.partnerai.FindPath(x, y, x2, y2);
                    try
                    {
                        if (path[path.Count - 1].x == x2 && path[path.Count - 1].y == y2)
                        {
                            if (i.type == ActorType.SHADOW && target != i)
                            {
                                distance = 0;
                                target = i;
                            }
                            else
                            {
                                distance = len;
                                target = i;
                            }
                        }
                    }
                    catch { }
                }
            }
            if (distance <= 1000)
            {
                if (partnerai.Hate.Count == 0)//保存怪物战斗前位置
                {
                    partnerai.X_pb = partnerai.Partner.X;
                    partnerai.Y_pb = partnerai.Partner.Y;
                }
                if (!partnerai.Hate.ContainsKey(target.ActorID))
                    partnerai.Hate.Add(target.ActorID, 20);
                SendAggroEffect();
            }
        }

        void SendAggroEffect()
        {
            EffectArg arg = new EffectArg();
            arg.actorID = partnerai.Partner.ActorID;
            arg.effectID = 4539;
            partnerai.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg, partnerai.Partner, true);
        }

        private bool hasPlayerInSight()
        {
            List<uint> visibleActorsCopy = new List<uint>(partnerai.Partner.VisibleActors);
            for (int j = 0; j < visibleActorsCopy.Count; j++)
            {
                Actor i = partnerai.map.GetActor(visibleActorsCopy[j]);
                if (i == null)
                    continue;
                if (i.MapID != this.partnerai.map.ID)
                    continue;
                if (i.type == ActorType.PC)
                {
                    if (((ActorPC)i).Online)
                        return true;
                }
            }
            return false;
        }
        /*  private bool hasPlayerInSight()
          {
              foreach (uint id in partnerai.Partner.VisibleActors)
              {
                  Actor i = partnerai.map.GetActor(id);
                  if (i == null)
                      continue;
                  if (i.MapID != this.partnerai.map.ID)
                      continue;
                  if (i.type == ActorType.PC)
                  {
                      if (((ActorPC)i).Online)
                          return true;
                  }
              }
              return false;
          }*/

        public void Update(object para)
        {
            gomaster(); //ECOKEY 讓寵物回原本的位置?
            ActorPartner partner = null;
            if (this.partnerai.Partner.type == ActorType.PARTNER)
                partner = (ActorPartner)this.partnerai.Partner;

            if (this.partnerai.Hate.Count == 0)
            {
                if (!hasPlayerInSight())
                {
                    counter++;
                    if (counter > 100)
                    {
                        this.partnerai.Pause();
                        counter = 0;
                        return;
                    }
                }
            }

            if (partner != null)
            {
                //NEw
                if (partner.Owner != null)
                {
                    if (!partnerai.Hate.ContainsKey(partner.Owner.ActorID))
                    {
                        partnerai.Hate.Add(partner.Owner.ActorID, 1); // 如果該鍵不存在，則添加新的鍵值對
                    }
                    else
                    {
                        partnerai.Hate[partner.Owner.ActorID] = 1; // 如果該鍵已存在，則更新該鍵的值
                    }
                }
                if (!partnerai.Hate.ContainsKey(partner.Owner.ActorID))
                {
                    partnerai.Hate.Add(partner.Owner.ActorID, 1); // 如果該鍵不存在，則添加新的鍵值對
                }
                else
                {
                    partnerai.Hate[partner.Owner.ActorID] = 1; // 如果該鍵已存在，則更新該鍵的值
                }
            }

            /*  if (partner != null)
              {
                  if (!partnerai.Hate.ContainsKey(partner.Owner.ActorID))
                  {
                      partnerai.Hate.Add(partner.Owner.ActorID, 1);
                  }
              }*/

            if (partnerai.Master != null)
                if (!partnerai.Hate.ContainsKey(this.partnerai.Master.ActorID))
                    partnerai.Hate.Add(this.partnerai.Master.ActorID, 1);

            if (partnerai.Partner.Tasks.ContainsKey("AutoCast"))
            {
                if (attacktask.Activated == true)
                    attacktask.Deactivate();

                attacking = false;
                return;
            }
            //ECOKEY 清除
            //施放主人战斗中技能，放在这个位置保证平时状态
            /*if ((DateTime.Now - partnerai.LastSkillCast).TotalSeconds >= 1)
            {
                if (partner != null)
                {
                    ActorPC pc = partner.Owner;
                    if (pc.BattleStatus == 1)
                    {
                        if (Global.Random.Next(0, 99) < partnerai.Mode.EventMasterCombatSkillRate)
                        {
                            partnerai.OnShouldCastSkill(partnerai.Mode.EventMasterCombat, pc);
                            partnerai.LastSkillCast = DateTime.Now;
                        }
                    }
                }
            }*/

            /* if (attacktask == null)
                  attacktask = new PartnerAttack(partnerai, dest);*/


            dest = CurrentTarget();
            //10.12error 卡寵
            if (attacktask == null)
                attacktask = new PartnerAttack(partnerai, dest);

            attacktask.dActor = dest;
            if ((partnerai.Mode.Active || partnerai.Partner.Buff.Zombie) && (dest == null || dest == partnerai.Master))
            {
                CheckAggro();
            }
            if (dest == null)
            {
                partnerai.AIActivity = Activity.IDLE;
                if (partnerai.commands.ContainsKey("Chase") == true)
                    partnerai.commands.Remove("Chase"); ;
                return;
            }
            partnerai.AIActivity = Activity.BUSY;
            if (partnerai.commands.ContainsKey("Move") == true) partnerai.commands.Remove("Move");

            if ((DateTime.Now - partnerai.LastSkillCast).TotalSeconds >= 2)//ECOKEY 新增放技冷卻
            {
                partnerai.LastSkillCast = DateTime.Now;
                if (partner.ai_mode == 3)
                {
                    CustomSkill(dest);//ECOKEY自定義技能
                }
                else if (partner.ai_mode == 4)//ECOKEY 自定義AI2
                {
                    CustomSkill2(dest);
                }
            }
            CustomPassiveSkill();//ECOKEY 每五秒回復HPMPSP




            //ECOKEY 清除
            /*if ((DateTime.Now - partnerai.LastSkillCast).TotalSeconds >= 10 && dest != partner.Owner)//施放技能，放在这个位置保证追踪模式下的技能优先
            {
                if (Global.Random.Next(0, 99) < partnerai.Mode.EventAttackingSkillRate)
                {
                    partnerai.OnShouldCastSkill(partnerai.Mode.EventAttacking, dest);
                    partnerai.LastSkillCast = DateTime.Now;
                }
            }*/
            if (partnerai.commands.ContainsKey("Chase") == true)
            {
                if (attacktask.Activated == true)
                    attacktask.Deactivate();
                attacking = false;
                return;
            }


            if (this.x != this.partnerai.Partner.X || this.y != this.partnerai.Partner.Y)
            {
                short x, y;
                this.partnerai.map.FindFreeCoord(this.partnerai.Partner.X, this.partnerai.Partner.Y, out x, out y, this.partnerai.Partner);
                bool skip = false;
                if (partnerai.Partner.type == ActorType.PET)
                {
                    if (((ActorPet)partnerai.Partner).BaseData.mobType == SagaDB.Mob.MobType.MAGIC_CREATURE)
                        skip = true;
                }
                if ((this.partnerai.Partner.X == x && this.partnerai.Partner.Y == y) || this.partnerai.Mode.RunAway || skip)
                {

                }
                else
                {
                    short[] dst = new short[2] { x, y };

                    partnerai.map.MoveActor(Map.MOVE_TYPE.START, partnerai.Partner, dst, PartnerAI.GetDir((short)(dst[0] - x), (short)(dst[1] - y)), (ushort)(partnerai.Partner.Speed / 20));
                    return;
                }
                this.x = partnerai.Partner.X;
                this.y = partnerai.Partner.Y;
            }
            if (dest.HP == 0)
            {
                if (partner != null)
                {
                    if (dest.ActorID != partner.Owner.ActorID)
                    {
                        if (partnerai.Hate.ContainsKey(dest.ActorID)) partnerai.Hate.Remove(dest.ActorID);
                        if (attacktask.Activated == true) attacktask.Deactivate();
                        attacktask = null;
                        return;
                    }
                }
                else
                {
                    if (partnerai.Hate.ContainsKey(dest.ActorID)) partnerai.Hate.Remove(dest.ActorID);
                    if (attacktask.Activated == true) attacktask.Deactivate();
                    attacktask = null;
                    return;
                }
            }
            float size;
            //ECOKEY 清除
            /*if (partnerai.Mode.isAnAI)
            {
                size = partnerai.needlen;
            }
            else */
            if (partnerai.Partner.type != ActorType.PC)
            {
                size = ((ActorPartner)partnerai.Partner).BaseData.range;
            }
            else
                size = 1;
            bool ifChase = false;
            if (PartnerAI.GetLengthD(partnerai.Partner.X, partnerai.Partner.Y, dest.X, dest.Y) > (size * 150) || ifChase)
            {
                if (PartnerAI.GetLengthD(partnerai.Partner.X, partnerai.Partner.Y, dest.X, dest.Y) < 2000)//&& partner.TTime["攻击僵直"] < DateTime.Now)//ECOKEY 這裡清掉
                {
                    Chase chase = new Chase(this.partnerai, dest);
                    partnerai.commands.Add("Chase", chase);
                    if (attacktask.Activated == true) attacktask.Deactivate();
                    attacking = false;
                }
            }
            else
            {
                if (Global.Random.Next(0, 99) < 100)
                {
                    if (partnerai.CanAttack)
                    {
                        if (partner != null)
                        {
                            if (dest.ActorID == partner.Owner.ActorID)
                                return;
                        }
                        if (attacktask.Activated == false)
                            attacktask.Activate();
                        attacking = true;
                    }
                }
                else
                {
                    Chase chase = new Chase(this.partnerai, dest);
                    partnerai.commands.Add("Chase", chase);
                    if (attacktask.Activated == true) attacktask.Deactivate();
                    attacking = false;
                }
            }

            partnerai.Partner.e.OnUpdate(partnerai.Partner);
        }

        public CommandStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public void Dispose()
        {
            if (dest == null) return;
            if (attacking == true && attacktask != null) attacktask.Deactivate();
            attacktask = null;
            this.status = CommandStatus.FINISHED;
        }

        //ECOKEY HPMPSP自然回復量計算
        /*int HPRecover = 0;
        int MPRecover = 0;
        int SPRecover = 0;
        void CustomPassiveSkill_once()
        {
            ActorPartner partner = (ActorPartner)this.partnerai.Partner;
            for (int i = 0; i < partner.equipcubes_passiveskill.Count; i++)
            {
                ushort cubeid = partner.equipcubes_passiveskill[i];
                ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[cubeid];
                if (acd.skillID == 7200)//HP
                {
                    HPRecover = HPRecover + acd.parameter1;
                }
                if (acd.skillID == 7201)//MP
                {
                    MPRecover = MPRecover + acd.parameter1;
                }
                if (acd.skillID == 7202)//SP
                {
                    SPRecover = SPRecover + acd.parameter1;
                }
            }
        }
        }*/
        //新增被動恢復Tasks
        /*  if (HPRecover != 0 || MPRecover != 0 || SPRecover != 0)
          {
              if (!partner.Tasks.ContainsKey("PassiveRecover"))
              {
                  Tasks.Partner.PassiveRecover task = new Tasks.Partner.PassiveRecover(partner, HPRecover, MPRecover, SPRecover, partnerai);
                  partner.Tasks.Add("PassiveRecover", task);
                  task.Activate();
              }
          }*/

        //ECOKEY 每五秒回復HPMPSP
        DateTime PassiveskillCD = DateTime.Now;
        bool heal = false;
        //ECOKEY 寵物放置判斷
        int masterX = 0;
        int masterY = 0;
        int sleep = 0;
        void CustomPassiveSkill()
        {
            ActorPartner partner = (ActorPartner)this.partnerai.Partner;
            if ((DateTime.Now - PassiveskillCD).TotalSeconds <= 5)
                return;
            //ECOKEY 寵物放置判斷
            if (partner.ai_mode != 2)
            {
                if (partner.Owner.X != masterX || partner.Owner.Y != masterY)
                {
                    masterX = partner.Owner.X;
                    masterY = partner.Owner.Y;
                    sleep = 0;
                }
                else
                {
                    sleep = sleep + 5;
                    PassiveskillCD = DateTime.Now;
                    //此處數字為指定秒數後切換跟隨  1800=30分鐘
                    if (sleep >= 1800)
                    {
                        ((ActorEventHandlers.PartnerEventHandler)partner.e).AI.Mode = new AIMode(2);
                        ((ActorEventHandlers.PartnerEventHandler)partner.e).AI.Hate.Clear();
                        partner.ai_mode = 2;

                        Packets.Server.SSMG_PARTNER_AI_MODE_SELECTION pr = new Packets.Server.SSMG_PARTNER_AI_MODE_SELECTION();
                        pr.PartnerInventorySlot = partner.Owner.Inventory.Equipments[EnumEquipSlot.PET].Slot;
                        pr.AIMode = partner.ai_mode;
                        MapClient.FromActorPC((ActorPC)partner.Owner).netIO.SendPacket(pr);
                    }
                }
            }
            //無法恢復時被動無效
            if (partner.Buff.NoRegen)
                return;
            //ECOKEY 被動回復判斷
            if (partner.HP < partner.MaxHP)
            {
                partner.HP = partner.HP + (uint)partner.Status.hp_medicine;
                if (partner.HP > partner.MaxHP)
                    partner.HP = partner.MaxHP;
                heal = true;
            }
            if (partner.MP < partner.MaxMP)
            {
                partner.MP = partner.MP + (uint)partner.Status.mp_medicine;
                if (partner.MP > partner.MaxMP)
                    partner.MP = partner.MaxMP;
                heal = true;
            }
            if (partner.SP < partner.MaxSP)
            {
                partner.SP = partner.SP + (uint)partner.Status.sp_medicine;
                if (partner.SP > partner.MaxSP)
                    partner.SP = partner.MaxSP;
                heal = true;
            }
            if (heal)
            {
                partnerai.PartnerHPMPSP();
                PassiveskillCD = DateTime.Now;
                heal = false;
            }
        }
        /*  void CustomPassiveSkill()
          {
              ActorPartner partner = (ActorPartner)this.partnerai.Partner;
              if ((DateTime.Now - PassiveskillCD).TotalSeconds <= 5)
                  return;
              if (partner.HP < partner.MaxHP)
              {
                  partner.HP = partner.HP + (uint)HPRecover;
                  heal = true;
              }
              if (partner.MP < partner.MaxMP)
              {
                  partner.MP = partner.MP + (uint)MPRecover;
                  heal = true;
              }
              if (partner.SP < partner.MaxSP)
              {
                  partner.SP = partner.SP + (uint)SPRecover;
                  heal = true;
              }
              if (heal)
              {
                  partnerai.PartnerHPMPSP();
                  PassiveskillCD = DateTime.Now;
                  heal = false;
              }
          }*/
        //ECOKEY自定義技能
        byte skillTarget = 0;
        DateTime skillCD = DateTime.Now;
        bool skillsuccess = false;
        void CustomSkill(Actor currentTarget)
        {
            ActorPartner partner = (ActorPartner)this.partnerai.Partner;
            ActorEventHandlers.PartnerEventHandler eh = (ActorEventHandlers.PartnerEventHandler)partner.e;
            foreach (byte i in partner.ai_states.Keys)
            {
                if (partner.ai_reactions.ContainsKey(i))
                {
                    if (PartnerFactory.Instance.actcubes_db_uniqueID.ContainsKey(partner.ai_reactions[i]))
                    {
                        ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[partner.ai_reactions[i]];
                        // 執行技能相關操作

                        // ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[partner.ai_reactions[i]]; //讀取技能ID

                        SagaDB.Skill.Skill skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(acd.skillID, 1);//技能


                        if ((DateTime.Now - skillCD).TotalSeconds <= 2)
                            continue;
                        if (partner.ai_states[i] == true)//判斷技能開關
                            continue;
                        if ((DateTime.Now - ai_lastSkillCast[i]).TotalSeconds <= partner.ai_intervals[i])//判斷冷卻時間
                            continue;
                        if (IsConditionSatisfied(partner.ai_conditions[i], currentTarget) == false)  //判斷條件
                            continue;
                        ActorPC pc = partner.Owner;
                        //新增復活判斷
                        /*if (pc.Buff.Dead)
                        {
                            if (!skill.DeadOnly)
                            {
                                continue;
                            }
                        }*/
                        //有復活效果的技能判斷
                        if (skillTarget == 1 && pc.Buff.Dead)
                        {
                            if (!skill.DeadOnly && skill.Target != 4)//死人專用和範圍技能
                            {
                                continue;
                            }
                        }
                        // ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[partner.ai_reactions[i]]; //讀取技能ID
                        //ECOKEY距離
                        double distanceToTarget = PartnerAI.GetLengthD(partnerai.Partner.X, partnerai.Partner.Y, currentTarget.X, currentTarget.Y);
                        //SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(acd.skillID, 1);

                        //ECOKEY 技能的TryCast判斷，不保證所有技能通用
                        SkillArg arg = new SkillArg();
                        /*if (SagaMap.Skill.SkillHandler.Instance.skillHandlers[acd.skillID].TryCast(pc, currentTarget, arg) < 0)
                            continue;*/
                        if (SagaMap.Skill.SkillHandler.Instance.skillHandlers.ContainsKey(acd.skillID))//ECOKEY 避免禁止技能時出現error
                        {
                            if (SagaMap.Skill.SkillHandler.Instance.skillHandlers[acd.skillID].TryCast(pc, currentTarget, arg) < 0)
                                continue;
                        }
                        if (skillTarget == 1)
                            skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, (Actor)pc);//對玩家放技能
                        if (skillTarget == 2)
                            skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, (Actor)partner);//對自己放技能
                        if (skillTarget == 4) //隨機選一個隊友放技能
                        {

                            Dictionary<byte, ActorPC> partyMembers = pc.Party.Members;

                            List<Actor> targetMembers = new List<Actor>(partyMembers.Values);
                            targetMembers.Remove(pc); // 移除主人自己
                                                      //避免技能放給PE中的人
                            foreach (Actor p in targetMembers.ToArray())
                            {
                                /*  if (p.type == ActorType.PC)
                                  {
                                      ActorPC p1 = (ActorPC)p;
                                      if (p1.PossessionTarget != 0)
                                      {
                                          targetMembers.Remove(p);
                                      }
                                      //避免放技給不同地圖的人
                                      if (p.MapID != partner.MapID)
                                      {
                                          targetMembers.Remove(p);
                                      }
                                      //超過一定距離的隊友不放技
                                      if (GetDistance(partner.X, partner.Y, p1.X, p1.Y) >= 1500)
                                      {
                                          targetMembers.Remove(p);
                                      }

                                  }*/
                                // 新增判斷避免對已下線的隊友放技能
                                if (p.type == ActorType.PC && ((ActorPC)p).Online)
                                {
                                    ActorPC p1 = (ActorPC)p;
                                    if (p1.PossessionTarget != 0)
                                    {
                                        targetMembers.Remove(p);
                                    }
                                    //避免放技給不同地圖的人
                                    if (p.MapID != partner.MapID)
                                    {
                                        targetMembers.Remove(p);
                                    }
                                    //超過一定距離的隊友不放技
                                    if (GetDistance(partner.X, partner.Y, p1.X, p1.Y) >= 1500)
                                    {
                                        targetMembers.Remove(p);
                                    }
                                }
                                else
                                {
                                    targetMembers.Remove(p);
                                }
                            }
                            if (targetMembers.Count > 0)
                            {
                                Random myObject = new Random();
                                int ranNum = myObject.Next(0, targetMembers.Count);
                                Actor target = targetMembers[ranNum];

                                skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, target);
                            }
                        }
                        if (distanceToTarget <= (skill.Range * 154)) // 距離值
                        {
                            if (skillTarget == 3)
                            {
                                if (currentTarget.ActorID == partner.Owner.ActorID)//判斷目前寵物目標是敵人還是主人
                                    continue;
                                skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, (Actor)currentTarget);//對敵人放技能
                            }
                        }

                        if (skillsuccess == true)
                        {
                            if (skill.CastTime == 0)//技能強制兩秒
                                skill.CastTime = 2000;
                            skillCD = DateTime.Now.AddSeconds(skill.CastTime / 1000);//新增詠唱時間，避免技中技
                            ai_lastSkillCast[i] = DateTime.Now.AddSeconds(skill.Delay / 1000); //紀錄技能施放時間，用於判斷冷卻，並加上單獨的冷卻技能

                            if (partner.Owner.TInt.ContainsKey("PartnerX"))//ECOKEY如果有坐下紀錄，直接更改位置紀錄
                            {
                                partner.Owner.TInt.Remove("PartnerX");
                                partner.Owner.TInt.Remove("PartnerY");
                            }
                            //skillCD = DateTime.Now;//避免兩個技能同時施放
                            // ai_lastSkillCast[i] = DateTime.Now; //紀錄技能施放時間，用於判斷冷卻
                            /*Logger.ShowInfo(acd.cubename);
                            Logger.ShowInfo("MP:" + partner.MP.ToString());
                            Logger.ShowInfo("SP:" + partner.SP.ToString());*/
                        }
                        //partnerai.CastSkill(3054, 1, (Actor)pc);
                    }
                    else
                    {
                        // 處理技能塊不存在的情況，例如輸出錯誤消息或者執行默認操作
                        MapClient.FromActorPC((ActorPC)partner.Owner).SendSystemMessage("你寵物沒有裝技能。");
                        // 或者執行默認操作
                        // ...
                    }
                    continue;
                }
                else
                {
                }
            }
        }
        //ECOKEY 自定義AI2技能
        void CustomSkill2(Actor currentTarget)
        {
            ActorPartner partner = (ActorPartner)this.partnerai.Partner;
            ActorEventHandlers.PartnerEventHandler eh = (ActorEventHandlers.PartnerEventHandler)partner.e;
            foreach (byte i in partner.ai_states2.Keys)
            {
                if (partner.ai_reactions.ContainsKey(i))
                {
                    if (PartnerFactory.Instance.actcubes_db_uniqueID.ContainsKey(partner.ai_reactions2[i]))
                    {
                        ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[partner.ai_reactions2[i]];
                        // 執行技能相關操作
                        SagaDB.Skill.Skill skill = SagaDB.Skill.SkillFactory.Instance.GetSkill(acd.skillID, 1);//技能
                        if ((DateTime.Now - skillCD).TotalSeconds <= 2)//判斷冷卻時間//判斷冷卻時間（關閉）
                            continue;
                        if (partner.ai_states2[i] == true)//判斷技能開關
                            continue;
                        if ((DateTime.Now - ai_lastSkillCast[i]).TotalSeconds <= partner.ai_intervals2[i])//判斷冷卻時間
                            continue;
                        if (IsConditionSatisfied(partner.ai_conditions2[i], currentTarget) == false)  //判斷條件
                            continue;
                        ActorPC pc = partner.Owner;
                        //新增復活判斷
                        /*if (pc.Buff.Dead)
                        {
                            if (!skill.DeadOnly)
                            {
                                continue;
                            }
                        }*/
                        //有復活效果的技能判斷
                        if (skillTarget == 1 && pc.Buff.Dead)
                        {
                            if (!skill.DeadOnly && skill.Target != 4)//死人專用和範圍技能
                            {
                                continue;
                            }
                        }
                        double distanceToTarget = PartnerAI.GetLengthD(partnerai.Partner.X, partnerai.Partner.Y, currentTarget.X, currentTarget.Y);
                        //ECOKEY 技能的TryCast判斷，不保證所有技能通用
                        SkillArg arg = new SkillArg();
                        if (SagaMap.Skill.SkillHandler.Instance.skillHandlers.ContainsKey(acd.skillID))//ECOKEY 避免禁止技能時出現error
                        {
                            if (SagaMap.Skill.SkillHandler.Instance.skillHandlers[acd.skillID].TryCast(pc, currentTarget, arg) < 0)
                                continue;
                        }
                        if (skillTarget == 1)
                            skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, (Actor)pc);//對玩家放技能
                        if (skillTarget == 2)
                            skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, (Actor)partner);//對自己放技能
                        if (skillTarget == 4) //隨機選一個隊友放技能
                        {
                            Dictionary<byte, ActorPC> partyMembers = pc.Party.Members;
                            List<Actor> targetMembers = new List<Actor>(partyMembers.Values);
                            targetMembers.Remove(pc); // 移除主人自己
                                                      //避免技能放給PE中的人
                            foreach (Actor p in targetMembers.ToArray())
                            {
                                // 新增判斷避免對已下線的隊友放技能
                                if (p.type == ActorType.PC && ((ActorPC)p).Online)
                                {
                                    ActorPC p1 = (ActorPC)p;
                                    if (p1.PossessionTarget != 0)
                                    {
                                        targetMembers.Remove(p);
                                    }
                                    //避免放技給不同地圖的人
                                    if (p.MapID != partner.MapID)
                                    {
                                        targetMembers.Remove(p);
                                    }
                                    //超過一定距離的隊友不放技
                                    if (GetDistance(partner.X, partner.Y, p1.X, p1.Y) >= 1500)
                                    {
                                        targetMembers.Remove(p);
                                    }
                                }
                                else
                                {
                                    targetMembers.Remove(p);
                                }
                            }
                            if (targetMembers.Count > 0)
                            {
                                Random myObject = new Random();
                                int ranNum = myObject.Next(0, targetMembers.Count);
                                Actor target = targetMembers[ranNum];

                                skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, target);
                            }
                        }
                        if (distanceToTarget <= (skill.Range * 154)) // 距離值
                        {
                            if (skillTarget == 3)
                            {
                                if (currentTarget.ActorID == partner.Owner.ActorID)//判斷目前寵物目標是敵人還是主人
                                    continue;
                                skillsuccess = partnerai.CustomCastSkill(acd.skillID, 1, skillTarget, (Actor)currentTarget);//對敵人放技能
                            }
                        }

                        if (skillsuccess == true)
                        {
                            if (skill.CastTime == 0)//技能強制兩秒
                                skill.CastTime = 2000;
                            skillCD = DateTime.Now.AddSeconds(skill.CastTime / 1000);//新增詠唱時間，避免技中技
                            ai_lastSkillCast[i] = DateTime.Now.AddSeconds(skill.Delay / 1000); //紀錄技能施放時間，用於判斷冷卻，並加上單獨的冷卻技能
                                                                                               //ECOKEY如果有坐下紀錄，直接更改位置紀錄
                            if (partner.Owner.TInt.ContainsKey("PartnerX"))
                            {
                                partner.Owner.TInt.Remove("PartnerX");
                                partner.Owner.TInt.Remove("PartnerY");
                            }
                        }
                    }
                    else
                    {
                        MapClient.FromActorPC((ActorPC)partner.Owner).SendSystemMessage("你寵物沒有裝技能。");
                    }
                    continue;
                }
                else
                {
                }
            }
        }
        //ECOKEY自定義技能-條件判斷
        private bool IsConditionSatisfied(ushort conditionID, Actor currentTarget)
        {
            ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[conditionID];
            ActorPartner partner = (ActorPartner)this.partnerai.Partner;
            ActorPC pc = partner.Owner;

            skillTarget = (byte)acd.parameter2;
            //目標是主人
            if (acd.parameter2 == 1)
            {

                if (acd.parameter2 == 1 && partner.Owner.PossessionTarget != 0)
                    return false;
                #region まで系列
                if (acd.actionID == 7 && acd.parameter2 == 1 && partner.HP >= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && acd.parameter2 == 1 && partner.SP >= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && acd.parameter2 == 1 && partner.MP >= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && acd.parameter2 == 1 && pc.HP >= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && acd.parameter2 == 1 && pc.SP >= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && acd.parameter2 == 1 && pc.MP >= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && acd.parameter2 == 1 && currentTarget.HP >= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && acd.parameter2 == 1 && pcTarget.HP >= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && acd.parameter2 == 1 && partner.HP <= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 2 && acd.parameter2 == 1 && partner.SP <= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 3 && acd.parameter2 == 1 && partner.MP <= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 4 && acd.parameter2 == 1 && pc.HP <= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 5 && acd.parameter2 == 1 && pc.SP <= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 6 && acd.parameter2 == 1 && pc.MP <= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 13 && acd.parameter2 == 1 && currentTarget.HP <= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && acd.parameter2 == 1 && pcTarget.HP <= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
            }
            //目標是自己
            if (acd.parameter2 == 2)
            {
                #region まで系列
                if (acd.actionID == 7 && acd.parameter2 == 2 && partner.HP >= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && acd.parameter2 == 2 && partner.SP >= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && acd.parameter2 == 2 && partner.MP >= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && acd.parameter2 == 2 && pc.HP >= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && acd.parameter2 == 2 && pc.SP >= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && acd.parameter2 == 2 && pc.MP >= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && acd.parameter2 == 2 && currentTarget.HP >= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && acd.parameter2 == 2 && pcTarget.HP >= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && acd.parameter2 == 2 && partner.HP <= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜自分＞
                    return true;
                if (acd.actionID == 2 && acd.parameter2 == 2 && partner.SP <= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜自分＞
                    return true;
                if (acd.actionID == 3 && acd.parameter2 == 2 && partner.MP <= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜自分＞
                    return true;
                if (acd.actionID == 4 && acd.parameter2 == 2 && pc.HP <= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜自分＞
                    return true;
                if (acd.actionID == 5 && acd.parameter2 == 2 && pc.SP <= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜自分＞
                    return true;
                if (acd.actionID == 6 && acd.parameter2 == 2 && pc.MP <= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜自分＞
                    return true;
                if (acd.actionID == 13 && acd.parameter2 == 2 && currentTarget.HP <= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜自分＞
                    return true;
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && acd.parameter2 == 2 && pcTarget.HP <= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜自分＞
                            return true;
                    }
                }
                #endregion
            }
            //目標是敵人
            if (acd.parameter2 == 3)
            {
                #region まで系列
                if (acd.actionID == 7 && acd.parameter2 == 3 && partner.HP >= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && acd.parameter2 == 3 && partner.SP >= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && acd.parameter2 == 3 && partner.MP >= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && acd.parameter2 == 3 && pc.HP >= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && acd.parameter2 == 3 && pc.SP >= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && acd.parameter2 == 3 && pc.MP >= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && acd.parameter2 == 3 && currentTarget.HP >= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && acd.parameter2 == 3 && pcTarget.HP >= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && acd.parameter2 == 3 && partner.HP <= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 2 && acd.parameter2 == 3 && partner.SP <= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 3 && acd.parameter2 == 3 && partner.MP <= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 4 && acd.parameter2 == 3 && pc.HP <= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 5 && acd.parameter2 == 3 && pc.SP <= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 6 && acd.parameter2 == 3 && pc.MP <= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜敵人＞
                    return true;
                if (acd.actionID == 13 && acd.parameter2 == 3 && currentTarget.HP <= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜敵人＞
                    return true;
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && acd.parameter2 == 3 && pcTarget.HP <= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜敵人＞
                            return true;
                    }
                }
                #endregion
            }


            if (pc.Party != null)
            {
                #region まで系列
                if (acd.actionID == 7 && acd.parameter2 == 4 && partner.HP >= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 8 && acd.parameter2 == 4 && partner.SP >= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 9 && acd.parameter2 == 4 && partner.MP >= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 10 && acd.parameter2 == 4 && pc.HP >= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜主人＞
                    return true;
                if (acd.actionID == 11 && acd.parameter2 == 4 && pc.SP >= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜主人＞
                    return true;
                if (acd.actionID == 12 && acd.parameter2 == 4 && pc.MP >= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜主人＞
                    return true;
                if (acd.actionID == 15 && acd.parameter2 == 4 && currentTarget.HP >= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜主人＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 16 && acd.parameter2 == 4 && pcTarget.HP >= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜主人＞
                            return true;
                    }
                }
                #endregion
                #region 數值以上
                if (acd.actionID == 1 && acd.parameter2 == 4 && partner.HP <= (partner.MaxHP * (acd.parameter1 * 0.01))) //自分のＨＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 2 && acd.parameter2 == 4 && partner.SP <= (partner.MaxSP * (acd.parameter1 * 0.01))) //自分のＳＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 3 && acd.parameter2 == 4 && partner.MP <= (partner.MaxMP * (acd.parameter1 * 0.01))) //自分のＭＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 4 && acd.parameter2 == 4 && pc.HP <= (pc.MaxHP * (acd.parameter1 * 0.01))) //主人のＨＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 5 && acd.parameter2 == 4 && pc.SP <= (pc.MaxSP * (acd.parameter1 * 0.01))) //主人のＳＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 6 && acd.parameter2 == 4 && pc.MP <= (pc.MaxMP * (acd.parameter1 * 0.01))) //主人のＭＰ≦＜仲間＞
                    return true;
                if (acd.actionID == 13 && acd.parameter2 == 4 && currentTarget.HP <= (currentTarget.MaxHP * (acd.parameter1 * 0.01)) && currentTarget != partner.Owner) //寵物的敵人のＨＰ≦＜仲間＞
                    return true;
                //判斷主人的敵人
                if (pc.LastAttackActorID != 0)
                {
                    Actor pcTarget = partnerai.map.GetActor(pc.LastAttackActorID);
                    if (pcTarget != null)
                    {
                        if (acd.actionID == 14 && acd.parameter2 == 4 && pcTarget.HP <= (pcTarget.MaxHP * (acd.parameter1 * 0.01))) //主人的敵人のＨＰ≦＜仲間＞
                            return true;
                    }
                }
                #endregion
            }
            return false;
        }


        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        //ECOKEY 讓寵物回原本的位置1009版本
        void gomaster()
        {
            ActorPartner partner = (ActorPartner)this.partnerai.Partner;
            double distanceToMaster = GetDistance(partner.X, partner.Y, partner.Owner.X, partner.Owner.Y);
            try
            {
                // 如果距離大於某個閾值，寵物返回主人身邊
                if (distanceToMaster > 1500)
                {
                    Manager.MapManager.Instance.GetMap(partner.MapID).TeleportActor(partner, partner.Owner.X, partner.Owner.Y);
                    ((ActorEventHandlers.PartnerEventHandler)partner.e).AI.Hate.Clear();
                    partnerai.AIActivity = Activity.IDLE;
                    if (partnerai.commands.ContainsKey("Chase") == true)
                        partnerai.commands.Remove("Chase");
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                // 不做任何处理，防止异常信息的显示
            }
        }

        //ECOKEY 讓寵物回原本的位置1004版本
        /*   void gomaster()
           {
               /* ActorPartner Partner = (ActorPartner)this.partnerai.Partner;
                double distanceToMaster = GetDistance(Partner.X, Partner.Y, Partner.Owner.X, Partner.Owner.Y);

                try
                {
                    // 如果距離大於某個閾值，寵物返回主人身邊
                    if (distanceToMaster > 1500)
                    {
                        ActorPC pc = (ActorPC)Partner.Owner;
                        Partner.MapID = Partner.Owner.MapID;

                        // 将寵物传送到主人的位置
                        Manager.MapManager.Instance.GetMap(Partner.MapID).TeleportActor(Partner, Partner.Owner.X, Partner.Owner.Y);

                        // 注册寵物回地图管理器
                        Manager.MapManager.Instance.GetMap(Partner.MapID).RegisterActor(Partner);

                        // 清除寵物的攻击目标和可见角色
                        // Partner.VisibleActors.Clear();
                        Partner.Status.attackingActors.Clear();

                        // 清除寵物的移动命令
                        if (partnerai.commands.ContainsKey("Move") == true) partnerai.commands.Remove("Move");

                        // 清除寵物的仇恨列表
                        ((ActorEventHandlers.PartnerEventHandler)Partner.e).AI.Hate.Clear();

                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    // 不做任何处理，防止异常信息的显示
                    return false;
                }*/
        /*     ActorPartner partner = (ActorPartner)this.partnerai.Partner;
             double distanceToMaster = GetDistance(partner.X, partner.Y, partner.Owner.X, partner.Owner.Y);

             try
             {
                 // 如果距離大於某個閾值，寵物返回主人身邊
                 if (distanceToMaster > 1499)
                 {
                     ActorPC pc = (ActorPC)partner.Owner;
                     partner.MapID = partner.Owner.MapID;//
                     ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)partner.Owner.e;
                     //  eh.Client.DeletePartner();
                     Manager.MapManager.Instance.GetMap(partner.MapID).DeleteActor(partner);

                     /* if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.PET))
                      {
                          if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.PET].BaseData.itemType == SagaDB.Item.ItemType.PARTNER)
                          {
                              //eh.Client.SendPartner(pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.PET]);
                             // Manager.MapManager.Instance.GetMap(partner.MapID).DeleteActor(partner);
                              Manager.MapManager.Instance.GetMap(partner.MapID).RegisterActor(partner);
                          }
                      }*/
    }
    /*      if (distanceToMaster > 1500)
          {
              ActorPC pc = (ActorPC)partner.Owner;
              partner.MapID = partner.Owner.MapID;//
              ActorEventHandlers.PCEventHandler eh = (ActorEventHandlers.PCEventHandler)partner.Owner.e;
              eh.Client.DeletePartner();
              // Manager.MapManager.Instance.GetMap(partner.MapID).DeleteActor(partner);

              if (pc.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.PET))
              {
                  if (pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.PET].BaseData.itemType == SagaDB.Item.ItemType.PARTNER)
                  {
                      eh.Client.SendPartner(pc.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.PET]);
                      Manager.MapManager.Instance.GetMap(partner.MapID).RegisterActor(partner);
                     // Manager.MapManager.Instance.GetMap(partner.MapID).TeleportActor(partner, partner.Owner.X, partner.Owner.Y);
                  }
              }
          }
      }
      catch (Exception ex)
      {
          Logger.ShowError(ex);
      }*/
}
