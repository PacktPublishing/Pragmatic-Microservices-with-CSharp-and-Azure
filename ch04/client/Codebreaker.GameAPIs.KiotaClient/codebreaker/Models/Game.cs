using Microsoft.Kiota.Abstractions.Serialization;
namespace Codebreaker.Client.Models;
public class Game : IParsable
{
    /// <summary>The codes property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public List<string>? Codes { get; set; }
#nullable restore
#else
    public List<string> Codes { get; set; }
#endif
    /// <summary>The duration property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public TimeSpanObject? Duration { get; set; }
#nullable restore
#else
    public TimeSpanObject Duration { get; set; }
#endif
    /// <summary>The endTime property</summary>
    public DateTimeOffset? EndTime { get; set; }
    /// <summary>The fieldValues property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public Game_fieldValues? FieldValues { get; set; }
#nullable restore
#else
    public Game_fieldValues FieldValues { get; set; }
#endif
    /// <summary>The gameId property</summary>
    public Guid? GameId { get; set; }
    /// <summary>The gameType property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public string? GameType { get; set; }
#nullable restore
#else
    public string GameType { get; set; }
#endif
    /// <summary>The isVictory property</summary>
    public bool? IsVictory { get; set; }
    /// <summary>The lastMoveNumber property</summary>
    public int? LastMoveNumber { get; set; }
    /// <summary>The maxMoves property</summary>
    public int? MaxMoves { get; set; }
    /// <summary>The moves property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public List<Move>? Moves { get; private set; }
#nullable restore
#else
    public List<Move> Moves { get; private set; }
#endif
    /// <summary>The numberCodes property</summary>
    public int? NumberCodes { get; set; }
    /// <summary>The playerIsAuthenticated property</summary>
    public bool? PlayerIsAuthenticated { get; set; }
    /// <summary>The playerName property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public string? PlayerName { get; set; }
#nullable restore
#else
    public string PlayerName { get; set; }
#endif
    /// <summary>The startTime property</summary>
    public DateTimeOffset? StartTime { get; set; }
    /// <summary>
    /// Creates a new instance of the appropriate class based on discriminator value
    /// </summary>
    /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
    public static Game CreateFromDiscriminatorValue(IParseNode parseNode)
    {
        _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
        return new Game();
    }
    /// <summary>
    /// The deserialization information for the current model
    /// </summary>
    public IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
    {
        return new Dictionary<string, Action<IParseNode>> {
            {"codes", n => { Codes = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
            {"duration", n => { Duration = n.GetObjectValue<TimeSpanObject>(TimeSpanObject.CreateFromDiscriminatorValue); } },
            {"endTime", n => { EndTime = n.GetDateTimeOffsetValue(); } },
            {"fieldValues", n => { FieldValues = n.GetObjectValue<Game_fieldValues>(Game_fieldValues.CreateFromDiscriminatorValue); } },
            {"gameId", n => { GameId = n.GetGuidValue(); } },
            {"gameType", n => { GameType = n.GetStringValue(); } },
            {"isVictory", n => { IsVictory = n.GetBoolValue(); } },
            {"lastMoveNumber", n => { LastMoveNumber = n.GetIntValue(); } },
            {"maxMoves", n => { MaxMoves = n.GetIntValue(); } },
            {"moves", n => { Moves = n.GetCollectionOfObjectValues<Move>(Move.CreateFromDiscriminatorValue)?.ToList(); } },
            {"numberCodes", n => { NumberCodes = n.GetIntValue(); } },
            {"playerIsAuthenticated", n => { PlayerIsAuthenticated = n.GetBoolValue(); } },
            {"playerName", n => { PlayerName = n.GetStringValue(); } },
            {"startTime", n => { StartTime = n.GetDateTimeOffsetValue(); } },
        };
    }
    /// <summary>
    /// Serializes information the current object
    /// </summary>
    /// <param name="writer">Serialization writer to use to serialize this model</param>
    public void Serialize(ISerializationWriter writer)
    {
        _ = writer ?? throw new ArgumentNullException(nameof(writer));
        writer.WriteCollectionOfPrimitiveValues<string>("codes", Codes);
        writer.WriteObjectValue<TimeSpanObject>("duration", Duration);
        writer.WriteDateTimeOffsetValue("endTime", EndTime);
        writer.WriteObjectValue<Game_fieldValues>("fieldValues", FieldValues);
        writer.WriteGuidValue("gameId", GameId);
        writer.WriteStringValue("gameType", GameType);
        writer.WriteBoolValue("isVictory", IsVictory);
        writer.WriteIntValue("lastMoveNumber", LastMoveNumber);
        writer.WriteIntValue("maxMoves", MaxMoves);
        writer.WriteIntValue("numberCodes", NumberCodes);
        writer.WriteBoolValue("playerIsAuthenticated", PlayerIsAuthenticated);
        writer.WriteStringValue("playerName", PlayerName);
        writer.WriteDateTimeOffsetValue("startTime", StartTime);
    }
}
