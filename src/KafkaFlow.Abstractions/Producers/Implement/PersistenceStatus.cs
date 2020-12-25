using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaFlow.Producers
{
    public enum PersistenceStatus
    {
        NotPersisted = 0,
        PossiblyPersisted = 1,
        Persisted = 2
    }
}
