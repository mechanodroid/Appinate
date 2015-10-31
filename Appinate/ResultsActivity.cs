
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
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.Results);

			ImageButton button1  =FindViewById<ImageButton>(Resource.Id.imageButton1);
			var imageBitmap = GetImageBitmapFromUrl(MainActivity.gameDataList[0].icon);
			button1.SetImageBitmap(imageBitmap);

			TextView text1 = FindViewById<TextView> (Resource.Id.textView1);
			text1.SetText(MainActivity.gameDataList [0].title,TextView.BufferType.Normal);

			ImageButton button2  =FindViewById<ImageButton>(Resource.Id.imageButton2);
			var imageBitmap2 = GetImageBitmapFromUrl(MainActivity.gameDataList[1].icon);
			button2.SetImageBitmap(imageBitmap2);

			TextView text2 = FindViewById<TextView> (Resource.Id.textView2);
			text2.SetText (MainActivity.gameDataList [1].title, TextView.BufferType.Normal);

			ImageButton button3  =FindViewById<ImageButton>(Resource.Id.imageButton3);
			var imageBitmap3 = GetImageBitmapFromUrl(MainActivity.gameDataList[2].icon);
			button3.SetImageBitmap(imageBitmap3);

			TextView text3 = FindViewById<TextView> (Resource.Id.textView3);
			text3.SetText (MainActivity.gameDataList [2].title,TextView.BufferType.Normal);

			ImageButton button4  =FindViewById<ImageButton>(Resource.Id.imageButton4);
			var imageBitmap4 = GetImageBitmapFromUrl(MainActivity.gameDataList[3].icon);
			button4.SetImageBitmap(imageBitmap4);

			TextView text4 = FindViewById<TextView> (Resource.Id.textView4);
			text4.SetText (MainActivity.gameDataList [3].title,TextView.BufferType.Normal);

			ImageButton button5  =FindViewById<ImageButton>(Resource.Id.imageButton5);
			var imageBitmap5 = GetImageBitmapFromUrl(MainActivity.gameDataList[4].icon);
			button5.SetImageBitmap(imageBitmap5);								

			TextView text5 = FindViewById<TextView> (Resource.Id.textView5);
			text5.SetText (MainActivity.gameDataList [4].title,TextView.BufferType.Normal);

			// Create your application here
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

