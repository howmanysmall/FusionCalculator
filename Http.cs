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
			return JsonConvert.DeserializeObject<T>(responseString);
		}
	}
}
