
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore.Models
{
    public class EfConfigJson
    {
        public const int NoError = 0;

        public static EfConfigJson Ok() => new EfConfigJson(NoError, "");
        public static EfConfigJson Error(int errcode, string errmsg)
        {
            return new EfConfigJson(errcode, errmsg);
        }

        [JsonIgnore]
        public bool Success { get { return ErrorCode.GetValueOrDefault(0) == NoError; } }

        /// <summary>
        /// 错误代码
        /// </summary>
        [JsonProperty("errcode", NullValueHandling = NullValueHandling.Ignore)]
        public int? ErrorCode { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }

        public EfConfigJson(int? errcode, string errmsg)
        {
            ErrorCode = errcode;
            ErrorMessage = errmsg;
        }
    }
}