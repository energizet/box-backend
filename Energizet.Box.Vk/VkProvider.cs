using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Energizet.Box.Exceptions;
using Energizet.Box.Vk.Abstractions;
using Energizet.Box.Vk.Abstractions.Model;
using Energizet.Box.Vk.Model;

namespace Energizet.Box.Vk;

public sealed class VkProvider : IVkProvider, IDisposable
{
	private readonly HttpClient _client;

	public VkProvider(VkConfig vkConfig, HttpClient client)
	{
		_client = client;

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", vkConfig.ServiceKey);
	}

	public async Task<VkUser> GetVkUser(string vkId, CancellationToken token)
	{
		using var formData = new MultipartFormDataContent();
		formData.Add(new StringContent(vkId), "user_ids");
		formData.Add(new StringContent("photo"), "fields");
		formData.Add(new StringContent("ru"), "lang");
		formData.Add(new StringContent("5.154"), "v");

		var res = await _client.PostAsync(
			"https://api.vk.com/method/users.get", formData, token
		);

		if (res.StatusCode == HttpStatusCode.NotFound)
		{
			throw new NotFoundException($"id({vkId}) not found");
		}

		var vkUserJson = await res.Content.ReadAsStringAsync(token);
		var vkResponse = JsonSerializer.Deserialize<VkResponse<VkUserResponse>>(vkUserJson);

		var vkUser = vkResponse?.Response.FirstOrDefault();

		if (vkUser == null)
		{
			throw new NotFoundException($"id({vkId}) not found");
		}

		return new()
		{
			Id = vkUser.Id,
			FirstName = vkUser.FirstName,
			LastName = vkUser.LastName,
			Photo = vkUser.Photo,
		};
	}

	public void Dispose()
	{
		_client.Dispose();
	}
}