using EVStationRental.Common.Enums;
using EVStationRental.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EVStationRental.Services.Base
{
    public class ServiceResult : IServiceResult
    {
        public ResultStatus StatusCode { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public List<ErrorDetail>? Errors { get; set; }
        public bool IsSuccess => StatusCode == ResultStatus.Success || StatusCode == ResultStatus.Created;

        public ServiceResult()
        {
            StatusCode = (ResultStatus)(-1);
            Message = "Action fail";
        }

        public ServiceResult(ResultStatus status, string message)
        {
            Validate(message);
            StatusCode = status;
            Message = message;
        }

        public ServiceResult(ResultStatus status, string message, object? data)
        {
            Validate(message);
            StatusCode = status;
            Message = message;
            Data = data;
        }

        public ServiceResult(ResultStatus status, string message, List<ErrorDetail> errors)
        {
            Validate(message);
            StatusCode = status;
            Message = message;
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public ServiceResult(ResultStatus status, string message, object? data, List<ErrorDetail>? errors)
        {
            Validate(message);
            StatusCode = status;
            Message = message;
            Data = data;
            Errors = errors;
        }

        private static void Validate(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty", nameof(message));
        }

        public override string ToString() => $"StatusCode: {StatusCode}, Message: {Message}, Data: {Data?.ToString() ?? "null"}";
    }
}
