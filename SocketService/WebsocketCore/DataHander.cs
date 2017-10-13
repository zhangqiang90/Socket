using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

using WebSocket.Model;

namespace WebSocketCore
{
    public static class DataHander
    {
        #region 1.0 进制转换公用方法

        // 把字节型转换成十六进制字符串  
        public static string ByteToString(byte[] InBytes)
        {
            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2} ", InByte);
            }
            return StringOut;
        }
        public static string ByteToString(byte[] InBytes, int len)
        {
            string StringOut = "";
            for (int i = 0; i < len; i++)
            {
                StringOut = StringOut + String.Format("{0:X2} ", InBytes[i]);
            }
            return StringOut;
        }
        /// <summary>
        /// 字节转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 16进制字符串转10进制
        /// </summary>
        /// <param name="strColorValue"></param>
        /// <returns></returns>
        public static int GetHexadecimalValue(String strColorValue)
        {
            char[] nums = strColorValue.ToCharArray();
            int total = 0;
            try
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    String strNum = nums[i].ToString().ToUpper();
                    switch (strNum)
                    {
                        case "A":
                            strNum = "10";
                            break;
                        case "B":
                            strNum = "11";
                            break;
                        case "C":
                            strNum = "12";
                            break;
                        case "D":
                            strNum = "13";
                            break;
                        case "E":
                            strNum = "14";
                            break;
                        case "F":
                            strNum = "15";
                            break;
                        default:
                            break;
                    }
                    double power = Math.Pow(16, Convert.ToDouble(nums.Length - i - 1));
                    total += Convert.ToInt32(strNum) * Convert.ToInt32(power);
                }

            }
            catch (System.Exception ex)
            {
                String strErorr = ex.ToString();
                return 0;
            }


            return total;
        }
        // 把十六进制字符串转换成字节型  
        public static byte[] StringToByte(string InString)
        {
            string[] ByteStrings;
            ByteStrings = InString.Split(" ".ToCharArray());
            byte[] ByteOut;
            ByteOut = new byte[ByteStrings.Length - 1];
            for (int i = 0; i == ByteStrings.Length - 1; i++)
            {
                ByteOut[i] = Convert.ToByte(("0x" + ByteStrings[i]));
            }
            return ByteOut;
        }
        private static int toByte(char c)
        {
            byte b = (byte)"0123456789ABCDEF".IndexOf(c);
            return b;
        }
        public static byte[] hexStringToByte(String hex)
        {
            int len = (hex.Length / 2);
            byte[] result = new byte[len];
            char[] achar = hex.ToCharArray();
            for (int i = 0; i < len; i++)
            {
                int pos = i * 2;
                result[i] = (byte)(toByte(achar[pos]) << 4 | toByte(achar[pos + 1]));
            }
            return result;
        }
        /// <summary>
        /// 将一条十六进制字符串转换为ASCII
        /// </summary>
        /// <param name="hexstring">一条十六进制字符串</param>
        /// <returns>返回一条ASCII码</returns>
        public static string HexStringToASCII(byte[] bt)
        {
            //byte[] bt = HexStringToBinary(hexstring);
            string lin = "";
            for (int i = 0; i < bt.Length; i++)
            {
                lin = lin + bt[i] + " ";
            }


            string[] ss = lin.Trim().Split(new char[] { ' ' });
            char[] c = new char[ss.Length];
            int a;
            for (int i = 0; i < c.Length; i++)
            {
                a = Convert.ToInt32(ss[i]);
                c[i] = Convert.ToChar(a);
            }

            string b = new string(c);
            return b;
        }
        /// <summary>
        /// 16进制字符串转换为二进制数组
        /// </summary>
        /// <param name="hexstring">用空格切割字符串</param>
        /// <returns>返回一个二进制字符串</returns>
        public static byte[] HexStringToBinary(string hexstring)
        {

            string[] tmpary = hexstring.Trim().Split(' ');
            byte[] buff = new byte[tmpary.Length];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(tmpary[i], 16);
            }
            return buff;
        }

        /// <summary>
        /// 将16进制转换成10禁止，两两转换 如0F	0C	19	0E	38 09
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HexTimeToStrIntTime(string str)
        {
            if (str.Length % 2 != 0)
            {
                return null;
            }
            else
            {
                string rs = "";
                for (int i = 0; i < str.Length; i++)
                {
                    if (i % 2 == 1)
                    {
                        if (i == 1)
                        {
                            rs += (hexStrToInt(str.Substring(i - 1, 2)) + 2000).ToString();
                        }
                        else
                        {
                            rs += hexStrToInt(str.Substring(i - 1, 2)).ToString().FullStr(2);
                        }
                    }
                }
                return rs;
            }
        }
        /// <summary>
        /// 16进制转10进制
        /// </summary>
        public static Int32 hexStrToInt(string hexToInt)
        {
            return Int32.Parse(hexToInt, System.Globalization.NumberStyles.HexNumber);//Convert.ToInt64(hexToInt, 16);
        }

        /// <summary>
        /// 补全位数
        /// </summary>
        /// <param name="str">需要补全的字符串</param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string FullStr(this string str, int len)
        {
            if (str.Length == len)
            {
                return str;
            }
            else if (str.Length < len)
            {
                string zero = "";
                for (int i = 0; i < len - str.Length; i++)
                {
                    zero += "0";
                }
                return zero + str;
            }
            else
            {
                return str.Substring(str.Length - len);
            }
        }
        #endregion

        #region 2.0 登录包解析

        /// <summary>
        /// 登录数据包处理，暂时只解析GPS数据
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static LoginModel LoginHander(DataHeader dh)
        {
            var loginModel = new LoginModel();
            string mDate = string.Empty;
            double lat = 0;
            double lng =0;
            double c = 3600000;

            #region 数据列解析
            byte[] content=dh.Content;
            //GPS数据包
            byte[] gps = new byte[20];
            Buffer.BlockCopy(content, 61, gps, 0, 20);
            //日期数据解析
            byte[] date = new byte[6];
            Buffer.BlockCopy(gps, 1, date, 0, 6);
            //日期
            mDate = HexTimeToStrIntTime(ByteToString(date).Replace(" ",""));
            var dateTime = GetDate(mDate);
            //纬度
            byte[] _lat = new byte[4];
            Buffer.BlockCopy(gps, 7, _lat, 0, 4);

            var newlat = ByteOrderByDes(_lat);
            double a = hexStrToInt(ByteToString(newlat).Replace(" ", ""));
            lat =a / c;
            //经度
            byte[] _lng = new byte[4];
            Buffer.BlockCopy(gps, 11, _lng, 0, 4);

            var newlng = ByteOrderByDes(_lng);

            double b = hexStrToInt(ByteToString(newlng).Replace(" ", ""));
            lng =b / c;
           
            //byte[] _speed = new byte[4];
            //Buffer.BlockCopy(gps, 11, _speed, 0, 4);

            #endregion
            //数据构造
            loginModel.Date = dateTime;
            loginModel.lat = lat;
            loginModel.lng = lng;
            loginModel.Type = dh.Type;
            loginModel.DeviceId = dh.DeviceId.ToString().Trim();
            return loginModel;

        }
        #endregion

        #region 3.0 GPS数据包解析
        public static LoginModel GPSHander(DataHeader dh)
        {
            var loginModel = new LoginModel();
            string mDate = string.Empty;
            double lat = 0;
            double lng = 0;
            double c = 3600000;

            #region 数据列解析
            byte[] content = dh.Content;
            //GPS数据包
            byte[] gps = new byte[20];
            Buffer.BlockCopy(content, 62, gps, 0, 20);
            //日期数据解析
            byte[] date = new byte[6];
            Buffer.BlockCopy(gps, 1, date, 0, 6);
            //日期
            mDate = HexTimeToStrIntTime(ByteToString(date).Replace(" ", ""));
            var dateTime = GetDate(mDate);
            //纬度
            byte[] _lat = new byte[4];
            Buffer.BlockCopy(gps, 7, _lat, 0, 4);

            var newlat = ByteOrderByDes(_lat);
            double a = hexStrToInt(ByteToString(newlat).Replace(" ", ""));
            lat =a / c;
            //经度
            byte[] _lng = new byte[4];
            Buffer.BlockCopy(gps, 11, _lng, 0, 4);

            var newlng = ByteOrderByDes(_lng);

            double b = hexStrToInt(ByteToString(newlng).Replace(" ", ""));
            lng = b / c;

            //byte[] _speed = new byte[4];
            //Buffer.BlockCopy(gps, 11, _speed, 0, 4);

            #endregion

            loginModel.Date = dateTime;
            loginModel.lat = lat;
            loginModel.lng = lng;
            loginModel.Type = dh.Type;
            loginModel.DeviceId = dh.DeviceId.ToString().Trim();

            return loginModel;
        }

        #endregion

        public static DateTime GetDate(string date)
        {
            date = date.Substring(0, 2) + date.Substring(6, 2) + "-" + date.Substring(4, 2) + "-" + date.Substring(2, 2) + " " + date.Substring(8, 2) + ":" + date.Substring(10, 2) + ":" + date.Substring(12, 2);
            DateTime ss = DateTime.Parse(date);
            return ss;
        }
        /// <summary>
        /// 16进制高低位交换
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] ByteOrderByDes(byte[] bytes)
        {
            byte[] newByte = new byte[bytes.Length];

            int a = 0;
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                newByte[a] = bytes[i];
                a++;
            }

            return newByte;
        }

    }
}
