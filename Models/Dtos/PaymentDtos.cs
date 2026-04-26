using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Dtos;

public class CheckoutRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public List<Guid> PaintingIds { get; set; }
}