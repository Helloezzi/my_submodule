using System;
using System.Runtime.InteropServices;

namespace ForetifyLinker
{
    [StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi)]
    public struct Header
    {
        // message id
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] rpc_id;
        
        // REQUEST = 0, REPLY = 1
        public byte rpc_direction;

        // data size
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] rpc_msg_size;

        // FAIL = 0, SUCCESS = 1
        public byte rpc_result;

        public int GetId()
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(rpc_id);

            return BitConverter.ToInt16(rpc_id, 0);
        }

        public int GetSize()
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(rpc_msg_size);

            return BitConverter.ToInt32(rpc_msg_size, 0);
        }

        public void Create(ushort id, byte direction, int msg_size, byte result)
        {
            rpc_id = new byte[2];
            rpc_id[0] = (byte)((id & 0x0000FF00) >> 8);
            rpc_id[1] = (byte)(id & 0x000000FF);
            rpc_direction = direction;
            rpc_msg_size = new byte[4];
            rpc_msg_size[0] = (byte)((msg_size & 0x0000FF00) >> 24);
            rpc_msg_size[1] = (byte)((msg_size & 0x0000FF00) >> 16);
            rpc_msg_size[2] = (byte)((msg_size & 0x0000FF00) >> 8);
            rpc_msg_size[3] = (byte)(msg_size & 0x000000FF);
            rpc_result = result;
        }
    }
}
