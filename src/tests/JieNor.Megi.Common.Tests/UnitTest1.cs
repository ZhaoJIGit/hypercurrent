using System;
using JieNor.Megi.Common.Encrypt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JieNor.Megi.Common.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine(DESEncrypt.Decrypt("D9E84F2144CF6DB4B79CC9E83EE21FD8"));
            Console.WriteLine(DESEncrypt.Decrypt("B3AFD6CE69623BB1"));
            //Console.WriteLine(DESEncrypt.Decrypt("BC6147DD0BEBEFBF"));
            //Console.WriteLine(DESEncrypt.Decrypt("D9E84F2144CF6DB4B79CC9E83EE21FD8"));

            //Console.WriteLine(DESEncrypt.Encrypt("#hypercu.CN#2019"));

            //// 用户密码
            //Console.WriteLine(MD5Service.MD5Encrypt("$111680zh"));

            Console.WriteLine(MD5Service.MD5Encrypt("172.18.75.6"));
            Console.WriteLine(MD5Service.MD5Encrypt("123456"));
        }
    }
}
