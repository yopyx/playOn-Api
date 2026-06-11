using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using ServiceLayer.CommentServices;
using ServiceLayer.VideoServices;
using static BFF.Utils;

namespace BFF.AuthEndpoints 
{
    public static class ProtectedResourcesEndpoints
    {
        public static void MapProtectedResourcesEndpoints(this WebApplication app) {

            app.MapGet("/resources_api/video_categories", [Authorize] async (HttpContext http, IHttpClientFactory factory,IConfiguration configuration) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                
                var response = await client.GetAsync("video_categories");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<IEnumerable<VideoCategoryDto>>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/recommended_videos", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration, string VideoCategoryId, string pageNum) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"recommended_videos?VideoCategoryId={VideoCategoryId}&pageNum={pageNum}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<VideoCardListDtoEnumerable>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/search_suggestions", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration,string q) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"search_suggestions?q={q}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<string[]>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/search_results", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration, string q, string pageNum) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"search_results?q={q}&pageNum={pageNum}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<VideoCardListDtoEnumerable>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/watch_video", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration, string videoId) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"watch_video?videoId={videoId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<VideoCardDto>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/comments", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration, string videoId, string pageNum) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"comments?videoId={videoId}&pageNum={pageNum}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<CommentsListDtoEnumerable>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/replies", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration, string commentId) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"replies?commentId={commentId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<IEnumerable<NestedCommentDto>>();
                    return TypedResults.Ok(json);
                }

                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/liveChat", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration, string videoId, string pageNum) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync($"liveChat?videoId={videoId}&pageNum={pageNum}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<LiveCommentListDtoEnumerable>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });

            app.MapGet("/resources_api/feed_history", [Authorize] async (HttpContext http, IHttpClientFactory factory, IConfiguration configuration) => {
                // Token was validated/refreshed by middleware; just pull it from context
                var accessToken = (http.Items["access_token"]) as string;

                var client = factory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync("feed_history");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<Dictionary<DateOnly, IEnumerable<VideoCardDto>>>();
                    return TypedResults.Ok(json);
                }
                //return Results.Problem($"API call failed: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();

                return Results.Content(
                    content,
                    "application/problem+json",
                    null,
                    (int)response.StatusCode
                );
            });
        }
    }
}
