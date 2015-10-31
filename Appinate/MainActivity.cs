using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using Newtonsoft.Json;
namespace Appinate
{
	[Activity (Label = "Appinate", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
		HttpWebRequest _httpRequest;
		HttpWebResponse _httpResponse;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);


			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += async (sender, e) => {

				const string Matters42=  "https://42matters.com/api/1/apps/search.json?q=racing+games&limit=50&page=2&&access_token=8baf2a81c06ef3af38cd6ee3bbfee42f74e2497a";
				string strUri=  string.Format ( Matters42 );

				//Download string using webclient object
				var webclient = new WebClient ();
				string strResultData;
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
					webclient = null; 
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


