using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoundMatchAPI.Data.DTOs.Requests
{
    // Used for updating existing user with spotify details
    public class UserUpdateRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty; // Spotify username (Spotify API: Spotify user id)

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string SpotifyUserId { get; set; } = string.Empty; // Spotify API: Spotify Id

        [StringLength(2, MinimumLength = 2)]
        public string CountryCode { get; set; } = string.Empty; // ex. "US", "GB"

        public DateTime CreatedAt { get; set; }

        [Url]
        public string ProfilePictureUrl { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Biography { get; set; } = string.Empty;

        public bool IsSynthetic { get; set; }
        public bool IsConnectedToSpotify { get; set; }
        public DateTime MusicTasteLastRefreshed { get; set; }

        public List<string> FavoriteSongIds { get; set; } = new List<string>();
        public List<string> FavoriteArtistIds { get; set; } = new List<string>();
        public List<string> FavoriteGenreIds { get; set; } = new List<string>();
    }
}
