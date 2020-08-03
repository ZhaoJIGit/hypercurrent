using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace JieNor.Megi.Core.Context
{
	public class JsonContentNegotiator
	{
		private readonly JsonMediaTypeFormatter _jsonFormatter;

		public JsonContentNegotiator(JsonMediaTypeFormatter formatter)
		{
			_jsonFormatter = formatter;
		}

		public ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			return new ContentNegotiationResult(_jsonFormatter, new MediaTypeHeaderValue("application/json"));
		}
	}
}
