using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static SerialPort ComDevice = new SerialPort();
        static StringBuilder SB = new StringBuilder();
        static void Main(string[] args)
        {
            init();
            OpenPort();
            Console.WriteLine(SendData(strToHexByte("7F0BF7")));
            Console.WriteLine(SendData(strToHexByte("7F01F7")));
            //Console.WriteLine(SendData(strToHexByte("7F01F7")));
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(SendData(strToHexByte("7F0352F7")));
                Thread.Sleep(50);
                Console.WriteLine(SendData(strToHexByte("7F04F7")));
                Thread.Sleep(1000);
            }
            Console.Read();
        }



        public static void init()
        {
            var serialPorts = SerialPort.GetPortNames();
            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);//绑定事件

        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OpenPort()
        {

            if (ComDevice.IsOpen == false)
            {
                ComDevice.PortName = SerialPort.GetPortNames()[1];
                ComDevice.BaudRate = 115200;
                ComDevice.Parity = Parity.None;
                ComDevice.DataBits = 8;
                ComDevice.StopBits = StopBits.One;
                try
                {
                    ComDevice.Open();
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            else
            {
                try
                {
                    ComDevice.Close();
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public static bool SendData(byte[] data)
        {
            if (ComDevice.IsOpen)
            {
                try
                {
                    ComDevice.Write(data, 0, data.Length);//发送数据
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 字符串转换16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
            AddData(ReDatas);//输出数据
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data">字节数组</param>
        public static void AddData(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:X2}" + " ", data[i]);
            }

            if (sb.ToString() == "55")
            {
                SB.Clear();
                return;
            }
            if (sb.ToString().Contains("F7"))
            {
                string now = sb.ToString();
                if (now.Replace(" ", "").Replace("F7", "").Replace("7F", "") == "0400")
                {
                    Console.WriteLine("获取到卡类型");
                }
                else if (now.Replace(" ", "").Replace("F7", "") == string.Empty)
                {

                }
                else
                {
                    Console.WriteLine("获取卡号:" + now.Replace(" ", "").Replace("F7", ""));
                }
                SB.Clear();
            }
            SB.Append(sb.ToString());
            //Console.WriteLine("Received:" + sb.ToString());
        }

    }
}
