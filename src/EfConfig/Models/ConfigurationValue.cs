using System;
using System.ComponentModel.DataAnnotations;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore
{
    public class ConfigurationValue : IEquatable<ConfigurationValue>
    {
        [Key]
        [Required]
        public string Key { get; set; }

        public string Value { get; set; }
        
        public bool Equals(ConfigurationValue other)
        {
            return Key.Equals(other.Key);
        }
    }
}
