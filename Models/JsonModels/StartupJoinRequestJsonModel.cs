using Newtonsoft.Json;
using StartupsBack.JsonConverters;
using System;

namespace StartupsBack.Models.JsonModels
{
    public class StartupJoinRequestJsonModel
    {
        [JsonProperty(JsonConstants.StartupId)]
        public int StartupId { get; set; }

        [JsonProperty(JsonConstants.StartupWantToJoinList)]
        public int[] UsersWantToJoin { get; set; } = Array.Empty<int>();
    }
}
