﻿namespace Click_Go.Models
{
    public class Image : BaseEntity
    {
        public long? PostId { get; set; }
        public Post? Post { get; set; }

        public long? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public string? ImagePath { get; set; }
    }
}
