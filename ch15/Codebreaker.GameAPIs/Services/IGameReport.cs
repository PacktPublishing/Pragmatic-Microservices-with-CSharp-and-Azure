
namespace Codebreaker.GameAPIs.Services;

public interface IGameReport
{
    Task ReportGameEndedAsync(GameSummary game, CancellationToken cancellationToken = default);
}