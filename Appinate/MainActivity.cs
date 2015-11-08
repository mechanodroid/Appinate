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
				res = extractor.FindKeyPhrases(likeGameDataList[i].description);
				toReturn = toReturn + res [0] + " ";	
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
				if(checkbox.Checked)
					recommendation = CollectRecommendations();
				TextView text = FindViewById<TextView>(Resource.Id.autoCompleteTextView1);
				string Matters42=  "https://42matters.com/api/1/apps/search.json?q="+ text.Text + " "+ recommendation + "&limit=50&page=2&&access_token=8baf2a81c06ef3af38cd6ee3bbfee42f74e2497a";
				string strUri=  string.Format ( Matters42 );

				//Download string using webclient object
				var webclient = new WebClient ();
				string strResultData ="";
				try
				{ 
					strResultData=  webclient.DownloadString (new System.Uri(strUri));  
				}
				catch
				{ 
					Toast.MakeText ( this , "Unable to connect to server!!!" , ToastLength.Short ).Show (); 
				}
				finally
				{
					webclient.Dispose ();   //dispose webclient object
					//List<GameDataResults> result = JsonConvert.DeserializeObject<List<GameDataResults>>(strResultData);
					var jObj = (JObject)JsonConvert.DeserializeObject(strResultData);
					var result = jObj["results"]
						.Select(item => new GameData
							{
								promo_video = (string)item["promo_video"],
								description = (string)item["description"],
								icon = (string)item["icon"],
								title = (string)item["title"],
								market_url = (string)item["market_url"]
							})
						.ToList();
					gameDataList = result;
					webclient = null; 
					StartActivity(typeof(ResultsActivity));
				}

				// Create a basic search request
				/*HttpWebRequest httpWebRequest = SetHttpRequest("your api credentials", “your URL encoded http url for the API");
					Console.WriteLine("httpWebRequest = " + httpWebRequest.Address.AbsoluteUri);

					// Perform search
					try
					{
						HttpWebResponse httpRes = Search();
						if (httpRes != null)
						{
							var responseStream = httpRes.GetResponseStream();

						    var json = (JsonObject)JsonObject.Load(responseStream);

							var deserializedJsonRecords = JsonConvert.DeserializeObject<ProductItem>(json.ToString());

							var searchResults = new List<ProductItem>();

							searchResults = (
								from result in deserializedJsonRecords
								select new ProductItem {
									name = result.name,
									price = result.price, 
									category = result.category
								}
							).ToList();
						
						}
					} catch (Exception ex) {
						Console.WriteLine(ex.Message);
					}
				// ParseAndDisplay (json);
				*/
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


