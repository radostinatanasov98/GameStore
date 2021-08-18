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
        [MaxLength(CPUMaxLength)]
        public string CPU { get; init; }

        [Required]
        [MaxLength(GPUMaxLength)]
        public string GPU { get; init; }

        [Required]
        [RegularExpression(MemoryRegularExpression)]
        public string VRAM { get; init; }

        [Required]
        [RegularExpression(MemoryRegularExpression)]
        public string RAM { get; init; }

        [Required]
        [RegularExpression(StorageRegularExpression)]
        public string Storage { get; init; }

        [Required]
        [MaxLength(OSMaxLength)]
        public string OS { get; init; }
    }
}