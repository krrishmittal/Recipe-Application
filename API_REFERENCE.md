# API Reference

This document explains the API response shape and the current endpoint groups in the project.

## Response Format

The backend uses a generic wrapper:

```json
{
  "success": true,
  "message": "Operation completed successfully.",
  "data": {},
  "errors": null
}
```

Failure shape:

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": [
    {
      "code": 400,
      "location": "FieldName",
      "detail": "Problem description"
    }
  ]
}
```

## Authentication

JWT bearer authentication is used.

Protected routes expect:

```http
Authorization: Bearer <token>
```

The token contains:

- user id
- email
- name
- role

## Auth Endpoints

Base route: `api/auth`

### `POST /register`

Creates a normal user account.

Request:

```json
{
  "name": "Jane",
  "email": "jane@example.com",
  "password": "Password123!"
}
```

Response data:

- token
- email
- name
- role
- expiresAt

### `POST /login`

Authenticates a user.

### `POST /forgot-password`

Starts the OTP password reset flow.

### `POST /reset-password`

Resets password using email + OTP.

### `GET /me`

Returns the current user profile.

Profile response includes:

- `id`
- `name`
- `email`
- `role`
- `bio`
- `profileImageUrl`
- `createdAt`
- `recipeCount`
- `favoriteCount`

### `PUT /me`

Updates the current profile.

Form-data fields:

- `name`
- `bio`
- `profileImage`

### `POST /change-password`

Changes the current password.

### `POST /delete-account`

Deletes the current account.

## Recipe Endpoints

Base route: `api/recipes`

### `GET /`

Returns public published recipes only.

Supported query params:

- `page`
- `pageSize`
- `search`
- `sortBy`
- `sortOrder`
- `category`
- `tag`

### `GET /my-recipes`

Returns the authenticated user's recipes, including unpublished ones.

### `GET /categories`

Returns the category name list.

### `GET /tags`

Returns the tag name list.

### `GET /favorites`

Returns the authenticated user's favorites, filtered to published recipes.

### `GET /{id}`

Returns a published recipe by id.

Recipe response includes:

- base recipe fields
- `category`
- `tags`
- `averageRating`
- `ratingCount`
- `comments`

### `POST /`

Creates a recipe.

Form-data fields:

- `title`
- `description`
- `prepTimeMinutes`
- `cookTimeMinutes`
- `ingredients`
- `steps`
- `image`
- `category`
- `tags` (comma-separated)

### `PUT /{id}`

Updates a recipe owned by the current user.

### `DELETE /{id}`

Deletes a recipe owned by the current user.

Admins can also delete any recipe through admin routes.

### `POST /{id}/favorite`

Adds a recipe to favorites.

### `DELETE /{id}/favorite`

Removes a recipe from favorites.

### `POST /{id}/comments`

Adds a comment to a recipe.

Request:

```json
{
  "content": "Looks great."
}
```

### `PUT /comments/{commentId}`

Updates the current user's comment.

Admins can also update comments.

### `DELETE /comments/{commentId}`

Deletes the current user's comment.

Admins can also delete comments.

### `POST /{id}/ratings`

Adds or updates the current user's rating.

Request:

```json
{
  "value": 5
}
```

Rating is numeric and validated from `1` to `5`.

### `DELETE /{id}/ratings`

Removes the current user's rating.

## Admin Endpoints

Base route: `api/admin`

All admin routes require:

- authenticated user
- role = `Admin`

### `GET /users`

Returns paginated users.

### `PATCH /users/{id}/role`

Current strict behavior:

- API does not allow promoting a user to admin
- it only allows role updates back to `User`
- the last admin cannot be demoted

Request:

```json
{
  "role": "User"
}
```

### `DELETE /users/{id}`

Deletes a user.

The last admin cannot be deleted.

### `GET /recipes`

Returns paginated recipes for admin moderation.

This route can include unpublished recipes.

### `PATCH /recipes/{id}/publish`

Publishes or unpublishes a recipe.

Request:

```json
{
  "isPublished": true
}
```

### `PATCH /recipes/{id}/feature`

Marks a recipe as featured or not featured.

Request:

```json
{
  "isFeatured": true
}
```

### `DELETE /recipes/{id}`

Deletes any recipe as admin.

## Notes

- IDs are `Guid`
- recipes are hard-deleted
- users are hard-deleted
- comments and ratings are hard-deleted
- public recipe endpoints filter unpublished recipes out
- startup auto-applies migrations through `MigrationManager`
