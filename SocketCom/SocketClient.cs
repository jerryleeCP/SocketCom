using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketCom
{
    public class SocketClient
    {
        public string IP = "127.0.0.1";
        public int Port = 8088;
        public int ReceiveBufferSize = 1024;
        public bool IsConnected = false;
        public Action<Socket, int, byte[]> MessageHandler;

        private byte[] receiveBuffer;
        private Socket clientSocket;

        public SocketClient()
        {
            receiveBuffer = new byte[ReceiveBufferSize];
        }
        public SocketClient(string ip, int port, int recSize = 1024)
        {
            this.IP = ip;
            this.Port = port;
            this.ReceiveBufferSize = recSize;
            receiveBuffer = new byte[ReceiveBufferSize];
        }
        public void Connect()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress mIp = IPAddress.Parse(IP);
            IPEndPoint ip_end_point = new IPEndPoint(mIp, Port);
            try
            {
                clientSocket.Connect(ip_end_point);
                IsConnected = true;
                Thread thread = new Thread(RecieveMessage);
                thread.Start(clientSocket);
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                IsConnected = false;
                Console.WriteLine("连接服务器失败");
                return;
            }
        }

        public void SendMessage(int mesType,string data="")
        {
            SendMessage(mesType,data.ToBytes());
        }
        public void SendMessage(int mesType,byte[] data)
        {
            if (IsConnected == false)
                return;
            try
            {
                clientSocket.Send(Common.WriteMessage(data, mesType));
            }
            catch
            {
                IsConnected = false;
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
        public void Close()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        private void RecieveMessage(object serverSocket)
        {
            Socket myServerSocket = (Socket)serverSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = myServerSocket.Receive(receiveBuffer);
                    ByteBuffer buff = new ByteBuffer(receiveBuffer);
                    int mesType = buff.ReadInt();
                    int len = buff.ReadInt();
                    byte[] data = buff.ReadBytes(len);
                    //Console.WriteLine("服务器发来消息：数据类型:{0} 数据长度：{1} 数据内容：{2}", mesType.ToString(), len, data);
                    if (MessageHandler != null)
                        MessageHandler(myServerSocket, mesType,data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myServerSocket.Shutdown(SocketShutdown.Both);
                    myServerSocket.Close();
                    break;
                }
            }
        }
    }
}
