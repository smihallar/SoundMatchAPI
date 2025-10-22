using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces;
using SoundMatchAPI.Data.Models;
using SoundMatchAPI.Data.Repositories;
using System.Net;

namespace SoundMatchAPI.Services
{
    public class MatchService
    {
        private readonly IMatchRepository matchRepository;
        private readonly IUserRepository userRepository;
        private readonly MusicProfileService musicProfileService;
        private readonly IMapper mapper;
        public MatchService(IMatchRepository matchRepository, IUserRepository userRepository, MusicProfileService musicProfileService, IMapper mapper)
        {
            this.matchRepository = matchRepository;
            this.userRepository = userRepository;
            this.musicProfileService = musicProfileService;
            this.mapper = mapper;
        }
        public async Task<ReturnResponse<MatchResponse>> GetMatchByIdWithDetailsAsync(string matchId)
        {
            try
            {
                var match = await matchRepository.GetMatchWithDetailsByIdAsync(matchId);
                if (match == null)
                {
                    return new ReturnResponse<MatchResponse>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "An error occurred while fetching match.",
                        Errors = new List<string> { "No match found." }
                    };
                }
                var matchResponse = mapper.Map<MatchResponse>(match);
                return new ReturnResponse<MatchResponse>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = matchResponse,
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<MatchResponse>
                {
                    Message = "Ett oväntat fel har inträffat.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private int GetPopularityScoreForCompatibility(int popularity) // Calculates compatibility score based on popularity 
        {
            if (popularity >= 90) return 0;
            if (popularity >= 80) return 1;
            if (popularity >= 70) return 2;
            if (popularity >= 60) return 3;
            if (popularity >= 50) return 4;
            if (popularity >= 40) return 5;
            if (popularity >= 30) return 6;
            if (popularity >= 20) return 7;
            if (popularity >= 10) return 8;
            return 9;
        }

        public async Task<ReturnResponse<IEnumerable<MatchResponse>>> AddMatches(string userId)
        {
            try
            {
                var allUsers = await userRepository.GetAllAsync(); // Get all users to find matches
                var initiatorUser = await userRepository.GetByIdAsync(userId); // Get user object for whom to find matches

                if (initiatorUser == null || allUsers == null)
                {
                    return new ReturnResponse<IEnumerable<MatchResponse>>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "User(s) not found.",
                        Errors = new List<string> { "No user(s) found." }
                    };
                }

                var matches = new List<Match>();
                foreach (var candidate in allUsers) // Iterate through all users to find matches
                {
                    if (candidate?.Id == null) continue; // Skip if candidate is null
                    if (candidate.Id == userId) continue; // Skip if candidate is the same as initiator
                    if (candidate.IsConnectedToSpotify == false) continue; // Skip if candidate is not connected to Spotify

                    // Get music profiles for both users, based on their favorite songs, artists, genres
                    var candidateProfile = await musicProfileService.GetProfileAsync(
                        candidate.FavoriteSongIds ?? new List<string>(),
                        candidate.FavoriteArtistIds ?? new List<string>(),
                        candidate.FavoriteGenreIds ?? new List<string>());

                    var initiatorProfile = await musicProfileService.GetProfileAsync(
                        initiatorUser.FavoriteSongIds ?? new List<string>(),
                        initiatorUser.FavoriteArtistIds ?? new List<string>(),
                        initiatorUser.FavoriteGenreIds ?? new List<string>());

                    var candidateSongs = candidateProfile.Songs;
                    var candidateArtists = candidateProfile.Artists;
                    var candidateGenres = candidateProfile.Genres;

                    var initiatorSongs = initiatorProfile.Songs;
                    var initiatorArtists = initiatorProfile.Artists;
                    var initiatorGenres = initiatorProfile.Genres;

                    // Find mutual songs, artists, genres by ID
                    var mutualSongs = initiatorSongs
                        .Where(s => s.SongId != null && candidateSongs.Any(cs => cs.SongId != null && cs.SongId == s.SongId)).ToList();
                    var mutualArtists = initiatorArtists
                        .Where(a => a.ArtistId != null && candidateArtists.Any(ca => ca.ArtistId != null && ca.ArtistId == a.ArtistId)).ToList();
                    var mutualGenres = initiatorGenres
                        .Where(g => g.GenreId != null && candidateGenres.Any(cg => cg.GenreId != null && cg.GenreId == g.GenreId)).ToList();

                    // Calculate popularity-based score for songs and artists, default 100
                    int songScore = mutualSongs.Sum(s => GetPopularityScoreForCompatibility(s.Popularity ?? 100));
                    int artistScore = mutualArtists.Sum(a => GetPopularityScoreForCompatibility(a.Popularity ?? 100));
                    int genreScore = mutualGenres.Count; // 1 point per mutual genre

                    int totalScore = songScore + artistScore + genreScore; // Total compatibility score

                    if (totalScore > 0) // No match if there is no mutual music taste
                    {
                        matches.Add(new Match
                        {
                            MatchId = Guid.NewGuid().ToString(),
                            InitiatorUserId = initiatorUser.Id,
                            RecipientUserId = candidate.Id,
                            CompatibilityScore = totalScore,
                            CreatedAt = DateTime.UtcNow,
                            InitiatorUser = initiatorUser,
                            RecipientUser = candidate,
                            MutualSongs = mutualSongs,
                            MutualArtists = mutualArtists,
                            MutualGenres = mutualGenres
                        });
                    }
                    candidate.MatchIdsAsRecipient.AddRange(matches.Select(m => m.MatchId)); // Add matches to candidate
                    await userRepository.UpdateAsync(candidate);
                }
                initiatorUser.MatchIdsAsInitiator.AddRange(matches.Select(m => m.MatchId)); // Add matches to initiator
                await userRepository.UpdateAsync(initiatorUser);

                await matchRepository.AddMatchesAsync(matches); // Save matches to repository

                var matchResponses = matches.Select(m => mapper.Map<MatchResponse>(m)).ToList(); // Map matches to MatchResponse DTOs

                return new ReturnResponse<IEnumerable<MatchResponse>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = matchResponses
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<IEnumerable<MatchResponse>>
                {
                    Message = "An error has occurred while creating matches.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ReturnResponse<IEnumerable<MatchResponse>>> GetMatchesByUserIdAsync(string userId)
        {
            try
            {
                var matches = await matchRepository.GetMatchesWithDetailsByUserIdAsync(userId);
                if (matches == null || !matches.Any())
                {
                    return new ReturnResponse<IEnumerable<MatchResponse>>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "No matches found for the user.",
                        Errors = new List<string> { "No matches available." }
                    };
                }
                var matchResponses = matches
                    .Where(m => m != null)
                    .Select(m => mapper.Map<MatchResponse>(m))
                    .ToList();
                return new ReturnResponse<IEnumerable<MatchResponse>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = matchResponses,
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<IEnumerable<MatchResponse>>
                {
                    Message = "An unexpected error has occurred.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ReturnResponse> DeleteMatchAsync (string matchId)
        {
            try
            {
                var match = await matchRepository.GetByIdAsync(matchId);
                if (match == null)
                {
                    return new ReturnResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Match not found.",
                        Errors = new List<string> { "No match found to delete." }
                    };
                }
                await matchRepository.DeleteAsync(matchId);
                return new ReturnResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Match deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse
                {
                    Message = "An unexpected error has occurred while deleting the match.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
