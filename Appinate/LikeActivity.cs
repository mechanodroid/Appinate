﻿
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
	[Activity (Label = "LikeActivity")]			
	public class LikeActivity : Activity
	{
		ListView _gdListView;
		LikeListViewAdapter _adapter;

		protected override void OnCreate (Bundle bundle)
		{
			
			base.OnCreate(bundle);

			SetContentView (Resource.Layout.Likes);
			_gdListView = FindViewById<ListView> (Resource.Id.listView1);
			_adapter = new LikeListViewAdapter (this);
			_gdListView.Adapter = _adapter;

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.button2);

			button.Click += async (sender, e) => {
				StartActivity(typeof(MainActivity));
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

