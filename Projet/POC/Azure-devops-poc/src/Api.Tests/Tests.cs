using System.Net;
using System.Net.Http.Json;
using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace Api.Tests;

public class Tests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public Tests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_should_return_OK()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var text = await response.Content.ReadAsStringAsync();
        text.Should().Be("OK");
    }

    [Fact]
    public async Task Test_controller_should_return_message()
    {
        var response = await _client.GetAsync("/api/test");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        payload.Should().ContainKey("message");
        payload.Should().ContainKey("timestamp");
    }

    [Fact]
    public async Task Ping_should_return_pong()
    {
        var response = await _client.GetAsync("/api/test/ping");

        response.EnsureSuccessStatusCode();

        var text = await response.Content.ReadAsStringAsync();
        text.Should().Be("pong");
    }
}
