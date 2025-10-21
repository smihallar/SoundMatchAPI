using AutoMapper;
using Azure;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Models;
using System.Net;

namespace SoundMatchAPI.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<ReturnResponse<IEnumerable<UserResponse>>> GetAllUsersAsync()
        {
            try
            {
                var users = await userRepository.GetAllAsync();

                if (users == null)
                {
                    return new ReturnResponse<IEnumerable<UserResponse>>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "An error occurred while fetching users",
                        Errors = new List<string> { "No users found." }
                    };
                }
                var userResponses = new List<UserResponse>();
                foreach (var user in users)
                {
                    mapper.Map(user, userResponses);
                }
                return new ReturnResponse<IEnumerable<UserResponse>>
                {
                    Data = userResponses,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<IEnumerable<UserResponse>>
                {
                    Message = "An unexpected error occurred.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        public async Task<ReturnResponse<UserResponse?>> GetUserWithDetailsByIdAsync(string id)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new ReturnResponse<UserResponse?>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "An error occurred while fetching user.",
                        Errors = new List<string> { "No user found." }
                    };
                }
                var userResponse = mapper.Map<UserResponse>(user);
                return new ReturnResponse<UserResponse?>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = userResponse,
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<UserResponse?>
                {
                    Message = "Ett oväntat fel har inträffat.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ReturnResponse> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ReturnResponse
                    {
                        Message = "An error has occurred while fetching user",
                        Errors = new List<string> {"User not found" },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                await userRepository.DeleteAsync(userId);
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

        public async Task<ReturnResponse> UpdateUserAsync(string userId)
        {
            try
            {
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ReturnResponse
                    {
                        Message = "An error has occurred while fetching user",
                        Errors = new List<string> { "User not found" },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                await userRepository.UpdateAsync(user);
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse
                {
                    Message = "An error occurred while updating user",
                    Errors = new List<string> { $"Error: {ex.Message}." },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        // Connect to spotify (update user with spotify information + set IsConnectedToSpotify to true)
        public async Task<ReturnResponse> ConnectUserToSpotifyAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    return new ReturnResponse
                    {
                        Message = "An error has occurred while fetching user",
                        Errors = new List<string> { "User not found" },
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                user.IsConnectedToSpotify = true;
                await userRepository.UpdateAsync(user);
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse
                {
                    Message = "An error occurred while connecting user to Spotify",
                    Errors = new List<string> { $"Error: {ex.Message}." },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
