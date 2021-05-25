// Heewon, Jung
// 2021.3.2
// foretify interface

using System.Net.Sockets;
using System.Threading.Tasks;
using Grpc.Core;
using Morai.Protobuf.Foretify;
using System.Collections.Generic;
using Google.Protobuf;

namespace ForetifyLinker
{
    // for fRPC 
    public interface IBase
    {
        IDebug xDebug { get; set; }
    }
    
    public interface IServerBase : IBase
    {
        TcpListener Listener { get; }
        TcpClient Client { get; }        
        bool Connected { get; }
        bool IsRunning { get; }
        void Listen(string addr, int port);
    }    

    public interface IServer : IResponse, IEvent
    {
        SSP_MSG_ID Last_id { get; }
        //IReceiver Receiver { get; set; }
        List<IReceiver> Recievers { get; set; }
        void Start(string addr, int port);
        void Stop();
        void AddReceiver(IReceiver receiver);
    }    

    public interface IReceiver : IBase
    {
        IResponse Response { get; set; }
        void Receive(SSP_MSG_ID id, byte[] arr);
    }

    public interface IResponse : IBase
    {
        void SendData(SSP_MSG_ID id, IMessage obj);
    }

    public interface IDebug
    {
        string Message { get; }
        void Write(string msg, object[] obj = null);
    }

    public delegate void ServerStatusEventHandler(string result);
    public interface IEvent
    {
        ServerStatusEventHandler StatusEvent { get; set; }
    }

    // for grpc
    interface ISSPForetify : IBase
    {
        Task<init_resp> init(init_req req, ServerCallContext context);
    }
}







//interface IHeader
//{
//    UInt16 rpc_id { get; set; }
//    byte rpc_direction { get; set; }
//    uint msg_size { get; set; }
//    byte result { get; set; }
//}

//[StructLayout(LayoutKind.Sequential, Size = 8, CharSet = CharSet.Ansi)]
//public struct test : IHeader
//{
//    public UInt16 rpc_id { get; set; }
//    public byte rpc_direction { get; set; }
//    public uint msg_size { get; set; }
//    public byte result { get; set; }
//}


