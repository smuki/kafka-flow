using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePipeline.Producers
{
    public enum XXXPersistenceStatus
    {
        NotPersisted = 0,
        PossiblyPersisted = 1,
        Persisted = 2
    }
}
