using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Click_Go.DTOs
{
    public class PostUpdateDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? SDT { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? City { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? LinkFacebook { get; set; }
        public string? LinkGoogleMap { get; set; }
        public long? CategoryId { get; set; }
        public List<long>? TagIds { get; set; }
        public IFormFile? LogoImage { get; set; }
        public IFormFile? BackgroundImage { get; set; }
        public List<IFormFile>? OtherImages { get; set; }
        
        public List<OpeningHourDto>? OpeningHours { get; set; }
    }
} 