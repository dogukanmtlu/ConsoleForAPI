using System;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleForAPI
{
    public class APIMethods
    {
		private WebClient client;
		//Connection parameters
		private static string endpoint = "7e4cf1a3.compilers.sphere-engine.com";
		private static string accessToken = "20b39e32a25c9c643e9fa9b7fc19f453";
        public APIMethods()
        {
			this.client = new WebClient();
		}

		//Returns a list of supported compilers.
		public void GetCompilerList()
		{
			try
			{
				// send request
				byte[] responseBytes = client.DownloadData("https://" + endpoint + "/api/v3/compilers?access_token=" + accessToken);
				string responseBody = Encoding.UTF8.GetString(responseBytes);

				//Json Formatter
				var options = new JsonSerializerOptions()
				{
					WriteIndented = true
				};

				var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseBody);

				responseBody = JsonSerializer.Serialize(jsonElement, options);

				// process response
				Console.WriteLine(responseBody);
				Console.ReadKey();

			}
			catch (WebException exception)
			{
				WebResponse response = exception.Response;
				HttpStatusCode statusCode = (((HttpWebResponse)response).StatusCode);

				// fetch errors
				if (statusCode == HttpStatusCode.Unauthorized)
				{
					Console.WriteLine("Invalid access token");
					Console.ReadKey();
				}

				response.Close();
			}
		}

		//Post requests
		public int PostRequest(string compilerId)
		{
			//Soruce Code and Compiler input from console option

			//Console.WriteLine("Source Code: ");
			//string sourceCode = Console.ReadLine();
			//Console.WriteLine("Compiler ID: ");
			//string compilerId = Console.ReadLine();

			try
			{

				// define request parameters
				NameValueCollection formData = new NameValueCollection();
				//Source Code
				formData.Add("source", "def plus(a,b):	return a + b; print(plus(2, 2))");

				//Compiler ID
				formData.Add("compilerId", compilerId);

				//Optional input (Source code must start with take input, like 'var = input()')
                //formData.Add("input", "2");


				// send request
				byte[] responseBytes = client.UploadValues("https://" + endpoint + "/api/v4/submissions?access_token=" + accessToken, "POST", formData);
				string responseBody = Encoding.UTF8.GetString(responseBytes);

				//Format bytes as JSON
				JObject json = JObject.Parse(responseBody);

				//return Submission ID
				return Convert.ToInt32(json["id"]);

			}
			catch (WebException exception)
			{
				WebResponse response = exception.Response;
				HttpStatusCode statusCode = (((HttpWebResponse)response).StatusCode);

				// fetch errors
				if (statusCode == HttpStatusCode.Unauthorized)
				{
					Console.WriteLine("Invalid access token");
					Console.ReadKey();
				}

				response.Close();
				return 0;
			}
		}

		//Returns information about submission
		public void GetResult(int submissionId)
		{

			try
			{
				// send request
				string requestURL = "https://" + endpoint + "/api/v4/submissions/" + submissionId + "?access_token=" + accessToken;
				byte[] responseBytes = client.DownloadData(requestURL);
				string responseBody = Encoding.UTF8.GetString(responseBytes);

				//Format bytes as JSON
				JObject json = JObject.Parse(responseBody);

				//Take input, result and output part from response
				string input = "";
				string result = "";
				var source = client.DownloadString(json["result"]["streams"]["source"]["uri"].ToString());

				//input could be empty
				try
				{
					input = client.DownloadString(json["result"]["streams"]["input"]["uri"].ToString());
				}
				catch (Exception)
				{

					input = "No İnput!";
				}

				//output could be empty
                try
                {
					result = client.DownloadString(json["result"]["streams"]["output"]["uri"].ToString());
				}
                catch (Exception)
                {

                    result = "No Result!";
                }

				//JSON fromatter for display

				//var options = new JsonSerializerOptions()
				//{
				//	WriteIndented = true
				//};

				//var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseBody);

				//responseBody = JsonSerializer.Serialize(jsonElement, options);

				Console.WriteLine("Source Code: " + source);
				Console.WriteLine("Input: " + input);
				Console.WriteLine("Result is: " + result);
				Console.ReadKey();

			}
			catch (WebException exception)
			{
				WebResponse response = exception.Response;
				HttpStatusCode statusCode = (((HttpWebResponse)response).StatusCode);

				// fetch errors
				if (statusCode == HttpStatusCode.Unauthorized)
				{
					Console.WriteLine("Invalid access token");
					Console.ReadKey();
				}
				else if (statusCode == HttpStatusCode.Forbidden)
				{
					Console.WriteLine("Access denied");
					Console.ReadKey();
				}
				else if (statusCode == HttpStatusCode.NotFound)
				{
					Console.WriteLine("Submission not found");
					Console.ReadKey();
				}

				response.Close();
			}
		}
	}
}
