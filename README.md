## Music App

A web application where users can upload their own songs, create playlists and listen to other users uploaded songs.

## Tech-stack

The project uses:
- .NET 9 for the backend
- React for the frontend
- PostgreSQL as the database
- Neon for the database provider
- CloudFlare R2 for saving songs
- fly.io for deployment

## Architecture

Frontend (React):
  - Handles user interactions, authentication, and playlist management.
  - Communicates with the backend via REST APIs endpoints.
  - Provides updates for song uploads, playlist changes, and streaming progress.

Backend (.NET 9):
  - Serves as the API layer, managing authentication, authorization, and business logic.
  - Handles file uploads for songs and stores metadata in the database.

Database (PostgreSQL):
  - Stores user data, playlists, song metadata, and relationships between users and content.
  - Optimized with indexes for search queries and supports relational integrity for playlists and song ownership.

## Feature plan

### Week 5
*Kick-off week - no features to be planned here*

### Week 6
**Feature 1:** Initial backend and frontend

**Feature 2:** Login, Create Account and Home Page

### Week 7
*Winter vacation - nothing planned.*

### Week 8
**Feature 1:** Database Setup

**Feature 2:** User Login

**Feature 3:** JWT Creation, Validation

### Week 9
**Feature 1:** Song Upload Page

**Feature 2:** Saving songs to CloudFlare

### Week 10
**Feature 1:** Saving songs to DB

**Feature 2:** Retrieving user's songs

### Week 11
**Feature 1:** [...]

**Feature 2:** [...]

### Week 12
**Feature 1:** [...]

**Feature 2:** [...]

### Week 13
**Feature 1:** [...]

**Feature 2:** [...]

### Week 14
*Easter vacation - nothing planned.*

### Week 15
**Feature 1:** [...]

**Feature 2:** [...]

### Week 16
**Feature 1:** [...]

**Feature 2:** [...]

### Week 17
**Feature 1:** [...]

**Feature 2:** [...]
