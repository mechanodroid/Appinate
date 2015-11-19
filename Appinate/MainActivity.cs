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
using Microsoft.WindowsAzure.MobileServices;

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
		public static List<GameData> likeCloudGameDataList{ get; set; }
		public static int numSeperateLists { get; set; }
		public static bool firstTime = true;
		public static string storagePath;
		public static string storageFile;
		public static string currentSelectedGamerType;

		public static MobileServiceClient MobileService = new MobileServiceClient(
			"https://appinateuwm.azure-mobile.net/",
			"vuFYEJOrEHRrAtKnDpaUrbcgfVGTMx66"
		);

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
		protected string CollectCloudRecommendations() { 
			string toReturn = "";
			KeywordExtractor extractor = new KeywordExtractor();
			string[] res = new string[100]; //<--magic numbers, ahh!, but I'll always only pick the first so its ok
			for (int j = 0; j < 100; j++)
				res [j] = "";
			//for entire like list, uses nrake and adds to the string the first term from each 
			//for now only take one of these! --> make it random I guess
			{
				Random rnd = new Random();
				int r = rnd.Next (0, likeCloudGameDataList.Count);
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
		private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

			string toast = string.Format ("The gamer type is is {0}", spinner.GetItemAtPosition (e.Position));
			Toast.MakeText (this, toast, ToastLength.Long).Show ();
			currentSelectedGamerType = string.Format ("{0}", spinner.GetItemAtPosition (e.Position));
		}
		protected override void OnCreate (Bundle bundle)
		{
			
			List<GameData> gameDataListTemp = new List<GameData>();
			List<GameData> likeCloudGameDataList = new List<GameData> ();

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

			Spinner spinner = FindViewById<Spinner> (Resource.Id.spinner1);

			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource (
				this, Resource.Array.gamer_type_array, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			Button buttonEditLikes = FindViewById<Button> (Resource.Id.button1);
			CheckBox checkbox2 = FindViewById<CheckBox> (Resource.Id.checkBox2);

			buttonEditLikes.Click += async (sender, e) => {
				if(!checkbox2.Checked)
				{//turn phase 3 completely off
					//MainActivity.currentSelectedGamerType="";
				}
				StartActivity (typeof(LikeActivity));
			};

			CheckBox checkbox = FindViewById<CheckBox> (Resource.Id.checkBox1);

			checkbox2.Click += async (sender, e) => {
				//Getting back the data:
				likeCloudGameDataList.Clear();
				if(currentSelectedGamerType!="")
				{
					switch(currentSelectedGamerType)
					{
					case "Casual":
						IMobileServiceTable<CasualGamer> casualTable =
							MobileService.GetTable<CasualGamer>();

						IMobileServiceTableQuery<CasualGamer> query = casualTable.CreateQuery().OrderByDescending(t => t.date);
						List<CasualGamer> games = await query.ToListAsync();
						foreach(GameData g in games.First().myList)
						{
							likeCloudGameDataList.Add(g);
						}
						break;
					case "Hardcore":
						IMobileServiceTable<HardcoreGamer> hardcoreTable =
							MobileService.GetTable<HardcoreGamer>();

						IMobileServiceTableQuery<HardcoreGamer> query2 = hardcoreTable.CreateQuery().OrderByDescending(t => t.date);
						List<HardcoreGamer> games2 = await query2.ToListAsync();
						foreach(GameData g in games2.First().myList)
						{
							likeCloudGameDataList.Add(g);
						}
						break;
					case "Puzzle":
						IMobileServiceTable<PuzzleGamer> puzzleTable =
							MobileService.GetTable<PuzzleGamer>();

						IMobileServiceTableQuery<PuzzleGamer> query3 = puzzleTable.CreateQuery().OrderByDescending(t => t.date);
						List<PuzzleGamer> games3 = await query3.ToListAsync();
						foreach(GameData g in games3.First().myList)
						{
							likeCloudGameDataList.Add(g);
						}
						break;
					case "Apps User":
						IMobileServiceTable<AppUser> appUserTable =
							MobileService.GetTable<AppUser>();

						IMobileServiceTableQuery<AppUser> query4 = appUserTable.CreateQuery().OrderByDescending(t => t.date);
						List<AppUser> games4 = await query4.ToListAsync();
						foreach(GameData g in games4.First().myList)
						{
							likeCloudGameDataList.Add(g);
						}
						break;

					case "Racing" : 
						IMobileServiceTable<RacingGamer> racingTable =
							MobileService.GetTable<RacingGamer>();

						IMobileServiceTableQuery<RacingGamer> query5 = racingTable.CreateQuery().OrderByDescending(t => t.date);
						List<RacingGamer> games5 = await query5.ToListAsync();
						foreach(GameData g in games5.First().myList)
						{
							likeCloudGameDataList.Add(g);
						}
						break;
					}
				}
			};


			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += async (sender, e) => {

				//The following is experimental
				//BEGIN EXPERIMENTAL
				//just an azure test for now
				CurrentPlatform.Init();

				//END EXPERIMENTAL
				
				//KeywordExtractor extractor = new KeywordExtractor();
				//var res = extractor.FindKeyPhrases(//TODO Get Description and pass in here);
				string recommendation = "";
				numSeperateLists = 1;
				int maxSize = 3;
				if(checkbox2.Checked)
				{	//we're using phase three: cloud recommendations
					maxSize = 4;
				}
				gameDataList.Clear();
				gameDataListTemp.Clear();
				string[] recommendations = new string[200];
				int sizeOfRecommendations;
				if (checkbox.Checked)
					recommendation = CollectRecommendations ();
				TextView text = FindViewById<TextView> (Resource.Id.autoCompleteTextView1);
				string apiUrl = "https://42matters.com/api/1/apps/query.json?access_token=8baf2a81c06ef3af38cd6ee3bbfee42f74e2497a";

				if(checkbox2.Checked)
				{
					//phase 3
					recommendation += CollectCloudRecommendations();
				}
				sizeOfRecommendations = this.TrimToStringArray(recommendation,ref recommendations);
				int currentRecommendationIndex = 0;
				int recommendationsToSearch = Math.Min(sizeOfRecommendations,maxSize);
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


