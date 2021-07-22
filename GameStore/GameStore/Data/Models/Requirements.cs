namespace GameStore.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Requirements
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(50)]
        public string CPU { get; init; }

        [Required]
        [MaxLength(50)]
        public string GPU { get; init; }

        public int VRAM { get; init; }

        public int RAM { get; init; }

        public int Storage { get; init; }

        [Required]
        [MaxLength(25)]
        public string OS { get; init; }
    }
}