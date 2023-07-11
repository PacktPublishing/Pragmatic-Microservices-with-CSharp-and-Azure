using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codebreaker.Data.Cosmos.Utilities;

internal class FieldValueComparer : ValueComparer<IDictionary<string, IEnumerable<string>>>
{
    public FieldValueComparer() : base(
        equalsExpression: (a, b) => a!.SequenceEqual(b!),
        hashCodeExpression: a => a.Aggregate(0, (result, next) => HashCode.Combine(result, next.GetHashCode())))
    {
        
    }
}
