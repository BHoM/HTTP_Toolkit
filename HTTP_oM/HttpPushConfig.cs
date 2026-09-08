using BH.oM.Adapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BH.oM.Adapters.HTTP
{
    public class HttpPushConfig : ActionConfig
    {
        [Description("The uri provided is appended to the base URI set in the adapter. If the base uri is empty, this must be the full URL for the requested resource.")]
        public virtual string RequestURL { get; set; } = "";

        [Description("If only one object is provided to the Push, this forces the adapter to push it as a json array instead of a single object. By default the adapter will push single objects alone instead of as a list.")]
        public virtual bool ForcePostAsList { get; set; } = false;

        [Description("If the response from the POST request is expected to be a BHoM object but doesn't have content type of 'application/bhom', this forces the adapter to deserialise the result as BHoM formatted json. Otherwise the adapter returns the response content as a string.")]
        public virtual bool ForceDeserialiseAsBHoM { get; set; } = false;

        [Description("Custom request headers to send with the POST request.")]
        public virtual Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        [Description("Custom parameters to provide to the POST request.")]
        public virtual Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
