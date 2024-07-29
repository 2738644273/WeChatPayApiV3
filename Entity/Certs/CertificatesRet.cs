using System.Collections.Generic;

namespace WeChatApiBase.Entity.Certs
{
    public class CertificatesRet
    {
        /// <summary>
        /// 证书集合
        /// </summary>
        public List<CertificatesInfo> data { get; set; }
    }

    /// <summary>
    /// 平台证书信息
    /// </summary>
    public class CertificatesInfo
    {
        /// <summary>
        /// 证书序列号
        /// </summary>
        public string serial_no { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public string effective_time { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public string expire_time { get; set; }

        /// <summary>
        /// 加密数据信息
        /// </summary>
        public EncryptCertificate encrypt_certificate { get; set; }
    }

    /// <summary>
    /// 加密数据
    /// </summary>
    public class EncryptCertificate
    {
        /// <summary>
        /// 加密算法类型
        /// </summary>
        public string algorithm { get; set; }

        /// <summary>
        /// 随机串
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string associated_data { get; set; }

        /// <summary>
        /// 数据密文
        /// </summary>
        public string ciphertext { get; set; }
    }
}
