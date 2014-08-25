﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;

namespace Remotion.Collections
{
  /// <summary>The <see cref="CacheInvalidationToken"/> can be used as a means to commicate that the cached information is no longer current.</summary>
  /// <remarks>
  /// Use <see cref="GetCurrent"/> to get the current revision and provide it to <see cref="IsCurrent"/> when checking whether the revision is still current.
  /// Invoke <see cref="Invalidate"/> to signal a cache invalidation.
  /// <note type="caution">
  /// If the <see cref="CacheInvalidationToken.Revision"/> is part of a serialized instance, the associated <see cref="CacheInvalidationToken"/> must also be serialized.
  /// </note>
  /// </remarks>
  /// <seealso cref="LockingCacheInvalidationToken"/>
  /// <threadsafety static="true" instance="true" />
  [Serializable]
  public abstract class CacheInvalidationToken
  {
    /// <summary>Represents a cache revision for the <see cref="CacheInvalidationToken"/> from which it was created.</summary>
    /// <threadsafety static="true" instance="true" />
    [Serializable]
    public struct Revision
    {
      private readonly long _value;

#if DEBUG
      // The CacheInvalidationToken should only be held in debug builds to allow the easier finding of mismatched use of CacheInvalidationToken.
      // For release builds, no hit should be taken for the extra field in the Revision.
      private readonly WeakReference<CacheInvalidationToken> _tokenReference;
#endif

      internal Revision (long value, CacheInvalidationToken token)
      {
        _value = value;

#if DEBUG
        _tokenReference = new WeakReference<CacheInvalidationToken> (token);
#endif
      }

      internal long Value
      {
        get { return _value; }
      }

#if DEBUG
      public CacheInvalidationToken Token
      {
        get
        {
          if (_tokenReference == null)
            return null;

          // Taking a lock on the _tokenReference is OK in this instance given that the_tokenReference is competely under control of the current type.
          lock (_tokenReference)
          {
            CacheInvalidationToken token;
            _tokenReference.TryGetTarget (out token);
            return token;
          }
        }
      }
#endif
    }

    /// <summary>
    /// Creates a non-threadsafe version of the <see cref="CacheInvalidationToken"/> class.
    /// </summary>
    public static CacheInvalidationToken Create ()
    {
      return new CacheInvalidationTokenImplementation();
    }

    /// <summary>
    /// Creates a threadsafe version of the <see cref="CacheInvalidationToken"/> class.
    /// </summary>
    public static LockingCacheInvalidationToken CreatWithLocking ()
    {
      return new LockingCacheInvalidationToken();
    }

    // Prevent creating derived types outside of what's predefined.
    internal CacheInvalidationToken ()
    {
    }

    public Revision GetCurrent ()
    {
      return new Revision (GetCurrentRevisionValue(), this);
    }

    public bool IsCurrent (Revision revision)
    {
#if DEBUG
      // Do not perform check if revision.Token == null because this can mean the Revision has been serialized and deserialized.
      var cacheInvalidationTokenFromRevision = revision.Token;
      if (cacheInvalidationTokenFromRevision == null)
      {
        throw new ArgumentException (
            "The Revision used for the comparision was either created via the default constructor or the associated CacheInvalidationToken has already been garbage collected.",
            "revision");
      }
      if (!ReferenceEquals (this, cacheInvalidationTokenFromRevision))
        throw new ArgumentException ("The Revision used for the comparision was not created by the current CacheInvalidationToken.", "revision");
#endif

      return GetCurrentRevisionValue() == revision.Value;
    }

    public abstract void Invalidate ();

    protected abstract long GetCurrentRevisionValue ();
  }
}