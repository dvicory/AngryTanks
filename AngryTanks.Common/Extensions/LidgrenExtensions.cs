using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Lidgren.Network;

namespace AngryTanks.Common.Extensions
{
    namespace LidgrenExtensions
    {
        public static class NetOutgoingMessageExtensionsClass
        {
            /// <summary>
            /// Writes a <see cref="Vector2"/> with two 32 bit floating point values
            /// </summary>
            /// <param name="msg"></param>
            /// <param name="source"></param>
            public static void Write(this NetOutgoingMessage msg, Vector2 source)
            {
                msg.Write(source.X);
                msg.Write(source.Y);
            }
        }

        public static class NetIncomingMessageExtensionsClass
        {
            /// <summary>
            /// Reads a <see cref="Vector2"/> written by Write(Vector2)
            /// </summary>
            /// <param name="msg"></param>
            /// <returns></returns>
            public static Vector2 ReadVector2(this NetIncomingMessage msg)
            {
                Single x = msg.ReadSingle();
                Single y = msg.ReadSingle();
                return new Vector2(x, y);
            }
        }
    }
}
