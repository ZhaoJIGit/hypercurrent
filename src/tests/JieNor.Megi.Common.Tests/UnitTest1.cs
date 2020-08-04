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
            Console.WriteLine(DESEncrypt.Decrypt("86A41E1E1BF2DBC61E6FEFD9C386FD8487CBF66F8A46A4E5BFA764F260B392C12D262B54E4E28DBA3F7B51C6FF7A638E40580AA605FC403486C271601B0BD316384B1A2103C6B835F196A2E64D4B24F65DF52B446CDCDA3043B3042C94DC35B2F68CF8D8B3A66D54F9EAC611AEBD6A9B5CD2B789BC6CCC5B180220A45D58B98EF627943AD0BE739608FF2A140A180B747E93DE5ADEA0AEAF"));
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
