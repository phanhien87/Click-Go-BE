using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Click_Go.DTOs
{
    public class PostCreateDto
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string SDT { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public IFormFile? LogoImage { get; set; } // Single file for logo
        public IFormFile? BackgroundImage { get; set; } // Single file for background
        public List<IFormFile>? OtherImages { get; set; } // Multiple files for general images
        public List<OpeningHourDto>? OpeningHours { get; set; } // Add opening hours
    }
} 