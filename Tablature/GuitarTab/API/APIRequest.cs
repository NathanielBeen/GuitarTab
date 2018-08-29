using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public class APIRequest
    {
        public const string BASE_ADDRESS = @"http://localhost:3000";
        public const string RATINGS = @"/ratings/";
        public const string SONGS = @"/songs/";
        public const string USERS = @"/users/";
        public const string TAGS = @"/tags/";

        static HttpClient client = new HttpClient();

        public static void configureClient()
        {
            client.BaseAddress = new Uri(BASE_ADDRESS);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void addToken(Credentials credentials)
        {
            client.DefaultRequestHeaders.Remove("x-auth-token");
            client.DefaultRequestHeaders.Add("x-auth-token", credentials.Token);
        }

        public static async Task<Result<SongModel>> getAllSongs()
        {
            var response = await client.GetAsync(SONGS);
            return await response.Content.ReadAsAsync<Result<SongModel>>();
        }

        public static async Task<IDMessageResult> createSong(SongFieldsRequest model)
        {
            var response = await client.PostAsJsonAsync(SONGS, model);
            return await response.Content.ReadAsAsync<IDMessageResult>();
        }

        public static async Task<Result<SongModel>> getSongById(int id)
        {
            var response = await client.GetAsync(SONGS + $"{id}");
            return await response.Content.ReadAsAsync<Result<SongModel>>();
        }

        public static async Task<MessageResult> updateSong(SongUpdateRequest model)
        {
            var response = await client.PatchAsJsonAsync(SONGS + $"{model.Id}", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> deleteSong(int id)
        {
            var response = await client.DeleteAsync(SONGS + $"{id}");
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> adminUpdateSong(SongUpdateRequest model)
        {
            var response = await client.PatchAsJsonAsync(SONGS + $"admin/{model.Id}", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> adminUpdateMultipleSongs(MultipleSongUpdateRequest model)
        {
            var response = await client.PatchAsJsonAsync(SONGS + "admin/multi-id", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> adminRemoveSong(int id)
        {
            var response = await client.DeleteAsync(SONGS + $"admin/{id}");
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> adminRemoveMultipleSong(MultipleSongIdRequest model)
        {
            var response = await client.DeleteAsJsonAsync(SONGS + "admin/multi-id", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<Result<SongModel>> searchSongs(SongSearchRequest model)
        {
            var response = await client.PostAsJsonAsync(SONGS + "search", model);
            return await response.Content.ReadAsAsync<Result<SongModel>>();
        }



        public static async Task<Result<RatingModel>> getAllRatings()
        {
            var response = await client.GetAsync(RATINGS);
            return await response.Content.ReadAsAsync<Result<RatingModel>>();
        }

        public static async Task<IDMessageResult> createRating(RatingFieldsRequest model)
        {
            var response = await client.PostAsJsonAsync(RATINGS, model);
            return await response.Content.ReadAsAsync<IDMessageResult>();
        }

        public static async Task<Result<RatingModel>> getRatingById(int id)
        {
            var response = await client.GetAsync(RATINGS + $"{id}");
            return await response.Content.ReadAsAsync<Result<RatingModel>>();
        }

        public static async Task<MessageResult> updateRating(RatingUpdateRequest model)
        {
            var response = await client.PostAsJsonAsync(RATINGS + $"{model.Id}", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> removeRating(int id)
        {
            var response = await client.DeleteAsync(RATINGS + $"{id}");
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> adminRemoveRating(int id)
        {
            var response = await client.DeleteAsync(RATINGS + $"/admin/{id}");
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> adminRemoveMultipleRatings(MultipleRatingIdRequest model)
        {
            var response = await client.DeleteAsJsonAsync(RATINGS + "/admin/multi-id", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<Result<RatingModel>> getRatingsBySongId(int id)
        {
            var response = await client.GetAsync(RATINGS + $"/song-ratings/{id}");
            return await response.Content.ReadAsAsync<Result<RatingModel>>();
        }

        public static async Task<Result<RatingModel>> getRatingsByUserId(int id)
        {
            var response = await client.GetAsync(RATINGS + $"/user-ratings/{id}");
            return await response.Content.ReadAsAsync<Result<RatingModel>>();
        }


        public static async Task<Result<TagModel>> getAllTags()
        {
            var response = await client.GetAsync(TAGS);
            return await response.Content.ReadAsAsync<Result<TagModel>>();
        }

        public static async Task<IDMessageResult> createTag(TagFieldsRequest model)
        {
            var response = await client.PostAsJsonAsync(TAGS, model);
            return await response.Content.ReadAsAsync<IDMessageResult>();
        }

        public static async Task<Result<TagModel>> getTagById(int id)
        {
            var response = await client.GetAsync(TAGS + $"{id}");
            return await response.Content.ReadAsAsync<Result<TagModel>>();
        }

        public static async Task<MessageResult> updateTag(TagUpdateRequest model)
        {
            var response = await client.PatchAsJsonAsync(TAGS + $"{model.Id}", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> removeTag(int id)
        {
            var response = await client.DeleteAsync(TAGS + $"{id}");
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> removeMultipleTags(MultipleTagIdRequest model)
        {
            var response = await client.DeleteAsJsonAsync(TAGS + "/multi-id", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<MessageResult> updateMultipleTags(MultipleTagUpdateRequest model)
        {
            var response = await client.PatchAsJsonAsync(TAGS + "/multi-id", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }


        public static async Task<Result<UserModel>> getAllUsers()
        {
            var response = await client.GetAsync(USERS);
            return await response.Content.ReadAsAsync<Result<UserModel>>();
        }

        public static async Task<IDMessageResult> createUser(UserFieldsRequest model)
        {
            var response = await client.PostAsJsonAsync(USERS+"signup", model);
            return await response.Content.ReadAsAsync<IDMessageResult>();
        }

        public static async Task<IDMessageResult> createAdmin(UserFieldsRequest model)
        {
            var response = await client.PostAsJsonAsync(USERS+"signup-admin", model);
            return await response.Content.ReadAsAsync<IDMessageResult>();
        }

        public static async Task<TokenIDMessageResult> login(UserLoginRequest login)
        {
            var response = await client.PostAsJsonAsync(USERS + "login", login);
            return await response.Content.ReadAsAsync<TokenIDMessageResult>();
        }

        public static async Task<Result<UserModel>> getUserById(int id)
        {
            var response = await client.GetAsync(USERS + $"{id}");
            return await response.Content.ReadAsAsync<Result<UserModel>>();
        }

        public static async Task<MessageResult> removeOwnAccount(UserUpdateRequest login)
        {
            var response = await client.DeleteAsJsonAsync(USERS, login);
            return await response.Content.ReadAsAsync<MessageResult>();
        }

        public static async Task<TokenIDMessageResult> adminRemoveOtherAccount(UserUpdateRequest model)
        {
            var response = await client.DeleteAsJsonAsync(USERS + $"admin/{model.Id}", model);
            return await response.Content.ReadAsAsync<TokenIDMessageResult>();
        }

        public static async Task<TokenIDMessageResult> changePassword(ChangePasswordRequest login)
        {
            var response = await client.PatchAsJsonAsync(USERS + "/change-password", login);
            return await response.Content.ReadAsAsync<TokenIDMessageResult>();
        }

        public static async Task<MessageResult> changeUserType(UserUpdateRequest model)
        {
            var response = await client.PatchAsJsonAsync(USERS + "/change-user-type", model);
            return await response.Content.ReadAsAsync<MessageResult>();
        }
    }

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data)
            => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data) });

        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(client.BaseAddress + requestUri),
                Content = content,
            };

            return client.SendAsync(request);
        }

        public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string request, T data)
            => client.PatchAsync(request, Serialize(data));

        private static HttpContent Serialize(object data) => new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
    }
}
