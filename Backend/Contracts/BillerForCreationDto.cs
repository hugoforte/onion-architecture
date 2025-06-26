using System;
using System.ComponentModel.DataAnnotations;

namespace Payments.Contracts
{
    public class BillerForCreationDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Name can't be longer than 255 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "API Key is required")]
        [StringLength(255, ErrorMessage = "API Key can't be longer than 255 characters")]
        public string ApiKey { get; set; }
    }
} 