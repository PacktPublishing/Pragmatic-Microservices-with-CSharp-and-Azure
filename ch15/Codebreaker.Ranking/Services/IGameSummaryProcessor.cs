
namespace Codebreaker.Ranking.Services
{
    public interface IGameSummaryProcessor
    {
        Task StartProcessingAsync(CancellationToken cancellationToken = default);
        Task StopProcessingAsync(CancellationToken cancellationToken = default);
    }
}