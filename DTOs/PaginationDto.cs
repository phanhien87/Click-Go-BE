using System.Collections.Generic;

namespace Click_Go.DTOs
{
    public class PaginationDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public IEnumerable<T> Items { get; set; }
    }
} 