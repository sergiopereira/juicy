﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Juicy.DirtCheapDaemons.Http
{
	public class RequestFactory
	{
		public IRequest Create(IEnumerable<string> requestLines, MountPoint mount, string vpath)
		{
			if (vpath == null) throw new ArgumentNullException("vpath");

			var request = new Request { MountPoint = mount, VirtualPath = vpath };

			PopulateRequestPostedData(request, requestLines);
			PopulateRequestHeaders(request, requestLines);
			PopulateRequestQueryStringValues(request, vpath);

			return request;
		}

		private void PopulateRequestQueryStringValues(IRequest request, string vpath)
		{
			if (vpath.IndexOf("?") >= 0)
			{
				var query = vpath.Substring(vpath.IndexOf("?"));
				var nvc = HttpUtility.ParseQueryString(query);
				foreach(var key in nvc.AllKeys)
				{
					request.QueryString.Add(key, nvc[key]);
				}
			}
		}

		private void PopulateRequestHeaders(IRequest request, IEnumerable<string> requestLines)
		{
			requestLines.Skip(1).ToList().ForEach(line =>
			{
				if (!string.IsNullOrEmpty(line))
				{
					int pos = line.IndexOf(":");
					if (pos > 0)
					{
						request[line.Substring(0, pos)] = line.Substring(pos + 2);
					}
				}
			});
		}

		private void PopulateRequestPostedData(IRequest request, IEnumerable<string> requestLines)
		{
			//copy the headers to the request object
			var headerLines = requestLines.Skip(1).ToList();
			int emptyLineIndex = 0;

			foreach(var line in headerLines)
			{
				if(line == "")
					break;

				emptyLineIndex++;
			}

			bool containsFormData = headerLines.Count(line => line.Contains("application/x-www-form-urlencoded")) > 0;

			if (containsFormData && headerLines.Count >= 2 && (emptyLineIndex == headerLines.Count - 2))
			{
				//found the separator line between the headers and the
				// form values
				var nvc = HttpUtility.ParseQueryString(headerLines.Last());
				foreach (var key in nvc.AllKeys)
				{
					request.Form.Add(key, nvc[key]);
				}

				//remove the separator and the form values from the header lines
				headerLines.RemoveAt(headerLines.Count - 1);
				headerLines.RemoveAt(headerLines.Count - 1);
			}

			if(!containsFormData && (emptyLineIndex <= headerLines.Count-2))
			{
				//not posting form data, maybe just an unencoded body
				string result = "";
				for(int i=emptyLineIndex+1; i<headerLines.Count; i++)
				{
					result += headerLines[i];
					if(i<headerLines.Count-1)
					{
						result += Environment.NewLine;
					}
				}

				request.PostBody = result;
			}
		}

	}
}
