using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Kwikly.Common;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using User = Kwikly.Common.User;
using Firebase.Auth;

namespace KwiklyBot
{
    public static class DashboardHelper
    {
        public static readonly Uri HumanCallback = new Uri("http://127.0.0.1:3978/api/HumanCallback");
        public static FirebaseClient _firebase = new FirebaseClient("https://kwikly-72df6.firebaseio.com");
        public static readonly string _channel = "travela";

        public static readonly string _uuid = "botId";
        private static Lazy<string> botId = new Lazy<string>(() => ConfigurationManager.AppSettings["MicrosoftAppId"]);
        private static string baseUri = "";

        static DashboardHelper()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (_firebase != null)
            {
              

                try
                {
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAe7djcdc6zs0LxwZDWZOzhAvwRnbo6qtQ"));
                    authProvider.SignInAnonymouslyAsync();

                    var observable = _firebase
                    .Child("messages")
                    .AsObservable<Msg>()
                    .Subscribe(ProcessBackendMsg);
                }
                catch (Exception)
                {
                    
                }
               
            }

          
        }

        private static void ProcessBackendMsg(FirebaseEvent<Msg> d)
        {
            //Dictionary<string, Msg> entryDict = JsonConvert.DeserializeObject<Dictionary<string, Msg>>(d.Object.ToString());

            var msg = d.Object;
            if(msg == null || string.IsNullOrEmpty(msg.message) || string.IsNullOrEmpty(msg.channelId) || string.IsNullOrEmpty(msg.conversationId) || string.IsNullOrEmpty(msg.UserId))
                return;
                try
            {
                //dynamic data = JsonConvert.DeserializeObject<dynamic>(d.Object.ToString());
                //var shorter = ((IDictionary<string, JToken>) data).Select(k =>
                //    JsonConvert.DeserializeObject<Msg>(k.Value.ToString())).ToList();
               // foreach (var msg in shorter)
                {
                    try
                    {
                        if (msg.who != "user" && msg.who != "bot")
                            ProcessReceivedMessage(msg);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
               Console.WriteLine("Error in processing message :" + e.Message);
            }


        }

        static void DisplayReturnMessage(string result)
        {
            Console.WriteLine("PUBLISH STATUS CALLBACK");
            Console.WriteLine(result);
        }

        public static async Task<string> SendBotReplyMessageToDashboard(Activity reply, ResumptionCookie resumptionCookie, bool isQuery = false, bool isBotReplied = true)
        {
            // await context.PostAsync(message);
            
            reply.ChannelId = resumptionCookie.ChannelId;
            reply.ServiceUrl = resumptionCookie.ServiceUrl;

            var request = CreateDashBoardRequest("", resumptionCookie, HumanCallback.ToString(), reply, isQuery, isBotReplied);

            await PushMessageToDashboardFirebase<Msg>(request);
            return "message sent";
        }

        public static async Task<Activity> SendMessageToDashboard(Activity message)
        {
            {
                try
                {
                    var resummptionCookie = new ResumptionCookie(message);
                        baseUri = message.ServiceUrl;

                    var request = CreateDashBoardRequest(message.From.Name, resummptionCookie, HumanCallback.ToString(),
                        message);

                    var user = new Kwikly.Common.User();
                    user.name = message.From.Name;
                    user.Id = message.From.Id;
                    user.avatar = "assets/images/avatars/alice.jpg";
                    //user.Address = message.From.Address;

                    await SaveUserInFirebase<string>(user);

                    var conv = Createconversation(resummptionCookie, request, user, message);
                    await PushConversationDetailsToDashboardFirebase<string>(conv);

                    await PushMessageToDashboardFirebase<string>(request);
                }
                catch (Exception exception)
                {
                    
                }

                return message.CreateReply("Query Forwarded to dashboard");

            }
            /* else
             {
                 context.Done(token);
             }*/
        }


        private static async Task<User> GetUser(string userid)
        {
            var user = await _firebase.Child("users").Child(userid).OnceSingleAsync<User>();
            return user;
        }

        private static Conv Createconversation(ResumptionCookie resumptionCookie, Msg msg, Kwikly.Common.User user, Activity activity)
        {
            var conv = new Conv();
            conv.conversationId = resumptionCookie.ConversationId;
            conv.channelId = resumptionCookie.ChannelId;
            conv.avatar = user.avatar;
            conv.lastMessage = msg;
            conv.timestamp = DateTime.UtcNow;
            conv.name = activity.Conversation.Name ?? user.name;
            conv.recipientId = user.Id;
            conv.botId = activity.Recipient.Id;
            conv.serviceurl = activity.ServiceUrl;
            return conv;
        }

        private static async Task<T> SaveUserInFirebase<T>(Kwikly.Common.User user)
        {
            var convDetails = _firebase.Child("users").Child(user.Id);

            await convDetails.PutAsync(user);
         
            return default(T);
        }
        private static async Task<T> PushConversationDetailsToDashboardFirebase<T>(Conv conv)
        {
            var convDetails = _firebase.Child("conversations").Child(conv.conversationId);
            
            await convDetails.PutAsync(conv);

            return default(T);

        }

        private static async Task<T> PushMessageToDashboardFirebase<T>(Msg request)
        {
        
         try
            {
                var conversation = _firebase.Child("messages").Child(request.conversationId);

                if (conversation != null)
                {
                    await conversation.Child(FirebaseKeyGenerator.Next()).PutAsync(request);
                }
            }
            catch (Exception)
            {
                
            }

            return default(T);

        }
        

        private static Uri GetUri(string endPoint, params Tuple<string, string>[] queryParams)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var queryparam in queryParams)
            {
                queryString[queryparam.Item1] = queryparam.Item2;
            }

            var builder = new UriBuilder(endPoint);
            builder.Query = queryString.ToString();
            return builder.Uri;
        }

        private static Msg CreateDashBoardRequest(string nameOfUser, ResumptionCookie resumptionCookie, string facebookOauthCallback, Activity message,  bool isQuery = false, bool isBotReply = false)
        {
            var botOwnerId = ConfigurationManager.AppSettings["CompanyId"];
            var redirectUri = GetHumanInteractionCallBackUri(resumptionCookie, facebookOauthCallback);


            Msg requestMsg = new Msg();
            requestMsg.Id = message.Id;
            requestMsg.UserId = String.IsNullOrEmpty(nameOfUser) ? resumptionCookie.BotId : resumptionCookie.UserId;
            string messageFrom = String.IsNullOrEmpty(nameOfUser) ? "bot" : "user";
            requestMsg.message = message.Text;
            requestMsg.who = messageFrom;
            requestMsg.conversationId = resumptionCookie.ConversationId;
            requestMsg.channelId = resumptionCookie.ChannelId;
            requestMsg.time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            requestMsg.Attachments = message.Attachments;
            return requestMsg;
        }

        private static string GetHumanInteractionCallBackUri(ResumptionCookie resumptionCookie,
            string humaninteractionCallback)
        {
            // because of a limitation on the characters in Facebook redirect_uri, we don't use the serialization of the cookie.
            var uri = GetUri(humaninteractionCallback,
                Tuple.Create("userId", resumptionCookie.UserId),
                Tuple.Create("conversationId", resumptionCookie.ConversationId),
                Tuple.Create("channelId", resumptionCookie.ChannelId)
                );
            return uri.ToString();
        }


        public static async void ProcessReceivedMessage(Msg receivedMsg)
        {
            // Get the resumption cookie

            if (string.IsNullOrEmpty(receivedMsg.message) || string.IsNullOrEmpty(receivedMsg.UserId) || string.IsNullOrEmpty(receivedMsg.conversationId) ||
                string.IsNullOrEmpty(receivedMsg.channelId))
                return;
            try
            {

                {
                    var resumptionCookie = new ResumptionCookie(receivedMsg.UserId, botId.Value, receivedMsg.conversationId, receivedMsg.channelId, baseUri);

                    var msg = resumptionCookie.GetMessage();
                    msg.Text = receivedMsg.message;
                    msg.ServiceUrl = receivedMsg.serviceurl;
              
                    if ( string.IsNullOrEmpty(receivedMsg.message))
                        return;

                    if (receivedMsg.message == "syspause" || receivedMsg.message == "sysresume")
                    {
                        try
                        {
                            var stateClient = ((Activity) msg).GetStateClient();
                            BotState botState = new BotState(stateClient);
                            BotData botData = new BotData(eTag: "*");
                            if (receivedMsg.message == "syspause")
                                botData.SetProperty<bool>("isConversationPaused", true);
                            else if (receivedMsg.message == "sysresume")
                            {
                                botData.SetProperty<bool>("isConversationPaused", false);
                            }
                            BotData response =
                                await
                                    stateClient.BotState.SetConversationDataAsync(msg.ChannelId, msg.Conversation.Id,
                                        botData);

                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Failed to save the Resumption Cookie");
                        }
                    }
                    else
                    {
                        var reply = ((Activity) msg).CreateReply(receivedMsg.message);
                        reply.Text = receivedMsg.message;
                        reply.ChannelData = "FromDashboard";
                        // Send the login success asynchronously to user
                        var connector = new ConnectorClient(new Uri(msg.ServiceUrl));
                        try
                        {
                            await connector.Conversations.SendToConversationAsync((Activity) reply);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

        }
    }
}