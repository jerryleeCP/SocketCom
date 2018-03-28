using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using SocketCom;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer server = new SocketServer();
            server.IP = "127.0.0.1";
            server.Port = 9001;
            server.ReceiveBufferSize = 102400;
            server.MessageHandler += MessageHandle;
            server.Start();
            Console.ReadKey();
        }

        public enum MessageType
        {
            Request,
            Data,
            Picture,
            Other
        }

        private static void MessageHandle(Socket mySocket,int type, byte[] data)
        {
            if (type == (int)MessageType.Request)
                mySocket.SendMessage((int)MessageType.Data,"服务器接收到Request请求");
            if (type == (int)MessageType.Picture)
            {
                //string path = Environment.CurrentDirectory + "/test.jpg";
                //FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                //fileStream.Seek(0, SeekOrigin.Begin);
                //byte[] bytes = new byte[fileStream.Length];
                //fileStream.Read(bytes, 0, (int)fileStream.Length);
                //fileStream.Close();
                //fileStream.Dispose();
                //Console.WriteLine("图片："+bytes.Length);
                //mySocket.SendMessage((int)MessageType.Picture,bytes);
                mySocket.SendMessage((int)MessageType.Data, "服务器接收到图片请求");
            }       
            if (type == (int)MessageType.Data)
                mySocket.SendMessage((int)MessageType.Data,"服务器接收到数据请求");
            if (type == (int)MessageType.Other)
                mySocket.SendMessage((int)MessageType.Data,"服务器接收到其他请求");
        }
    }
}