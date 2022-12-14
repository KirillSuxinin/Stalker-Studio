using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Stalker_Studio.Json.XML
{



    public class Rootobject
    {
        [JsonProperty("xml")]
        public Xml xml { get; set; }
    }

    public class Xml_ver
    {
        public string version { get; set; }
        public string encoding { get; set; }
    }

    public class Xml
    {
        public Specific_Character[] specific_character { get; set; }
    }

    public class Specific_Character
    {
        [JsonProperty("@id")]
        public string id { get; set; }
        [JsonProperty("@team_default",NullValueHandling = NullValueHandling.Ignore)]
        public string team_default { get; set; }
        public string name { get; set; }
        [JsonProperty("no_random",NullValueHandling = NullValueHandling.Ignore)]
        public string no_random { get; set; }
        public string icon { get; set; }
        [JsonProperty("map_icon",NullValueHandling = NullValueHandling.Ignore)]
        public Map_Icon map_icon { get; set; }
        public string bio { get; set; }
        [JsonProperty("class")]
        public string _class { get; set; }
        public string community { get; set; }
        [JsonProperty("terrain_sect",NullValueHandling = NullValueHandling.Ignore)]
        public string terrain_sect { get; set; }
        [JsonProperty("snd_config",NullValueHandling = NullValueHandling.Ignore)]
        public string snd_config { get; set; }
        public string rank { get; set; }
        public string reputation { get; set; }
        [JsonProperty("crouch_type",NullValueHandling = NullValueHandling.Ignore)]
        public string crouch_type { get; set; }
        [JsonProperty("panic_treshold", NullValueHandling = NullValueHandling.Ignore)]
        public string panic_treshold { get; set; }
        [JsonProperty("money",NullValueHandling = NullValueHandling.Ignore)]
        public Money money { get; set; }
        public string visual { get; set; }
        [JsonProperty("supplies",NullValueHandling = NullValueHandling.Ignore)]
        public string supplies { get; set; }
        [JsonProperty("actor_dialog",NullValueHandling = NullValueHandling.Ignore)]
        public object actor_dialog { get; set; }
        [JsonProperty("text",NullValueHandling = NullValueHandling.Ignore)]
        public object text { get; set; }
        [JsonProperty("start_dialog", NullValueHandling = NullValueHandling.Ignore)]
        public object start_dialog { get; set; }
        [JsonProperty("mechanic_mode", NullValueHandling = NullValueHandling.Ignore)]
        public string mechanic_mode { get; set; }

    }

    public class Map_Icon
    {
        [JsonProperty("@x")]
        public string x { get; set; }
        [JsonProperty("@y")]
        public string y { get; set; }
    }

    public class Money
    {
        [JsonProperty("@min")]
        public string min { get; set; }
        [JsonProperty("@max")]
        public string max { get; set; }
        [JsonProperty("@infinitive")]
        public string infinitive { get; set; }
    }



}
