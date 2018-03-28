using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketCom;
using System.Net.Sockets;

namespace SocketClient1
{
    class Program
    {      
        static void Main(string[] args)
        {
            SocketClient mSocketClient;
            mSocketClient = new SocketClient();
            mSocketClient.IP = "127.0.0.1";
            mSocketClient.Port = 9001;
            mSocketClient.ReceiveBufferSize = 102400;
            mSocketClient.MessageHandler += MessageHandle;
            mSocketClient.Connect();

            //测试代码
            Console.WriteLine("输入请求：0(请求) 1(数据) 2(图片) 3(其他) -1(退出)");
            while (true)
            {
                try
                {           
                    int type =int.Parse(Console.ReadLine());
                    mSocketClient.SendMessage(type);
                    if (type == -1)
                        break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("InputError Error:"+e.Message);
                }           
            }
        }
        public enum MessageType
        {
            Request,
            Data,
            Picture,
            Other
        }
        private static void MessageHandle(Socket socket, int mesType, byte[] data)
        {
            Console.WriteLine("类型：" + mesType + "  " + data.ToStr());
            if (mesType == (int)MessageType.Picture)
            {
                Console.WriteLine("接收图片："+data.Length);
            }
        }
    }
   
}
