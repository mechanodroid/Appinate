
using System;
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
using Android.Graphics;
using Microsoft.WindowsAzure.MobileServices;


namespace Appinate
{
	[Activity (Label = "LikeActivity")]			
	public class LikeActivity : Activity
	{
		ListView _gdListView;
		LikeListViewAdapter _adapter;

		private void spinner_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			if (e.Position == 0) {
				return;
			}
			string toast = string.Format ("Click Save Button to Upload to Cloud for: {0}", spinner.GetItemAtPosition (e.Position));
			Toast.MakeText (this, toast, ToastLength.Long).Show ();
			MainActivity.currentSelectedGamerType = string.Format ("{0}", spinner.GetItemAtPosition (e.Position));
		}

		protected override void OnCreate (Bundle bundle)
		{
			
			base.OnCreate(bundle);

			SetContentView (Resource.Layout.Likes);
			_gdListView = FindViewById<ListView> (Resource.Id.listView1);
			_adapter = new LikeListViewAdapter (this);
			_gdListView.Adapter = _adapter;

			Spinner spinner = FindViewById<Spinner> (Resource.Id.spinner1);

			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource (
				this, Resource.Array.gamer_type_array2, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			// Get our button from the layout resource,
			// and attach an event to it
			Button buttonNewSearch  =FindViewById<Button>(Resource.Id.button2);
			buttonNewSearch.Click += delegate {
				StartActivity (typeof(MainActivity));
			};

			Button buttonSave  =FindViewById<Button>(Resource.Id.button4);
			buttonSave.Click  += async (sender, e) => {
				string likeString = JsonConvert.SerializeObject(MainActivity.likeGameDataList);
				string stringsFromLikeList = MainActivity.CollectRecommendations();
				File.WriteAllText(MainActivity.storageFile, likeString);
				//Inserting the data into the cloud
				if(MainActivity.currentSelectedGamerType!="")
				{
					switch(MainActivity.currentSelectedGamerType)
					{
					case "Racing":
						RacingGamer rg = new RacingGamer { 
							date = DateTime.Now,
							myList = stringsFromLikeList };
						await MainActivity.MobileService.GetTable<RacingGamer>().InsertAsync(rg);
						break;
					case "Casual":
						CasualGamer cg = new CasualGamer { 
							date = DateTime.Now,
							myList = stringsFromLikeList };
						await MainActivity.MobileService.GetTable<CasualGamer>().InsertAsync(cg);
						break;
					case "Hardcore":
						HardcoreGamer hg = new HardcoreGamer { 
							date = DateTime.Now,
							myList = stringsFromLikeList };
						await MainActivity.MobileService.GetTable<HardcoreGamer>().InsertAsync(hg);
						break;
					case "Puzzle":
						PuzzleGamer pg = new PuzzleGamer { 
							date = DateTime.Now,
							myList = stringsFromLikeList };
						await MainActivity.MobileService.GetTable<PuzzleGamer>().InsertAsync(pg);
						break;
					case "Apps User":
						AppUser au = new AppUser { 
							date = DateTime.Now,
							myList = stringsFromLikeList };
						await MainActivity.MobileService.GetTable<AppUser>().InsertAsync(au);
						break;
					}
				}
			};
		}
		private Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}
	}
}

