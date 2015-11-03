
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
	public class ResultsListViewAdapter : BaseAdapter<GameData>
	{
		private readonly Activity _context;

		public ResultsListViewAdapter(Activity context)
		{
			_context = context;
		}
		public override int Count
		{
			get { return MainActivity.gameDataList.Count; }
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override GameData this[int position]
		{
			get { return MainActivity.gameDataList [position]; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)	{
			View view = convertView;
			if (view == null)
				view = _context.LayoutInflater.Inflate(Resource.Layout.ResultsListItem, null);

			GameData gd = MainActivity.gameDataList[position];
			view.FindViewById<TextView> (Resource.Id.textView1).Text = gd.title;

			ImageButton button  =view.FindViewById<ImageButton>(Resource.Id.imageButton1);
			var imageBitmap = GetImageBitmapFromUrl(gd.icon);
			button.SetImageBitmap(imageBitmap);	

			button.Click += delegate {
				var uri = Android.Net.Uri.Parse (gd.market_url);
				var intent = new Intent (Intent.ActionView, uri);
				_context.StartActivity (intent);
			};
			return view;
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

