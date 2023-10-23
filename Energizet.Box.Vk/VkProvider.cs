using System.Net.Http.Headers;
using System.Text.Json;
using Energizet.Box.Vk.Model;

namespace Energizet.Box.Vk;

public class VkProvider : IDisposable
{
	private readonly HttpClient _client;

	public VkProvider(VkConfig vkConfig, HttpClient client)
	{
		_client = client;

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", vkConfig.ServiceKey);
	}

	public async Task<VkUser?> GetVkUser(string vkId, CancellationToken token)
	{
		using var formData = new MultipartFormDataContent();
		formData.Add(new StringContent(vkId), "user_ids");
		formData.Add(new StringContent("photo"), "fields");
		formData.Add(new StringContent("ru"), "lang");
		formData.Add(new StringContent("5.154"), "v");

		var res = await _client.PostAsync("https://api.vk.com/method/users.get", formData,
			token);
		var vkUserJson = await res.Content.ReadAsStringAsync(token);
		var vkUser = JsonSerializer.Deserialize<VkResponse<VkUser>>(vkUserJson)
			?.Response.FirstOrDefault();
		return vkUser;
	}

	public void Dispose()
	{
		_client.Dispose();
	}
}