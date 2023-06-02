using System.Text;
using Newtonsoft.Json;

namespace FusionCalculator
{
	/// <summary>
	/// This is a utility library used to make HTTP requests.
	/// </summary>
	internal interface Http
	{
		/// <summary>
		/// Does a GET request to the specified URL and returns the response as a string.
		/// </summary>
		/// <param name="url">The url to get from.</param>
		/// <typeparam name="T">The type of the response.</typeparam>
		/// <returns>The decoded</returns>
		internal static async Task<T> GetAsync<T>(string url)
		{
			if (url is null)
				throw new ArgumentNullException(nameof(url), "The URL cannot be null.");

			using HttpClient httpClient = new HttpClient();
			HttpResponseMessage response = await httpClient.GetAsync(url);
			response.EnsureSuccessStatusCode();

			string responseString = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(responseString)!;
		}

		/// <summary>
		/// Performs a HTTP request to the specified URL and returns the response as a string.
		/// </summary>
		/// <param name="httpMethod">The request method.</param>
		/// <param name="url">The url you are requesting from.</param>
		/// <param name="content">The optional content to use for POST-likes requests.</param>
		/// <typeparam name="TResponse">The type of response.</typeparam>
		/// <returns>The response.</returns>
		internal static async Task<TResponse> RequestAsync<TResponse>(
			HttpMethod httpMethod,
			string url,
			object? content = null
		)
		{
			if (url is null)
				throw new ArgumentNullException(nameof(url), "The URL cannot be null.");

			using HttpClient httpClient = new HttpClient();
			using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, url);

			if (content is not null)
			{
				string json = JsonConvert.SerializeObject(content);
				httpRequestMessage.Content = new StringContent(
					json,
					Encoding.UTF8,
					"application/json"
				);
			}

			HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
			response.EnsureSuccessStatusCode();

			string responseString = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<TResponse>(responseString)!;
		}
	}
}
