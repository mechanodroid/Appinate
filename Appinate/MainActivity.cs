using System;
using System.Text.RegularExpressions;
using System.IO;
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
		public static int numSeperateLists { get; set; }
		public static bool firstTime = true;
		public static string storagePath;
		public static string storageFile;

		protected string CollectRecommendations() { 
			string toReturn = "";
			KeywordExtractor extractor = new KeywordExtractor();
			string[] res = new string[100]; //<--magic numbers, ahh!, but I'll always only pick the first so its ok
			for (int j = 0; j < 100; j++)
				res [j] = "";
			//for entire like list, uses nrake and adds to the string the first term from each 
			//for now only take one of these! --> make it random I guess
			{
				Random rnd = new Random();
				int r = rnd.Next (0, likeGameDataList.Count);
				res = extractor.FindKeyPhrases (likeGameDataList [r].description);
				res[0] = likeGameDataList [r].title;
				toReturn = toReturn + res [0] + " ";
			}
			return toReturn;
		}
		protected int TrimToStringArray(string completeString, ref string[] theArray)
		{
			string input = completeString;
			string pattern = " ";            // Split on spaces
			int countSpaces = completeString.Count(Char.IsWhiteSpace);
			theArray = Regex.Split(input, pattern);
			return countSpaces+1;

		}
		protected void pruneGameDataListWithLikeList()
		{
			List<GameData> removeList = new List<GameData>();
			foreach (GameData g in gameDataList) 
			{
				GameData findResult = likeGameDataList.Find (x => x.title == g.title);
				if (findResult != null) {
					removeList.Add (g);
				}
			}
			foreach (GameData g in removeList) {
				gameDataList.Remove (g);
			}
		}
		protected override void OnCreate (Bundle bundle)
		{
			List<GameData> gameDataListTemp = new List<GameData>();

			//read in like list
			storagePath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments);
			storageFile = storagePath + "/appinate.json";
			if (File.Exists (storageFile)) {
				string likeString = File.ReadAllText (storageFile);
				likeGameDataList = JsonConvert.DeserializeObject<List<GameData>> (likeString);
				gameDataList = new List<GameData> ();
				firstTime = false;
			}

			base.OnCreate (bundle);
			if (firstTime) {
				likeGameDataList = new List<GameData> ();
				gameDataList = new List<GameData> ();
			}
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
				numSeperateLists = 1;
				gameDataList.Clear();
				gameDataListTemp.Clear();
				string[] recommendations = new string[200];
				int sizeOfRecommendations;
				CheckBox checkbox = FindViewById<CheckBox> (Resource.Id.checkBox1);
				if (checkbox.Checked)
					recommendation = CollectRecommendations ();
				TextView text = FindViewById<TextView> (Resource.Id.autoCompleteTextView1);
				string apiUrl = "https://42matters.com/api/1/apps/query.json?access_token=8baf2a81c06ef3af38cd6ee3bbfee42f74e2497a";

				sizeOfRecommendations = this.TrimToStringArray(recommendation,ref recommendations);
				int currentRecommendationIndex = 0;
				int recommendationsToSearch = Math.Min(sizeOfRecommendations,3);
				string searchTerm = text.Text;
				do
				{
					Query data = new Query();
					InnerQuery iq = new InnerQuery();
					QueryParms qp = new QueryParms();
					qp.sort = "number_ratings";
					qp.from = 0;
					qp.num = 100;
					qp.sort_order = "desc";
					qp.full_text_term = searchTerm;//the actual search term
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
							gameDataListTemp = result;
							int index = 0;
							foreach(GameData g in gameDataListTemp)
							{
								gameDataList.Insert(index*numSeperateLists, g);
								index++;
							}
							//webclient = null; 
							StartActivity (typeof(ResultsActivity));
							// From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
						}
					}
					if(currentRecommendationIndex<recommendations.Length)
						searchTerm = recommendations[currentRecommendationIndex];
					else 
						currentRecommendationIndex = recommendationsToSearch; //skip to end
					currentRecommendationIndex++;
					numSeperateLists++;
				} while (currentRecommendationIndex < recommendationsToSearch);
				pruneGameDataListWithLikeList();//remove games which are already in users like list!
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


