namespace GameStore.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Requirements;

    public class Requirements
    {
        [Required]
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(CPULength)]
        public string CPU { get; init; }

        [Required]
        [MaxLength(GPULength)]
        public string GPU { get; init; }

        public int VRAM { get; init; }

        public int RAM { get; init; }

        public int Storage { get; init; }

        [Required]
        [MaxLength(OSLength)]
        public string OS { get; init; }
    }
}