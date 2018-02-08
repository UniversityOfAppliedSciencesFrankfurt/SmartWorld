namespace GetTweetsModule
{
    using System;
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Twitter{

        /// <summary>
        /// Get Tweets 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <param name="oauth"></param>
        public static string getTweetId(string name, int count, string oauth)
        {
            string tweetId = string.Empty;

            System.Console.WriteLine($"Getting latest Tweet in {nameof(getTweetId)}");

            var client = getHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", oauth);

            var res = client.GetAsync($"1.1/statuses/user_timeline.json?screen_name={name}&count={count}").Result;

            if (res.IsSuccessStatusCode)
            {
                var tweets = JsonConvert.DeserializeObject<JArray>(res.Content.ReadAsStringAsync().Result);

                foreach (var tweet in tweets)
                {
                    System.Console.WriteLine($"Time: {tweet["created_at"]}");
                    System.Console.WriteLine($"Tweet: {tweet["text"]}");
                    System.Console.WriteLine($"Tweet Id: {tweet["id"]}");

                    tweetId = tweet["id"].ToString();
                }
            }
            else
                System.Console.WriteLine(res.Content.ReadAsStringAsync().Result);

            return tweetId;
        }


        /// <summary>
        /// Get access token
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        public static string getToken(string credential)
        {
            string aToken = string.Empty;
            var client = getHttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credential);

            var body = new StringContent("grant_type=client_credentials");
            body.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var res = client.PostAsync("oauth2/token", body).Result;

            if (res.IsSuccessStatusCode)
            {
                System.Console.WriteLine($"Got access token successfully.");

                var jobject = JsonConvert.DeserializeObject<JObject>(res.Content.ReadAsStringAsync().Result);
                aToken = jobject["access_token"].Value<string>();
            }
            else
               System.Console.WriteLine(res.Content.ReadAsStringAsync().Result);

            return aToken;
        }


        /// <summary>
        /// Convert base 64 bit string with Twitter Consumer Key and Consumer Secret
        /// </summary>
        /// <param name="key">Twitter Consumer Key</param>
        /// <param name="secret">Twitter Consumer Secret</param>
        /// <returns></returns>
        public static string getCredential(string key, string secret)
        {
            var strCredential = $"{key}:{secret}";
            var bArray = Encoding.UTF8.GetBytes(strCredential);
            var base64Str = Convert.ToBase64String(bArray);
            return base64Str;
        }


        /// <summary>
        /// Http Client with base uri, https://api.twitter.com/
        /// </summary>
        /// <returns></returns>
        public static HttpClient getHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.twitter.com/");

            return client;
        }

    }
}