using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Azurlane
{
    internal static class LuaMgr
    {
        internal enum State
        {
            None,
            Encrypted,
            Decrypted
        }

        internal static State CheckLuaState(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            if (bytes[3] == 0x80)
                return State.Encrypted;
            else if (bytes[3] == 0x02)
                return State.Decrypted;
            return State.None;
        }
    }
}
