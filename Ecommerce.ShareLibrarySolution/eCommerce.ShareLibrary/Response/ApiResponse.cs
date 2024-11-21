﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace eCommerce.ShareLibrary.Response
{
    public class ApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("errors")]
        public List<ValidationErrorDTO>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("data")]
        public object? Data { get; set; }

        public override string? ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ValidationErrorDTO
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("message")]
        public List<string>? Message { get; set; }
    }
}
