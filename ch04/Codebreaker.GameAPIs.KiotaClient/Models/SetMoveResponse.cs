using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Codebreaker.Client.Models {
    public class SetMoveResponse : IParsable {
        /// <summary>The gameId property</summary>
        public Guid? GameId { get; set; }
        /// <summary>The gameType property</summary>
        public Codebreaker.Client.Models.GameType? GameType { get; set; }
        /// <summary>The moveNumber property</summary>
        public int? MoveNumber { get; set; }
        /// <summary>The results property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? Results { get; set; }
#nullable restore
#else
        public List<string> Results { get; set; }
#endif
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static SetMoveResponse CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new SetMoveResponse();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"gameId", n => { GameId = n.GetGuidValue(); } },
                {"gameType", n => { GameType = n.GetEnumValue<GameType>(); } },
                {"moveNumber", n => { MoveNumber = n.GetIntValue(); } },
                {"results", n => { Results = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteGuidValue("gameId", GameId);
            writer.WriteEnumValue<GameType>("gameType", GameType);
            writer.WriteIntValue("moveNumber", MoveNumber);
            writer.WriteCollectionOfPrimitiveValues<string>("results", Results);
        }
    }
}
