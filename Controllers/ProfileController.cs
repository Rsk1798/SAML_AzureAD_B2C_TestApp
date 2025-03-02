using ITfoxtec.Identity.Saml2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SAML_AzureAD_B2C_TestApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private readonly Saml2Configuration config;
        private readonly IConfiguration _configuration;


        public ProfileController(IOptions<Saml2Configuration> configAccessor, IConfiguration configuration)
        {
            config = configAccessor.Value;
            _configuration = configuration;
        }


        public IActionResult Index()
        {
            var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            return View(claims);
        }


        [HttpGet]
        public IActionResult EditProfile()
        {
            var b2cEditProfileUrl = "https://hcliamtrainingb2c.b2clogin.com/hcliamtrainingb2c.onmicrosoft.com/B2C_1A_Raja_SAML2_signup_signin/oauth2/v2.0/authorize";
            return Redirect(b2cEditProfileUrl);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var b2cResetPasswordUrl = "https://hcliamtrainingb2c.b2clogin.com/hcliamtrainingb2c.onmicrosoft.com/B2C_1A_Raja_SAML2_signup_signin/oauth2/v2.0/authorize?p=B2C_1A_PasswordReset";
            return Redirect(b2cResetPasswordUrl);
        }


    }
}
