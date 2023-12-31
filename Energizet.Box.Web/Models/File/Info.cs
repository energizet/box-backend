﻿using Energizet.Box.Entities;

namespace Energizet.Box.Web.Models.File;

public class InfoResponse
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public VkUser? VkUser { get; set; }
}