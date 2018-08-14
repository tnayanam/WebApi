using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwitterWebClient.Controllers
{
    public class Tweets
    {
        public Tweet[] resuls;
    }
    public class Tweet
    {
        [JsonProperty("from_user")] //  this name is in json response..
        public string UserName { get; set; }
        public string TweetText { get; set; }
    }

}
// if you are trying to make an api call and you want the data name to be different from the
// one that is coming from reponse then you need to apply jsonproperty in them.
// check the screenshot
