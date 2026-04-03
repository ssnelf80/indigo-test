namespace IndigoTestTask.DAL.Outbox.Entities;

public record OutboxMessage(long Id, byte[] Message);