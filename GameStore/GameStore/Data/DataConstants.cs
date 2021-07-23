namespace GameStore.Data
{
    public class DataConstants
    {
        public class Client
        {
            public const int NameMaxLength = 40;
        }

        public class Publisher
        {
            public const int NameMaxLength = 20;
        }

        public class Game
        {
            public const int NameMaxLength = 25;
            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 5000;
            public const int UrlMaxLength = 25;
        }

        public class Genre
        {
            public const int NameMaxLength = 15;
        }

        public class PEGIRating
        {
            public const int NameMaxLength = 7;
        }

        public class Requirements
        {
            public const int CPULength = 50;
            public const int GPULength = 50;
            public const int OSLength = 25;
        }

        public class Review
        {
            public const int CaptionLength = 25;
            public const int ContentMinLength = 15;
            public const int ContentMaxLength = 1000;
        }
    }
}
