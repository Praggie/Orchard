using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Logging;

namespace codeathon.connectors.Services {
    public class TweetMessageChannel : Component, ITweetRelay, IDisposable {
        public static readonly string MessageType = "Tweet";
        private ITwitterService _twitterService;
        public TweetMessageChannel(ITwitterService twitterService
            ) {
            _twitterService = twitterService;
        }

        public void Dispose() {
           
        }

        public void Process(IDictionary<string, object> parameters) {

            var tweet = new {
                TwitterUser = Read(parameters, "TwitterUser"),
                TextToSend = Read(parameters,"TextToSend"),
                InReplyToTweet = Read(parameters, "InReplyToTweet"),
                SendAsPM = parameters.ContainsKey("SendAsPM") && (bool) parameters["SendAsPM"]
            };

            if (String.IsNullOrWhiteSpace(tweet.TextToSend)) {
                Logger.Error("Tweet message doesn't have any text to send");
                return;
            }
           
            try {

                if (tweet.SendAsPM)
                {
                    _twitterService.SendPrivateMessage(tweet.TwitterUser, tweet.TextToSend);
                }
                else
                {
                    if (tweet.TwitterUser != "dumptyhumpty80")
                        _twitterService.ReplyToTweet(tweet.InReplyToTweet, tweet.TwitterUser + ' ' + tweet.TextToSend);
                }
            }
            catch (Exception e) {
                Logger.Error(e, "Could not send email");
            }
        }

        private string Read(IDictionary<string, object> dictionary, string key) {
            return dictionary.ContainsKey(key) ? dictionary[key] as string : null;
        }

        private IEnumerable<string> ParseRecipients(string recipients) {
            return recipients.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}