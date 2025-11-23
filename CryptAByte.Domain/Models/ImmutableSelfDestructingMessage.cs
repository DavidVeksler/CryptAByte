using System;
using CryptAByte.Domain.DataContext;
using CryptAByte.Domain.Functional;

namespace CryptAByte.Domain.Models
{
    /// <summary>
    /// Immutable representation of a self-destructing message for business logic.
    /// </summary>
    public sealed class ImmutableSelfDestructingMessage
    {
        public int MessageId { get; }
        public string Message { get; }
        public DateTime? SentDate { get; }
        public bool HasAttachment { get; }
        public Option<ImmutableSelfDestructingMessageAttachment> Attachment { get; }

        public ImmutableSelfDestructingMessage(
            int messageId,
            string message,
            DateTime? sentDate,
            Option<ImmutableSelfDestructingMessageAttachment> attachment)
        {
            MessageId = messageId;
            Message = message;
            SentDate = sentDate;
            Attachment = attachment ?? Option.None<ImmutableSelfDestructingMessageAttachment>();
            HasAttachment = attachment?.IsSome ?? false;
        }

        /// <summary>
        /// Creates a copy with a sent date.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableSelfDestructingMessage WithSentDate(DateTime sentDate)
        {
            return new ImmutableSelfDestructingMessage(
                MessageId, Message, sentDate, Attachment
            );
        }

        /// <summary>
        /// Creates a copy with an attachment.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableSelfDestructingMessage WithAttachment(ImmutableSelfDestructingMessageAttachment attachment)
        {
            return new ImmutableSelfDestructingMessage(
                MessageId, Message, SentDate, Option.Some(attachment)
            );
        }

        /// <summary>
        /// Converts from EF entity to immutable domain model.
        /// Pure transformation function.
        /// </summary>
        public static ImmutableSelfDestructingMessage FromEntity(SelfDestructingMessage entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var attachment = entity.SelfDestructingMessageAttachment != null
                ? Option.Some(ImmutableSelfDestructingMessageAttachment.FromEntity(entity.SelfDestructingMessageAttachment))
                : Option.None<ImmutableSelfDestructingMessageAttachment>();

            return new ImmutableSelfDestructingMessage(
                messageId: entity.MessageId,
                message: entity.Message,
                sentDate: entity.SentDate,
                attachment: attachment
            );
        }

        /// <summary>
        /// Converts to EF entity for persistence.
        /// This is the I/O boundary transformation.
        /// </summary>
        public SelfDestructingMessage ToEntity()
        {
            var entity = new SelfDestructingMessage
            {
                MessageId = MessageId,
                Message = Message,
                SentDate = SentDate,
                HasAttachment = HasAttachment
            };

            Attachment.Match(
                onSome: att => entity.SelfDestructingMessageAttachment = att.ToEntity(),
                onNone: () => { }
            );

            return entity;
        }
    }

    /// <summary>
    /// Immutable representation of a self-destructing message attachment for business logic.
    /// </summary>
    public sealed class ImmutableSelfDestructingMessageAttachment
    {
        public int AttachmentId { get; }
        public int MessageId { get; }
        public string Attachment { get; }
        public DateTime? SentDate { get; }

        public ImmutableSelfDestructingMessageAttachment(
            int attachmentId,
            int messageId,
            string attachment,
            DateTime? sentDate)
        {
            AttachmentId = attachmentId;
            MessageId = messageId;
            Attachment = attachment;
            SentDate = sentDate;
        }

        /// <summary>
        /// Creates a copy with a sent date.
        /// Pure function: returns new instance without mutating the original.
        /// </summary>
        public ImmutableSelfDestructingMessageAttachment WithSentDate(DateTime sentDate)
        {
            return new ImmutableSelfDestructingMessageAttachment(
                AttachmentId, MessageId, Attachment, sentDate
            );
        }

        /// <summary>
        /// Converts from EF entity to immutable domain model.
        /// Pure transformation function.
        /// </summary>
        public static ImmutableSelfDestructingMessageAttachment FromEntity(SelfDestructingMessageAttachment entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ImmutableSelfDestructingMessageAttachment(
                attachmentId: entity.AttachmentId,
                messageId: entity.MessageId,
                attachment: entity.Attachment,
                sentDate: entity.SentDate
            );
        }

        /// <summary>
        /// Converts to EF entity for persistence.
        /// This is the I/O boundary transformation.
        /// </summary>
        public SelfDestructingMessageAttachment ToEntity()
        {
            return new SelfDestructingMessageAttachment
            {
                AttachmentId = AttachmentId,
                MessageId = MessageId,
                Attachment = Attachment,
                SentDate = SentDate
            };
        }
    }
}
