using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;//二哈更改測試

using SagaLib;
using SagaDB.Actor;
using SagaMap.Manager;

namespace SagaMap.Partner
{
    public enum Activity
    {
        IDLE,
        LAZY,
        BUSY,
    }

    public partial class PartnerAI
    {
        Actor actor;
        public Map map;
        Activity aiActivity = Activity.LAZY;
        public bool Activated = false;
        public Dictionary<string, AICommand> commands = new Dictionary<string, AICommand>();
        Dictionary<int, MapNode> openedNode = new Dictionary<int, MapNode>();
        public ConcurrentDictionary<uint, uint> Hate = new ConcurrentDictionary<uint, uint>();//二哈更改測試
        public short MoveRange, X_Ori, Y_Ori, X_Spawn, Y_Spawn;
        public int SpawnDelay;
        AIMode mode;
        public int period;
        public DateTime NextUpdateTime = DateTime.Now;

        public string Announce;

        public DateTime BackTimer = DateTime.Now;
        public short X_pb, Y_pb;

        //伤害表，掉宝归属
        public Dictionary<uint, int> DamageTable = new Dictionary<uint, int>();
        public DateTime attackStamp = DateTime.Now;
        public Actor firstAttacker;
        Actor master;

        /// <summary>
        /// AI的模式
        /// </summary>
        public AIMode Mode { get { return this.mode; } set { this.mode = value; } }

        public Actor Master { get { return this.master; } set { this.master = value; } }

        public Activity AIActivity
        {
            get
            {
                return aiActivity;
            }
            set
            {
                aiActivity = value;
                if (this.Partner.Speed == 0)
                    return;
                if (value == Activity.BUSY)
                {
                    this.period = (100000 / this.Partner.Speed);
                }
                else if (value == Activity.LAZY)
                {
                    this.period = (200000 / this.Partner.Speed);
                }
                else if (value == Activity.IDLE)
                {
                    this.period = 1000;
                }
            }
        }

        public Actor Partner
        {
            get
            {
                return this.actor;
            }
        }

        public bool CanMove
        {
            get
            {
                return !(this.Mode.NoMove || Partner.Buff.CannotMove || Partner.Buff.Stun || Partner.Buff.Stone || Partner.Buff.Frosen ||
                   Partner.Buff.Stiff || Partner.Buff.Sleep || Partner.Buff.Paralysis ||
                   this.Partner.Tasks.ContainsKey("SkillCast"));
                /*  return !(this.Mode.NoMove || Partner.Buff.CannotMove || Partner.Buff.Stun || Partner.Buff.Stone || Partner.Buff.Frosen ||
                      Partner.Buff.Stiff || this.Partner.Tasks.ContainsKey("SkillCast"));*/
            }
        }

        public bool CanAttack
        {
            get
            {
                return !(this.Mode.NoAttack || Partner.Buff.Stone || Partner.Buff.Stun || Partner.Buff.Frosen ||
                   Partner.Buff.Stiff || Partner.Buff.Sleep || Partner.Buff.Paralysis ||
                   this.Partner.Tasks.ContainsKey("SkillCast"));
                // return !(this.Mode.NoAttack || Partner.Buff.Stone || Partner.Buff.Stun || Partner.Buff.Frosen || this.Partner.Tasks.ContainsKey("SkillCast"));
            }
        }
        public bool CanUseSkill
        {
            get
            {
                if (Partner.Buff.Silence || Partner.Buff.Stun || Partner.Buff.Stone || Partner.Buff.Frosen ||
                       Partner.Buff.Stiff || Partner.Buff.Sleep || Partner.Buff.Paralysis)
                    return false;
                else
                    return true;
                // return !(Partner.Buff.Silence || Partner.Buff.Stun || Partner.Buff.Stone || Partner.Buff.Frosen);
            }
        }
        public PartnerAI(ActorPartner partner, bool idle)
        {
            this.period = 1000;//process 1 command every second            
            actor = partner;
            map = MapManager.Instance.GetMap(partner.MapID);
        }

        public PartnerAI(ActorPartner partner)
        {
            this.period = 1000;//process 1 command every second            
            actor = partner;
            map = MapManager.Instance.GetMap(Partner.MapID);
            //this.commands.Add("Attack", new AICommands.Attack(this));
        }

        public void Start()
        {
            AIThread.Instance.RegisterAI(this);
            this.Hate.Clear();//Hate table should be cleard at respawn
            //this.mob.Actor.BattleStatus.Status = new List<uint>();
            this.commands = new Dictionary<string, AICommand>();
            this.commands.Add("Attack", new AICommands.Attack(this));
            this.AIActivity = Activity.LAZY;
            Activated = true;
        }

        public void Pause()
        {
            try
            {
                foreach (string i in commands.Keys)
                {
                    commands[i].Dispose();
                }
                commands.Clear();
                Partner.VisibleActors.Clear();
                Partner.Status.attackingActors.Clear();
                lastAttacker = null;
                AIThread.Instance.RemoveAI(this);
                Activated = false;
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex, null);
            }

        }

        //ECOKEY

        private void ReturnToMaster()
        {
            double directionX = master.X - Partner.X;
            double directionY = master.Y - Partner.Y;

            double length = Math.Sqrt(directionX * directionX + directionY * directionY);

            double normalizedX = directionX / length;
            double normalizedY = directionY / length;

            // 計算新的寵物座標
            short newX = (short)(Partner.X + normalizedX * FollowSpeed);
            short newY = (short)(Partner.Y + normalizedY * FollowSpeed);

            // 更新寵物座標
            Partner.X = newX;
            Partner.Y = newY;

            //Partner.VisibleActors.Clear();
            //Partner.Status.attackingActors.Clear();
            // ((ActorEventHandlers.PartnerEventHandler)Partner.e).AI.Hate.Clear();
        }



        private double GetDistance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private const double FollowSpeed = 1.0; // 使用適當的值

        public void CallBack(object o)
        {

            // 檢查寵物和主人的距離
            /*  double distanceToMaster = GetDistance(Partner.X, Partner.Y, master.X, master.Y);

               // 如果距離大於某個閾值，寵物返回主人身邊
               if (distanceToMaster > 1400)
               {
                   // 設定寵物的地圖ID、座標和朝向為主人的地圖ID、座標和朝向
                   Partner.MapID = master.MapID;
                   ReturnToMaster();
               }*/

            //ECOKEY 清除
            /*double distanceToMaster = GetDistance(Partner.X, Partner.Y, master.X, master.Y);

            // 如果距離大於某個閾值，寵物返回主人身邊
            if (distanceToMaster > 1400)
            {
                // 設定寵物的地圖ID、座標和朝向為主人的地圖ID、座標和朝向
                Partner.MapID = master.MapID;
                    // Partner.X = master.X;
                    // Partner.Y = master.Y;
                    // Partner.Dir = master.Dir;
              //  Partner.VisibleActors.Clear();
              //  Partner.Status.attackingActors.Clear();
               // ((ActorEventHandlers.PartnerEventHandler)Partner.e).AI.Hate.Clear();
                ReturnToMaster();
            }*/

            List<string> deletequeue = new List<string>();
            //ClientManager.EnterCriticalArea();
            try
            {
                string[] keys;
                if (this.actor.Buff.Dead)
                    return;
                //ECOKEY 測試刪除這段，危!!!!!!
                /*if (this.master != null)
                {
                    if (this.master.MapID != this.Partner.MapID)
                    {
                        this.Partner.e.OnDie();
                        return;
                    }
                    if (this.master.type == ActorType.PC)
                    {
                        ActorPC pc = (ActorPC)this.master;
                        if (!pc.Online)
                        {
                            this.Partner.e.OnDie();
                            return;
                        }
                    }
                }*/
                if (this.master != null)//試圖優化寵物遊蕩問題
                {
                    if (this.master.MapID != this.Partner.MapID)
                    {
                        Manager.MapManager.Instance.GetMap(this.Partner.MapID).DeleteActor(this.Partner);
                        return;
                    }
                    if (this.master.type == ActorType.PC)
                    {
                        ActorPC pc = (ActorPC)this.master;
                        if (!pc.Online)
                        {
                            Manager.MapManager.Instance.GetMap(this.Partner.MapID).DeleteActor(this.Partner);
                            return;
                        }
                    }
                }
                if (this.commands.Count == 1)
                {
                    if (this.commands.ContainsKey("Attack"))
                    {
                        if (this.Hate.Count == 0)
                        {
                            this.AIActivity = Activity.IDLE;
                            if (Global.Random.Next(0, 99) < 10)
                            {
                                this.AIActivity = Activity.LAZY;
                                if ((Math.Abs(Partner.X - X_Spawn) > 1000 || Math.Abs(Partner.Y - Y_Spawn) > 1000) && this.MoveRange != 0)
                                {
                                    short x, y;
                                    double len = GetLengthD(X_Spawn, Y_Spawn, Partner.X, Partner.Y);
                                    x = (short)(Partner.X + ((X_Spawn - Partner.X) / len * this.Partner.Speed));
                                    y = (short)(Partner.Y + ((Y_Spawn - Partner.Y) / len * this.Partner.Speed));

                                    AICommands.Move mov = new SagaMap.Partner.AICommands.Move(this, (short)x, (short)y);
                                    this.commands.Add("Move", mov);
                                }
                                else
                                {
                                    double x, y;
                                    byte _x, _y;
                                    int counter = 0;
                                    do
                                    {
                                        x = Global.Random.Next(-100, 100);
                                        y = Global.Random.Next(-100, 100);
                                        double len = GetLengthD(0, 0, (short)x, (short)y);
                                        x = (x / len) * 500;
                                        y = (y / len) * 500;
                                        x += this.Partner.X;
                                        y += this.Partner.Y;
                                        _x = Global.PosX16to8((short)x, this.map.Width);
                                        _y = Global.PosY16to8((short)y, this.map.Height);
                                        if (_x >= this.map.Width)
                                            _x = (byte)(this.map.Width - 1);
                                        if (_y >= this.map.Height)
                                            _y = (byte)(this.map.Height - 1);
                                        counter++;
                                    } while (this.map.Info.walkable[_x, _y] != 2 && counter < 1000);
                                    AICommands.Move mov = new SagaMap.Partner.AICommands.Move(this, (short)x, (short)y);
                                    this.commands.Add("Move", mov);
                                }
                            }
                        }
                    }
                }
                keys = new string[commands.Count];
                commands.Keys.CopyTo(keys, 0);
                int count = commands.Count;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        string j;
                        j = keys[i];
                        AICommand command;
                        commands.TryGetValue(j, out command);
                        if (command != null)
                        {
                            if (command.Status != CommandStatus.FINISHED && command.Status != CommandStatus.DELETING && command.Status != CommandStatus.PAUSED)
                            {
                                lock (command)
                                {
                                    command.Update(null);
                                }
                            }
                            if (command.Status == CommandStatus.FINISHED)
                            {
                                deletequeue.Add(j);//删除队列
                                command.Status = CommandStatus.DELETING;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SagaLib.Logger.ShowError(ex);
                    }
                }
                lock (commands)
                {
                    foreach (string i in deletequeue)
                    {
                        commands.Remove(i);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex, null);
                Logger.ShowError(ex.StackTrace, null);
            }
            //ClientManager.LeaveCriticalArea();
        }
        public List<MapNode> FindPath(byte x, byte y, byte x2, byte y2)
        {
            MapNode src = new MapNode();
            DateTime now = DateTime.Now;
            int count = 0;
            src.x = x;
            src.y = y;
            src.F = 0;
            src.G = 0;
            src.H = 0;
            List<MapNode> path = new List<MapNode>();
            MapNode current = src;
            if (x2 > (map.Info.width - 1) || y2 > (map.Info.height - 1))
            {
                path.Add(current);
                return path;
            }
            if (map.Info.walkable[x2, y2] != 2)
            {
                path.Add(current);
                return path;
            }
            if (x == x2 && y == y2)
            {
                path.Add(current);
                return path;
            }
            openedNode = new Dictionary<int, MapNode>();
            GetNeighbor(src, x2, y2);
            while (openedNode.Count != 0)
            {
                MapNode shortest = new MapNode();
                shortest.F = int.MaxValue;
                if (count > 1000)
                    break;
                foreach (MapNode i in openedNode.Values)
                {
                    if (i.x == x2 && i.y == y2)
                    {
                        openedNode.Clear();
                        shortest = i;
                        break;
                    }
                    if (i.F < shortest.F)
                        shortest = i;
                }
                current = shortest;
                if (openedNode.Count == 0)
                    break;
                openedNode.Remove(shortest.x * 1000 + shortest.y);
                current = GetNeighbor(shortest, x2, y2);
                count++;
            }

            while (current.Previous != null)
            {
                path.Add(current);
                current = current.Previous;
            }
            path.Reverse();
            return path;

        }

        private int GetPathLength(MapNode node)
        {
            int count = 0;
            MapNode src = node;
            while (src.Previous != null)
            {
                count++;
                src = src.Previous;
            }
            return count;
        }

        public static int GetLength(byte x, byte y, byte x2, byte y2)
        {
            return (int)Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y));
        }

        public static double GetLengthD(short x, short y, short x2, short y2)
        {
            return Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y));
        }

        public static ushort GetDir(short x, short y)
        {
            double len = GetLengthD(0, 0, x, y);
            int degree = (int)(Math.Acos((double)y / len) / Math.PI * 180);
            if (x < 0)
                return (ushort)degree;
            else
                return (ushort)(360 - degree);
        }

        private MapNode GetNeighbor(MapNode node, byte x, byte y)
        {
            MapNode res = node;
            for (int i = node.x - 1; i <= node.x + 1; i++)
            {
                for (int j = node.y - 1; j <= node.y + 1; j++)
                {
                    if (j == -1 || i == -1)
                        continue;
                    if (j == node.y && i == node.x)
                        continue;
                    if (i >= map.Info.width || j >= map.Info.height)
                        continue;
                    if (map.Info.walkable[i, j] == 2)
                    {
                        if (!openedNode.ContainsKey(i * 1000 + j))
                        {
                            MapNode node2 = new MapNode();
                            node2.x = (byte)i;
                            node2.y = (byte)j;
                            node2.Previous = node;
                            if (i == node.x || j == node.y)
                            {
                                node2.G = node.G + 10;
                            }
                            else
                            {
                                node2.G = node.G + 14;
                            }
                            node2.H = Math.Abs(x - node2.x) * 10 + Math.Abs(y - node2.y) * 10;
                            node2.F = node2.G + node2.H;
                            openedNode.Add(i * 1000 + j, node2);
                        }
                        else
                        {
                            MapNode tmp = openedNode[i * 1000 + j];
                            int G;
                            if (i == node.x || j == node.y)
                            {
                                G = 10;
                            }
                            else
                            {
                                G = 14;
                            }
                            if (node.G + G > tmp.G)
                            {
                                res = tmp;
                            }
                        }
                    }
                }
            }
            return res;
        }
    }
}
