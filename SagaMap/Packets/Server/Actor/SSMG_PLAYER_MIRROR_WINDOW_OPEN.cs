using SagaLib;
using System.Collections.Generic;

namespace SagaMap.Packets.Server
{
    public class SSMG_PLAYER_MIRROR_WINDOW_OPEN : Packet
    {
        public SSMG_PLAYER_MIRROR_WINDOW_OPEN()
        {
            this.data = new byte[100];
            this.ID = 0x02B3;
            this.offset = 2;
        }

        public List<ushort> SetFace
        {
            set
            {
                this.PutByte(20);

                for (int i = 0; i < 20; i++)
                {
                    if (i < value.Count)
                        this.PutUShort(value[i]);
                    else
                        this.PutUShort(0xFFFF);
                }
            }
        }

        public List<ushort> SetHairStyle
        {
            set
            {
                this.PutByte(20);
                for (int i = 0; i < 20; i++)
                {
                    if (i < value.Count)
                        this.PutUShort(value[i]);
                    else
                        this.PutUShort(0xFFFF);
                }
            }
        }

        public List<ushort> SetWigStyle
        {
            set
            {
                /*this.PutByte(20);
                for (int i = 0; i < 20; i++)
                {
                    if (i < value.Count)
                        this.PutUShort(value[i]);
                    else
                        this.PutUShort(0xFFFF);
                }*/
                this.PutByte(20);
                for (int i = 0; i < 20; i++)
                {
                    if (i < value.Count)
                    {
                        if(value[i] == 65535)
                            this.PutShort((short)-1);
                        else
                            this.PutUShort(value[i]);
                    }
                    else
                        this.PutUShort(0xFFFF);
                }
            }
        }

        public List<uint> SetHairLimit
        {
            set
            {
                this.PutByte(20);
                for (int i = 0; i < 20; i++)
                {
                    if (i < value.Count)
                        this.PutUInt(0xFFFFFFFF);
                    else
                        this.PutUInt(0x00000000);
                }
            }
        }

        public List<byte> SetHairColor
        {
            set
            {
                this.PutByte(20);
                for (int i = 0; i < 20; i++)
                {
                    if (i < value.Count)
                        this.PutByte(value[i]);
                    else
                        this.PutByte(0xFF);
                }
            }
        }
    }
}
