using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using YOLO.CaseStudy.Business.Interfaces;
using YOLO.CaseStudy.Entities;

namespace YOLO.CaseStudy.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Produces("application/json")]
    public class CaseStudyController : ControllerBase
    {
        private readonly ICaseStudyBusiness _business;

        public CaseStudyController(ICaseStudyBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public Result ReverseText([FromQuery] WordProcessType processType = WordProcessType.ReverseCharacters)
        {
            var result = this._business.ReverseText(processType);

            return result;
        }

        [HttpGet]
        public async Task<Result> Iterator(CancellationToken cancellationToken)
        {
            var result = await this._business.Iterator(cancellationToken);

            return result;
        }

        [HttpGet]
        public async Task<Result> CalculateChecksum(CancellationToken cancellationToken)
        {
            var result = await this._business.CalculateChecksum(cancellationToken);

            return result;
        }

        [HttpGet]
        public async Task<Result> Assets(CancellationToken cancellationToken)
        {
            var result = await this._business.GetAssets(cancellationToken);

            return result;
        }
    }
}
