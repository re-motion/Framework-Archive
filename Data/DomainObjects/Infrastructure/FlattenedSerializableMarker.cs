using System;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  [Serializable]
  internal class FlattenedSerializableMarker
  {
    public static readonly FlattenedSerializableMarker Instance = new FlattenedSerializableMarker();
  }
}