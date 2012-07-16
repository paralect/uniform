using System;

namespace Uniform
{
    public class DocumentConfiguration
    {
        public Type DocumentType { get; set; }

        public DocumentConfiguration(Type documentType)
        {
            DocumentType = documentType;
        }

        public bool Equals(DocumentConfiguration other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.DocumentType, DocumentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof (DocumentConfiguration))
            {
                return false;
            }
            return Equals((DocumentConfiguration) obj);
        }

        public override int GetHashCode()
        {
            return (DocumentType != null ? DocumentType.GetHashCode() : 0);
        }
    }
}