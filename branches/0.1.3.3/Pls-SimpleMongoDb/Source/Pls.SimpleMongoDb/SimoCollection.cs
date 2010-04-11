using System.Collections.Generic;
using System.Linq;
using Pls.SimpleMongoDb.Commands;

namespace Pls.SimpleMongoDb
{
    public class SimoCollection
        : ISimoCollection
    {
        public ISimoDatabase Database { get; private set; }

        public string Name { get; private set; }

        public string FullCollectionName
        {
            get { return string.Format("{0}.{1}", Database.Name, Name); }
        }

        public SimoCollection(ISimoDatabase database, string name)
        {
            Database = database;
            Name = Database.Session.Pluralizer.Pluralize(name);
        }

        public void Drop()
        {
            Database.DropCollections(Name);
        }

        public void Insert(object document)
        {
            Insert(new[] { document });
        }

        public void Insert(IEnumerable<object> documents)
        {
            var cmd = new InsertDocumentsCommand(Database.Session.Connection)
                          {
                              Documents = documents.ToList(),
                              NodeName = FullCollectionName
                          };
            cmd.Execute();
        }

        public void Update(object selector, object document)
        {
            var cmd = new UpdateDocumentsCommand(Database.Session.Connection)
                      {
                          Mode = UpdateModes.Upsert,
                          NodeName = FullCollectionName,
                          QuerySelector = selector,
                          Document = document
                      };
            cmd.Execute();
        }

        public void UpdateMany(object selector, object document)
        {
            var cmd = new UpdateDocumentsCommand(Database.Session.Connection)
            {
                Mode = UpdateModes.MultiUpdate,
                NodeName = FullCollectionName,
                QuerySelector = selector,
                Document = document
            };
            cmd.Execute();
        }

        public void Delete(object selector)
        {
            var cmd = new DeleteDocumentsCommand(Database.Session.Connection)
                          {
                              Selector = selector,
                              NodeName = FullCollectionName
                          };
            cmd.Execute();
        }

        public T FindOne<T>(object selector, object documentSchema)
            where T : class
        {
            var result = Find<T>(selector, documentSchema);

            return result.SingleOrDefault();
        }

        public IList<T> Find<T>(object selector, object documentSchema = null)
            where T : class
        {
            var cmd = new QueryDocumentsCommand<T>(Database.Session.Connection)
                          {
                              NodeName = FullCollectionName,
                              QuerySelector = selector,
                              DocumentSchema = documentSchema
                          };
            cmd.Execute();

            return cmd.Response.Documents;
        }

        public T FindOneInfered<T>(T inferedTemplate, object selector)
            where T : class
        {
            return FindInfered(inferedTemplate, selector).SingleOrDefault();
        }

        public IList<T> FindInfered<T>(T inferedTemplate, object selector)
            where T : class
        {
            var cmd = new QueryDocumentsCommand<T>(Database.Session.Connection)
            {
                NodeName = FullCollectionName,
                QuerySelector = selector
            };
            cmd.Execute();

            return cmd.Response.Documents;
        }

        public int Count(object selector = null)
        {
            var queryCommand = new InferedCommandFactory().CreateInfered(Database.Session.Connection, new { _id = "" });
            queryCommand.NodeName = FullCollectionName;
            queryCommand.QuerySelector = selector;
            queryCommand.Execute();

            return queryCommand.Response.NumberOfDocuments;
        }
    }
}