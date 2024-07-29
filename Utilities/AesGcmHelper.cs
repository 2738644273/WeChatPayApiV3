using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace WeChatApiBase.Utilities
{
    /// <summary>
    /// 解密微信通知结果帮助类
    /// 安装Portable.BouncyCastle
    /// </summary>
    public class AesGcmHelper
    {
        private static string ALGORITHM = "AES/GCM/NoPadding";
        private static int TAG_LENGTH_BIT = 128;
        private static int NONCE_LENGTH_BYTE = 12;
        private static string AES_KEY = string.Empty;

        /// <summary>
        ///  解密微信通知结果帮助类
        /// </summary>
        /// <param name="associatedData">通知数据resource下的associated_data</param>
        /// <param name="nonce">通知数据resource.nonce</param>
        /// <param name="ciphertext">通知数据resource.ciphertext</param>
        /// <param name="APIV3Key">APIV3的密钥key</param>
        /// <returns></returns>
        public static string AesGcmDecrypt(string associatedData, string nonce, string ciphertext, string _privateKey)
        {
            GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            AeadParameters aeadParameters = new AeadParameters(
                new KeyParameter(Encoding.UTF8.GetBytes(_privateKey)),
                128,
                Encoding.UTF8.GetBytes(nonce),
                Encoding.UTF8.GetBytes(associatedData));
            gcmBlockCipher.Init(false, aeadParameters);

            byte[] data = Convert.FromBase64String(ciphertext);
            byte[] plaintext = new byte[gcmBlockCipher.GetOutputSize(data.Length)];
            int length = gcmBlockCipher.ProcessBytes(data, 0, data.Length, plaintext, 0);
            gcmBlockCipher.DoFinal(plaintext, length);
            return Encoding.UTF8.GetString(plaintext);
        }
    }

    
}
