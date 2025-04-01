﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace revit_mcp_sdk.API.Models
{
    /// <summary>
    /// JSON-RPC请求和响应的序列化/反序列化工具类
    /// </summary>
    public static class JsonRPCSerializer
    {
        /// <summary>
        /// 将JSON字符串反序列化为JSON-RPC请求对象
        /// </summary>
        /// <param name="jsonString">JSON-RPC请求字符串</param>
        /// <returns>反序列化后的请求对象</returns>
        public static JsonRPCRequest DeserializeRequest(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<JsonRPCRequest>(jsonString);
            }
            catch (JsonException ex)
            {
                throw new JsonRPCSerializationException("Failed to deserialize JSON-RPC request", ex);
            }
        }

        /// <summary>
        /// 创建成功响应
        /// </summary>
        /// <param name="id">请求ID</param>
        /// <param name="result">响应结果</param>
        /// <returns>JSON格式的成功响应</returns>
        public static string CreateSuccessResponse(string id, object result)
        {
            var response = new JsonRPCSuccessResponse
            {
                Id = id,
                Result = result is JToken jToken ? jToken : JToken.FromObject(result)
            };

            return response.ToJson();
        }

        /// <summary>
        /// 创建错误响应
        /// </summary>
        /// <param name="id">请求ID</param>
        /// <param name="code">错误代码</param>
        /// <param name="message">错误消息</param>
        /// <param name="data">附加错误数据</param>
        /// <returns>JSON格式的错误响应</returns>
        public static string CreateErrorResponse(string id, int code, string message, object data = null)
        {
            var response = new JsonRPCErrorResponse
            {
                Id = id,
                Error = new JsonRPCError
                {
                    Code = code,
                    Message = message,
                    Data = data != null ? JToken.FromObject(data) : null
                }
            };

            return response.ToJson();
        }

        /// <summary>
        /// 创建解析错误响应
        /// </summary>
        /// <returns>JSON格式的解析错误响应</returns>
        public static string CreateParseErrorResponse()
        {
            return CreateErrorResponse(null, JsonRPCErrorCodes.ParseError, "Parse error");
        }

        /// <summary>
        /// 创建无效请求响应
        /// </summary>
        /// <returns>JSON格式的无效请求响应</returns>
        public static string CreateInvalidRequestResponse()
        {
            return CreateErrorResponse(null, JsonRPCErrorCodes.InvalidRequest, "Invalid Request");
        }

        /// <summary>
        /// 尝试解析JSON-RPC请求
        /// </summary>
        /// <param name="jsonString">JSON字符串</param>
        /// <param name="request">解析出的请求对象</param>
        /// <param name="errorResponse">错误响应</param>
        /// <returns>是否成功解析</returns>
        public static bool TryParseRequest(string jsonString, out JsonRPCRequest request, out string errorResponse)
        {
            request = null;
            errorResponse = null;

            try
            {
                request = DeserializeRequest(jsonString);

                if (request == null || !request.IsValid())
                {
                    errorResponse = CreateInvalidRequestResponse();
                    return false;
                }

                return true;
            }
            catch (JsonException)
            {
                errorResponse = CreateParseErrorResponse();
                return false;
            }
            catch (Exception ex)
            {
                errorResponse = CreateErrorResponse(
                    null,
                    JsonRPCErrorCodes.InternalError,
                    $"Error processing request: {ex.Message}"
                );
                return false;
            }
        }
    }

    /// <summary>
    /// JSON-RPC序列化异常
    /// </summary>
    public class JsonRPCSerializationException : Exception
    {
        public JsonRPCSerializationException(string message) : base(message)
        {
        }

        public JsonRPCSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
