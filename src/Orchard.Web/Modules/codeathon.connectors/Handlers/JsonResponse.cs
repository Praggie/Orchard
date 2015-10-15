// ---------------------------------------------------------------------------------------
//  $HeadURL: $
//  $Date: $
//  $Revision: $
//  $LastChangedBy: $
// ---------------------------------------------------------------------------------------
namespace codeathon.connectors.Handlers
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The JSON response.
    /// </summary>
    [DataContract]
    public class JsonResponse
    {
        public JsonResponse(Message[] responseMessages)
        {
            // TODO: Complete member initialization
            this.Messages = responseMessages;
        }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public Message[] Messages { get; private set; }
    }

    /// <summary>
    /// The message.
    /// </summary>
    [DataContract]
    public class Message
    {
        /// <summary>
        /// Gets or sets the message sid.
        /// </summary>
        [DataMember(Name = "MessageSid")]
        public string MessageSid { get; set; }

        /// <summary>
        /// Gets or sets the from.
        /// </summary>
        [DataMember(Name = "From")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the to.
        /// </summary>
        [DataMember(Name = "To")]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [DataMember(Name = "Body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the message status.
        /// </summary>
        [DataMember(Name = "MessageStatus")]
        public string MessageStatus { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember(Name = "ErrorCode")]
        public object ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the from city.
        /// </summary>
        [DataMember(Name = "FromCity")]
        public string FromCity { get; set; }

        /// <summary>
        /// Gets or sets the from state.
        /// </summary>
        [DataMember(Name = "FromState")]
        public string FromState { get; set; }

        /// <summary>
        /// Gets or sets the from zip.
        /// </summary>
        [DataMember(Name = "FromZip")]
        public string FromZip { get; set; }

        /// <summary>
        /// Gets or sets the from country.
        /// </summary>
        [DataMember(Name = "FromCountry")]
        public string FromCountry { get; set; }

        /// <summary>
        /// Gets or sets the to city.
        /// </summary>
        [DataMember(Name = "ToCity")]
        public string ToCity { get; set; }

        /// <summary>
        /// Gets or sets the to state.
        /// </summary>
        [DataMember(Name = "ToState")]
        public string ToState { get; set; }

        /// <summary>
        /// Gets or sets the to zip.
        /// </summary>
        [DataMember(Name = "ToZip")]
        public string ToZip { get; set; }

        /// <summary>
        /// Gets or sets the to country.
        /// </summary>
        [DataMember(Name = "ToCountry")]
        public string ToCountry { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        [DataMember(Name = "Direction")]
        public string Direction { get; set; }
    }
}