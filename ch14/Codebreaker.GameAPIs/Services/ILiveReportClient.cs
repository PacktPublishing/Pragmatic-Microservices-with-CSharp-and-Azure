
namespace Codebreaker.GameAPIs.Services
{
    public interface ILiveReportClient
    {
        Task ReportGameEndedAsync(GameSummary game, CancellationToken cancellationToken = default);
    }
}