﻿using Newtonsoft.Json;

namespace Pls.SimpleMongoDb.Serialization
{
    internal class JsonSerializerFactory
    {
        internal JsonSerializer Create()
        {
            return new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }
    }
}
