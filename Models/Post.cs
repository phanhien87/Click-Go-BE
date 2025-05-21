namespace Click_Go.Models
{
    public class Post : BaseEntity
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Logo_Image { get; set; }
        public string? Background { get; set; }
        public string? SDT { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }

        public long CategoryId { get; set; }
        public Category Category { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<OpeningHour> Opening_Hours { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Wishlist> WishlistedByUsers { get; set; }
    }
}
