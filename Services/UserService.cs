using AutoMapper;
using Azure;
using SoundMatchAPI.Data.DTOs.Requests;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using System.Net;

namespace SoundMatchAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMusicService musicService;
        private readonly IMatchRepository matchRepository;
        private readonly IMapper mapper;

        public UserService(IUserRepository userRepository, IMusicService musicService, IMatchRepository matchRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.musicService = musicService;
            this.matchRepository = matchRepository;
            this.mapper = mapper;
        }

        public async Task<ReturnResponse<List<UserResponse>>> GetAllUsersAsync()
        {
            try
            {
                var users = await userRepository.GetAllAsync();

                if (users == null)
                {
                    return new ReturnResponse<List<UserResponse>>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "An error occurred while fetching users",
                        Errors = new List<string> { "No users found." }
                    };
                }
                // Map each user to a UserResponse
                var userResponses = users.Select(user => mapper.Map<UserResponse>(user)).ToList();

                return new ReturnResponse<List<UserResponse>>
                {
                    Data = userResponses,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<List<UserResponse>>
                {
                    Message = "An unexpected error occurred.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        public async Task<ReturnResponse<UserResponse>> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new ReturnResponse<UserResponse>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "An error occurred while fetching user.",
                        Errors = new List<string> { "No user found." }
                    };
                }
                var userResponse = mapper.Map<UserResponse>(user);
                return new ReturnResponse<UserResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = userResponse,
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<UserResponse>
                {
                    Message = "Ett oväntat fel har inträffat.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ReturnResponse> DeleteUserAsync(string userId, string loggedInUserId)
        {
            try
            {
                if (userId != loggedInUserId)
                {
                    return new ReturnResponse
                    {
                        Message = "An error has occurred while deleting user.",
                        Errors = new List<string> { "User is not authorized to delete this resource."},
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ReturnResponse
                    {
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                await userRepository.DeleteAsync(userId);
                await matchRepository.DeleteMatchesByUserIdAsync(userId);
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse
                {
                    Message = "An error occurred while deleting user",
                    Errors = new List<string> { $"Error: {ex.Message}." },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

       
        public async Task<ReturnResponse<UserProfileResponse>> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ReturnResponse<UserProfileResponse>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "User not found",
                        Errors = new List<string> { "No user found." }
                    };
                }

                var musicProfile = await musicService.GetProfileAsync(user.FavoriteSongIds, user.FavoriteArtistIds, user.FavoriteGenreIds); // Get current music profile
                user.FavoriteSongs = musicProfile.Songs.ToList();
                user.FavoriteArtists = musicProfile.Artists.ToList();
                user.FavoriteGenres = musicProfile.Genres.ToList();

                var userProfileResponse = mapper.Map<UserProfileResponse>(user);
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = userProfileResponse
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    Message = "An unexpected error occurred.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ReturnResponse<UserProfileResponse>> UpdateUserBioAsync(string userId, string bio, string loggedInUserId)
        {
            try
            {
                if (userId != loggedInUserId)
                {
                    return new ReturnResponse<UserProfileResponse>
                    {
                        Message = "An error has occurred while updating user bio.",
                        Errors = new List<string> { "User is not authorized to update this resource." },
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ReturnResponse<UserProfileResponse>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "User not found",
                        Errors = new List<string> { "No user found." }
                    };
                }
                user.Biography = bio;
                await userRepository.UpdateAsync(user);
                var userProfileResponse = mapper.Map<UserProfileResponse>(user);
                return new ReturnResponse<UserProfileResponse>
                {
                    StatusCode = HttpStatusCode.NoContent,
                    Data = userProfileResponse
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<UserProfileResponse>
                {
                    Message = "An unexpected error occurred.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
