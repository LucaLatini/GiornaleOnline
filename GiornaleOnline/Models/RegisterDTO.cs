namespace GiornaleOnline.Models
{
    public class RegisterDTO

    {
        [Required]
        [MaxLength(100)]

		public string? Nome { get; set; }

		[Required]
		[MaxLength(100)]

		public string? Username { get; set; }

		[Required]
		[MaxLength(100)]

		public string? Password { get; set; }
	}
}
