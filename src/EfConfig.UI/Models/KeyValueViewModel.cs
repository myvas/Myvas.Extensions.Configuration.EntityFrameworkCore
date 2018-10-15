using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore.Models
{
    public interface IKeyValueViewModel<TKey, TValue>
    {
        TKey Key { get; set; }
        TValue Value { get; set; }
    }

    public class KeyValueViewModel
        : KeyValueViewModel<string, string>
    {
    }

    public class KeyValueViewModel<TKey, TValue>
        : IKeyValueViewModel<TKey, TValue>
    {
        [JsonProperty("key")]
        public TKey Key { get; set; }

        [JsonProperty("value")]
        public TValue Value { get; set; }
    }
}
