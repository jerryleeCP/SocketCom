using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace SocketCom
{
    public class SocketServer
    {
        public string IP = "127.0.0.1";
        public int Port = 8088;
        public int MaxConnectCount = 10;
        public int ReceiveBufferSize = 1024;
        public Action<Socket, int, byte[]> MessageHandler;
        public int ConnectedCount { get { return clientSockets.Count; } }

        private byte[] receiveBuffer;
        private Socket serverSocket;
        private List<Socket> clientSockets;

        public SocketServer()
        {
            receiveBuffer = new byte[ReceiveBufferSize];
            clientSockets = new List<Socket>();
        }
        public SocketServer(string ip, int port, int maxCount = 10, int recSize = 1024)
        {
            this.IP = ip;
            this.Port = port;
            this.MaxConnectCount = maxCount;
            this.ReceiveBufferSize = recSize;
            receiveBuffer = new byte[ReceiveBufferSize];
            clientSockets = new List<Socket>();
        }
        public void Start()
        {
            IPAddress ip = IPAddress.Parse(IP);
            IPEndPoint ip_end_point = new IPEndPoint(ip, Port);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ip_end_point);
            serverSocket.Listen(MaxConnectCount);
            Thread thread = new Thread(ConnectServer);
            thread.Start();
            Console.WriteLine("服务器已启动！");
        }

        public void SendMessage(Socket socket, int mesType, byte[] data)
        {
            socket.Send(Common.WriteMessage(data,mesType));
        }
        public void SendMessageToAll(int mesType,byte[] data)
        {
            for (int i = 0; i < clientSockets.Count; i++)
            {
                if(clientSockets[i].Connected)
                clientSockets[i].Send(Common.WriteMessage(data, mesType));
            }
        }

        private void RecieveMessage(object clientSocket)
        {
            Socket mClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = mClientSocket.Receive(receiveBuffer);
                    ByteBuffer buff = new ByteBuffer(receiveBuffer);
                    int mesType = buff.ReadInt();
                    int len = buff.ReadInt();
                    byte[] data = buff.ReadBytes(len);
                    //Console.WriteLine("客户端发来消息：数据类型:{0} 数据长度：{1} 数据内容：{2}", mesType.ToString(), len, data);
                    if (MessageHandler != null)
                        MessageHandler(mClientSocket,mesType,data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Close();
                    break;
                }
            }
        }
        private void ConnectServer()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("客户端{0}成功连接", clientSocket.RemoteEndPoint.ToString());
                clientSockets.Add(clientSocket);
                string mes = "Connected To Server Successfully";
                clientSocket.Send(Common.WriteMessage(mes.ToBytes(),200));
                Thread thread = new Thread(RecieveMessage);
                thread.Start(clientSocket);
            }
        }  
    }
}
