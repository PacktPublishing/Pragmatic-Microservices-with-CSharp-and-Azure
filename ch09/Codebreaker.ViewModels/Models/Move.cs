namespace Codebreaker.ViewModels.Models;

public partial class Move(Guid moveId, int moveNumber) : ObservableObject
{
    public Guid MoveId { get; private set; } = moveId;

    /// <summary>
    /// The move number for this move within the associated game.
    /// </summary>
    [ObservableProperty]
    private int _moveNumber = moveNumber;

    /// <summary>
    /// The guess pegs from the user for this move.
    /// </summary>
    public ObservableCollection<string> GuessPegs { get; } = [];

    /// <summary>
    /// The result from the analyer for this move based on the associated game that contains the move.
    /// </summary>
    public ObservableCollection<string> KeyPegs { get; } = [];

    public override string ToString() => $"{MoveNumber}. " +
        $"{string.Join('#', GuessPegs)} : " +
        $"{string.Join('#', KeyPegs)}";
}
