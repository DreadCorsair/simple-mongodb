using System;
using Pls.SimpleMongoDb.Resources;
using Pls.SimpleMongoDb.Serialization;

namespace Pls.SimpleMongoDb.Commands
{
    /// <summary>
    /// Base-class which you can build your commands upon.
    /// </summary>
    public abstract class SimoCommand
    {
        public virtual ISimoConnection Connection { get; set; }

        public virtual bool CanHandleResponses
        {
            get { return false; }
        }

        protected SimoCommand(ISimoConnection connection)
        {
            Connection = connection;
        }

        public virtual void Execute()
        {
            EnsureOpenConnection();
            OnEnsureValidForExecution();
            OnExecute(Connection);
        }

        protected abstract void OnEnsureValidForExecution();

        protected virtual void EnsureOpenConnection()
        {
            if (!Connection.IsConnected)
                Connection.Connect();
        }

        protected virtual void OnExecute(ISimoConnection connection)
        {
            var request = GenerateRequest();
            using (var requestStream = connection.GetPipeStream())
            {
                using (var requestWriter = new RequestWriter(requestStream))
                {
                    requestWriter.Write(request);

                    if (CanHandleResponses)
                    {
                        using (var responseStream = Connection.GetPipeStream())
                        {
                            using (var responseReader = new ResponseReader(responseStream))
                            {
                                OnReadResponse(responseReader);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Implement if the command is supposed to handle responses from MongoDb.
        /// </summary>
        /// <param name="responseReader"></param>
        protected virtual void OnReadResponse(ResponseReader responseReader)
        {
            throw new NotImplementedException(Exceptions.MongoCommand_OnReadResponseNotImplemented);
        }

        /// <summary>
        /// Should generate the Request that will be serialized
        /// to the Network stream, hence sent to the server.
        /// </summary>
        /// <returns></returns>
        protected abstract Request GenerateRequest();
    }
}