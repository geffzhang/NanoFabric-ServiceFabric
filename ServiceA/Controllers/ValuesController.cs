using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly StatelessServiceContext context;

        private readonly IHttpContextAccessor accessor;

        public ValuesController(StatelessServiceContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var city = this.accessor.HttpContext.Request.Headers["claims_City"];
            var state = this.accessor.HttpContext.Request.Headers["claims_State"];

            return new string[] { "NodeName", context.NodeContext.NodeName ,city , state};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
