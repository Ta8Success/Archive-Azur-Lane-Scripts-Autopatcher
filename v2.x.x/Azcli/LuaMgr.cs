using System;
using System.IO;
using System.Text;

namespace Azurlane
{
    internal static class LuaMgr
    {
        internal static int SuccessCount, FailedCount;

        internal enum State
        {
            None,
            Encrypted,
            Decrypted
        }

        internal static void CheckAndExecute(string lua, Tasks task)
        {
            var bytes = File.ReadAllBytes(lua);
            var state = State.None;

            if (bytes[3] == 0x80)
            {
                state = State.Encrypted;
                if (task == Tasks.Encrypt)
                {
                    Utils.pInfoln(string.Format("{0} is already encrypted... <Aborted>", Path.GetFileName(lua)));
                    return;
                }
                else if (task == Tasks.Decompile)
                {
                    Execute(lua, bytes, Tasks.Decrypt, state);
                }
            }
            else if (bytes[3] == 0x02)
            {
                state = State.Decrypted;
                if (task == Tasks.Decrypt)
                {
                    Utils.pInfoln(string.Format("{0} is already decrypted... <Aborted>", Path.GetFileName(lua)));
                    return;
                }
            }
            else if (task != Tasks.Recompile)
            {
                Utils.pInfoln("Not a valid or damaged lua file... <Aborted>");
                return;
            }

            if (task == Tasks.Decrypt || task == Tasks.Encrypt)
            {
                Execute(lua, bytes, task, state);
            }
            else if (task == Tasks.Decompile || task == Tasks.Recompile)
            {
                Execute(lua, task);
            }
            Program.isInvalid = false;
        }

        private static void Execute(string lua, byte[] bytes, Tasks task, State state)
        {
            try
            {
                Utils.pInfof(string.Format("{0} {1}...", (task == Tasks.Decrypt ? "Decrypting" : "Encrypting"), Path.GetFileName(lua)));
                using (var stream = new MemoryStream(bytes))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        // ljd\rawdump\header.py + Perfare
                        var magic = reader.ReadBytes(3);
                        var version = reader.ReadByte();
                        var bits = reader.ReadUleb128();

                        var is_stripped = ((bits & 2u) != 0u);
                        if (!is_stripped)
                        {
                            var length = reader.ReadUleb128();
                            var name = Encoding.UTF8.GetString(reader.ReadBytes((int)length));
                        }

                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            var size = reader.ReadUleb128();

                            if (size == 0)
                                break;

                            var next = reader.BaseStream.Position + size;
                            bits = reader.ReadByte();

                            var arguments_count = reader.ReadByte();
                            var framesize = reader.ReadByte();
                            var upvalues_count = reader.ReadByte();
                            var complex_constants_count = reader.ReadUleb128();
                            var numeric_constants_count = reader.ReadUleb128();
                            var instructions_count = reader.ReadUleb128();

                            var start = (int)reader.BaseStream.Position;

                            if (state == State.Encrypted && task == Tasks.Decrypt)
                            {
                                bytes[3] = 0x02;
                                bytes = Unlock(start, bytes, (int)instructions_count);
                            }
                            else if (state == State.Decrypted && task == Tasks.Encrypt)
                            {
                                bytes[3] = 0x80;
                                bytes = Lock(start, bytes, (int)instructions_count);
                            }
                            else break;

                            reader.BaseStream.Position = next;
                        }
                    }
                }
                File.WriteAllBytes(lua, bytes);
            }
            catch (Exception e)
            {
                Utils.eLogger(string.Format("Exception detected during {0} {1}", (task == Tasks.Decrypt ? "decrypting" : "encrypting"), Path.GetFileName(lua)), e);
            }
        }

        private static void Execute(string lua, Tasks task)
        {
            Utils.pInfof(string.Format("{0} {1}...", (task == Tasks.Decompile ? "Decompiling" : "Recompiling"), Path.GetFileName(lua)));
            try
            {
                Utils.NewCommand(task == Tasks.Decompile ? $"python main.py -f \"{lua}\" -o \"{lua}\"" : $"luajit.exe -b \"{lua}\" \"{lua}\"");
            }
            catch (Exception e)
            {
                Utils.eLogger(string.Format("Exception detected during {0} {1}", (task == Tasks.Decompile ? "decompiling" : "recompiling"), Path.GetFileName(lua)), e);
            }
        }

        private static byte[] Lock(int start, byte[] bytes, int count)
        {
            var result = start;
            result += 4;
            var v2 = 0;
            do
            {
                var v3 = bytes[result - 4];
                result += 4;
                var v4 = bytes[result - 7] ^ v2++;
                bytes[result - 8] = (byte)(Properties.Resources.Lock[v3] ^ v4);
            }
            while (v2 != count);
            return bytes;
        }

        private static uint ReadUleb128(this BinaryReader reader)
        {
            // ljd\util\binstream.py + Perfare
            uint value = reader.ReadByte();
            if (value >= 0x80)
            {
                var bitshift = 0;
                value &= 0x7f;
                while (true)
                {
                    var b = reader.ReadByte();
                    bitshift += 7;
                    value |= (uint)((b & 0x7f) << bitshift);
                    if (b < 0x80)
                        break;
                }
            }
            return value;
        }

        private static byte[] Unlock(int start, byte[] bytes, int count)
        {
            var result = start;
            result += 4;
            var v2 = 0;
            do
            {
                var v3 = bytes[result - 4];
                result += 4;
                var v4 = bytes[result - 7] ^ v3 ^ (v2++ & 0xFF);
                bytes[result - 8] = Properties.Resources.Unlock[v4];
            }
            while (v2 != count);
            return bytes;
        }
    }
}