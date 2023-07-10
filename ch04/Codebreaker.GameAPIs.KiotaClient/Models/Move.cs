using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Codebreaker.Client.Models {
    public class Move : IParsable {
        /// <summary>The guessPegs property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? GuessPegs { get; set; }
#nullable restore
#else
        public List<string> GuessPegs { get; set; }
#endif
        /// <summary>The keyPegs property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? KeyPegs { get; set; }
#nullable restore
#else
        public List<string> KeyPegs { get; set; }
#endif
        /// <summary>The moveId property</summary>
        public Guid? MoveId { get; set; }
        /// <summary>The moveNumber property</summary>
        public int? MoveNumber { get; set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static Move CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new Move();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"guessPegs", n => { GuessPegs = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
                {"keyPegs", n => { KeyPegs = n.GetCollectionOfPrimitiveValues<string>()?.ToList(); } },
                {"moveId", n => { MoveId = n.GetGuidValue(); } },
                {"moveNumber", n => { MoveNumber = n.GetIntValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteCollectionOfPrimitiveValues<string>("guessPegs", GuessPegs);
            writer.WriteCollectionOfPrimitiveValues<string>("keyPegs", KeyPegs);
            writer.WriteGuidValue("moveId", MoveId);
            writer.WriteIntValue("moveNumber", MoveNumber);
        }
    }
}
