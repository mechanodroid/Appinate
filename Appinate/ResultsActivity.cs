
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
using Android.Graphics;

namespace Appinate
{
	[Activity (Label = "ResultsActivity")]			
	public class ResultsActivity : Activity
	{
		string[] items;
		ListView _gdListView;
		ResultsListViewAdapter _adapter;

		protected override void OnCreate (Bundle bundle)
		{
			
			base.OnCreate(bundle);

			SetContentView (Resource.Layout.Results);
			_gdListView = FindViewById<ListView> (Resource.Id.listView1);
			_adapter = new ResultsListViewAdapter (this);
			_gdListView.Adapter = _adapter;
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

