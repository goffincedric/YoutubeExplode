using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeReExplode.Common;
using YoutubeReExplode.Exceptions;
using YoutubeReExplode.Videos.ClosedCaptions;
using YoutubeReExplode.Videos.Streams;

namespace YoutubeReExplode.Videos;

/// <summary>
/// Operations related to YouTube videos.
/// </summary>
public class VideoClient
{
    private readonly VideoController _controller;

    /// <summary>
    /// Operations related to media streams of YouTube videos.
    /// </summary>
    public StreamClient Streams { get; }

    /// <summary>
    /// Operations related to closed captions of YouTube videos.
    /// </summary>
    public ClosedCaptionClient ClosedCaptions { get; }

    /// <summary>
    /// Initializes an instance of <see cref="VideoClient" />.
    /// </summary>
    public VideoClient(HttpClient http)
    {
        _controller = new VideoController(http);

        Streams = new StreamClient(http);
        ClosedCaptions = new ClosedCaptionClient(http);
    }

    /// <summary>
    /// Gets the metadata associated with the specified video.
    /// </summary>
    public async ValueTask<Video> GetAsync(
        VideoId videoId,
        CancellationToken cancellationToken = default)
    {
        var watchPage = await _controller.GetVideoWatchPageAsync(videoId, cancellationToken);

        var playerResponse =
            watchPage.PlayerResponse ??
            await _controller.GetPlayerResponseAsync(videoId, cancellationToken);

        var title =
            playerResponse.Title ??
            throw new YoutubeExplodeException("Could not extract video title.");

        var channelTitle =
            playerResponse.Author ??
            throw new YoutubeExplodeException("Could not extract video author.");

        var channelId =
            playerResponse.ChannelId ??
            throw new YoutubeExplodeException("Could not extract video channel ID.");

        var uploadDate =
            playerResponse.UploadDate ??
            throw new YoutubeExplodeException("Could not extract video upload date.");

        var thumbnails = playerResponse.Thumbnails.Select(t =>
        {
            var thumbnailUrl =
                t.Url ??
                throw new YoutubeExplodeException("Could not extract thumbnail URL.");

            var thumbnailWidth =
                t.Width ??
                throw new YoutubeExplodeException("Could not extract thumbnail width.");

            var thumbnailHeight =
                t.Height ??
                throw new YoutubeExplodeException("Could not extract thumbnail height.");

            var thumbnailResolution = new Resolution(thumbnailWidth, thumbnailHeight);

            return new Thumbnail(thumbnailUrl, thumbnailResolution);
        }).Concat(Thumbnail.GetDefaultSet(videoId)).ToArray();

        Music? musicData = null;
        if (
            watchPage.InitialData?.EngagementPanels is [_, { Items: [_, _, { CarouselLockups: [{ InfoRows.Length: > 0 }, ..] }, ..] }, ..]
        )
        {
            var infoRows = watchPage.InitialData?.EngagementPanels[1].Items[2].CarouselLockups[0].InfoRows;
            musicData = new Music(
                infoRows?.FirstOrDefault(row => row.Title?.ToUpper() == "SONG")?.Values?.FirstOrDefault(),
                infoRows?.FirstOrDefault(row => row.Title?.ToUpper() == "ARTIST")?.Values,
                infoRows?.FirstOrDefault(row => row.Title?.ToUpper() == "ALBUM")?.Values?.FirstOrDefault()
            );
        }

        return new Video(
            videoId,
            title,
            new Author(channelId, channelTitle),
            uploadDate,
            playerResponse.Description ?? "",
            playerResponse.Duration,
            thumbnails,
            playerResponse.Keywords,
            // Engagement statistics may be hidden
            new Engagement(
                playerResponse.ViewCount ?? 0,
                watchPage.LikeCount ?? 0,
                watchPage.DislikeCount ?? 0
            ),
            musicData
        );
    }
}