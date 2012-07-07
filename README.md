Uniform
=======

Uniform defines simple abstraction for key-value database. Two implementations available: InMemoryDatabase 
and MongodbDatabase. By using one abstraction, you are able to run your database entirely in-memory or using MongoDB
as a storage.


## Uniform Database


Abstraction for document database. Here is an interface:

```csharp
/// <summary>
/// Simple abstraction for document database. Documents organized in collections. 
/// Each collection can contains only one type of documents.
/// </summary>
public interface IDatabase
{
    /// <summary>
    /// Database metadata, contains all document types and provides some
    /// metadata related services.
    /// </summary>
    DatabaseMetadata Metadata { get; }

    /// <summary>
    /// Gets collection with specifed name that contains documents of specified type (TDocument)
    /// Will be created, if not already exists.
    /// </summary>
    ICollection<TDocument> GetCollection<TDocument>(String name);

    /// <summary>
    /// Gets collection that contains documents of specified type (TDocument). Will be created, if not already exists.
    /// Name of collection will be taken from [Collection] attribute, that you can put on document class.
    /// If no [Collection] attribute found - type(TDocument).Name will be used for name.
    /// </summary>
    ICollection<TDocument> GetCollection<TDocument>();
}
```


## Uniform Collection


Abstraction for document collection. Collection can contains only documents of one type.

```csharp
/// <summary>
/// Simple abstraction that represents typed collection of documents.
/// Collection always consists only of one type of documents.
/// </summary>
public interface ICollection<TDocument>
{
    /// <summary>
    /// Returns document by it's key. 
    /// If document doesn't exists - default(TDocument) will be returned.
    /// </summary>
    TDocument GetById(String key);

    /// <summary>
    /// Saves document to collection using specified key.
    /// If document with such key already exists, it will be silently overwritten.
    /// </summary>
    void Save(String key, TDocument obj);

    /// <summary>
    /// Saves document to collection using specified key. 
    /// 'Creator' function will be applied to automatically created document of type TDocument.
    /// If document with such key already exists, it will be silently overwritten.
    /// </summary>
    void Save(String key, Action<TDocument> creator);

    /// <summary>
    /// Updates document with specified key.
    /// If document with such key doesn't exist, update will be discarded - i.e. no changes  to collection will be made. 
    /// See UpdateOrSave() method, if you need a kind of "upsert" behaviour.
    /// </summary>
    void Update(String key, Action<TDocument> updater);

    /// <summary>
    /// Updates document with specified key.
    /// If document with such key doesn't exists, new document will be created and 'updater' function will be applied to 
    /// this newly created document.
    /// </summary>
    void UpdateOrSave(String key, Action<TDocument> updater);

    /// <summary>
    /// Deletes document with specified key.
    /// If document with such key doesn't exists - no changes to collection will be made.
    /// </summary>
    void Delete(String key);
}
```

