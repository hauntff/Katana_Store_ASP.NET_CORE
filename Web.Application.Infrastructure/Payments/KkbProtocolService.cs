using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Application.Domain.Configs;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace Web.Application.Infrastructure.Interfaces
{
    public class KkbProtocolService : IKkbProtocolService
    {
        private readonly X509Certificate2 _certificate;

        private const string KkbRequestPattern =
            "<merchant cert_id=\"{0}\" name=\"{1}\"><order order_id=\"%ORDER%\" amount=\"%AMOUNT%\" currency=\"{2}\"><department merchant_id=\"{3}\" amount=\"%AMOUNT%\"/></order></merchant>";
        private const string KkbStatusPattern =
            "<merchant id=\"{0}\"><order id=\"{1}\"/></merchant>";
        private const string KkbApprovePattern =
            "<merchant id=\"{0}\"><command type=\"complete\"/><payment reference=\"{1}\" approval_code=\"{2}\" orderid=\"{3}\" amount=\"{4}\" currency_code=\"{5}\"/></merchant>";

        public bool IsValid { get; private set; }

        private readonly KkbConfig _config;
        private readonly ILogger<KkbProtocolService> _logger;
        public KkbProtocolService(IOptions<KkbConfig> config,
             ILogger<KkbProtocolService> logger)
        {
            _config = config?.Value;
            _logger = logger;
            _certificate = LoadCertificate(_config);
        }

        private X509Certificate2 LoadCertificate(KkbConfig config)
        {
            try
            {
                IsValid = true;
                return new X509Certificate2(config.KkbPfxFile, config.KkbPfxPass);
            }
            catch (Exception exception)
            {
                IsValid = false;
                _logger.LogError($"LoadCertificate error: {exception.Message}");
                return null;
            }
        }

        // Функция Build64 генерирует запрос который отправляется на https://epay.kkb.kz/jsp/process/logon.jsp
        // В качестве входящих параметров ожидает idOrder (номер заказа в магазине) и amount (сумма к оплате)
        // Возвращает строку в Base64

        public string Build64Sync(string idOrder, decimal amount)
        {
            var pattern = string.Format(KkbRequestPattern,
                _config.KkbCertId,
                _config.KkbShopName,
                _config.KkbCurrency,
                _config.KkbMerchantId);
            var forSign = pattern.Replace("%ORDER%", idOrder)
                .Replace("%AMOUNT%", $"{amount:f}".Replace(",", "."));
            if (!_certificate.HasPrivateKey) throw new InvalidOperationException("Внутренняя ошибка сервера: Некорректный приватный сертификат");
            byte[] signData= { };
            if (_certificate.PrivateKey is RSACng)
            {
                RSACng rSACng = (RSACng)_certificate.PrivateKey;
                signData =  rSACng.SignData(ToBytes(forSign), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
            //var rsaCsp = (RSACryptoServiceProvider)_certificate.PrivateKey;
            //var signData = rsaCsp.SignData(ToBytes(forSign), "SHA1");
            Array.Reverse(signData);
            var result = "<document>" + forSign + "<merchant_sign type=\"RSA\">" +
                         Convert.ToBase64String(signData, Base64FormattingOptions.None) + "</merchant_sign></document>";
            return Convert.ToBase64String(ToBytes(result), Base64FormattingOptions.None);
        }

        // Функция  Verify проверяет корректность подписи, полученной от банка
        // В качестве входящих параметров ожидает strForVerify (строка, которую получили от банка) и Sign (ЭЦП к данной строке)

        public async Task<bool> Verify(string forVerify, string sign)
        {
            var certificate = new X509Certificate2(_config.KkbCaFile);
            var rsaCsp = (RSACryptoServiceProvider)certificate.PrivateKey;
            var bStrForVerify = ToBytes(forVerify);
            var bSign = Convert.FromBase64String(sign);
            Array.Reverse(bSign);
            bool result;
            try
            {
                result = rsaCsp.VerifyData(bStrForVerify, "SHA1", bSign);
            }
            catch
            {
                result = false;
            }
            return await Task.FromResult(result);
        }

        // Функция Sign64 подписывает произвольную строку
        // В качестве входящих параметров ожидает StrForSign (подписываемая строка)
        // Возвращает ЭЦП кодированный в Base64

        private async Task<string> Sign64(string forSign)
        {
            var rsaCsp = (RSACryptoServiceProvider)_certificate.PrivateKey;
            var signData = rsaCsp.SignData(ToBytes(forSign), "SHA1");
            Array.Reverse(signData);
            return await Task.FromResult(Convert.ToBase64String(signData, Base64FormattingOptions.None));
        }

        public async Task<string> OrderPay(string orderId, decimal amount, string email)
        {
            return
                await
                    Task.FromResult("<body><form name='SendOrder' method='post' action='" + _config.KkbLogonUrl + "'>" +
                                    "<input type='hidden' name='email' value='" + email + "'><input type='hidden' name='Signed_Order_B64' value='" +
                                    Build64Sync(orderId, amount) + "'><input type='hidden' name='Language' value='rus'>" +
                                    "<input type='hidden' name='BackLink' value='" +
                                    _config.BackLink + "'><input type='hidden' name='PostLink' value='" +
                                    _config.PostLink + "'></form><script type='text/javascript'>document.forms['SendOrder'].submit();</script></body>");
        }

        public string Status(string orderId)
        {
            var pattern = string.Format(KkbStatusPattern, _config.KkbMerchantId, orderId);
            if (!IsValid)
                return "<Error>Необработанная ошибка</Error>";
            var rsaCsp = (RSACryptoServiceProvider)_certificate.PrivateKey;
            var signData = rsaCsp.SignData(ToBytes(pattern), "SHA1");
            Array.Reverse(signData);

            var result = "<document>" + pattern + "<merchant_sign type=\"RSA\" cert_id=\"" + _config.KkbCertId +
                "\">" + Convert.ToBase64String(signData, Base64FormattingOptions.None) + "</merchant_sign></document>";
            return result;
        }

        public string Approve(string orderId, string reference, string approval, string amount, string currency)
        {
            //"<merchant id=\"{0}\"><command type=\"complete\"/><payment reference=\"{1}\" approval_code=\"{2}\" orderid=\"{3}\" amount=\"{4}\" currency_code=\"{5}\"/></merchant>";
            var pattern = string.Format(KkbApprovePattern, _config.KkbMerchantId, reference, approval, orderId, amount, currency);
            if (!IsValid)
                return "<Error>Необработанная ошибка</Error>";
            var rsaCsp = (RSACryptoServiceProvider)_certificate.PrivateKey;
            var signData = rsaCsp.SignData(ToBytes(pattern), "SHA1");
            Array.Reverse(signData);

            var result = "<document>" + pattern + "<merchant_sign type=\"RSA\" cert_id=\"" + _config.KkbCertId +
                "\">" + Convert.ToBase64String(signData, Base64FormattingOptions.None) + "</merchant_sign></document>";
            return result;
        }

        public static byte[] ToBytes(string s)
        {
            return new UTF8Encoding().GetBytes(s);
        }
    }
}
