using System;

namespace Uniform
{
    public class DocumentInfo
    {
        public Type DocumentType { get; set; }
        public String CollectionName { get; set; }

        public DocumentInfo(Type documentType)
        {
            DocumentType = documentType;
        }
    }
}