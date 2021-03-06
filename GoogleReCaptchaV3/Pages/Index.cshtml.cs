using LazZiya.TagHelpers.Alerts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GoogleReCaptchaV3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ReCaptchaService _captcha;
        public readonly string SiteKey;

        public IndexModel(
            ILogger<IndexModel> logger,
            ReCaptchaService captcha,
            IConfiguration configuration)
        {
            _logger = logger;
            _captcha = captcha;

            // get SiteKey from user secrets
            SiteKey = configuration.GetValue<string>("GoogleReCAPTCHAv3:SiteKey");
        }

        // the token will be generated by google reCAPTCHA API js call
        // add as relevant script and filed in the login form
        [FromForm]
        public string Token { get; set; }

        [FromForm]
        public Person InputModel { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrWhiteSpace(SiteKey))
            {
                // show alert if no site key is defined!
                TempData.Danger("Please add SiteKey and SecretKey!", dismissable: false);
            }
            else
            {
                TempData.Info("This form is protected by Google reCAPTCHA v3");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var captchaChallenge = await _captcha.ValidateReCaptchaAsync(Token);
            if (captchaChallenge == null)
            {
                // reCAPTCHA validiation is not possible
                // could be a connection problem
                TempData.Danger("Can't connect to reCAPTCHA servers");

                return Page();
            }
            else if (!captchaChallenge.Value)
            {
                TempData.Warning("You didn't pass the reCAPTCHA challenge!");
                return Page();
            }

            // if we get here, so far we passed the reCAPTCHA challenge :)
            TempData.Success($"Yeaaa! welcome {InputModel.Name}, you proved you are not a robot :)");

            return Page();
        }
    }
}
