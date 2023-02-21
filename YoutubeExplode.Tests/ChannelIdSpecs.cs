using System;
using FluentAssertions;
using Xunit;
using YoutubeReExplode.Channels;

namespace YoutubeExplode.Tests;

public class ChannelIdSpecs
{
    [Theory]
    [InlineData("UCEnBXANsKmyj2r9xVyKoDiQ")]
    [InlineData("UC46807r_RiRjH8IU-h_DrDQ")]
    public void Channel_ID_can_be_parsed_from_an_ID_string(string channelId)
    {
        // Act
        var parsed = ChannelId.Parse(channelId);

        // Assert
        parsed.Value.Should().Be(channelId);
    }

    [Theory]
    [InlineData("youtube.com/channel/UC3xnGqlcL3y-GXz5N3wiTJQ", "UC3xnGqlcL3y-GXz5N3wiTJQ")]
    [InlineData("youtube.com/channel/UCkQO3QsgTpNTsOw6ujimT5Q", "UCkQO3QsgTpNTsOw6ujimT5Q")]
    [InlineData("youtube.com/channel/UCQtjJDOYluum87LA4sI6xcg", "UCQtjJDOYluum87LA4sI6xcg")]
    public void Channel_ID_can_be_parsed_from_a_URL_string(string channelUrl, string expectedChannelId)
    {
        // Act
        var parsed = ChannelId.Parse(channelUrl);

        // Assert
        parsed.Value.Should().Be(expectedChannelId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("UC3xnGqlcL3y-GXz5N3wiTJ")]
    [InlineData("UC3xnGqlcL y-GXz5N3wiTJQ")]
    [InlineData("youtube.com/?channel=UCUC3xnGqlcL3y-GXz5N3wiTJQ")]
    [InlineData("youtube.com/channel/asd")]
    [InlineData("youtube.com/")]
    public void Channel_ID_cannot_be_parsed_from_an_invalid_string(string channelIdOrUrl)
    {
        // Act & assert
        Assert.Throws<ArgumentException>(() => ChannelId.Parse(channelIdOrUrl));
    }
}