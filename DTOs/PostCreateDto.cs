using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Click_Go.DTOs
{
    public class PostCreateDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string SDT { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        
        public string? City { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public long CategoryId { get; set; }
        public List<long>? TagIds { get; set; }
        public IFormFile? LogoImage { get; set; } // Single file for logo
        public IFormFile? BackgroundImage { get; set; } // Single file for background
        public List<IFormFile>? OtherImages { get; set; } // Multiple files for general images
        public List<OpeningHourDto>? OpeningHours { get; set; } // Add opening hours
    }
} 