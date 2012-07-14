using System;

namespace Uniform
{
    public class DocumentConfiguration
    {
        public String DatabaseName { get; set; }
        public String CollectionName { get; set; }
        public Type DocumentType { get; set; }

        public DocumentConfiguration(String databaseName, String collectionName, Type documentType)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
            DocumentType = documentType;
        }

        public bool Equals(DocumentConfiguration other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(other.DatabaseName, DatabaseName) 
                && Equals(other.CollectionName, CollectionName) 
                && other.DocumentType == DocumentType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof (DocumentConfiguration))
                return false;

            return Equals((DocumentConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (DatabaseName != null ? DatabaseName.GetHashCode() : 0);
                result = (result*397) ^ (CollectionName != null ? CollectionName.GetHashCode() : 0);
                result = (result*397) ^ (DocumentType != null ? DocumentType.GetHashCode() : 0);
                return result;
            }
        }
    }
}