using BH.oM.Adapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BH.oM.Adapters.HTTP
{
    public class HttpPullConfig: ActionConfig
    {
        [Description("If the response from the GET request is expected to be a BHoM object but doesn't have content type of 'application/bhom', this forces the adapter to deserialise the result as BHoM formatted json. Otherwise the adapter returns the response content as a string.")]
        public virtual bool ForceDeserialiseAsBHoM { get; set; } = true;
    }
}
