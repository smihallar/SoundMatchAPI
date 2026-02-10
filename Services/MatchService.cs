using AutoMapper;
using SoundMatchAPI.Data.DTOs.Responses;
using SoundMatchAPI.Data.Interfaces.RepositoryInterfaces;
using SoundMatchAPI.Data.Interfaces.ServiceInterfaces;
using SoundMatchAPI.Data.Models;
using SoundMatchAPI.Data.Repositories;
using System.Net;

namespace SoundMatchAPI.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository matchRepository;
        private readonly IUserRepository userRepository;
        private readonly IMusicService musicService;
        private readonly IMapper mapper;
        public MatchService(IMatchRepository matchRepository, IUserRepository userRepository, IMusicService musicService, IMapper mapper)
        {
            this.matchRepository = matchRepository;
            this.userRepository = userRepository;
            this.musicService = musicService;
            this.mapper = mapper;
        }
        public async Task<ReturnResponse<MatchResponse>> GetMatchByIdWithDetailsAsync(string matchId, string loggedInUserId)
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
                if (match.InitiatorUserId != loggedInUserId && match.RecipientUserId != loggedInUserId)
                {
                    return new ReturnResponse<MatchResponse>
                    {
                        Message = "An error has occurred while fetching match.",
                        Errors = new List<string> { "User is not authorized to access this resource." },
                        StatusCode = HttpStatusCode.Forbidden
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

        public async Task<ReturnResponse<List<MatchResponse>>> AddMatches(string userId, string loggedInUserId)
        {
            try
            {
                if (userId != loggedInUserId)
                {
                    return new ReturnResponse<List<MatchResponse>>
                    {
                        Message = "An error has occurred while creating matches.",
                        Errors = new List<string> { "User is not authorized to create these resources." },
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
                var allUsers = await userRepository.GetAllAsync();
                var initiatorUser = await userRepository.GetByIdAsync(userId);

                if (initiatorUser == null || allUsers == null)
                {
                    return new ReturnResponse<List<MatchResponse>>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "User(s) not found.",
                        Errors = new List<string> { "No user(s) found." }
                    };
                }

                var initiatorProfile = await musicService.GetProfileAsync(
                    initiatorUser.FavoriteSongIds ?? new List<string>(),
                    initiatorUser.FavoriteArtistIds ?? new List<string>(),
                    initiatorUser.FavoriteGenreIds ?? new List<string>()
                );

                var initiatorSongs = initiatorProfile.Songs;
                var initiatorArtists = initiatorProfile.Artists;
                var initiatorGenres = initiatorProfile.Genres;

                var candidates = allUsers
                    .Where(u => u?.Id != null && u.Id != userId && u.IsConnectedToSpotify)
                    .ToList();

                var newMatches = new List<Match>();
                var updatedMatches = new List<Match>();

                foreach (var candidate in candidates)
                {
                    var result = await ProcessCandidateAsync(
                        candidate, initiatorUser, initiatorSongs, initiatorArtists, initiatorGenres);

                    if (result.newMatch != null)
                        newMatches.Add(result.newMatch);
                    if (result.updatedMatch != null)
                        updatedMatches.Add(result.updatedMatch);
                }

                initiatorUser.MatchIdsAsInitiator.AddRange(newMatches.Select(m => m.MatchId));
                initiatorUser.MatchIdsAsInitiator = initiatorUser.MatchIdsAsInitiator.Distinct().ToList();
                await userRepository.UpdateAsync(initiatorUser);

                await matchRepository.AddMatchesAsync(newMatches);

                var allMatchesForResponse = newMatches.Concat(updatedMatches).ToList();
                var matchResponses = allMatchesForResponse.Select(m => mapper.Map<MatchResponse>(m)).ToList();

                return new ReturnResponse<List<MatchResponse>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = matchResponses
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<List<MatchResponse>>
                {
                    Message = "An error has occurred while creating matches.",
                    Errors = new List<string> { ex.Message },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        // Helper to process each candidate
        private async Task<(Match? newMatch, Match? updatedMatch)> ProcessCandidateAsync(
            User candidate,
            User initiatorUser,
            IEnumerable<Song> initiatorSongs,
            IEnumerable<Artist> initiatorArtists,
            IEnumerable<Genre> initiatorGenres)
        {
            var candidateProfile = await musicService.GetProfileAsync(
                candidate.FavoriteSongIds ?? new List<string>(),
                candidate.FavoriteArtistIds ?? new List<string>(),
                candidate.FavoriteGenreIds ?? new List<string>()
            );

            var candidateSongs = candidateProfile.Songs;
            var candidateArtists = candidateProfile.Artists;
            var candidateGenres = candidateProfile.Genres;

            var mutualSongs = GetMutualSongs(initiatorSongs, candidateSongs);
            var mutualArtists = GetMutualArtists(initiatorArtists, candidateArtists);
            var mutualGenres = GetMutualGenres(initiatorGenres, candidateGenres);

            int totalScore = CalculateTotalScore(mutualSongs, mutualArtists, mutualGenres);
            if (totalScore == 0) return (null, null);

            var existingMatch = await matchRepository.GetExistingMatchAsync(initiatorUser.Id, candidate.Id)
                ?? await matchRepository.GetExistingMatchAsync(candidate.Id, initiatorUser.Id);
            if (existingMatch != null)
            {
                UpdateExistingMatch(existingMatch, mutualSongs, mutualArtists, mutualGenres);
                await matchRepository.UpdateAsync(existingMatch);
                return (null, existingMatch);
            }

            var match = CreateNewMatch(initiatorUser, candidate, mutualSongs, mutualArtists, mutualGenres, totalScore);
            candidate.MatchIdsAsRecipient.Add(match.MatchId);
            candidate.MatchIdsAsRecipient = candidate.MatchIdsAsRecipient.Distinct().ToList();
            candidate.MatchesAsRecipient.Add(match);
            candidate.MatchesAsRecipient = candidate.MatchesAsRecipient.Distinct().ToList();
            await userRepository.UpdateAsync(candidate);

            return (match, null);
        }

        // Helper methods
        private List<Song> GetMutualSongs(IEnumerable<Song> initiator, IEnumerable<Song> candidate)
        {
            var candidateIds = candidate.Where(s => s.SpotifyId != null).Select(s => s.SpotifyId!).ToHashSet();
            return initiator.Where(s => s.SpotifyId != null && candidateIds.Contains(s.SpotifyId!)).ToList();
        }

        private List<Artist> GetMutualArtists(IEnumerable<Artist> initiator, IEnumerable<Artist> candidate)
        {
            var candidateIds = candidate.Where(a => a.SpotifyId != null).Select(a => a.SpotifyId!).ToHashSet();
            return initiator.Where(a => a.SpotifyId != null && candidateIds.Contains(a.SpotifyId!)).ToList();
        }

        private List<Genre> GetMutualGenres(IEnumerable<Genre> initiator, IEnumerable<Genre> candidate)
        {
            var candidateNames = candidate.Select(g => g.Name.ToLower()).ToHashSet();
            return initiator.Where(g => candidateNames.Contains(g.Name.ToLower())).ToList();
        }

        private int CalculateTotalScore(List<Song> songs, List<Artist> artists, List<Genre> genres)
        {
            int songScore = songs.Sum(s => GetPopularityScoreForCompatibility(s.Popularity ?? 100)) + songs.Count * 3;
            int artistScore = artists.Sum(a => GetPopularityScoreForCompatibility(a.Popularity ?? 100)) + artists.Count * 2;
            int genreScore = genres.Count;
            return songScore + artistScore + genreScore;
        }

        private void UpdateExistingMatch(Match match, List<Song> newMutualSongs, List<Artist> newMutualArtists, List<Genre> newMutualGenres)
        {
            var songsToAdd = newMutualSongs.Where(s => match.MutualSongs.All(ms => ms.SpotifyId != s.SpotifyId)).ToList();
            var artistsToAdd = newMutualArtists.Where(a => match.MutualArtists.All(ma => ma.SpotifyId != a.SpotifyId)).ToList();
            var genresToAdd = newMutualGenres.Where(g => match.MutualGenres.All(mg => !mg.Name.Equals(g.Name, StringComparison.OrdinalIgnoreCase))).ToList();

            match.MutualSongs.AddRange(songsToAdd);
            match.MutualArtists.AddRange(artistsToAdd);
            match.MutualGenres.AddRange(genresToAdd);

            int scoreIncrease = songsToAdd.Sum(s => GetPopularityScoreForCompatibility(s.Popularity ?? 100)) + songsToAdd.Count * 3
                                + artistsToAdd.Sum(a => GetPopularityScoreForCompatibility(a.Popularity ?? 100)) + artistsToAdd.Count * 2
                                + genresToAdd.Count;

            match.CompatibilityScore += scoreIncrease;
        }

        private Match CreateNewMatch(User initiator, User candidate, List<Song> mutualSongs, List<Artist> mutualArtists, List<Genre> mutualGenres, int totalScore)
        {
            return new Match
            {
                MatchId = Guid.NewGuid().ToString(),
                InitiatorUserId = initiator.Id,
                RecipientUserId = candidate.Id,
                CompatibilityScore = totalScore,
                CreatedAt = DateTime.UtcNow,
                InitiatorUser = initiator,
                RecipientUser = candidate,
                MutualSongs = mutualSongs,
                MutualArtists = mutualArtists,
                MutualGenres = mutualGenres
            };
        }

        public async Task<ReturnResponse<List<MatchResponse>>> GetMatchesByUserIdAsync(string userId, string loggedInUserId)
        {
            try
            {
                if (userId != loggedInUserId)
                {
                    return new ReturnResponse<List<MatchResponse>>
                    {
                        Message = "An error has occurred while fetching matches.",
                        Errors = new List<string> { "User is not authorized to access these resources." },
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
                var matches = await matchRepository.GetMatchesWithDetailsByUserIdAsync(userId);
                if (matches == null || !matches.Any())
                {
                    return new ReturnResponse<List<MatchResponse>>
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
                
                return new ReturnResponse<List<MatchResponse>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Data = matchResponses,
                };
            }
            catch (Exception ex)
            {
                return new ReturnResponse<List<MatchResponse>>
                {
                    Message = "An unexpected error has occurred.",
                    Errors = new List<string> { $"Error: {ex.Message}" },
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ReturnResponse> DeleteMatchAsync(string matchId, string loggedInUserId)
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
                if (match.InitiatorUserId != loggedInUserId && match.RecipientUserId != loggedInUserId)
                {
                    return new ReturnResponse
                    {
                        Message = "An error has occurred while deleting the match.",
                        Errors = new List<string> { "User is not authorized to delete this resource." },
                        StatusCode = HttpStatusCode.Forbidden
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
