using Microsoft.Kiota.Abstractions.Serialization;
namespace Codebreaker.Client.Models;
public class CreateGameResponse : IParsable
{
    /// <summary>The fieldValues property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public CreateGameResponse_fieldValues? FieldValues { get; set; }
#nullable restore
#else
    public CreateGameResponse_fieldValues FieldValues { get; set; }
#endif
    /// <summary>The gameId property</summary>
    public Guid? GameId { get; set; }
    /// <summary>The gameType property</summary>
    public Codebreaker.Client.Models.GameType? GameType { get; set; }
    /// <summary>The playerName property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public string? PlayerName { get; set; }
#nullable restore
#else
    public string PlayerName { get; set; }
#endif
    /// <summary>
    /// Creates a new instance of the appropriate class based on discriminator value
    /// </summary>
    /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
    public static CreateGameResponse CreateFromDiscriminatorValue(IParseNode parseNode)
    {
        _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
        return new CreateGameResponse();
    }
    /// <summary>
    /// The deserialization information for the current model
    /// </summary>
    public IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
    {
        return new Dictionary<string, Action<IParseNode>> {
            {"fieldValues", n => { FieldValues = n.GetObjectValue<CreateGameResponse_fieldValues>(CreateGameResponse_fieldValues.CreateFromDiscriminatorValue); } },
            {"gameId", n => { GameId = n.GetGuidValue(); } },
            {"gameType", n => { GameType = n.GetEnumValue<GameType>(); } },
            {"playerName", n => { PlayerName = n.GetStringValue(); } },
        };
    }
    /// <summary>
    /// Serializes information the current object
    /// </summary>
    /// <param name="writer">Serialization writer to use to serialize this model</param>
    public void Serialize(ISerializationWriter writer)
    {
        _ = writer ?? throw new ArgumentNullException(nameof(writer));
        writer.WriteObjectValue<CreateGameResponse_fieldValues>("fieldValues", FieldValues);
        writer.WriteGuidValue("gameId", GameId);
        writer.WriteEnumValue<GameType>("gameType", GameType);
        writer.WriteStringValue("playerName", PlayerName);
    }
}
