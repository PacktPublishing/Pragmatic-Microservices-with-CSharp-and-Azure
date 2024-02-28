
namespace Codebreaker.GameAPIs.Services
{
    public interface ILiveClient
    {
        Task ReportGameEndedAsync(Game game, CancellationToken cancellationToken = default);
    }
}