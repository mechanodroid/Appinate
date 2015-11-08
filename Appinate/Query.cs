using System;
/*
 * {
  "query": {
    "name": "Most Popular Apps",
    "platform": "android",
    "query_params": {
      "sort": "number_ratings",
      "from": 0,
      "num": 10,
      "sort_order": "desc"
    }
  }
}
*/
namespace Appinate
{
	public class Query
	{
		public InnerQuery query;
		public Query ()
		{
		}
	}
	public class InnerQuery
	{
		public string name;
		public string platform;
		public QueryParms query_params;
		public string _id;
		public string user_id;
		public InnerQuery()
		{
		}
	}
	public class QueryParms
	{
		public string sort;
		public int from;
		public int num;
		public string sort_order;
		public string full_text_term;
		public bool include_full_text_desc;
		public string cat_int;
		public QueryParms()
		{
		}
	}
}

