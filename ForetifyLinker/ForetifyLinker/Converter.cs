using Google.Protobuf;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Morai.Protobuf.Foretify;

namespace ForetifyLinker
{
    public static class Converter
    {
        // refer : https://developers.google.com/protocol-buffers/docs/reference/csharp/class/google/protobuf/message-parser
        public static T ToObject<T>(this byte[] buf) where T : IMessage<T>, new()
        {
            if (buf == null)
                return default(T);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(buf, 0, buf.Length);
                ms.Seek(0, SeekOrigin.Begin);
                MessageParser<T> parser = new MessageParser<T>(() => new T());
                return parser.ParseFrom(ms);
            }
        }

        public static byte[] StructToBytes<T>(object obj) where T : struct
        {
            int iSize = Marshal.SizeOf(obj);
            byte[] arr = new byte[iSize];
            IntPtr ptr = Marshal.AllocHGlobal(iSize);
            Marshal.StructureToPtr(obj, ptr, false);
            Marshal.Copy(ptr, arr, 0, iSize);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static T ByteToStruct<T>(Byte[] buffer) where T : struct
        {
            try
            {
                IntPtr ptr = Marshal.AllocHGlobal(buffer.Length);
                Marshal.Copy(buffer, 0, ptr, buffer.Length);
                T ret = (T)Marshal.PtrToStructure(ptr, typeof(T));
                Marshal.FreeHGlobal(ptr);
                return ret;
            }
            catch
            {
                return default(T);
            }
        }

        public static string BytesToStringConversion(byte[] bytes)
        {
            using (MemoryStream Stream = new MemoryStream(bytes))
            {
                using (StreamReader streamReader = new StreamReader(Stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public static string GetStringToBuffer(byte[] buffer)
        {
            return Encoding.Default.GetString(buffer, 0, buffer.Length).TrimEnd('\0');
        }

        public static coord_6dof ToCoord6dof(double x, double y, double z, double roll, double pitch, double yaw)
        {
            coord_6dof res = new coord_6dof();
            res.X = new unit();
            res.Y = new unit();
            res.Z = new unit();
            res.Roll = new unit();
            res.Pitch = new unit();
            res.Yaw = new unit();

            res.X.Value = x;
            res.Y.Value = y;
            res.Z.Value = z;
            res.Roll.Value = roll;
            res.Pitch.Value = pitch;
            res.Yaw.Value = yaw;

            //res.X = x;
            //res.Y = y;
            //res.Z = z;
            //res.Roll = roll;
            //res.Pitch = pitch;
            //res.Yaw = yaw;
            return res;
        }
    }
}
