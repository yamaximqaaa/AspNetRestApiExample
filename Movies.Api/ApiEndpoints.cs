namespace Movies.Api;

public class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class V1
    {
        private const string VersionBase = $"{ApiBase}/v1";
        public class Movies
        {
            private const string Base = $"{VersionBase}/movies";

            public const string Create = Base;
            public const string Get = $"{Base}/{{idOrSlug}}";
            public const string GetAll = Base;
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";

            public const string Rate = $"{Base}/{{movieId:guid}}/ratings";
            public const string DeleteRating = $"{Base}/{{movieId:guid}}/ratings";
        }

        public static class Ratings
        {
            private const string Base = $"{VersionBase}/ratings";
        
            public const string GetUserRatings = $"{Base}/me";
            public const string Create = Base;
        }
    }
    
    public static class V2
    {
        private const string VersionBase = $"{ApiBase}/v2";
        
        // -- New endpoints with new contracts ... -- 
        
        public class Movies
        {
            private const string Base = $"{VersionBase}/movies";

            public const string Create = Base;
            public const string Get = $"{Base}/{{idOrSlug}}";
            public const string GetAll = Base;
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";

            public const string Rate = $"{Base}/{{movieId:guid}}/ratings";
            public const string DeleteRating = $"{Base}/{{movieId:guid}}/ratings";
        }
        
        public static class Ratings
        {
            private const string Base = $"{VersionBase}/ratings";
        
            public const string GetUserRatings = $"{Base}/me";
            public const string Create = Base;
        }
        
    }
    
}