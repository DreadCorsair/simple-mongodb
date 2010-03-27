using System.IO;
using Pls.SimpleMongoDb.Resources;
using Pls.SimpleMongoDb.Serialization;

namespace Pls.SimpleMongoDb.Commands
{
    /// <summary>
    /// Command used for querying for documents.
    /// </summary>
    public class QueryDocumentsCommand<TDocument>
        : SimoResponseCommand<TDocument>
        where TDocument : class
    {
        /// <summary>
        /// Defines how the result will be returned.
        /// </summary>
        private QueryOptions QueryOption { get; set; }

        /// <summary>
        /// Sets the number of documents to omit - starting from
        /// the first document in the resulting dataset - when
        /// returning the result of the query.
        /// </summary>
        public virtual int NumberOfDocumentsToSkip { get; set; }

        /// <summary>
        /// Number of documents to return in the first reply.
        /// If numberToReturn is 0, the db will used the default
        /// return size. If the number is negative, then the
        /// database will return that number and close the cursor.
        /// No futher results for that query can be fetched.
        /// </summary>
        public virtual int NumberOfDocumentsToReturn { get; set; }

        /// <summary>
        /// QuerySelector - defines the query criterias.
        /// </summary>
        public object QuerySelector { get; set; }

        /// <summary>
        /// Optional. Lets you control the fields to return.
        /// </summary>
        public object DocumentSchema { get; set; }

        public QueryDocumentsCommand(ISimoConnection connection)
            : base(connection)
        {
            QueryOption = QueryOptions.None;
            NumberOfDocumentsToSkip = 0;
            NumberOfDocumentsToReturn = 0;
        }

        protected override void OnEnsureValidForExecution()
        {
            if (string.IsNullOrEmpty(NodeName))
                throw new SimoCommandException(ExceptionMessages.SimoCommand_IsMissingNodeName);
        }

        protected override Request GenerateRequest()
        {
            return new Request(OpCodes.Query, GenerateBody());
        }

        protected virtual byte[] GenerateBody()
        {
            //http://www.mongodb.org/display/DOCS/Mongo+Wire+Protocol#MongoWireProtocol-OPQUERY
            //int32     opts;                   // query options
            //cstring   fullCollectionName;     // "dbname.collectionname"
            //int32     numberToSkip;           // number of documents to skip when returning results
            //int32     numberToReturn;         // number of documents to return in the first OP_REPLY
            //BSON      query;                  // query object.  See below for details.
            //BSON      returnFieldSelector;    // OPTIONAL : selector indicating the fields to return.  See below for details.

            using (var stream = new MemoryStream())
            {
                using (var writer = new BodyWriter(stream))
                {
                    writer.Write((int)QueryOption);
                    writer.Write(NodeName);
                    writer.WriteTerminator();
                    writer.Write(NumberOfDocumentsToSkip);
                    writer.Write(NumberOfDocumentsToReturn);

                    writer.WriteSelector(QuerySelector ?? new object());
                    
                    if(DocumentSchema != null)
                        writer.WriteDocument(DocumentSchema);

                    return stream.ToArray();
                }
            }
        }
    }
}