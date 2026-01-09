using System.ComponentModel.DataAnnotations;

namespace Options;

public class SportsApiOptions
{
  [Required]
  public string BaseUrl { get; set; } = string.Empty;
}
