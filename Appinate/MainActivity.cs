using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using NRakeCore;
using ModernHttpClient;
using System.Net.Http;
using System.Text;

namespace Appinate
{
	[Activity (Label = "Appinate", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
		HttpWebRequest _httpRequest;
		HttpWebResponse _httpResponse;

		public static List<GameData> gameDataList{ get; set; }
		public static List<GameData> likeGameDataList{ get; set; }
		public static bool firstTime = true;

		protected string CollectRecommendations() { 
			string toReturn = "";
			KeywordExtractor extractor = new KeywordExtractor();
			string[] res = new string[100]; //<--magic numbers, ahh!, but I'll always only pick the first so its ok
			for (int j = 0; j < 100; j++)
				res [j] = "";
			//for entire like list, uses nrake and adds to the string the first term from each 
			for (int i = 0; i < likeGameDataList.Count; i++) {
				//for now only take one of these!
				if (i == 0) {
					res = extractor.FindKeyPhrases (likeGameDataList [i].description);
					toReturn = toReturn + res [0] + " ";
				}
			}
			return toReturn;
		}
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			if(firstTime)
				likeGameDataList = new List<GameData> ();
			firstTime = false;
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			Button buttonEditLikes = FindViewById<Button> (Resource.Id.button1);

			buttonEditLikes.Click += async (sender, e) => {
				StartActivity (typeof(LikeActivity));
			};
				

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += async (sender, e) => {

				//KeywordExtractor extractor = new KeywordExtractor();
				//var res = extractor.FindKeyPhrases(//TODO Get Description and pass in here);
				string recommendation = "";
				CheckBox checkbox = FindViewById<CheckBox> (Resource.Id.checkBox1);
				if (checkbox.Checked)
					recommendation = CollectRecommendations ();
				TextView text = FindViewById<TextView> (Resource.Id.autoCompleteTextView1);
				string apiUrl = "https://42matters.com/api/1/apps/query.json?access_token=8baf2a81c06ef3af38cd6ee3bbfee42f74e2497a";

				Query data = new Query();
				InnerQuery iq = new InnerQuery();
				QueryParms qp = new QueryParms();
				qp.sort = "number_ratings";
				qp.cat_int = "2";
				qp.from = 0;
				qp.num = 100;
				qp.sort_order = "desc";
				qp.full_text_term = text.Text + " "+recommendation;//the actual search term
				qp.include_full_text_desc = true;
				iq.platform = "android";
				iq.query_params = qp;
				data.query = iq;

				// Serialize our concrete class into a JSON String
				var stringPayload = JsonConvert.SerializeObject(data);

				// Wrap our JSON inside a StringContent which then can be used by the HttpClient class
				var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

				using (var httpClient = new HttpClient()) {

					// Do the actual request and await the response
					var httpResponse = await httpClient.PostAsync(apiUrl, httpContent);

					// If the response contains content we want to read it!
					if (httpResponse.Content != null) {
						var responseContent = await httpResponse.Content.ReadAsStringAsync();
						var jObj = (JObject)JsonConvert.DeserializeObject (responseContent);
						var result = jObj ["results"]
							.Select (item => new GameData {
								promo_video = (string)item ["promo_video"],
								description = (string)item ["description"],
								icon = (string)item ["icon"],
								title = (string)item ["title"],
								market_url = (string)item ["market_url"]
							})
							.ToList ();
						gameDataList = result;
						//webclient = null; 
						StartActivity (typeof(ResultsActivity));
						// From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
					}
				}

	


			};

		}
		private string UrlEncodeParameter(string paramToEncode)
		{
			string urlEncodedParam = string.Empty;

			// remove whitespace from search parameter and URL encode it
			urlEncodedParam = paramToEncode.Trim();
			urlEncodedParam = Uri.EscapeDataString(urlEncodedParam);

			return urlEncodedParam;
		}

		private HttpWebRequest SetHttpRequest(string apiCredentials, string SearchRequest)
		{
			_httpRequest = (HttpWebRequest)null;

			if (string.IsNullOrEmpty(SearchRequest))
				throw new ArgumentNullException(string.Format("A search request is required by the Search method"));

			// Attempt to establish http request
			try
			{
				_httpRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(SearchRequest));
				_httpRequest.Credentials = new NetworkCredential (apiCredentials, apiCredentials);
			} catch (Exception ex) {
				string message = string.Format("Error creating request.  Check inner details for more info: {0}", SearchRequest);
				var requestException = new ApplicationException(message, ex);
				throw requestException;
			}

			return _httpRequest;
		}


		public HttpWebResponse Search()
		{
			// Attempt to get http response
			HttpWebResponse response = (HttpWebResponse)null;
			try
			{
				response = _httpResponse;//(HttpWebResponse)apiSearchParameters.HttpRequest.GetResponse();
				if (response.StatusCode != HttpStatusCode.OK) {
					throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode,
						response.StatusDescription));
				}
				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return response;
			}
		}

	}

}


