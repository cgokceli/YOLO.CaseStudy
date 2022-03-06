using System.Threading;
using System.Threading.Tasks;
using YOLO.CaseStudy.Entities;

namespace YOLO.CaseStudy.Business.Interfaces
{
    public interface ICaseStudyBusiness
    {
        Result ReverseText(WordProcessType processType);
        Task<Result> Iterator(CancellationToken cancellationToken);
        Task<Result> CalculateChecksum(CancellationToken cancellationToken);
        Task<Result> GetAssets(CancellationToken cancellationToken);
    }
}
