using System;
using System.Collections.Generic;

namespace Uniform.Temp.Metadata
{
    public class DocumentInfo
    {
        public Type DocumentType { get; set; }
        public String CollectionName { get; set; }
        public List<DependentDocumentMetadata> Dependents { get; set; }

        public DocumentInfo(Type documentType)
        {
            DocumentType = documentType;
            Dependents = new List<DependentDocumentMetadata>();
        }
    }
}