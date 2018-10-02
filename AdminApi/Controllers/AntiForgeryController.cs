using Microsoft.AspNetCore.Antiforgery;
using LTMCompanyNameFree.YoyoCmsTemplate.Controllers;

namespace AdminApi.Controllers
{
    public class AntiForgeryController : YoyoCmsTemplateControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
