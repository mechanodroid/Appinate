using System;

namespace Appinate
{
	public class GameDataResults
	{
		//[JsonProperty("results")]
		public GameData GameData { get; set; }
	}

	public class GameData
	{
		//[JsonProperty("promo_video")]
		public string promo_video { get; set; }

		//[JsonProperty("iap")]
		public string iap { get; set; }

		//[JsonProperty("created")]
		public string created{ get; set; }

		//[JsonProperty("description")]
		public string description{ get; set; }

		//[JsonProperty("screenshots")]
		public string screenshots{ get; set; }

		//[JsonProperty("cat_type")]
		public string cat_type{ get; set; }

		//[JsonProperty("downloads_maxsize")]
		public string downloads_maxsize{ get; set; }

		//[JsonProperty("category")]
		public string category{ get; set; }

		//[JsonProperty("price")]
		public string price{ get; set; }

		//[JsonProperty("price_numeric")]
		public string price_numeric{ get; set; }

		//[JsonProperty("downloads")]
		public string downloads{ get; set; }

		//[JsonProperty("lang")]
		public string lang{ get; set; }

		//[JsonProperty("version")]
		public string version{ get; set; }

		//[JsonProperty("title")]
		public string title{ get; set; }

		//[JsonProperty("i18_lang")]
		public string i18_lang{ get; set; }

		//[JsonProperty("content_rating")]
		public string content_rating{ get; set; }

		//[JsonProperty("cat_keyprice_i18_contries")]
		public string cat_keyprice_i18_contries{ get; set; }

		//[JsonProperty("website")]
		public string website{ get; set; }

		//[JsonProperty("package_name")]
		public string package_name{ get; set; }

		//[JsonProperty("iap_max")]
		public string iap_max{ get; set; }

		//[JsonProperty("market_update")]
		public string market_update{ get; set; }

		//[JsonProperty("iap_min")]
		public string iap_min{ get; set; }

		//[JsonProperty("cat_int")]
		public string cat_int{ get; set; }

		//[JsonProperty("rating")]
		public string rating{ get; set; }

		//[JsonProperty("downloads_min")]
		public string downloads_min{ get; set; }

		//[JsonProperty("version_code")]
		public string version_code{ get; set; }

		//[JsonProperty("developer")]
		public string developer{ get; set; }

		//[JsonProperty("number_ratings")]
		public string number_ratings{ get; set; }

		//[JsonProperty("icon")]
		public string icon{ get; set; }

		//[JsonProperty("icon_72")]
		public string icon_72{ get; set; }

		//[JsonProperty("market_url")]
		public string market_url{ get; set; }

		//[JsonProperty("deep_link")]
		public string deep_link{ get; set; }

	}
}

