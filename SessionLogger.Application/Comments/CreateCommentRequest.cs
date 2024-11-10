namespace SessionLogger.Comments;

public record CreateCommentRequest(Guid TaskId, string Value, Guid? ParentId = null);