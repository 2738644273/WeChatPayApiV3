namespace WeChatApiBase.Entity.Notify;
/// <summary>
/// 通知资源数据 json格式，
/// </summary>
public class NotifyResourceModel
{
        /// <summary>
        /// 原始回调类型，为transaction
        /// </summary>
        public string original_type { get; set; }
        /// <summary>
        /// 对开启结果数据进行加密的加密算法，目前只支持AEAD_AES_256_GCM
        /// </summary>
        public string algorithm { get; set; }
        /// <summary>
        /// Base64编码后的开启/停用结果数据密文
        /// </summary>
        public string ciphertext { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string associated_data { get; set; }
        /// <summary>
        /// 加密使用的随机串
        /// </summary>
        public string nonce { get; set; }
}