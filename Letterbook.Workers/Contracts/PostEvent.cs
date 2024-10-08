﻿using Letterbook.Core.Models.Dto;
using Medo;

namespace Letterbook.Workers.Contracts;

public record PostEvent
{
	public Uuid7 Sender { get; init; }
	public required PostDto NextData { get; init; }
	public PostDto? PrevData { get; init; }
	public required string Subject { get; init; }
	public required Claim[] Claims { get; init; }
	public required string Type { get; init; }

}