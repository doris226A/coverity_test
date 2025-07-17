using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Item;

namespace SagaMap.Packets.Server
{
    public class SSMG_PLAYER_EXP_DEM : Packet
    {
        public SSMG_PLAYER_EXP_DEM()
        {
            this.data = new byte[35];//8bytes unknowns
            this.offset = 2;
            this.ID = 0x0235;
            this.PutByte(1, 10);
        }

        /*public long EXPPercentage
        {
            set
            {
                this.PutLong(value, 2);
            }
        }

        public long JEXPPercentage
        {
            set
            {
                this.PutLong(value, 10);
            }
        }*/
        public uint EXPPercentage
        {
            set
            {
                this.PutUInt(value, 2);
            }
        }

        public uint JEXPPercentage
        {
            set
            {
                this.PutUInt(value, 6);
            }
        }

        public int WRP
        {
            set
            {
                this.PutInt(value, 10);
            }
        }

        public uint ECoin
        {
            set
            {
                this.PutUInt(value, 14);
            }
        }

        public long Exp
        {
            set
            {
                if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                {
                    this.PutLong(value, 18);
                }
            }
        }

        public long JExp
        {
            set
            {
                if (Configuration.Instance.Version >= SagaLib.Version.Saga10)
                {
                    this.PutLong(value, 26);
                }
            }
        }

    }
}

