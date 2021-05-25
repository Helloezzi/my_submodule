// Copyright 2015 gRPC authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace ForetifyLinker
{
    class Program
    {
        public static void Main(string[] args)
        {
            // how to use
            IServer manager = new Server();
            manager.AddReceiver(new Receiver());
            manager.StatusEvent += ServerStatus;
            manager.Start(Define.URL, Define.Port);

            // test code            
            KeyInput();
            manager.Stop();
            Console.ReadKey();
            manager.Start(Define.URL, Define.Port);
            KeyInput();
            manager.Stop();

            #region gRPC example
            /*
            Server server = new Server
            {
                Services = { Foretify.BindService(new SSPForetify()) },
                Ports = { new ServerPort("localhost", Define.Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Morai gRPC server listening on port " + Define.Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            server.ShutdownAsync().Wait();
            */
            #endregion

            // sprint4
        }

        public static void ServerStatus(string msg)
        {
            // event message
            Console.WriteLine(msg);
        }

        public static void KeyInput()
        {
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                Console.Write(" --- You pressed ");
                if ((cki.Modifiers & ConsoleModifiers.Alt) != 0) Console.Write("ALT+");
                if ((cki.Modifiers & ConsoleModifiers.Shift) != 0) Console.Write("SHIFT+");
                if ((cki.Modifiers & ConsoleModifiers.Control) != 0) Console.Write("CTL+");
                Console.WriteLine(cki.Key.ToString());
            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}
