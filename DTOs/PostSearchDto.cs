using System.Collections.Generic;

namespace Click_Go.DTOs
{
        public class PostSearchDto
        {
            public string? PostName { get; set; }
            public string? District { get; set; }
            public string? Ward { get; set; }
            public string? City { get; set; }
            public List<string>? TagNames { get; set; }
            public double? MinPrice { get; set; }
            public double? MaxPrice { get; set; }
        }
} 