using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PefectMoney.Core.Settings
{

    public class ResultHandler
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string[] Message { get; set; }

        public static ResultHandler MapToResultHandler(ResultOperation resultOperation)
        {
            return new ResultHandler
            {
                IsSuccess = resultOperation.IsSuccess,
                Message = resultOperation?.Message?.ToArray(),
            };
        }

    }
    public class ResultHandler<T>
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonPropertyName("message")]
        public string[] Message { get; set; }
        [JsonPropertyName("data")]
        public T Data { get; set; }
        public static ResultHandler<T> MapToResultHandler(ResultOperation<T> resultOperation)
        {
            return new ResultHandler<T>
            {
                IsSuccess = resultOperation.IsSuccess,
                Message = resultOperation?.Message?.ToArray(),
                Data = resultOperation.Data

            };
        }

    }

}
