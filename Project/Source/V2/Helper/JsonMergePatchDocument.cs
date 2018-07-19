using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Api213.V2.Helper
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class JsonMergePatchDocument<TEntity> : Dictionary<string, object>
        where TEntity : class
    {
        /// <summary>
        ///     JsonPatchDocument
        /// </summary>
        private JsonPatchDocument<TEntity> _jsonPatchDocument;

        /// <summary>
        ///     ApplyTo
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="modelState"></param>
        public void ApplyTo(TEntity entity, ModelStateDictionary modelState)
        {
            var serialized = "[";
            var conca = string.Empty;
            foreach (var(key, value) in this)
            {
                serialized += conca;
                serialized += ToJsonPatchReplace(value, key);
                conca = ",";
            }

            serialized += "]";

            _jsonPatchDocument = JsonConvert.DeserializeObject<JsonPatchDocument<TEntity>>(serialized);
            _jsonPatchDocument.ApplyTo(entity, modelState);
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string ToJsonPatchReplace(object value, string key)
        {
            return " {\"value\":\"" + value + "\",\"path\":\"/" + key + "\",\"op\":\"replace\"} ";
        }
    }
}