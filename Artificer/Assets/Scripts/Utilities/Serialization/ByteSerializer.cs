using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Data.Shared;

using Space.Projectiles;

namespace Serializer
{ 
    public class ByteSerializer
    {
        public static void WriteSock(BinaryWriter writer, Data.Shared.Socket sock)
        {
            writer.Write(sock.SocketID);
            writer.Write(sock.OtherID);
            writer.Write(sock.OtherLinkID);
        }

        public static void WriteComp(BinaryWriter writer, Data.Shared.Component comp)
        {
            writer.Write(comp.Name);
            writer.Write(comp.InstanceID);
            writer.Write(comp.Direction);
            writer.Write(comp.Trigger);
            writer.Write(comp.CTrigger);
            writer.Write(comp.Folder);
            writer.Write(comp.Style);
            writer.Write(comp.AutoLock);
            writer.Write(comp.behaviour);
            writer.Write(comp.AutoFire);
            writer.Write(comp.sockets.Length);
            foreach (Socket sock in comp.sockets)
                WriteSock(writer, sock);
        }

        public static byte[] getBytes(ShipData ship)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    WriteComp(writer, ship.Head);
                    writer.Write(ship.GetComponents().Length);
                    foreach (Data.Shared.Component comp in ship.components)
                        WriteComp(writer, comp);
                    writer.Write(ship.Name);
                    writer.Write(ship.Description);
                    writer.Write(ship.Category);
                    writer.Write(ship.PlayerMade);
                    writer.Write(ship.CombatResponsive);
                    writer.Write(ship.CombatActive);
                    writer.Write(ship.Aligned);
                }
                return stream.ToArray();
            }
        }

        public static Data.Shared.Socket ReadSock(BinaryReader reader)
        {
            Data.Shared.Socket sock = new Socket();

            sock.SocketID = reader.ReadInt32();
            sock.OtherID = reader.ReadInt32();
            sock.OtherLinkID = reader.ReadInt32();

            return sock;
        }

        public static Data.Shared.Component ReadComp(BinaryReader reader)
        {
            Data.Shared.Component comp = new Data.Shared.Component();

            comp.Name = reader.ReadString();
            comp.InstanceID = reader.ReadInt32();
            comp.Direction = reader.ReadString();
            comp.Trigger = reader.ReadString();
            comp.CTrigger = reader.ReadString();
            comp.Folder = reader.ReadString();
            comp.Style = reader.ReadString();
            comp.AutoLock = reader.ReadBoolean();
            comp.behaviour = reader.ReadInt32();
            comp.AutoFire = reader.ReadBoolean();

            int length = reader.ReadInt32();
            comp.sockets = new Socket[length];

            for (int i = 0; i < length; i++)
            {
                comp.sockets[i] = ReadSock(reader);
            }

            return comp;
        }

            public static Data.Shared.ShipData fromBytes(byte[] arr)
            {
                Data.Shared.ShipData ship = new Data.Shared.ShipData();

                using (MemoryStream stream = new MemoryStream(arr))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        ship.Head = ReadComp(reader);

                        int length = reader.ReadInt32();
                        for (int i = 0; i < length; i++)
                            ship.AddComponent(ReadComp(reader), false);

                        ship.Name = reader.ReadString();
                        ship.Description = reader.ReadString();
                        ship.Category = reader.ReadString();
                        ship.PlayerMade = reader.ReadBoolean();
                        ship.CombatResponsive = reader.ReadBoolean();
                        ship.CombatActive = reader.ReadBoolean();
                        ship.Aligned = reader.ReadBoolean();
                        ship.Initialized = true;
                    }
                    return ship;
                }
            }
            public static WeaponData bytesToWeapon(byte[] arr)
            {
                WeaponData wData = new WeaponData();
                using (MemoryStream stream = new MemoryStream(arr))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                    wData.Damage = reader.Read();

                    wData.Direction = new Vector3();
                    wData.Direction.x = reader.Read();
                    wData.Direction.y = reader.Read();
                    wData.Direction.z = reader.Read();

                    wData.Distance = reader.Read();

                    //wData.Self = reader.Read() as Transform;
                    }
                }
                return wData;
            }
    }
}
