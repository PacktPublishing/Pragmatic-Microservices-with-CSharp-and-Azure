namespace Codebreaker.GameAPIs.Models;

public readonly partial struct SimpleColorResult : IEquatable<SimpleColorResult>
{
    public override bool Equals(object? obj)
    {
        if (obj is SimpleColorResult result)
        {
            return Equals(result);
        }
        return false;
    }

    public bool Equals(SimpleColorResult other) =>
        Results.SequenceEqual(other.Results);

    public override int GetHashCode() =>
        Results.GetHashCode();

    public static bool operator==(SimpleColorResult left, SimpleColorResult right) => left.Equals(right);

    public static bool operator!=(SimpleColorResult left, SimpleColorResult right) => !(left == right);
}
