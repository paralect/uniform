using System;

namespace Uniform
{
    public class CollectionInfo
    {
        public Type DocumentType { get; set; }
        public String CollectionName { get; set; }

        public CollectionInfo(Type documentType, string collectionName)
        {
            DocumentType = documentType;
            CollectionName = collectionName;
        }

        public bool Equals(CollectionInfo other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return other.DocumentType == DocumentType && Equals(other.CollectionName, CollectionName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof (CollectionInfo))
                return false;

            return Equals((CollectionInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DocumentType != null ? DocumentType.GetHashCode() : 0)*397) ^ (CollectionName != null ? CollectionName.GetHashCode() : 0);
            }
        }
    }
}