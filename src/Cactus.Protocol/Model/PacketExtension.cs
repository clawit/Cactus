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

        private static Packet FromByteString(this ByteString byteString)
        {
            return Packet.Parser.ParseFrom(byteString);
        }
    }
}
