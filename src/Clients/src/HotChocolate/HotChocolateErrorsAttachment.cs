﻿using Thor.Core.Abstractions;

namespace Thor.Extensions.HotChocolate
{
    internal class HotChocolateErrorsAttachment
        : IAttachment
    {
        public AttachmentId Id { get; set; }
        public string Name { get; set; }
        public byte[] Value { get; set; }
    }
}