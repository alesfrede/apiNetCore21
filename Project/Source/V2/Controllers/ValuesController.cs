using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Api213.V2.Controllers
{
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] { "value1", "value2" };
        }
    }
}
