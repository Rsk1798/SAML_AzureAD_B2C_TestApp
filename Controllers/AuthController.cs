﻿using System.Security.Authentication;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SAML_AzureAD_B2C_TestApp.Controllers
{
    [AllowAnonymous]
    [Route("Auth")]
    public class AuthController : Controller
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration config;
        public AuthController(IOptions<Saml2Configuration> configAccessor)
        {
            config = configAccessor.Value;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            var binding = new Saml2RedirectBinding();
            // return binding.Bind(new Saml2AuthnRequest(config)).ToActionResult();
            var authnRequest = new Saml2AuthnRequest(config)
            {
                // Enforce SHA-256 in the request
                SignatureAlgorithm = config.SignatureAlgorithm
            };
            return binding.Bind(authnRequest).ToActionResult();

        }

        [Route("SamlResponse")]
        public async Task<IActionResult> SamlResponse()
        {
            var binding = new Saml2PostBinding();
            var saml2AuthnResponse = new Saml2AuthnResponse(config);
            try
            {
                binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);
                if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
                {
                    throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
                }

                // Validate SHA-256 signature
                binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
                // saml2AuthnResponse.Validate();
                await saml2AuthnResponse.CreateSession(HttpContext);
                /*return Redirect("https://localhost:44389");*/
                // return Redirect("https://samltestappb2c-gwgweka4aeg4aeau.westeurope-01.azurewebsites.net");
                // return Redirect("https://localhost:7242");
                return Redirect(config.Issuer);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }

        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                var binding = new Saml2PostBinding();
                var saml2LogoutRequest = new Saml2LogoutRequest(config);
                await HttpContext.SignOutAsync();
                // await HttpContext.Session.ClearAsync();
                HttpContext.Session.Clear();
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
