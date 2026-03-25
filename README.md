# SoundMatch API

A .NET 8 Web API that connects music lovers based on their Spotify listening habits. Users are matched with others who share similar musical tastes, with compatibility scores calculated based on mutual songs, artists, and genres.

## ?? Features

- **Spotify Integration** – Connect your Spotify account to import your top songs, artists, and genres
- **Music-Based Matching** – Find users with similar music taste using a weighted compatibility algorithm
- **JWT Authentication** – Secure API endpoints with token-based authentication
- **User Profiles** – View and manage user profiles with music preferences and bio
- **Real-time Chat** *(Coming Soon)* – Chat with your matches

## ??? Tech Stack

- **.NET 8** – ASP.NET Core Web API
- **Entity Framework Core** – SQL Server database with Code-First migrations
- **ASP.NET Identity** – User authentication and management
- **AutoMapper** – Object-to-object mapping
- **Swagger/OpenAPI** – API documentation
- **Spotify Web API** – Music data integration

## ?? Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server)
- [Spotify Developer Account](https://developer.spotify.com/) with registered application
- NUGET PACKAGES:
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore
  - AutoMapper.Extensions.Microsoft.DependencyInjection
  - Swashbuckle.AspNetCore
  - Microsoft.AspNetCore.Authentication.JwtBearer
  - Microsoft.EntityFrameworkCore.Tools

## ?? Configuration

### 1. App Settings

Configure `appsettings.json` with JWT settings and store sensitive credentials in a safe location (ex user secrets).

### 2. User Secrets

Store sensitive credentials with User secrets (https://learn.microsoft.com/aspnet/core/security/app-secrets).

## ?? Getting Started

1. **Clone the repository**

2. **Install .NET 8 SDK**

3. **Configure App Settings**

4. **Configure User Secrets**

Connection string, Spotify API credentials, and JWT secret key.

5. **Run the Application**


## ????? API Endpoints

**AUTH**

POST
/api/Auth/register


POST
/api/Auth/login


**MATCH**


GET
/api/Match/{matchId}


DELETE
/api/Match/{matchId}


POST
/api/Match/find-matches/{userId}


GET
/api/Match/all/{userId}


**SPOTIFY**


GET
/api/Spotify/login/{userId}


GET
/api/Spotify/callback


POST
/api/Spotify/refresh-top-items


POST
/api/Spotify/refresh-profile


**USER**


GET
/api/User/{userId}


DELETE
/api/User/{userId}


PUT
/api/User/bio/{userId}


GET
/api/User/profile/{userId}

## ?? Matching Algorithm

Compatibility scores are calculated based on shared music preferences:

Mutual Songs: 3p each
Mutual Artists: 2p each
Mutual Genres: 1p each

+ Popularity bonus (less popular items score higher)