using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GoogleReCaptchaV3
{
    public class ReCaptchaService
    {
        private readonly HttpClient _client;
        private readonly ILogger _log;

        private readonly string _secretKey;

        public ReCaptchaService(
            HttpClient httpClient,
            ILogger<ReCaptchaService> logger,
            IConfiguration configuration)
        {
            _log = logger;
            _client = httpClient ?? throw new NullReferenceException(nameof(httpClient));
            _client.BaseAddress = new Uri("https://www.google.com");

            _secretKey = configuration.GetValue<string>("GoogleReCAPTCHAv3:SecretKey")
                      ?? throw new NullReferenceException("GoogleReCAPTCHAv3:SecretKey");
        }

        public async Task<bool?> ValidateReCaptchaAsync(string token)
        {
            try
            {
                var response = await _client.GetAsync($"/recaptcha/api/siteverify?secret={_secretKey}&response={token}");

                if (response.StatusCode != HttpStatusCode.OK)
                    return false;

                string JSONresponse = await response.Content.ReadAsStringAsync();
                dynamic JSONdata = JObject.Parse(JSONresponse);

                if (JSONdata.success != "true")
                    return false;
            }
            catch (SocketException e)
            {
                _log.LogCritical("CAN'T CONNECT TO GOOGLE CAPTCHA SERVER!");
                _log.LogError(e.Message);
                return null;
            }
            catch (HttpRequestException e)
            {
                _log.LogCritical("CAN'T CONNECT TO GOOGLE CAPTCHA SERVER!");
                _log.LogError(e.Message);
                return null;
            }

            return true;
        }
    }
}
