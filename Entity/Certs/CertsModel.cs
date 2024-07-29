namespace WeChatApiBase.Entity.Certs
{
    public class CertsModel
    {
        /// <summary>
        /// 证书序列号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public string EffectiveTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public string ExpireTime { get; set; }

        /// <summary>
        /// 解密后字符串
        /// </summary>
        public string PlainCertificate { get; set; }
    }
}
