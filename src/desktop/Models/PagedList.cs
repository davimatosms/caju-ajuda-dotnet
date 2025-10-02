// CajuAjuda.Desktop/Models/PagedList.cs

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CajuAjuda.Desktop.Models
{
   
    public class PagedList<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new List<T>();

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}