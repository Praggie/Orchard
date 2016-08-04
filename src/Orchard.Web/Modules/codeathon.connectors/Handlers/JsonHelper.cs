//    ---------------------------------------------------------------------------------------
//     $HeadURL: http://svn.fid-intl.com:8080/svn/dev/crd-screens/trunk/Fidelity.CharlesRiver.Common/Serialization/JsonHelper.cs $
//     $Date: 2015-07-28 07:55:40 +0100 (Tue, 28 Jul 2015) $
//     $Revision: 3787 $
//     $LastChangedBy: a527546 $
//     Description: JSON Serialize / Deserialize helper
//    ---------------------------------------------------------------------------------------

namespace codeathon.connectors.Handlers
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// JSON helper for serialization and de-serialization. 
    /// </summary>
    public class JsonHelper 
    {
        /// <summary>
        /// The settings
        /// </summary>
        private static readonly DataContractJsonSerializerSettings Settings = new DataContractJsonSerializerSettings
        {
            UseSimpleDictionaryFormat = true,
            DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ssZ")
        };

        /// <summary>
        /// Serializes the specified source object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObject">The source object.</param>
        /// <returns>String content.</returns>
        public string Serialize<T>(T sourceObject)
        {
            using (var stream = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(T), Settings);

                ser.WriteObject(stream, sourceObject);
                stream.Position = 0;
                using (var streamReader = new StreamReader(stream))
                {
                    var jsonMsg = streamReader.ReadToEnd();
                    streamReader.Close();
                    stream.Close();
                    return jsonMsg;
                }
            }
        }

        /// <summary>
        /// Deserialize the specified JSON text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonText">The JSON text.</param>
        /// <returns></returns>
        public T Deserialize<T>(string jsonText)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonText);
            using (var stream = new MemoryStream(bytes))
            {    
                var ser = new DataContractJsonSerializer(typeof(T), Settings);
                var deserializerObject = (T)ser.ReadObject(stream);
                return deserializerObject;
            }
        }
    }
}
