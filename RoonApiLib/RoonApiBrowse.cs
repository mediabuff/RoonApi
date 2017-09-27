﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 Browse Hierarchie
Library
	Artists
		artist1
			Play Artist
				Play Now
				Start Radio
			album1
				Play Album
					Play Now
					Start Radio
					Add Next
					Add to Queue
				track1
					Play Now
					Start Radio
					Add Next
					Add to Queue
				track2
			album2
		artist2
	Albums
		album1
			Play Album
				Play Now
				Start Radio
				Add Next
				Add to Queue
			track1
				Play Now
				Start Radio
				Add Next
				Add to Queue
			track2
		album2
	Tracks

Internet Radio
	radio1
	radio2
Genres
	genre1
		Play Genre
				Play Now
				Start Radio
		Artists
			artist1
				Play Artist
					Play Now
					Start Radio
				album1
					Play Album
						Play Now
						Start Radio
						Add Next
						Add to Queue
					track1
						Play Now
						Start Radio
						Add Next
						Add to Queue
					track2
				album2
			artist2
		Albums
			album1
				Play Album
					Play Now
					Start Radio
					Add Next
					Add to Queue
				track1
					Play Now
					Start Radio
					Add Next
					Add to Queue
				track2
			album2
	genre2
*/
namespace RoonApiLib
{
    public class RoonApiBrowse
    {
        public const string BrowseLibrary            = "Library";
        public const string BrowseArtists            = "Artists";
        public const string BrowseAlbums             = "Albums";
        public const string BrowseTracks             = "Tracks";
        public const string BrowseInternetRadio      = "Internet Radio";
        public const string BrowseGenres             = "Genres";
        public const string BrowseTIDAL              = "TIDAL";
        public const string BrowseTIDALFavorites     = "Your Favorites";
        public const string ActionPlayArtist         = "Play Artist";
        public const string ActionPlayAlbum          = "Play Album";
        public const string ActionPlayGenre          = "Play Genre";
        public const string ActionPlayNow            = "Play Now";
        public const string ActionStartRadio         = "Start Radio";
        public const string ActionAddNext            = "Add Next";
        public const string ActionAddtoQueue         = "Add to Queue";

        public class RoonBrowseOptions
        {
            [JsonProperty("hierarchy")]
            public string Hierarchy { get; set; }
            [JsonProperty("zone_or_output_id")]
            public string ZoneOrOutputId { get; set; }
            [JsonProperty("item_key")]
            public string ItemKey { get; set; }
            [JsonProperty("refresh_list")]
            public bool RefreshList { get; set; }
            [JsonProperty("pop_all")]
            public bool PopAll { get; set; }
            [JsonProperty("input")]
            public string Input { get; set; }
        }
        public class RoonBrowseList
        {
            [JsonProperty("level")]
            public int Level { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("subtitle")]
            public string SubTitle { get; set; }
            [JsonProperty("image_key")]
            public string ImageKey { get; set; }
            [JsonProperty("count")]
            public int Count { get; set; }
            [JsonProperty("display_offset")]
            public int? DisplayOffset { get; set; }
        }
        public class RoonBrowseResult
        {
            [JsonProperty("action")]
            public string Action { get; set; }
            [JsonProperty("list")]
            public RoonBrowseList List { get; set; }
        }
        public class RoonLoadOptions
        {
            [JsonProperty("hierarchy")]
            public string Hierarchy { get; set; }
            [JsonProperty("offset")]
            public int Offset { get; set; }
            [JsonProperty("set_display_offset")]
            public int SetDisplayOffset { get; set; }
        }
        public class RoonLoadItem
        {
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("subtitle")]
            public string SubTitle { get; set; }
            [JsonProperty("image_key")]
            public string ImageKey { get; set; }
            [JsonProperty("item_key")]
            public string ItemKey { get; set; }
            [JsonProperty("hint")]
            public string Hint { get; set; }
        }
        public class RoonLoadResult
        {
            [JsonProperty("items")]
            public RoonLoadItem[] Items { get; set; }
            [JsonProperty("offset")]
            public int Offset { get; set; }
            [JsonProperty("list")]
            public RoonBrowseList List { get; set; }

            public RoonLoadItem FindItem (string title)
            {
                return Items.FirstOrDefault((item) => item.Title == title);
            }
        }

        RoonApi _api;
        public RoonApiBrowse(RoonApi api)
        {
            _api = api;
        }
        public async Task<RoonBrowseResult> Browse(RoonBrowseOptions options)
        {
            var result = await _api.SendReceive<RoonBrowseResult, RoonBrowseOptions>(RoonApi.ServiceBrowse + "/browse", options);
            return result;
        }
        public async Task<RoonLoadResult> Load(RoonLoadOptions options)
        {
            var result = await _api.SendReceive<RoonLoadResult, RoonLoadOptions>(RoonApi.ServiceBrowse + "/load", options);
            return result;
        }
        public async Task<RoonLoadResult> BrowseAndLoad(RoonBrowseOptions options)
        {
            RoonLoadResult result = null;
            var browseResult = await Browse(options);
            List<RoonLoadItem> items = new List<RoonLoadItem>();
            for (int i = 0; i < browseResult.List.Count; i += 100)
            {
                result = await Load(new RoonLoadOptions { Hierarchy = options.Hierarchy, Offset = i, SetDisplayOffset = i });
                items.AddRange(result.Items);
            }
            result.Items = items.ToArray();
            return result;
        }

    }
}
