//---------------------------------------------------------------------------------------
// $HeadURL: http://svn.fid-intl.com:8080/svn/dev/crd-screens/trunk/Fidelity.CharlesRiver.Common/RestService/RestClientService.cs $
// $Date: 2015-08-04 10:56:17 +0100 (Tue, 04 Aug 2015) $
// $Revision: 3827 $
// $LastChangedBy: a488475 $
// Description: Rest Client Service Implementation   
//---------------------------------------------------------------------------------------
namespace codeathon.connectors.Handlers
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// REST client service.
    /// This class is used to send web request to CRIL services.
    /// </summary>
    public class RestClientService
    {
        /// <summary>
        /// The CRIL application
        /// </summary>
        private const string ApiUrl = "http://twiliosmsreceiver.azurewebsites.net/api/values";
        
        private const string ApiQueryUrl = "http://twiliosmsreceiver.azurewebsites.net/api/values/{0}";

        /// <summary>
        /// The serialize helper.
        /// </summary>
        private readonly JsonHelper serializeHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientService"/> class.
        /// </summary>
        public RestClientService()
        {
            this.serializeHelper = new JsonHelper();
        }

        /// <summary>
        /// Sends the asynchronous request to CRIL.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="endPoint">
        /// The end point.
        /// </param>
        /// <param name="payload">
        /// The payload.
        /// </param>
        /// <param name="httpMethod">
        /// The HTTP method.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<JsonResponse> GetResponseAsync()
        {
            var response = default(JsonResponse);

            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse = await httpClient.GetAsync(ApiUrl);

                    if (httpResponse.IsSuccessStatusCode && httpResponse.Content != null)
                    {
                        var responseText = await httpResponse.Content.ReadAsStringAsync();
                        var responseMessages = this.serializeHelper.Deserialize<Message[]>(responseText);
                        response = new JsonResponse(responseMessages);
                    }
                }
            }

            return response;
        }

        public async Task<JsonResponse> GetResponseAsync(int lastIndex)
        {
            var response = default(JsonResponse);

            {
                using (var httpClient = new HttpClient())
                {
                    var httpResponse = await httpClient.GetAsync(string.Format(ApiUrl, lastIndex));

                    if (httpResponse.IsSuccessStatusCode && httpResponse.Content != null)
                    {
                        var responseText = await httpResponse.Content.ReadAsStringAsync();
                        var responseMessages = this.serializeHelper.Deserialize<Message[]>(responseText);
                        response = new JsonResponse(responseMessages);
                    }
                }
            }

            return response;
        }
    }
}