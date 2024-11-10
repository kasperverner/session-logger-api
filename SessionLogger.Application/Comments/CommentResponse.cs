namespace SessionLogger.Comments;

public record CommentResponse(Guid Id, Guid? ParentId, AuthorResponse Author, string Value, DateTime CreatedDate);

public record AuthorResponse(Guid Id, string Name);