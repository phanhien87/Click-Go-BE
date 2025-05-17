using System;
using System.Collections.Generic;

namespace Click_Go.DTOs
{
    public class PostReadDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Logo_Image { get; set; }
        public string Background { get; set; }
        public string SDT { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public CategoryDto Category { get; set; }
        public UserDto User { get; set; }
        public List<OpeningHourDto> OpeningHours { get; set; }
        public List<ImageDto> Images { get; set; }
        // Add other collections like Comments, Ratings if needed in the future

        public PostReadDto()
        {
            OpeningHours = new List<OpeningHourDto>();
            Images = new List<ImageDto>();
        }
    }
} 