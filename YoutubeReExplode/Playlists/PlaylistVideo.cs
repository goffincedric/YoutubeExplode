﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YoutubeReExplode.Common;
using YoutubeReExplode.Videos;

namespace YoutubeReExplode.Playlists;

/// <summary>
/// Metadata associated with a YouTube video included in a playlist.
/// </summary>
public class PlaylistVideo : IVideo, IBatchItem
{
    /// <summary>
    /// ID of the playlist that contains this video.
    /// </summary>
    public PlaylistId PlaylistId { get; }

    /// <inheritdoc />
    public VideoId Id { get; }

    /// <inheritdoc />
    public string Url => $"https://www.youtube.com/watch?v={Id}&list={PlaylistId}";

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public Author Author { get; }

    /// <inheritdoc />
    public bool IsLive { get; }

    /// <inheritdoc />
    public bool? IsLiveContent { get; }

    /// <inheritdoc />
    public TimeSpan? Duration { get; }

    /// <inheritdoc />
    public IReadOnlyList<Thumbnail> Thumbnails { get; }

    /// <summary>
    /// Initializes an instance of <see cref="PlaylistVideo" />.
    /// </summary>
    public PlaylistVideo(
        PlaylistId playlistId,
        VideoId id,
        string title,
        Author author,
        bool isLive,
        bool? isLiveContent,
        TimeSpan? duration,
        IReadOnlyList<Thumbnail> thumbnails)
    {
        PlaylistId = playlistId;
        Id = id;
        Title = title;
        Author = author;
        IsLive = isLive;
        IsLiveContent = isLiveContent;
        Duration = duration;
        Thumbnails = thumbnails;
    }

    /// <summary>
    /// Initializes an instance of <see cref="PlaylistVideo" />.
    /// </summary>
    // Binary backwards compatibility (PlaylistId was added)
    [Obsolete("Use the other constructor instead."), ExcludeFromCodeCoverage]
    public PlaylistVideo(
        VideoId id,
        string title,
        Author author,
        bool isLive,
        bool? isLiveContent,
        TimeSpan? duration,
        IReadOnlyList<Thumbnail> thumbnails)
        : this(default, id, title, author, isLive, isLiveContent, duration, thumbnails)
    {
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"Video ({Title})";
}