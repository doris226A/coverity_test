using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap.Manager;

namespace SagaMap.Packets.Server
{
    public class SSMG_KNIGHTWAR_RESULT_DETAIL : Packet
    {
        public SSMG_KNIGHTWAR_RESULT_DETAIL()
        {
            this.data = new byte[49];
            this.offset = 2;
            this.ID = 0x1B71;
        }
        //頁數
        public byte page
        {
            set
            {
                this.PutByte((byte)value, 2);
            }
        }
        //種類
        public int type
        {
            set
            {
                this.PutInt((byte)value, 3);
            }
        }
        //顯示幾個分數
        public byte RankCount
        {
            set
            {
                this.PutByte((byte)value, 7);
            }
        }

        public int Rank1Point
        {
            set
            {
                this.PutInt(value, 8);
            }
        }
        public int Rank2Point
        {
            set
            {
                this.PutInt(value, 12);
            }
        }

        public int Rank3Point
        {
            set
            {
                this.PutInt(value, 16);
            }
        }

        public int Rank4Point
        {
            set
            {
                this.PutInt(value, 20);
            }
        }

        public int Rank5Point
        {
            set
            {
                this.PutInt(value, 24);
            }
        }

        public int Rank6Point
        {
            set
            {
                this.PutInt(value, 28);
            }
        }

        public int Rank7Point
        {
            set
            {
                this.PutInt(value, 32);
            }
        }

        public int Rank8Point
        {
            set
            {
                this.PutInt(value, 36);
            }
        }

        public int Rank9Point
        {
            set
            {
                this.PutInt(value, 40);
            }
        }

        public int Rank10Point
        {
            set
            {
                this.PutInt(value, 44);
            }
        }
        //顯示幾個玩家
        public byte num
        {
            set
            {
                this.PutByte(value, 48);
            }
        }

        public ushort offset = 0;
        public SagaDB.Actor.ActorPC player1
        {
            set
            {
                byte[] buf, buff;
                byte size;

                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;//角色名长度
                buff = new byte[this.data.Length - 1 + size];
                this.data.CopyTo(buff, 0);
                this.data = buff;
                //this.PutByte(size, 49);
                //this.PutBytes(buf, 50);
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player2
        {
            set
            {
                byte[] buf, buff;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                buff = new byte[this.data.Length - 1 + size];
                this.data.CopyTo(buff, 0);
                this.data = buff;
                //this.PutByte(size, 49 + offset);
                //this.PutBytes(buf, 50 + offset);
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player3
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player4
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player5
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player6
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player7
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player8
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player9
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
        public SagaDB.Actor.ActorPC player10
        {
            set
            {
                byte[] buf;
                byte size;
                buf = Global.Unicode.GetBytes(value.Name + "\0");
                size = (byte)buf.Length;
                this.PutByte(size);
                this.PutBytes(buf);
                offset = (ushort)(offset + size - 1);
            }
        }
    }
}

