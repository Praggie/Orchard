using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Parameters;

namespace codeathon.connectors
{
    public class TwitterService : ITwitterService

    {
        public TwitterService()
        {
            TwitterCredentials tc = new TwitterCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
            Auth.SetUserCredentials(ConfigurationManager.AppSettings["APIKey"], ConfigurationManager.AppSettings["APISecret"], ConfigurationManager.AppSettings["AccessToken"], ConfigurationManager.AppSettings["AccessTokenSecret"]);
            Auth.ApplicationCredentials = tc;
        }
        public void SendTweet(string twitterUserId, string textToSend)
        {
            throw new NotImplementedException();
        }
        public void SendPrivateMessage(string twitterUserId, string textToSend)
        {
            var message = Message.PublishMessage(textToSend, twitterUserId);

        }
        public void ReplyToTweet(string tweet, string textToSend)
        {

            var publishParameter = new PublishTweetParameters("textToSend", new PublishTweetOptionalParameters
            {

            });
            var tweetpublished = Tweet.PublishTweetInReplyTo(textToSend, long.Parse(tweet));
        }

        public void GenerateOmbed(long tweetId) {
            
            var ombed = Tweet.GenerateOEmbedTweet(tweetId);
        }

    }

    public interface ITwitterService : IDependency
    {

        void SendTweet(string twitterUserId, string textToSend);

        void ReplyToTweet(string tweet, string textToSend);

        void SendPrivateMessage(string twitterUserId, string textToSend);




    }
}
