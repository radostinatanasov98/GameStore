namespace GameStore.Data
{
    public class DataConstants
    {
        public class Shared
        {
            public const string ImageUrlRegularExpression = "(https:\\/\\/)([^\\s([\"<,>/]*)(\\/)[^\\s[\",><]*(.png|.jpg)(\\?[^\\s[\",><]*)?";
            public const string InvalidUrlErrorMessage = "Url should be in valid format and end in .jpg or .png.";
        }

        public class User
        {
            public const int NameMaxLength = 20;
        }

        public class Client
        {
            public const int NameMaxLength = 40;
            public const string DefaultProfilePictureUrl = "https://www.clipartkey.com/mpngs/m/152-1520367_user-profile-default-image-png-clipart-png-download.png";
            public const int DescriptionMaxLength = 500;
        }

        public class Publisher
        {
            public const int NameMaxLength = 20;
        }

        public class Game
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 50;
            public const int DescriptionMinLength = 10;
            public const int DescriptionMaxLength = 5000;
            public const int UrlMaxLength = 25;
            public const string TrailerUrlRegularExpression = @"^(http:\/\/|https:\/\/)(youtu\.be|www\.youtube\.com)\/watch\?v=([\w\/]+)([\?].*)?$";
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
            public const int CPUMaxLength = 50;
            public const int CPUMinLength = 10;
            public const int GPUMaxLength = 50;
            public const int GPUMinLength = 10;
            public const int OSMaxLength = 25;
            public const int OSMinLength = 10;
            public const string StorageRegularExpression = "[0-9]{1,3}(TB|MB|GB)";
            public const string MemoryRegularExpression = "[0-9]{1,3}(MB|GB)";
        }

        public class Review
        {
            public const int CaptionLength = 25;
            public const int ContentMinLength = 15;
            public const int ContentMaxLength = 1000;
        }
    }
}
