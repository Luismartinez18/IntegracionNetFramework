using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegrationWS.Models
{
    public class IntegrationResult
    {
        public string Id { get; set; }
        public string OperationResult { get; set; }
        public string ErrorMessage { get; set; }
        public static IntegrationResult GetErrorResult(string errorMessage)
        {
            return new IntegrationResult
            {
                ErrorMessage = errorMessage,
                OperationResult = ERROR
            };
        }
        public static IntegrationResult GetErrorResult(string errorMessage, string id) => new IntegrationResult
        {
            ErrorMessage = errorMessage,
            OperationResult = ERROR,
            Id = id
        };
        public static IntegrationResult GetSuccessResult(string id) => new IntegrationResult
        {
            OperationResult = SUCCESS,
            Id = id
        };
        public static IntegrationResult GetNotFoundResult(string errorMessage) => new IntegrationResult
        {
            ErrorMessage = errorMessage,
            OperationResult = NOT_FOUND
        };
        public static IntegrationResult GetBadRequestResult(string errorMessage) => new IntegrationResult 
        {
            ErrorMessage = errorMessage,
            OperationResult = BADREQUEST
        };
        [JsonIgnore]
        public const string ERROR = "ERROR";
        [JsonIgnore]
        public const string SUCCESS = "SUCCESS";
        [JsonIgnore]
        public const string NOT_FOUND = "NOT_FOUND";
        [JsonIgnore]
        public const string BADREQUEST = "BADREQUEST";
    }
}