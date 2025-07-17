using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;//二哈更改Hate

using SagaLib;
using SagaDB.Actor;
using SagaMap.Manager;

namespace SagaMap.Mob
{
    public enum Activity
    {
        IDLE,
        LAZY,
        BUSY,
    }

    public partial class MobAI
    {
        Actor actor;
        public Map map;
        Activity aiActivity = Activity.LAZY;
        public bool Activated = false;
        public Dictionary<string, AICommand> commands = new Dictionary<string, AICommand>();
        Dictionary<int, MapNode> openedNode = new Dictionary<int, MapNode>();
        public ConcurrentDictionary<uint, uint> Hate = new ConcurrentDictionary<uint, uint>();//二哈更改Hate
        public short MoveRange, X_Ori, Y_Ori, X_Spawn, Y_Spawn;
        public int SpawnDelay;
        AIMode mode;
        public int period;
        public DateTime NextUpdateTime = DateTime.Now;

        public string Announce;

        public DateTime BackTimer = DateTime.Now;
        public short X_pb, Y_pb;
        /// <summary>
        /// 是否可以在战斗前随意移动
        /// </summary>
        public bool Cannotmovebeforefight = false;
        //伤害表，掉宝归属
        public ConcurrentDictionary<uint, int> DamageTable = new ConcurrentDictionary<uint, int>();//二哈更改1206
        public DateTime attackStamp = DateTime.Now;
        public Actor firstAttacker;
        Actor master;

        /// <summary>
        /// 设置被技能锁定中，避免技能释放过程行动
        /// </summary>
        public bool locked;
        /// <summary>
        /// AI的模式
        /// </summary>
        public AIMode Mode { get { return this.mode; } set { this.mode = value; } }

        public Actor Master { get { if (master == null) return null; return this.master; } set { this.master = value; } }

        public Activity AIActivity
        {
            get
            {
                return aiActivity;
            }
            set
            {
                aiActivity = value;
                if (this.Mob.Speed == 0)
                    return;
                if (value == Activity.BUSY)
                {
                    this.period = (100000 / this.Mob.Speed);
                }
                else if (value == Activity.LAZY)
                {
                    this.period = (200000 / this.Mob.Speed);
                }
                else if (value == Activity.IDLE)
                {
                    this.period = 1000;
                }
            }
        }

        public Actor Mob
        {
            get
            {
                return this.actor;
            }
        }

        //ECOKEY 新增僵直/睡眠/麻痺無法移動
        public bool CanMove
        {
            get
            {
                return !(this.Mode.NoMove || Mob.Buff.CannotMove || Mob.Buff.Stun || Mob.Buff.Stone || Mob.Buff.Frosen ||
                    Mob.Buff.Stiff || Mob.Buff.Sleep || Mob.Buff.Paralysis ||
                    this.Mob.Tasks.ContainsKey("SkillCast"));
            }
        }
        //ECOKEY 新增僵直/睡眠/麻痺無法攻擊
        public bool CanAttack
        {
            get
            {
                return !(this.Mode.NoAttack || Mob.Buff.Stone || Mob.Buff.Stun || Mob.Buff.Frosen ||
                    Mob.Buff.Stiff || Mob.Buff.Sleep || Mob.Buff.Paralysis ||
                    this.Mob.Tasks.ContainsKey("SkillCast"));
            }
        }
        //ECOKEY 新增僵直/睡眠/麻痺/沉默無法放技能
        public bool CanUseSkill
        {
            get
            {
                if (Mob.Buff.Silence || Mob.Buff.Stun || Mob.Buff.Stone || Mob.Buff.Frosen ||
                        Mob.Buff.Stiff || Mob.Buff.Sleep || Mob.Buff.Paralysis)
                    return false;
                else
                    return true;
            }
        }
        public MobAI(Actor mob, bool idle)
        {
            this.period = 1000;//process 1 command every second            
            actor = mob;
            map = MapManager.Instance.GetMap(mob.MapID);
        }

        public MobAI(Actor mob)
        {
            this.period = 1000;//process 1 command every second            
            actor = mob;
            map = MapManager.Instance.GetMap(mob.MapID);
        }

        public void Start()
        {
            AIThread.Instance.RegisterAI(this);
            //12.11
            if (this.Hate != null && this.Hate.Count > 0)
            {
                this.Hate.Clear(); // 確保 this.Hate 不為 null 且包含元素
            }

            //
            if (this.Hate.Count > 0)
            {
                this.Hate.Clear();
            }
            //
            if (this.Hate != null)
            {
                this.Hate.Clear();
            }
            this.Hate.Clear();//Hate table should be cleard at respawn
                              //
                              //this.mob.Actor.BattleStatus.Status = new List<uint>();
            this.commands = new Dictionary<string, AICommand>();
            this.commands.Add("Attack", new AICommands.Attack(this));
            this.AIActivity = Activity.LAZY;
            Activated = true;

            /*if (Skill.SkillHandler.Instance.isBossMob(this.Mob))
            {
                Tasks.Mob.MobRecover MobRecover = new SagaMap.Tasks.Mob.MobRecover((ActorMob)this.Mob);
                if (!this.Mob.Tasks.ContainsKey("MobRecover"))
                    this.Mob.Tasks.Add("MobRecover", MobRecover);
                MobRecover.Activate();
            }*///关闭怪物回复线程以节省资源
        }

        public void Pause()
        {
            try
            {
                /* for (int i = 0; i < commands.Keys.Count; i++)
                 {
                     string key = commands.Keys.ElementAt(i);
                     commands[key].Dispose();
                 }
                 commands.Clear();
                 Mob.VisibleActors.Clear();
                 Mob.Status.attackingActors.Clear();
                 lastAttacker = null;
                 AIThread.Instance.RemoveAI(this);
                 Activated = false;*/
                //new
                foreach (string i in commands.Keys)
                {
                    commands[i].Dispose();
                }
                commands.Clear();
                Mob.VisibleActors.Clear();
                Mob.Status.attackingActors.Clear();
                lastAttacker = null;
                AIThread.Instance.RemoveAI(this);
                Activated = false;
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex, null);
            }

        }

        public void CallBack(object o)
        {
            List<string> deletequeue = new List<string>();
            //ClientManager.EnterCriticalArea();
            try
            {
                string[] keys;
                if (locked)
                    return;
                if (this.actor.Buff.Dead)
                    return;
                if (this.master != null)
                {
                    if (this.master.MapID != this.Mob.MapID)
                    {
                        this.Mob.e.OnDie();
                        return;
                    }
                    if (this.master.type == ActorType.PC)
                    {
                        ActorPC pc = (ActorPC)this.master;
                        if (!pc.Online)
                        {
                            this.Mob.e.OnDie();
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
                                if ((Math.Abs(Mob.X - X_Spawn) > 1000 || Math.Abs(Mob.Y - Y_Spawn) > 1000) && this.MoveRange != 0)
                                {
                                    short x, y;
                                    double len = GetLengthD(X_Spawn, Y_Spawn, Mob.X, Mob.Y);
                                    x = (short)(Mob.X + ((X_Spawn - Mob.X) / len * this.Mob.Speed));
                                    y = (short)(Mob.Y + ((Y_Spawn - Mob.Y) / len * this.Mob.Speed));

                                    AICommands.Move mov = new SagaMap.Mob.AICommands.Move(this, (short)x, (short)y);
                                    this.commands.Add("Move", mov);//二哈更改測試-command
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
                                        x += this.Mob.X;
                                        y += this.Mob.Y;
                                        _x = Global.PosX16to8((short)x, this.map.Width);
                                        _y = Global.PosY16to8((short)y, this.map.Height);
                                        if (_x >= this.map.Width)
                                            _x = (byte)(this.map.Width - 1);
                                        if (_y >= this.map.Height)
                                            _y = (byte)(this.map.Height - 1);
                                        counter++;
                                    } while (this.map.Info.walkable[_x, _y] != 2 && counter < 1000);
                                    AICommands.Move mov = new SagaMap.Mob.AICommands.Move(this, (short)x, (short)y);
                                    this.commands.Add("Move", mov);//二哈更改測試-commandv
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
                //二哈更改測試-command 
                lock (commands)
                {
                    foreach (string i in deletequeue)
                    {
                        commands.Remove(i);
                    }
                }
               /* foreach (string i in deletequeue)
                {
                    commands.TryRemove(i, out AICommand data);
                }*/

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
