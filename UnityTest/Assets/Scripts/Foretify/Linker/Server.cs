using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ForetifyLinker
{
    public class Server : IServerBase, IServer
    {
        // event        
        public ServerStatusEventHandler StatusEvent { get; set; }

        // interface members
        public TcpListener Listener { get; private set; }

        public TcpClient Client { get; private set; }

        public bool Connected { get; private set; }

        public SSP_MSG_ID Last_id { get; private set; } = SSP_MSG_ID.None;        

        //public IReceiver Receiver { get; set; }

        public IDebug xDebug { get; set; } = new XDebug();

        public bool IsRunning { get; set; } = false;

        public List<IReceiver> Recievers { get; set; }

        /*
        public Server(IReceiver receiver)
        {
            Receiver = receiver;
            Receiver.Response = this;
        }*/

        public Server()
        {
            Recievers = new List<IReceiver>();
        }

        public void Start(string addr, int port)
        {
            if (IsRunning)
            {
                StatusEvent?.Invoke("The Server is already running");
                return;
            }

            var t = Task.Run(() =>
            {
                StatusEvent?.Invoke("======= Start SSP Server =======");
                Listen(addr, port);
            });
        }

        public void Listen(string addr, int port)
        {
            IsRunning = true;
            Listener = new TcpListener(IPAddress.Parse(addr), port);
            Listener.Start();
            StatusEvent?.Invoke("-Start listen");

            try
            {
                while (true)
                {
                    if (Connected == false)
                    {
                        StatusEvent?.Invoke("-Waiting for a connection...");
                        var client = Listener.AcceptTcpClient();
                        Connected = true;
                        StatusEvent?.Invoke($"-Connected client : {client.Client.RemoteEndPoint}");                        

                        var t = Task.Run(() => {                            
                            OnReceive(client);
                        });                        
                    }
                }                
            }
            catch (Exception e)
            {
                Connected = false;
                StatusEvent?.Invoke("Terminated client");                
            }     
        }

        private void OnReceive(object tcpClient)
        {
            try
            {
                if (Connected == false)
                    return;

                TcpClient client = tcpClient as TcpClient;
                Client = client;

                // Buffer for reading data
                byte[] buffer = new byte[Define.MAX_MESSAGE_SIZE];

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int index;
                // Loop to receive all the data sent by the client.
                while ((index = stream.Read(buffer, 0, buffer.Length)) != 0 && Connected)
                {
                    byte[] arr = new byte[index];
                    Array.Copy(buffer, arr, index);

                    if (Last_id == SSP_MSG_ID.None)
                    {
                        Header header = Converter.ByteToStruct<Header>(arr);
                        Last_id = (SSP_MSG_ID)header.GetId();
                        //xDebug.Write($"request id : {Last_id}");

                        if (Last_id == SSP_MSG_ID.end_sim || Last_id == SSP_MSG_ID.terminate_sim)
                        {
                            foreach (IReceiver rec in Recievers)
                            {
                                rec.Receive(Last_id, arr);
                            }
                        }
                    }
                    else
                    {
                        foreach (IReceiver rec in Recievers)
                        {
                            rec.Receive(Last_id, arr);
                        }
                    }                    
                }
            }
            catch
            {
                StatusEvent?.Invoke("Terminated client");
                Connected = false;

                if (Client != null)
                {
                    Client.Close();
                    Client = null;
                }                    
            }
        }

        public void SendData(SSP_MSG_ID id, IMessage msg)
        {
            NetworkStream stream = Client.GetStream();
            byte[] datas = msg.ToByteArray();

            // create header
            Header reply = new Header();
            reply.Create((ushort)id, 1, datas.Length, 1);
            byte[] hData = Converter.StructToBytes<Header>(reply);

            stream.Write(hData, 0, hData.Length);
            stream.Write(datas, 0, datas.Length);

            // response 보내고 나면 id 초기화
            Last_id = SSP_MSG_ID.None;
        }

        public void Stop()
        {
            Connected = false;
            IsRunning = false;

            if (Client != null)
            {
                Client.Close();
                Client = null;
            }                

            if (Listener != null)
            {
                Listener.Stop();
                Listener = null;
            }

            //debug.Write("Stop server");
            StatusEvent?.Invoke("======= Stop SSP Server =======");
        }

        public void AddReceiver(IReceiver receiver)
        {
            receiver.Response = this;
            receiver.xDebug = this.xDebug;
            Recievers.Add(receiver);            
        }
    }
}
