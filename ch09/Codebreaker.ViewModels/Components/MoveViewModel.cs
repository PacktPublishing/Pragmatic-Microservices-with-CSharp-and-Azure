namespace Codebreaker.ViewModels.Components;

public class MoveViewModel(Move move)
{
    private readonly Move _move = move;

    public int MoveNumber => _move.MoveNumber;

    // TODO: read-only or updates?
    public IReadOnlyList<string> GuessPegs => _move.GuessPegs;

    // TODO: read-only or updates?
    public string[] KeyPegs => _move.KeyPegs.ToArray();
}
