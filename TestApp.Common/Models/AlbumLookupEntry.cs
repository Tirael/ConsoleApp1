using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace TestApp.Common.Models
{
    [DebuggerDisplay("ArtistId:{ArtistId}, ArtistName:{ArtistName}, CollectionName:{CollectionName}")]
    public class AlbumLookupEntry
    {
        [JsonProperty("wrapperType", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string WrapperType { get; set; }

        [JsonProperty("kind", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Kind { get; set; }

        [JsonProperty("artistId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? ArtistId { get; set; }

        [JsonProperty("collectionId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? CollectionId { get; set; }

        [JsonProperty("trackId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? TrackId { get; set; }

        [JsonProperty("artistName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ArtistName { get; set; }

        [JsonProperty("collectionName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionName { get; set; }

        [JsonProperty("trackName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrackName { get; set; }

        [JsonProperty("collectionCensoredName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionCensoredName { get; set; }

        [JsonProperty("trackCensoredName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrackCensoredName { get; set; }

        [JsonProperty("artistViewUrl", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri ArtistViewUrl { get; set; }

        [JsonProperty("collectionViewUrl", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri CollectionViewUrl { get; set; }

        [JsonProperty("trackViewUrl", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri TrackViewUrl { get; set; }

        [JsonProperty("previewUrl", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri PreviewUrl { get; set; }

        [JsonProperty("artworkUrl30", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri ArtworkUrl30 { get; set; }

        [JsonProperty("artworkUrl60", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri ArtworkUrl60 { get; set; }

        [JsonProperty("artworkUrl100", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri ArtworkUrl100 { get; set; }

        [JsonProperty("collectionPrice", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double? CollectionPrice { get; set; }

        [JsonProperty("trackPrice", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double? TrackPrice { get; set; }

        [JsonProperty("releaseDate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? ReleaseDate { get; set; }

        [JsonProperty("collectionExplicitness", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionExplicitness { get; set; }

        [JsonProperty("trackExplicitness", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrackExplicitness { get; set; }

        [JsonProperty("discCount", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? DiscCount { get; set; }

        [JsonProperty("discNumber", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? DiscNumber { get; set; }

        [JsonProperty("trackCount", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? TrackCount { get; set; }

        [JsonProperty("trackNumber", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? TrackNumber { get; set; }

        [JsonProperty("trackTimeMillis", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? TrackTimeMillis { get; set; }

        [JsonProperty("country", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        [JsonProperty("currency", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        [JsonProperty("primaryGenreName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryGenreName { get; set; }

        [JsonProperty("isStreamable", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsStreamable { get; set; }

        [JsonProperty("collectionArtistName", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionArtistName { get; set; }
    }
}