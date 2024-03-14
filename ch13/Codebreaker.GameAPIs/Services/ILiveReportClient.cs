
namespace Codebreaker.GameAPIs.Services
{
    public interface ILiveReportClient
    {
        Task ReportGameEndedAsync(GameSummary1 game, CancellationToken cancellationToken = default);
    }
}