using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Protobuf;
using Cactus.Protocol.Model;

namespace Cactus.Protocol.Model
{
    public static class PacketExtension
    {
        public static byte[] GetByteArray(this Packet packet)
        {
            using (var ms = new MemoryStream())
            {
                packet.WriteTo(ms);
                return ms.ToArray();
            }
        }

        public static ByteString GetByteString(this Packet packet)
        {
            using (var ms = new MemoryStream())
            {
                packet.WriteTo(ms);
                return ByteString.CopyFrom(ms.ToArray());
            }
        }

        public static Packet FromByteArray(this byte[] bytes)
        {
            return Packet.Parser.ParseFrom(bytes);
        }

        public static Packet FromByteString(this ByteString byteString)
        {
            return Packet.Parser.ParseFrom(byteString);
        }

        public static bool SetOption(this Packet packet, string key, string value)
        {
            if (packet == null || string.IsNullOrWhiteSpace(key))
                return false;
            else
            {
                if (packet.Options.ContainsKey(key))
                {
                    packet.Options[key] = value;
                }
                else
                {
                    packet.Options.Add(key, value);
                }

                return true;
            }
        }

        public static bool TryGetOption(this Packet packet, string key, out string value)
        {
            return packet.Options.TryGetValue(key, out value);
        }
    }
}
